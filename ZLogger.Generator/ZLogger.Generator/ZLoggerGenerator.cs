using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace ZLogger.Generator;

// reference: https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator

[Generator(LanguageNames.CSharp)]
public partial class PrivateProxyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var source = context.SyntaxProvider.ForAttributeWithMetadataName(
                 "ZLogger.ZLoggerMessageAttribute",
                 static (node, token) => node is MethodDeclarationSyntax,
                 static (context, token) => context);

        context.RegisterSourceOutput(source.Collect(), Emit);
    }

    static void Emit(SourceProductionContext context, ImmutableArray<GeneratorAttributeSyntaxContext> sources)
    {
        if (sources.Length == 0) return;

        // grouping by type(TypeDeclarationSyntax)
        foreach (var item in sources.GroupBy(x => x.TargetNode.Parent))
        {
            var targetType = item.Key as TypeDeclarationSyntax;

            foreach (var source in item)
            {
                var attr = GetAttribute(source);

                if (!Verify(context))
                {
                    continue;
                }

                var msg = attr.Message;

                if (!MessageParser.TryParseFormat(attr.Message, out var segments))
                {
                    // TODO:Verify
                    continue;
                }

                // Verify2



                // template match(case insentsitive)
                // ILOgger order
            }
        }
    }

    static ZLoggerMessageAttribute GetAttribute(GeneratorAttributeSyntaxContext source)
    {
        // TODO: Attribute verify.

        var attributeData = source.Attributes[0];

        int eventId = -1;
        string? eventName = null;
        LogLevel level = LogLevel.None;
        string message = "";
        bool skipEnabledCheck = false;

        var ctorItems = attributeData.ConstructorArguments;

        switch (ctorItems.Length)
        {
            case 2:
                // ZLoggerMessageAttribute(LogLevel level, string message)
                level = ctorItems[0].IsNull ? LogLevel.None : (LogLevel)ctorItems[0].Value!;
                message = ctorItems[1].IsNull ? "" : (string)ctorItems[1].Value!;
                break;

            case 3:
                // ZLoggerMessageAttribute(int eventId, LogLevel level, string message)
                eventId = ctorItems[0].IsNull ? -1 : (int)ctorItems[0].Value!;
                level = ctorItems[1].IsNull ? LogLevel.None : (LogLevel)ctorItems[1].Value!;
                message = ctorItems[2].IsNull ? "" : (string)ctorItems[2].Value!;
                break;
        }

        if (attributeData.NamedArguments.Any())
        {
            foreach (var namedArgument in attributeData.NamedArguments)
            {
                var typedConstant = namedArgument.Value;
                if (typedConstant.Kind == TypedConstantKind.Error)
                {
                    break;
                }
                else
                {
                    var value = namedArgument.Value;
                    switch (namedArgument.Key)
                    {
                        case "EventId":
                            eventId = (int)value.Value!;
                            break;
                        case "Level":
                            level = value.IsNull ? LogLevel.None : (LogLevel)value.Value!;
                            break;
                        case "SkipEnabledCheck":
                            skipEnabledCheck = (bool)value.Value!;
                            break;
                        case "EventName":
                            eventName = (string?)value.Value;
                            break;
                        case "Message":
                            message = value.IsNull ? "" : (string)value.Value!;
                            break;
                    }
                }
            }
        }

        return new ZLoggerMessageAttribute()
        {
            EventId = eventId,
            EventName = eventName,
            Level = level,
            Message = message,
            SkipEnabledCheck = skipEnabledCheck,
        };
    }

    static bool Verify(SourceProductionContext context)
    {
        // LogLevel not found
        // must retrun void
        // missing ILogger
        // Must be static
        // Must be partial
        // generic is not supported
        // template has no corrresponding argument
        // argument has no corresponding template
        // invalid template
        // alow in, ref qualifier(out is fail)
        // primitives or IUtf8Formattbale
        // TODO:exception
        // TODO:logLevel from paramter?

        // check Duplicate EventId(allow -1)
        return true;
    }


    static string EmitMethodCode(MethodDeclarationSyntax method, ZLoggerMessageAttribute attribute, MessageSegment[] messageSegments)
    {
        var sb = new StringBuilder();

        var stateTypeName = $"{method.Identifier}State";

        sb.AppendLine($$"""
    file readonly struct {{stateTypeName}}: IZLoggerFormattable
    {
        const int Count = 2;
        static readonly JsonEncodedText jsonParameter1 = JsonEncodedText.Encode("hostName");
        static readonly JsonEncodedText jsonParameter2 = JsonEncodedText.Encode("ipAddress");

        readonly string hostName;
        readonly int ipAddress;

        public {{stateTypeName}}(string hostName, int ipAddress)
        {
            this.hostName = hostName;
            this.ipAddress = ipAddress;
        }
""");

        return sb.ToString();
    }

    static void AddSource(SourceProductionContext context, ISymbol targetSymbol, string code, string fileExtension = ".g.cs")
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

