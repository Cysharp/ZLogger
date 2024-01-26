using System.IO;

namespace ZLogger.Tests;

public class LogCallerInfoTest
{
    [Fact]
    public void CallerInfo()
    {
        var processor = new TestProcessor(new ZLoggerOptions());

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(_ => processor);
        });
        var logger = loggerFactory.CreateLogger("test");

        logger.ZLogInformation($"aaaa");

        var x = processor.Entries.Dequeue();
        x.LogInfo.MemberName.Should().Be("CallerInfo");
        Path.GetFileName(x.LogInfo.FilePath).Should().Be("CallerInfoTest.cs");
        x.LogInfo.LineNumber.Should().Be(19);
    }
}