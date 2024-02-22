using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace ZLogger.Generator;

public partial class ZLoggerGenerator
{
    internal record ParseResult(
        TypeDeclarationSyntax TargetTypeSyntax,
        INamedTypeSymbol TargetTypeSymbol,
        LogMethodDeclaration[] LogMethods);

    internal record LogMethodDeclaration(
        ZLoggerMessageAttribute Attribute,
        IMethodSymbol TargetMethod,
        MethodDeclarationSyntax TargetSyntax,
        MessageSegment[] MessageSegments,
        MethodParameter[] MethodParameters);

    public partial class MethodParameter
    {
        public IParameterSymbol Symbol { get; }
        public bool IsFirstLogger { get; init; }
        public bool IsFirstLogLevel { get; init; }
        public bool IsFirstException { get; init; }
        public bool IsCallerMemberName { get; init; }
        public bool IsCallerFilePath { get; init; }
        public bool IsCallerLineNumber { get; init; }
        public bool IsZLoggerContext { get; init; }

        public bool IsParameter => !IsFirstLogger && !IsFirstLogLevel && !IsFirstException && !IsAdditionalParameter;
        public bool IsAdditionalParameter => IsCallerMemberName || IsCallerFilePath || IsCallerLineNumber || IsZLoggerContext;

        // set from outside, if many segments was linked, use first-one.
        public MessageSegment LinkedMessageSegment { get; set; } = default!;

        public MethodParameter(IParameterSymbol symbol)
        {
            Symbol = symbol;
        }

        public bool IsEnumerable()
        {
            if (!IsParameter) return false;

            if (Symbol.Type.SpecialType == SpecialType.System_String) return false;

            return Symbol.Type.AllInterfaces.Any(y => y.SpecialType == SpecialType.System_Collections_IEnumerable);
        }
    }

    internal class Parser
    {
        SourceProductionContext context;
        ImmutableArray<GeneratorAttributeSyntaxContext> sources;

        readonly INamedTypeSymbol loggerSymbol;
        readonly INamedTypeSymbol logLevelSymbol;
        readonly INamedTypeSymbol exceptionSymbol;
        readonly INamedTypeSymbol callerMemberNameAttributeSymbol;
        readonly INamedTypeSymbol callerFilePathAttributeSymbol;
        readonly INamedTypeSymbol callerLineNumberAttributeSymbol;
        readonly INamedTypeSymbol zloggerContextAttributeSymbol;

        public Parser(SourceProductionContext context, ImmutableArray<GeneratorAttributeSyntaxContext> sources)
        {
            this.context = context;
            this.sources = sources;

            var compilation = sources[0].SemanticModel.Compilation;
            this.loggerSymbol = GetTypeByMetadataName(compilation, "Microsoft.Extensions.Logging.ILogger");
            this.logLevelSymbol = GetTypeByMetadataName(compilation, "Microsoft.Extensions.Logging.LogLevel");
            this.exceptionSymbol = GetTypeByMetadataName(compilation, "System.Exception");
            this.callerMemberNameAttributeSymbol = GetTypeByMetadataName(compilation, "System.Runtime.CompilerServices.CallerMemberNameAttribute");
            this.callerFilePathAttributeSymbol = GetTypeByMetadataName(compilation, "System.Runtime.CompilerServices.CallerFilePathAttribute");
            this.callerLineNumberAttributeSymbol = GetTypeByMetadataName(compilation, "System.Runtime.CompilerServices.CallerLineNumberAttribute");
            this.zloggerContextAttributeSymbol = GetTypeByMetadataName(compilation, "ZLogger.ZLoggerContextAttribute");
        }

        static INamedTypeSymbol GetTypeByMetadataName(Compilation compilation, string metadataName)
        {
            var symbol = compilation.GetTypeByMetadataName(metadataName);
            if (symbol == null)
            {
                throw new InvalidOperationException($"Type {metadataName} is not found in compilation.");
            }
            return symbol;
        }

        public ParseResult[] Parse()
        {
            var list = new List<ParseResult>();

            // grouping by type(TypeDeclarationSyntax)
            foreach (var item in sources.GroupBy(x => x.TargetNode.Parent))
            {
                if (item.Key == null) continue;
                var targetType = (TypeDeclarationSyntax)item.Key;
                var symbol = item.First().SemanticModel.GetDeclaredSymbol(targetType);
                if (symbol == null) continue;

                // verify is partial
                if (!IsPartial(targetType))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustBePartial, targetType.Identifier.GetLocation(), symbol.Name));
                    continue;
                }

