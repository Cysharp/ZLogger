using System.Linq;

namespace ZLogger.Generator.Tests;

public class InstanceLoggingDiagnosticsTest
{
    [Theory]
    [InlineData("", "", "", "ZLOG008")]
    [InlineData("", "static ILogger logger;", "", "ZLOG008")]
    [InlineData("", "ILogger Logger { get; }", "", "ZLOG008")]
    [InlineData("", "public Example(ILogger logger) { }", "", "ZLOG008")]
    [InlineData("", "ILogger logger;", "static ", "ZLOG008")]
    [InlineData("(ILogger logger)", "", "static ", "ZLOG008")]
    [InlineData("", "ILogger first; ILogger<Example> second;", "", "ZLOG014")]
    [InlineData("(ILogger first, ILogger<Example> second)", "", "", "ZLOG014")]
    [InlineData("(ILogger logger)", "ILogger first; ILogger second;", "", "ZLOG014")]
    public void InvalidLoggerSources_ReportDiagnostic(string constructor, string members, string modifiers, string diagnosticId)
    {
        var diagnostics = CSharpGeneratorRunner.RunGenerator($$"""
using ZLogger;
using Microsoft.Extensions.Logging;
public partial class Example{{constructor}}
{
    {{members}}
    [ZLoggerMessage(LogLevel.Information, "Value: {value}")]
    public {{modifiers}}partial void Log(int value);
}
""");

        diagnostics.Where(x => x.Id != "CS8795").Select(x => x.Id).Should().Equal(diagnosticId);
    }

    [Theory]
    [InlineData("", "int logger", "{logger}")]
    [InlineData("int logger;", "int value", "{value}")]
    [InlineData("int logger { get; }", "int value", "{value}")]
    public void ShadowedPrimaryConstructor_ReportsDiagnostic(string members, string parameter, string template)
    {
        var diagnostics = CSharpGeneratorRunner.RunGenerator($$"""
using ZLogger;
using Microsoft.Extensions.Logging;
public partial class Example(ILogger logger)
{
    {{members}}
    [ZLoggerMessage(LogLevel.Information, "{{template}}")]
    public partial void Log({{parameter}});
}
""");

        diagnostics.Where(x => x.Id != "CS8795").Select(x => x.Id).Should().Equal("ZLOG015");
    }

    [Theory]
    [InlineData("", "ILogger first; ILogger second;")]
    [InlineData("(ILogger first, ILogger second)", "")]
    public void ExplicitLogger_BypassesAmbiguousInstanceSources(string constructor, string members)
    {
        var diagnostics = CSharpGeneratorRunner.RunGenerator($$"""
using ZLogger;
using Microsoft.Extensions.Logging;
public partial class Example{{constructor}}
{
    {{members}}
    [ZLoggerMessage(LogLevel.Information, "Value: {value}")]
    public partial void Log(ILogger logger, int value);
}
""");

        diagnostics.Should().BeEmpty();
    }

    [Theory]
    [InlineData("protected ILogger logger;", "ZLOG015")]
    [InlineData("protected object logger;", "ZLOG015")]
    [InlineData("protected object logger { get; }", "ZLOG015")]
    [InlineData("private object logger;", null)]
    public void InheritedMembers_OnlyShadowPrimaryConstructorWhenAccessible(string member, string diagnosticId)
    {
        var diagnostics = CSharpGeneratorRunner.RunGenerator($$"""
using ZLogger;
using Microsoft.Extensions.Logging;
public class Base
{
    {{member}}
}
public partial class Example(ILogger logger) : Base
{
    [ZLoggerMessage(LogLevel.Information, "Value: {value}")]
    public partial void Log(int value);
}
""");

        var diagnosticIds = diagnostics.Where(x => x.Id != "CS8795").Select(x => x.Id);
        if (diagnosticId == null)
        {
            diagnosticIds.Should().BeEmpty();
        }
        else
        {
            diagnosticIds.Should().Equal(diagnosticId);
        }
    }
}
