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

            var stateTypeName = $"{method.TargetMethod.Name}State";
            var parameterCount = methodParameters.Length;

            var jsonParameters = methodParameters
                .Select(x => $"        static readonly JsonEncodedText _jsonParameter_{x.LinkedMessageSegment.NameParameter} = JsonEncodedText.Encode(\"{x.LinkedMessageSegment.GetPropertyName()}\");")
                .StringJoinNewLine();

            var fieldParameters = methodParameters
                .Select(x => $"        readonly {x.Symbol.Type.ToFullyQualifiedFormatString()} {x.LinkedMessageSegment.NameParameter};")
                .StringJoinNewLine();

            var constructorParameters = methodParameters
                .Select(x => $"{x.Symbol.Type.ToFullyQualifiedFormatString()} {x.Symbol.Name}")
                .StringJoinComma();

            var constructorBody = methodParameters
                .Select(x => $"            this.{x.LinkedMessageSegment.NameParameter} = {x.Symbol.Name};")
                .StringJoinNewLine();

            sb.AppendLine($$"""
    readonly struct {{stateTypeName}} : IZLoggerFormattable
    {
{{jsonParameters}}

{{fieldParameters}}

        public {{stateTypeName}}({{constructorParameters}})
        {
{{constructorBody}}
        }

        public IZLoggerEntry CreateEntry(LogInfo info)
        {
            return ZLoggerEntry<{{stateTypeName}}>.Create(info, this);
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
                                return $"            CodeGeneratorUtil.AppendAsJson(ref stringWriter, {x.NameParameter});";
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
        public void ToString(IBufferWriter<byte> writer)
        {
            var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(literalLength: {{literalLength}}, formattedCount: {{formattedCount}}, bufferWriter: writer);

{{appendValues}}            

            stringWriter.Flush();
        }

""");
            }

            //void WriteJsonParameterKeyValues(Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions);
            sb.AppendLine($$"""
        public void WriteJsonParameterKeyValues(Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions, IKeyNameMutator? keyNameMutator = null)
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
            CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public string GetParameterKeyAsString(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return \"{x.LinkedMessageSegment.GetPropertyName()}\";")}}
            }
            CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public object GetParameterValue(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return this.{x.LinkedMessageSegment.NameParameter}!;")}}
            }
            CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
            return default!;
        }

        public T GetParameterValue<T>(int index)
        {
            switch (index)
            {
{{ForEachLine("                ", methodParameters, (x, i) => $"case {i}: return Unsafe.As<{x.Symbol.Type.ToFullyQualifiedFormatString()}, T>(ref Unsafe.AsRef(in this.{x.LinkedMessageSegment.NameParameter}));")}}
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

            var eventName = method.Attribute.EventName is { } name ? $"\"{name}\"" : $"nameof({method.TargetMethod.Name})";

            var loggerName = method.MethodParameters.First(x => x.IsFirstLogger).Symbol.Name;

            var logLevelParameter = method.MethodParameters.FirstOrDefault(x => x.IsFirstLogLevel);
            var logLevel = (logLevelParameter != null) ? logLevelParameter.Symbol.Name : "LogLevel." + method.Attribute.Level;

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
                        return $"CodeGeneratorUtil.WriteJsonEnum(writer, _jsonParameter_{LinkedMessageSegment.NameParameter}, this.{LinkedMessageSegment.NameParameter}{emitDotValueString});";
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
            return $"writer.WritePropertyName(_jsonParameter_{LinkedMessageSegment.NameParameter}); JsonSerializer.Serialize(writer, this.{LinkedMessageSegment.NameParameter}{emitDotValueString});";
        }
    }
}
