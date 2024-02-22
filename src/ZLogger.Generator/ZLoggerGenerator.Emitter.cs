using Microsoft.CodeAnalysis;
using System.Text;
using static ZLogger.Generator.EmitHelper;

namespace ZLogger.Generator;

public partial class ZLoggerGenerator
{
    internal class Emitter
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
                sb.Clear();

                var keyword = parseResult.TargetTypeSyntax.Keyword.ToString();
                var typeName = parseResult.TargetTypeSyntax.Identifier.ToString();
                var staticKey = parseResult.TargetTypeSyntax.Modifiers.Any(x => x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword)) ? "static " : "";

                sb.AppendLine($"{staticKey}partial {keyword} {typeName}");
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
            var additionalParameters = method.MethodParameters.Where(x => x.IsAdditionalParameter).ToArray();

            var stateTypeName = $"{method.TargetMethod.Name}State";

            var jsonParameters = methodParameters
                .Select(x => $"        static readonly global::System.Text.Json.JsonEncodedText _jsonParameter_{x.LinkedMessageSegment.NameParameter} = global::System.Text.Json.JsonEncodedText.Encode(\"{x.LinkedMessageSegment.GetPropertyName()}\");")
                .StringJoinNewLine();

            var fieldParameters = methodParameters
                .Select(x => $"        readonly {x.Symbol.Type.ToFullyQualifiedFormatString()} {x.LinkedMessageSegment.NameParameter};")
                .StringJoinNewLine();

            var constructorParameters = methodParameters
                .Concat(additionalParameters)
                .Select(x => $"{x.Symbol.Type.ToFullyQualifiedFormatString()} {x.Symbol.Name}")
                .StringJoinComma();

            var constructorBody = methodParameters
                .Select(x => $"            this.{x.LinkedMessageSegment.NameParameter} = {x.Symbol.Name};")
                .StringJoinNewLine();

            var implAdditionalInfo = additionalParameters.Length > 0 ? ", IZLoggerAdditionalInfo" : "";
            var additionalMembers = implAdditionalInfo != "" ? $"""
        readonly object? zloggerContext;        
        readonly string? callerMemberName;
        readonly string? callerFilePath;
        readonly int callerLineNumber;
""" : "";

            sb.AppendLine($$"""
    readonly struct {{stateTypeName}} : IZLoggerFormattable{{implAdditionalInfo}}, IReadOnlyList<KeyValuePair<string, object?>>
    {
{{additionalMembers}}
    
{{jsonParameters}}

{{fieldParameters}}

        public {{stateTypeName}}({{constructorParameters}})
        {
{{constructorBody}}
""");
            if (additionalParameters.Length > 0)
            {
                if (additionalParameters.FirstOrDefault(x => x.IsZLoggerContext) is { } contextArg)
                {
                    sb.AppendLine($$"""
            this.zloggerContext = {{contextArg.Symbol.Name}};
""");
                }
                else
                {
                    sb.AppendLine($$"""
            this.zloggerContext = null;
""");
                }
                if (additionalParameters.FirstOrDefault(x => x.IsCallerMemberName) is { } callerMemberNameArg)
                {
                    sb.AppendLine($$"""
            this.callerMemberName = {{callerMemberNameArg.Symbol.Name}};
""");
                }
                else
                {
                    sb.AppendLine($$"""
            this.callerMemberName = null;
""");
                }
                if (additionalParameters.FirstOrDefault(x => x.IsCallerFilePath) is { } callerFilePathArg)
                {
                    sb.AppendLine($$"""
            this.callerFilePath = {{callerFilePathArg.Symbol.Name}};
""");
                }
                else
                {
                    sb.AppendLine($$"""
            this.callerFilePath = null;
""");
                }
                if (additionalParameters.FirstOrDefault(x => x.IsCallerLineNumber) is { } callerLineNumberArg)
                {
                    sb.AppendLine($$"""
            this.callerLineNumber = {{callerLineNumberArg.Symbol.Name}};
""");
                }
                else
                {
                    sb.AppendLine($$"""
            this.callerLineNumber = 0;
""");
                }
            }
            sb.AppendLine($$"""
        }
            
        public IZLoggerEntry CreateEntry(in LogInfo info)
        {
            return ZLoggerEntry<{{stateTypeName}}>.Create(info, this);
        }
        
""");

            if (implAdditionalInfo != "")
            {
                sb.AppendLine($$"""
        public (object? Context, string? MemberName, string? FilePath, int LineNumber) GetAdditionalInfo() => (zloggerContext, callerMemberName, callerFilePath, callerLineNumber);

""");
            }

            EmitIZLoggerFormattableMethods(method);
            EmitKeyValuePairEnumerator(method);
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        void EmitIZLoggerFormattableMethods(LogMethodDeclaration method)
        {
            var stateTypeName = $"{method.TargetMethod.Name}State";
            var methodParameters = method.MethodParameters.Where(x => x.IsParameter).ToArray();

            // UTF8 Encoded literal length
            var literalLength = method.MessageSegments.Where(x => x.Kind == MessageSegmentKind.Text).Sum(x => Encoding.UTF8.GetByteCount(x.TextSegment));
            var formattedCount = methodParameters.Length;

            //int ParameterCount { get; }
            //bool IsSupportStructuredLogging { get; }
            //string ToString();
            sb.AppendLine($$"""
        public int ParameterCount => {{formattedCount}};
        public bool IsSupportUtf8ParameterKey => true;
        public override string ToString() => $"{{string.Concat(method.MessageSegments.Select(x => x.ToString()))}}";

""");
            //void ToString(IBufferWriter<byte> writer);
            {
                var appendValues = method.MessageSegments
                    .Select(x =>
                    {
                        if (x.Kind == MessageSegmentKind.Text)
                        {
                            return $"            stringWriter.AppendUtf8(\"{x.TextSegment}\"u8);";
                        }
                        else if (x.Kind == MessageSegmentKind.NameParameter)
                        {
                            var method = methodParameters.First(y => x == y.LinkedMessageSegment);
                            if (method.IsEnumerable() || x.FormatString == "json")
                            {
                                return $"            global::ZLogger.Internal.CodeGeneratorUtil.AppendAsJson(ref stringWriter, {x.NameParameter});";
                            }
                            else
                            {
                                var format = x.FormatString == null ? "null" : $"\"{x.FormatString}\"";
                                return $"            stringWriter.AppendFormatted({x.NameParameter}, {x.Alignment ?? "0"}, {format});";
                            }
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    })
                    .StringJoinNewLine();

                sb.AppendLine($$"""
        public void ToString(global::System.Buffers.IBufferWriter<byte> writer)
        {
            var stringWriter = new global::Utf8StringInterpolation.Utf8StringWriter<global::System.Buffers.IBufferWriter<byte>>(literalLength: {{literalLength}}, formattedCount: {{formattedCount}}, bufferWriter: writer);

{{appendValues}}            

            stringWriter.Flush();
        }

""");
            }

            // string GetOriginalFormat()
            // void WriteOriginalFormat(IBufferWriter<byte> writer)
            sb.AppendLine($$"""
        public string GetOriginalFormat() => "{{string.Concat(method.MessageSegments.Select(x => x.ToString()))}}";

        public void WriteOriginalFormat(global::System.Buffers.IBufferWriter<byte> writer)
        {
            writer.Write("{{string.Concat(method.MessageSegments.Select(x => x.ToString()))}}"u8);
        }

""");
            //void WriteJsonParameterKeyValues(Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions);
            sb.AppendLine($$"""
        public void WriteJsonParameterKeyValues(global::System.Text.Json.Utf8JsonWriter writer, global::System.Text.Json.JsonSerializerOptions jsonSerializerOptions, global::ZLogger.IKeyNameMutator? keyNameMutator = null)
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
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return \"{x.LinkedMessageSegment.GetPropertyName()}\"u8;")}}
            }
            global::ZLogger.Internal.CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public string GetParameterKeyAsString(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return \"{x.LinkedMessageSegment.GetPropertyName()}\";")}}
            }
            global::ZLogger.Internal.CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public object GetParameterValue(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return this.{x.LinkedMessageSegment.NameParameter}!;")}}
            }
            global::ZLogger.Internal.CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public T GetParameterValue<T>(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return Unsafe.As<{x.Symbol.Type.ToFullyQualifiedFormatString()}, T>(ref Unsafe.AsRef(in this.{x.LinkedMessageSegment.NameParameter}));")}}
            }
            global::ZLogger.Internal.CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public Type GetParameterType(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return typeof({x.Symbol.Type.ToFullyQualifiedFormatString()});")}}
            }
            global::ZLogger.Internal.CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

""");
        }

        void EmitKeyValuePairEnumerator(LogMethodDeclaration method)
        {
            var stateTypeName = $"{method.TargetMethod.Name}State";
            var methodParameters = method.MethodParameters.Where(x => x.IsParameter).ToArray();
            sb.AppendLine($$"""
        public int Count => {{methodParameters.Length}};
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public KeyValuePair<string, object?> this[int index]
        {
            get
            {
                switch (index)
                {
{{ForEachLine("                        ", methodParameters, (x, i) => $"case {i}: return new(\"{x.LinkedMessageSegment.GetPropertyName()}\", {x.LinkedMessageSegment.NameParameter});")}}
                }
                global::ZLogger.Internal.CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
                return default!;
            }
        }

        struct Enumerator : IEnumerator<KeyValuePair<string, object?>>
        {
            int currentIndex;
            {{stateTypeName}} state;
            
            public Enumerator({{stateTypeName}} state)
            {
                this.state = state;
                currentIndex = -1;
            }
        
            public bool MoveNext() => ++currentIndex < {{methodParameters.Length}};
            public void Reset() => currentIndex = -1;
            public KeyValuePair<string, object?> Current => state[currentIndex];
            object IEnumerator.Current => Current;
            public void Dispose() { }
        }

""");
        }

        void EmitLogBody(LogMethodDeclaration method)
        {
            var methodParameters = method.MethodParameters.Where(x => x.IsParameter).ToArray();
            var additionalParameters = method.MethodParameters.Where(x => x.IsAdditionalParameter).ToArray();
            var modifiers = method.TargetSyntax.Modifiers.ToString();
            var extension = method.TargetMethod.IsExtensionMethod ? "this " : string.Empty;

            var eventName = method.Attribute.EventName is { } name ? $"\"{name}\"" : $"nameof({method.TargetMethod.Name})";

            var loggerName = method.MethodParameters.First(x => x.IsFirstLogger).Symbol.Name;

            var logLevelParameter = method.MethodParameters.FirstOrDefault(x => x.IsFirstLogLevel);
            var logLevel = (logLevelParameter != null) ? logLevelParameter.Symbol.ToFullyQualifiedFormatString() : "global::Microsoft.Extensions.Logging.LogLevel." + method.Attribute.Level;

            var exceptionParameter = method.MethodParameters.FirstOrDefault(x => x.IsFirstException);
            var exception = (exceptionParameter != null) ? exceptionParameter.Symbol.Name : "null";

            var stateTypeName = $"{method.TargetMethod.Name}State";

            var methodArgument = method.MethodParameters
                .Select(x =>
                {
                    var t = x.Symbol.Type;
                    var nullableSuffix = t.NullableAnnotation is NullableAnnotation.Annotated && t.IsValueType is false ? "?" : "";
                    return $"{t.ToFullyQualifiedFormatString()}{nullableSuffix} {x.Symbol.Name}";
                })
                .StringJoinComma();

            var newParameters = methodParameters
                .Concat(additionalParameters)
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
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Utf8StringInterpolation;
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
        }
    }

    public partial class MethodParameter
    {
        public string ConvertJsonWriteMethod()
        {
            return ConvertJsonWriteMethodCore(Symbol.Type, false);
        }

        string ConvertJsonWriteMethodCore(ITypeSymbol type, bool emitDotValue)
        {
            var emitDotValueString = emitDotValue ? ".Value" : string.Empty;
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                    return $"writer.WriteBoolean(_jsonParameter_{LinkedMessageSegment.NameParameter}, this.{LinkedMessageSegment.NameParameter}{emitDotValueString});";
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    return $"writer.WriteNumber(_jsonParameter_{LinkedMessageSegment.NameParameter}, this.{LinkedMessageSegment.NameParameter}{emitDotValueString});";
                case SpecialType.System_String:
                case SpecialType.System_DateTime:
                    return $"writer.WriteString(_jsonParameter_{LinkedMessageSegment.NameParameter}, this.{LinkedMessageSegment.NameParameter}{emitDotValueString});";
                default:
                    if (type.TypeKind == TypeKind.Enum)
                    {
                        return $"global::ZLogger.Internal.CodeGeneratorUtil.WriteJsonEnum(writer, _jsonParameter_{LinkedMessageSegment.NameParameter}, this.{LinkedMessageSegment.NameParameter}{emitDotValueString});";
                    }

                    var fullString = type.ToFullyQualifiedFormatString();
                    if (fullString is "global::System.DateTimeOffset" or "global::System.Guid")
                    {
                        return $"writer.WriteString(_jsonParameter_{LinkedMessageSegment.NameParameter}, this.{LinkedMessageSegment.NameParameter}{emitDotValueString});";
                    }

                    if (type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                    {
                        var typeArgument = namedTypeSymbol.TypeArguments.First();

                        var nullIf = $"if (this.{LinkedMessageSegment.NameParameter} == null) {{ writer.WriteNull(_jsonParameter_{LinkedMessageSegment.NameParameter}); }} else {{ ";
                        var nullElse = ConvertJsonWriteMethodCore(typeArgument, true);
                        var nullEnd = " }";

                        return nullIf + nullElse + nullEnd;
                    }

                    break;
            }

            // final fallback, use Serialize
            return $"writer.WritePropertyName(_jsonParameter_{LinkedMessageSegment.NameParameter}); global::System.Text.Json.JsonSerializer.Serialize(writer, this.{LinkedMessageSegment.NameParameter}{emitDotValueString});";
        }
    }
}
