using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLogger.Generator.Tests;

public class GeneratorDiagnosticsTest(ITestOutputHelper output)
{
    void Compile(int id, string code, bool allowMultipleError = false)
    {
        var diagnostics = CSharpGeneratorRunner.RunGenerator(code);

        // ignore CS0759: No defining declaration found for implementing declaration of partial method 'method'.
        // ignore CS8795: Partial method 'method' must have an implementation part because it has accessibility modifiers.
        diagnostics = diagnostics.Where(x => x.Id != "CS0759" && x.Id != "CS8795").ToArray();

        foreach (var item in diagnostics)
        {
            output.WriteLine(item.ToString());
        }

        if (!allowMultipleError)
        {
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Id.Should().Be("ZLOG" + id.ToString("000"));
        }
        else
        {
            diagnostics.Select(x => x.Id).Should().Contain("ZLOG" + id.ToString("000"));
        }
    }

    [Fact]
    public void ZLOG001_MuestBePartial()
    {
        Compile(1, """
using ZLogger;
using Microsoft.Extensions.Logging;

public class Hoge
{
    [ZLoggerMessage(LogLevel.Information, "{message}")]
    public void Method(string message) {}
}
""");
    }

    [Fact]
    public void ZLOG002_NestedNotAllow()
    {
        Compile(2, """
using ZLogger;
using Microsoft.Extensions.Logging;

public class Foo
{
    public partial class Hoge
    {
        [ZLoggerMessage(LogLevel.Information, "{message}")]
        public void Method(string message) {}
    }
}
""");
    }

    [Fact]
    public void ZLOG003_MethodMustBePartial()
    {
        Compile(3, """
using ZLogger;
using Microsoft.Extensions.Logging;

public partial class Hoge
{
    [ZLoggerMessage(LogLevel.Information, "{message}")]
    public void Method(string message) {}
}
""");
    }

    [Fact]
    public void ZLOG004_MessageTemplateParseFailed()
    {
        Compile(4, """
using ZLogger;
using Microsoft.Extensions.Logging;

public partial class Hoge
{
    [ZLoggerMessage(LogLevel.Information, "{message}}")]
    public partial void Method(string message);
}
""");
    }










}
