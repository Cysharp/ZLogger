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

        var currentThread = System.Threading.Thread.CurrentThread;

        x.LogInfo.ThreadInfo.ThreadId.Should().NotBe(-1);
        x.LogInfo.ThreadInfo.ThreadId.Should().Be(currentThread.ManagedThreadId);
        x.LogInfo.ThreadInfo.IsThreadPoolThread.Should().Be(currentThread.IsThreadPoolThread);
        x.LogInfo.ThreadInfo.ThreadName.Should().Be(currentThread.Name);
    }
}

