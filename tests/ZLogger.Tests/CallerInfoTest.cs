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
        Assert.Equal("CallerInfo", x.LogInfo.MemberName);
        Assert.Equal("CallerInfoTest.cs", Path.GetFileName(x.LogInfo.FilePath));
        Assert.Equal(23, x.LogInfo.LineNumber);

        var currentThread = System.Threading.Thread.CurrentThread;

        Assert.NotEqual(-1, x.LogInfo.ThreadInfo.ThreadId);
        Assert.Equal(currentThread.ManagedThreadId, x.LogInfo.ThreadInfo.ThreadId);
        Assert.Equal(currentThread.IsThreadPoolThread, x.LogInfo.ThreadInfo.IsThreadPoolThread);
        Assert.Equal(currentThread.Name, x.LogInfo.ThreadInfo.ThreadName);
    }
}

