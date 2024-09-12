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
            x.AddZLoggerLogProcessor(options =>
            {
                options.CaptureThreadInfo = true;
                return processor;
            });
        });
        var logger = loggerFactory.CreateLogger("test");

        logger.ZLogInformation($"aaaa");

        var x = processor.Entries.Dequeue();
        x.LogInfo.MemberName.Should().Be("CallerInfo");
        Path.GetFileName(x.LogInfo.FilePath).Should().Be("CallerInfoTest.cs");
        x.LogInfo.LineNumber.Should().Be(23);

        x.LogInfo.ThreadInfo.Should().NotBeNull();
        x.LogInfo.ThreadInfo!.Value.ThreadId.Should().Be(System.Environment.CurrentManagedThreadId);
        x.LogInfo.ThreadInfo!.Value.IsThreadPoolThread.Should().Be(true);
        x.LogInfo.ThreadInfo!.Value.ThreadName.Should().Be(".NET TP Worker");
    }
}

