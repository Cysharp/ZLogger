using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace ZLogger.Generator;

// reference: https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator

[Generator(LanguageNames.CSharp)]
public partial class ZLoggerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var source = context.SyntaxProvider.ForAttributeWithMetadataName(
            "ZLogger.ZLoggerMessageAttribute",
            static (node, token) => node is MethodDeclarationSyntax,
            static (context, token) => context);

        context.RegisterSourceOutput(source.Collect(), Execute);
    }

    static void Execute(SourceProductionContext context, ImmutableArray<GeneratorAttributeSyntaxContext> sources)
    {
        if (sources.Length == 0) return;

        var result = new Parser(context, sources).Parse();
        if (result.Length != 0)
        {
            var emitter = new Emitter(context, result);
            emitter.Emit();
        }
    }
}
