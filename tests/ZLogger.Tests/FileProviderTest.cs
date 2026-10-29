using Microsoft.Extensions.Logging;
using System.IO;

namespace ZLogger.Tests;

public class FileProviderTest
{
    [Fact]
    public void CreateAppend()
    {
        const string Path = "ZLoggerTest.log";
        if (File.Exists(Path)) File.Delete(Path);
        {
            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerFile(Path);
            });

            var logger = loggerFactory.CreateLogger("mytest");

            logger.LogDebug("foo");
            logger.LogDebug("bar");
            logger.LogDebug("baz");

            loggerFactory.Dispose();

            using (var fs = File.OpenText(Path))
            {
                Assert.Equal("foo", fs.ReadLine());
                Assert.Equal("bar", fs.ReadLine());
                Assert.Equal("baz", fs.ReadLine());
                Assert.Null(fs.ReadLine());
            }
        }

        {
            // append check

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerFile(Path);
            });

            var logger = loggerFactory.CreateLogger("mytest");

            logger.LogDebug("abc");
            logger.LogDebug("def");
            logger.LogDebug("ghi");

            loggerFactory.Dispose();

            using (var fs = File.OpenText(Path))
            {
                Assert.Equal("foo", fs.ReadLine());
                Assert.Equal("bar", fs.ReadLine());
                Assert.Equal("baz", fs.ReadLine());
                Assert.Equal("abc", fs.ReadLine());
                Assert.Equal("def", fs.ReadLine());
                Assert.Equal("ghi", fs.ReadLine());
                Assert.Null(fs.ReadLine());
            }
        }
    }
}