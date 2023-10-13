using Microsoft.CodeAnalysis;
using System.Text;
using static ZLogger.Generator.EmitHelper;

namespace ZLogger.Generator;

public partial class ZLoggerGenerator
{
    public class Emitter
    {
        SourceProductionContext context;
        ParseResult[] result;
        StringBuilder sb;

        public Emitter(SourceProductionContext context, ParseResult[] result)
        {
            this.context = context;
            this.result = result;
            this.sb = new StringBuilder(1024);
        }

        public void Emit()
        {
            // per class
            foreach (var parseResult in result)
            {
                var keyword = parseResult.TargetTypeSyntax.Keyword.ToString();
                var typeName = parseResult.TargetTypeSyntax.Identifier.ToString();

                sb.AppendLine($"partial {keyword} {typeName}");
                sb.AppendLine("{");
                sb.AppendLine("");

                // per method
                foreach (var method in parseResult.LogMethods)
                {
                    EmitStructState(method);
                    EmitLogBody(method);
                }

                sb.AppendLine("}");

                AddSource(context, parseResult.TargetTypeSymbol, sb.ToString());
            }
        }

        public void EmitStructState(LogMethodDeclaration method)
        {
            var methodParameters = method.MethodParameters.Where(x => x.IsParameter).ToArray();

            var stateTypeName = $"{method.TargetMethod.Name}State";
            var parameterCount = methodParameters.Length;

            var jsonParameters = methodParameters
                .Select(x => $"        static readonly JsonEncodedText _jsonParameter_{x.LinkedMessageSegment.NameParameter} = JsonEncodedText.Encode(\"{x.LinkedMessageSegment.NameParameter}\");")
                .StringJoinNewLine();

            var fieldParameters = methodParameters
                .Select(x => $"        readonly {x.Symbol.Type.ToFullyQualifiedFormatString()} {x.Symbol.Name};")
                .StringJoinNewLine();

            var constructorParameters = methodParameters
                .Select(x => $"{x.Symbol.Type.ToFullyQualifiedFormatString()} {x.Symbol.Name}")
                .StringJoinComma();

            var constructorBody = methodParameters
                .Select(x => $"            this.{x.Symbol.Name} = {x.Symbol.Name};")
                .StringJoinNewLine();

            sb.AppendLine($$"""
    readonly struct {{stateTypeName}} : IZLoggerFormattable
    {
        const int _parameterCount = {{parameterCount}};

{{jsonParameters}}

{{fieldParameters}}

        public {{stateTypeName}}({{constructorParameters}})
        {
{{constructorBody}}
        }

        static {{stateTypeName}}()
        {
            LogEntryFactory<{{stateTypeName}}>.Create = CreateEntry;
        }

        static IZLoggerEntry CreateEntry(in LogInfo logInfo, in {{stateTypeName}} state)
        {
            return ZLoggerEntry<{{stateTypeName}}>.Create(logInfo, state);
        }

""");

            EmitIZLoggerFormattableMethods(method);
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        void EmitIZLoggerFormattableMethods(LogMethodDeclaration method)
        {
            var stateTypeName = $"{method.TargetMethod.Name}State";
            var methodParameters = method.MethodParameters.Where(x => x.IsParameter).ToArray();
            var messageSegmentWithIndex = method.MessageSegments.Select((segment, index) => (segment, index)).ToArray();

            //int ParameterCount { get; }
            //bool IsSupportStructuredLogging { get; }
            //string ToString();
            sb.AppendLine($$"""
        public int ParameterCount => _parameterCount;
        public bool IsSupportUtf8ParameterKey => true;
        public override string ToString() => "{{string.Concat(method.MessageSegments.Select(x => x.ToString()))}}";

""");
            //void ToString(IBufferWriter<byte> writer);
            {
                var chunks = messageSegmentWithIndex
                    .Where(x => x.segment.Kind == MessageSegmentKind.Text)
                    .Select(x => $"            var chunk{x.index} = \"{x.segment.UnescapedTextSegment}\"u8;")
                    .StringJoinNewLine();

                // Text -> .Length, String -> GetStringMaxByteCount + GuessedParameterByteCount(Count - StringParameterCount)
                var textChunkLength = messageSegmentWithIndex
                    .Where(x => x.segment.Kind == MessageSegmentKind.Text)
                    .Select(x => $"chunk{x.index}.Length")
                    .StringJoin(" + ");

                // TODO: Guess String and Parameter Count
                var spanLength = textChunkLength;

                var writeValues = messageSegmentWithIndex
                    .Select(x =>
                    {
                        // TODO: WriteValue Enumerable(Emit WriteValues?)
                        var name = (x.segment.Kind == MessageSegmentKind.Text)
                            ? $"chunk{x.index}"
                            : x.segment.NameParameter;
                        return $"            CodeGeneratorUtil.WriteValue(writer, {name}, ref dest, ref written);";
                    })
                    .StringJoinNewLine();

                sb.AppendLine($$"""
        public void ToString(IBufferWriter<byte> writer)
        {
{{chunks}}            

            var dest = writer.GetSpan({{spanLength}});
            var written = 0;

{{writeValues}}

            if (written != 0)
            {
                writer.Advance(written);
            }
        }

""");
            }

            //void WriteJsonParameterKeyValues(Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions);
            sb.AppendLine($$"""
        public void WriteJsonParameterKeyValues(Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions)
        {
{{ForEachLine("            ", methodParameters, x => x.ConvertJsonWriteMethod())}}
        }

""");
            //ReadOnlySpan<byte> GetParameterKey(int index);
            //object GetParameterValue(int index);
            //T GetParameterValue<T>(int index);
            //Type GetParameterType(int index);
            sb.AppendLine($$"""
        public ReadOnlySpan<byte> GetParameterKey(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return \"{x.LinkedMessageSegment.NameParameter}\"u8;")}}
            }
            CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public string GetParameterKeyAsString(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return \"{x.LinkedMessageSegment.NameParameter}\";")}}
            }
            CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public object GetParameterValue(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return this.{x.LinkedMessageSegment.NameParameter};")}}
            }
            CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public T GetParameterValue<T>(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return Unsafe.As<{x.Symbol.Type.ToFullyQualifiedFormatString()}, T>(ref Unsafe.AsRef(this.{x.LinkedMessageSegment.NameParameter}));")}}
            }
            CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public Type GetParameterType(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return typeof({x.Symbol.Type.ToFullyQualifiedFormatString()});")}}
            }
            CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

""");
        }

        void EmitLogBody(LogMethodDeclaration method)
        {
            var methodParameters = method.MethodParameters.Where(x => x.IsParameter).ToArray();
            var modifiers = method.TargetSyntax.Modifiers.ToString();
            var extension = method.TargetMethod.IsExtensionMethod ? "this " : string.Empty;

            var eventName = method.Attribute.EventName ?? $"nameof({method.TargetMethod.Name})";

            var loggerName = method.MethodParameters.First(x => x.IsFirstLogger).Symbol.Name;

            var logLevelParameter = method.MethodParameters.FirstOrDefault(x => x.IsFirstLogLevel);
            var logLevel = (logLevelParameter != null) ? logLevelParameter.Symbol.Name : "LogLevel." + method.Attribute.Level;

            var exceptionParameter = method.MethodParameters.FirstOrDefault(x => x.IsFirstException);
            var exception = (exceptionParameter != null) ? exceptionParameter.Symbol.Name : "null";

            var stateTypeName = $"{method.TargetMethod.Name}State";

            // TODO: ref, out, in...?
            var methodArgument = method.MethodParameters
                .Select(x => $"{x.Symbol.Type.ToFullyQualifiedFormatString()} {x.Symbol.Name}")
                .StringJoinComma();

            var newParameters = methodParameters
                .Select(x => $"{x.Symbol.Name}")
                .StringJoinComma();

            sb.AppendLine($$"""
    {{modifiers}} void {{method.TargetMethod.Name}}({{extension}}{{methodArgument}})
    {
{{If(!method.Attribute.SkipEnabledCheck, $"        if (!{loggerName}.IsEnabled({logLevel})) return;")}}
        {{loggerName}}.Log(
            {{logLevel}},
            new EventId({{method.Attribute.EventId}}, {{eventName}}),
            new {{stateTypeName}}({{newParameters}}),
            {{exception}},
            (state, ex) => state.ToString()
        );
    }

""");
        }

        public void AddSource(SourceProductionContext context, ISymbol targetSymbol, string code, string fileExtension = ".g.cs")
        {
            var fullType = targetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
              .Replace("global::", "")
              .Replace("<", "_")
              .Replace(">", "_");

            var sb = new StringBuilder();

            sb.AppendLine("""
// <auto-generated/>
#nullable enable
#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0164
#pragma warning disable CS0219
#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8604
#pragma warning disable CS8619
#pragma warning disable CS8620
#pragma warning disable CS8631
#pragma warning disable CS8765
#pragma warning disable CS9074
#pragma warning disable CA1050

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ZLogger;
using ZLogger.Internal;
using System.Buffers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
""");

            var ns = targetSymbol.ContainingNamespace;
            if (!ns.IsGlobalNamespace)
            {
                sb.AppendLine($"namespace {ns} {{");
            }
            sb.AppendLine();

            sb.AppendLine(code);

            if (!ns.IsGlobalNamespace)
            {
                sb.AppendLine($"}}");
            }

            var sourceCode = sb.ToString();
            context.AddSource($"{fullType}{fileExtension}", sourceCode);
            File.WriteAllText($"/Users/s24061/tmp/{fullType}{fileExtension}", sourceCode);
        }
    }
}
