namespace ZLogger.Tests;

public class CallerInfoTest
{
    [Fact]
    public void CaptureCallerInfo()
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
        x.LogInfo.CallerInfo!.Value.MemberName.Should().NotBeEmpty();
        x.LogInfo.CallerInfo!.Value.FilePath.Should().NotBeEmpty();
        x.LogInfo.CallerInfo!.Value.LineNumber.Should().BeGreaterThan(1);
    }
}