                // nested is not allowed
                if (IsNested(targetType))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NestedNotAllow, targetType.Identifier.GetLocation(), symbol.Name));
                    continue;
                }

                // verify is generis type
                if (symbol.TypeParameters.Length > 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.GenericTypeNotSupported, targetType.Identifier.GetLocation(), symbol.Name));
                    continue;
                }

                var logMethods = new List<LogMethodDeclaration>();

                foreach (var source in item)
                {
                    var method = (IMethodSymbol)source.TargetSymbol;

                    // verify is partial
                    if (!method.IsPartialDefinition)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MethodMustBePartial, (source.TargetNode as MethodDeclarationSyntax)!.Identifier.GetLocation(), method.Name));
                        continue;
                    }

                    var (attr, setLogLevel) = GetAttribute(source);
                    var msg = attr.Message;

                    // parse and verify
                    if (!MessageParser.TryParseFormat(attr.Message, out var segments))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MessageTemplateParseFailed, (source.TargetNode as MethodDeclarationSyntax)!.Identifier.GetLocation(), method.Name));
                        continue;
                    }

                    var (parameters, foundLogLevel) = GetMethodParameters(method, setLogLevel);

                    // Set LinkedParameters
                    foreach (var p in parameters.Where(x => x.IsParameter))
                    {
                        p.LinkedMessageSegment = segments
                            .Where(x => x.Kind == MessageSegmentKind.NameParameter)
                            .FirstOrDefault(x => x.NameParameter.Equals(p.Symbol.Name, StringComparison.OrdinalIgnoreCase));
                    }

                    var methodDecl = new LogMethodDeclaration(
                        Attribute: attr,
                        TargetMethod: (IMethodSymbol)source.TargetSymbol,
                        TargetSyntax: (MethodDeclarationSyntax)source.TargetNode,
                        MessageSegments: segments,
                        MethodParameters: parameters);

                    if (!Verify(methodDecl, foundLogLevel, targetType, symbol))
                    {
                        continue;
                    }

                    logMethods.Add(methodDecl);
                }

                var result = new ParseResult(targetType, symbol!, logMethods.ToArray());
                list.Add(result);
            }

            var duplicateEventIds = new HashSet<int>();
            foreach (var item in list.SelectMany(x => x.LogMethods).Where(x => x.Attribute.EventId != -1))
            {
                if (!duplicateEventIds.Add(item.Attribute.EventId))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.DuplicateEventIdIsNotAllowed, item.TargetSyntax.Identifier.GetLocation(), item.TargetMethod.Name, item.Attribute.EventId));
                }
            }

            return list.ToArray();
        }

        static (ZLoggerMessageAttribute attr, bool setLogLevel) GetAttribute(GeneratorAttributeSyntaxContext source)
        {
            var attributeData = source.Attributes[0];

            int eventId = -1;
            string? eventName = null;
            LogLevel level = LogLevel.None;
            string message = "";
            bool skipEnabledCheck = false;

            // check logLevel is set for verify.
            var setLogLevel = false;
            var ctorItems = attributeData.ConstructorArguments;

            switch (ctorItems.Length)
            {
                case 0:
                    // public ZLoggerMessageAttribute() { }
                    setLogLevel = false;
                    message = "";
                    break;
                case 1:
                    if (attributeData.AttributeConstructor!.Parameters[0].Type.SpecialType == SpecialType.System_String)
                    {
                        // public ZLoggerMessageAttribute(string message)
                        setLogLevel = false;
                        message = ctorItems[0].IsNull ? "" : (string)ctorItems[0].Value!;
                    }
                    else
                    {
                        // public ZLoggerMessageAttribute(LogLevel level)
                        setLogLevel = true;
                        level = ctorItems[0].IsNull ? LogLevel.None : (LogLevel)ctorItems[0].Value!;
                    }
                    break;
                case 2:
                    // ZLoggerMessageAttribute(LogLevel level, string message)
                    setLogLevel = true;
                    level = ctorItems[0].IsNull ? LogLevel.None : (LogLevel)ctorItems[0].Value!;
                    message = ctorItems[1].IsNull ? "" : (string)ctorItems[1].Value!;
                    break;

                case 3:
                    // ZLoggerMessageAttribute(int eventId, LogLevel level, string message)
                    setLogLevel = true;
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
                                setLogLevel = !value.IsNull;
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

            return (new ZLoggerMessageAttribute()
            {
                EventId = eventId,
                EventName = eventName,
                Level = level,
                Message = message,
                SkipEnabledCheck = skipEnabledCheck,
            }, setLogLevel);
        }

        (MethodParameter[] parameters, bool foundLogLevel) GetMethodParameters(IMethodSymbol method, bool setLogLevel)
        {
            var result = new MethodParameter[method.Parameters.Length];

            var foundFirstLogger = false;
            var foundFirstLogLevel = false;
            var foundFirstException = false;

            if (setLogLevel)
            {
                // If LogLevel is set from Attribute, does not use LogLevel on method parameter.
                foundFirstLogLevel = true;
            }

            for (int i = 0; i < method.Parameters.Length; i++)
            {
                var p = method.Parameters[i];

                if (!foundFirstLogger)
                {
                    var isLogger = p.Type.AllInterfaces.Concat(new[] { p.Type }).Any(x => SymbolEqualityComparer.Default.Equals(x, loggerSymbol));
                    if (isLogger)
                    {
                        foundFirstLogger = true;
                        result[i] = new MethodParameter(p)
                        {
                            IsFirstLogger = true,
                        };
                        continue;
                    }
                }
                if (!foundFirstLogLevel)
                {
                    var isLogLevel = SymbolEqualityComparer.Default.Equals(p.Type, logLevelSymbol);
                    if (isLogLevel)
                    {
                        foundFirstLogLevel = true;
                        result[i] = new MethodParameter(p)
                        {
                            IsFirstLogLevel = true,
                        };
                        continue;
                    }
                }
                if (!foundFirstException)
                {
                    var isException = SymbolEqualityComparer.Default.Equals(p.Type, exceptionSymbol);
                    if (isException)
                    {
                        foundFirstException = true;
                        result[i] = new MethodParameter(p)
                        {
                            IsFirstException = true,
                        };
                        continue;
                    }
                }

                var attributes = p.GetAttributes();                
                if (attributes.Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, callerMemberNameAttributeSymbol)))
                {
                    result[i] = new MethodParameter(p) { IsCallerMemberName = true };
                    continue;
                }
                if (attributes.Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, callerFilePathAttributeSymbol)))
                {
                    result[i] = new MethodParameter(p) { IsCallerFilePath = true };
                    continue;
                }
                if (attributes.Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, callerLineNumberAttributeSymbol)))
                {
                    result[i] = new MethodParameter(p) { IsCallerLineNumber = true };
                    continue;
                }
                if (attributes.Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, zloggerContextAttributeSymbol)))
                {
                    result[i] = new MethodParameter(p) { IsZLoggerContext = true };
                    continue;
                }

                result[i] = new MethodParameter(p);
            }

            return (result, foundFirstLogLevel);
        }

        static bool IsPartial(TypeDeclarationSyntax typeDeclaration)
        {
            return typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        }

        static bool IsNested(TypeDeclarationSyntax typeDeclaration)
        {
            return typeDeclaration.Parent is TypeDeclarationSyntax;
        }

        bool Verify(LogMethodDeclaration methodDeclaration, bool foundLogLevel, TypeDeclarationSyntax declaredTypeSyntax, INamedTypeSymbol declaredTypeSymbol)
        {
            var verifyResult = true;

            var methodLocation = methodDeclaration.TargetSyntax.Identifier.GetLocation();
            var methodName = methodDeclaration.TargetMethod.Name;

            // must retrun void
            if (methodDeclaration.TargetMethod.ReturnType.SpecialType != SpecialType.System_Void)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustReturnVoid, methodLocation, methodName));
                verifyResult = false;
            }

            // generic is not supported
            if (methodDeclaration.TargetMethod.TypeParameters.Length > 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.GenericNotSupported, methodLocation, methodName));
                verifyResult = false;
            }

            // LogLevel not found
            if (!foundLogLevel)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.LogLevelNotFound, methodLocation, methodName));
                verifyResult = false;
            }

            // missing ILogger
            if (!methodDeclaration.MethodParameters.Any(x => x.IsFirstLogger))
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingLogger, methodLocation, methodName));
                verifyResult = false;
            }

            var templateParameters = methodDeclaration.MessageSegments.Where(x => x.Kind == MessageSegmentKind.NameParameter).ToArray();
            var argumentParameters = methodDeclaration.MethodParameters.Where(x => x.IsParameter).ToArray();

            foreach (var templateParameter in templateParameters)
            {
                // template parameter must match argument parameter
                if (!argumentParameters.Any(x => x.Symbol.Name.Equals(templateParameter.NameParameter, StringComparison.OrdinalIgnoreCase)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TemplateHasNoCorrespondingArgument, methodLocation, methodName, templateParameter.NameParameter));
                    verifyResult = false;
                }
            }

            foreach (var argumentParameter in argumentParameters)
            {
                // argument parameter must match template parameter
                if (!templateParameters.Any(x => x.NameParameter.Equals(argumentParameter.Symbol.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ArgumentHasNoCorrespondingTemplate, methodLocation, methodName, argumentParameter.Symbol.Name));
                    verifyResult = false;
                }
            }

            // disallow ref, in, out in parameter
            if (argumentParameters.Any(x => x.Symbol.RefKind != RefKind.None))
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.RefKindNotSupported, methodLocation, methodName));
                verifyResult = false;
            }

            return verifyResult;
        }
    }
}
