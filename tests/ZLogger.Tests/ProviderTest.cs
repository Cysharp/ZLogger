using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ZLogger.Tests
{
    public class ProviderTest
    {
        public static bool ENcoding { get; private set; }

        [Fact]
        public void FileProvider()
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
                    fs.ReadLine().Should().Be("foo");
                    fs.ReadLine().Should().Be("bar");
                    fs.ReadLine().Should().Be("baz");
                    fs.ReadLine().Should().BeNull();
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
                    fs.ReadLine().Should().Be("foo");
                    fs.ReadLine().Should().Be("bar");
                    fs.ReadLine().Should().Be("baz");
                    fs.ReadLine().Should().Be("abc");
                    fs.ReadLine().Should().Be("def");
                    fs.ReadLine().Should().Be("ghi");
                    fs.ReadLine().Should().BeNull();
                }
            }

        }

        static StreamReader OpenFile(string path)
        {
            return new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8);
        }

        [Fact]
        public async Task RollingFileProvider()
        {
            {
                var now = new DateTime(2000, 12, 12);

                var path = $"ZLoggerRollingTest_{now.Date.ToString("yyyy-MM-dd")}-0.log";
                if (File.Exists(path)) File.Delete(path);

                var loggerFactory = LoggerFactory.Create(x =>
                {
                    x.SetMinimumLevel(LogLevel.Debug);
                    x.AddZLoggerRollingFile((dt, seq) => $"ZLoggerRollingTest_{now.Date.ToString("yyyy-MM-dd")}-{seq}.log",
                        x => now, 5);
                });

                var logger = loggerFactory.CreateLogger("mytest");

                logger.LogDebug("foo");
                logger.LogDebug("bar");
                logger.LogDebug("baz");

                await Task.Delay(TimeSpan.FromSeconds(1)); // wait for flush

                File.Exists(path).Should().BeTrue();
                var path1 = path;

                // roll next file
                now = now.AddDays(1);
                path = $"ZLoggerRollingTest_{now.Date.ToString("yyyy-MM-dd")}-0.log";
                if (File.Exists(path)) File.Delete(path);

                logger.LogDebug("a");
                logger.LogDebug("v");
                logger.LogDebug("c");

                await Task.Delay(TimeSpan.FromSeconds(1)); // wait for flush

                File.Exists(path).Should().BeTrue();
                var path2 = path;

                // roll by filesize
                path = $"ZLoggerRollingTest_{now.Date.ToString("yyyy-MM-dd")}-1.log";
                if (File.Exists(path)) File.Delete(path);

                logger.LogDebug(new string('a', 10000));
                await Task.Delay(TimeSpan.FromSeconds(1)); // wait for flush
                logger.LogDebug("tako");
                await Task.Delay(TimeSpan.FromSeconds(1)); // wait for flush

                File.Exists(path).Should().BeTrue();

                loggerFactory.Dispose();

                using (var fs = OpenFile(path1))
                {
                    fs.ReadLine().Should().Be("foo");
                    fs.ReadLine().Should().Be("bar");
                    fs.ReadLine().Should().Be("baz");
                    fs.ReadLine().Should().BeNull();
                }

                using (var fs = OpenFile(path2))
                {
                    fs.ReadLine().Should().Be("a");
                    fs.ReadLine().Should().Be("v");
                    fs.ReadLine().Should().Be("c");
                }
            }
        }
    }
}
