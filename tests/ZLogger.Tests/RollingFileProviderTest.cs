using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using ZLogger.Providers;

namespace ZLogger.Tests;

public class RollingFileProviderTest
{
    // Unique per test instance, so that test runs for different target frameworks, which run in parallel, do not share it.
    readonly string directory = Path.Join(Path.GetTempPath(), "zlogger-test", Guid.NewGuid().ToString("N"));
    
    public RollingFileProviderTest()
    {
        try
        {
            Directory.Delete(directory, true);
        }
        catch (FileNotFoundException) { }
        catch (DirectoryNotFoundException) { }
        
        Directory.CreateDirectory(directory);
    }

    [Fact]
    public async Task RollByInterval()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2000, 1, 2, 3, 4, 5, TimeSpan.Zero));
                
        var path1= Path.Join(directory, $"ZLoggerRollingTest_{timeProvider.GetUtcNow():yyyy-MM-dd}-0.log");
        if (File.Exists(path1)) File.Delete(path1);

        using var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerRollingFile(options =>
            {
                options.FilePathSelector = (dt, seq) => Path.Join(directory, $"ZLoggerRollingTest_{dt:yyyy-MM-dd}-{seq}.log");
                options.RollingInterval = RollingInterval.Day;
                options.RollingSizeKB = 5;
                options.TimeProvider = timeProvider;
            });
        });
        
        Assert.True(File.Exists(path1));
        
        var logger = loggerFactory.CreateLogger("mytest");
        logger.LogDebug("foo");
        logger.LogDebug("bar");
        logger.LogDebug("baz");
   
        await Task.Delay(100, TestContext.Current.CancellationToken); // wait for flush
        Assert.True(File.Exists(path1));
        
        // Next day
        timeProvider.Advance(TimeSpan.FromDays(1));
        var path2 = Path.Join(directory, $"ZLoggerRollingTest_{timeProvider.GetUtcNow():yyyy-MM-dd}-0.log");
        logger.LogDebug("a");
        logger.LogDebug("v");
        logger.LogDebug("c");

        await Task.Delay(100, TestContext.Current.CancellationToken); // wait for flush
        Assert.True(File.Exists(path2));
        

        loggerFactory.Dispose();

        using (var fs = OpenFile(path1))
        {
            Assert.Equal("foo", fs.ReadLine());
            Assert.Equal("bar", fs.ReadLine());
            Assert.Equal("baz", fs.ReadLine());
            Assert.Null(fs.ReadLine());
        }

        using (var fs = OpenFile(path2))
        {
            Assert.Equal("a", fs.ReadLine());
            Assert.Equal("v", fs.ReadLine());
            Assert.Equal("c", fs.ReadLine());
        }        
    }
    
    [Fact]
    public async Task RollBySize()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2000, 1, 2, 3, 4, 5, TimeSpan.Zero));
                
        var path1 = Path.Join(directory, $"ZLoggerRollingTest_{timeProvider.GetUtcNow():yyyy-MM-dd}-0.log");
        var path2 = Path.Join(directory, $"ZLoggerRollingTest_{timeProvider.GetUtcNow():yyyy-MM-dd}-1.log");

        using var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerRollingFile(options =>
            {
                options.FilePathSelector = (dt, seq) => Path.Join(directory, $"ZLoggerRollingTest_{dt:yyyy-MM-dd}-{seq}.log");
                options.RollingInterval = RollingInterval.Day;
                options.RollingSizeKB = 5;
                options.TimeProvider = timeProvider;
            });
        });
        
        Assert.True(File.Exists(path1));
        
        var logger = loggerFactory.CreateLogger("mytest");
        logger.LogDebug(new string('a', 10000));
        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken); // wait for flush
        logger.LogDebug("tako");
        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken); // wait for flush

        Assert.True(File.Exists(path2));
    }

    [Fact]
    public async Task RollingIntervalUsesLocalTimeForCheckpoint()
    {
        var timeZone = TimeZoneInfo.CreateCustomTimeZone("UTC+09", TimeSpan.FromHours(9), "UTC+09", "UTC+09");
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 14, 59, 0, TimeSpan.Zero)); // 23:59 local
        timeProvider.SetLocalTimeZone(timeZone);

        using var loggerFactory = CreateLocalDateFactory("local-time", timeProvider, timeZone);
        var logger = loggerFactory.CreateLogger("local-time");
        logger.LogDebug("before local midnight");
        await Task.Delay(100); // wait for flush

        // 00:01 local, still 2000-01-01 in UTC
        timeProvider.Advance(TimeSpan.FromMinutes(2));
        logger.LogDebug("after local midnight");
        await Task.Delay(100); // wait for flush

        loggerFactory.Dispose();

        Assert.Equal(new[] { "before local midnight" }, ReadLines("local-time-2000-01-01-0.log"));
        Assert.Equal(new[] { "after local midnight" }, ReadLines("local-time-2000-01-02-0.log"));
    }

    [Fact]
    public async Task RollingIntervalAdvancesSequenceWhenSelectorReturnsCurrentFileName()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 14, 59, 0, TimeSpan.Zero)); // 23:59 local
        timeProvider.SetLocalTimeZone(TimeZoneInfo.CreateCustomTimeZone("UTC+09", TimeSpan.FromHours(9), "UTC+09", "UTC+09"));

        using var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerRollingFile(options =>
            {
                // UTC date, which does not change at local midnight
                options.FilePathSelector = (timestamp, sequence) => Path.Join(directory, $"utc-name-{timestamp:yyyy-MM-dd}-{sequence}.log");
                options.RollingInterval = RollingInterval.Day;
                options.RollingSizeKB = 5;
                options.TimeProvider = timeProvider;
            });
        });
        var logger = loggerFactory.CreateLogger("utc-name");
        logger.LogDebug("before local midnight");
        await Task.Delay(100); // wait for flush

        timeProvider.Advance(TimeSpan.FromMinutes(2));
        logger.LogDebug("after local midnight");
        await Task.Delay(100); // wait for flush

        loggerFactory.Dispose();

        Assert.Equal(new[] { "before local midnight" }, ReadLines("utc-name-2000-01-01-0.log"));
        Assert.Equal(new[] { "after local midnight" }, ReadLines("utc-name-2000-01-01-1.log"));
    }

    [Fact]
    public async Task RollingIntervalRollsAtLocalMidnightWhenDaylightSavingTimeStarts()
    {
        var timeZone = CreateCentralEuropeanTimeZone();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 3, 28, 23, 30, 0, TimeSpan.Zero)); // 2026-03-29 00:30 CET
        timeProvider.SetLocalTimeZone(timeZone);

        using var loggerFactory = CreateLocalDateFactory("dst-start", timeProvider, timeZone);
        var logger = loggerFactory.CreateLogger("dst-start");
        logger.LogDebug("a");
        await Task.Delay(100); // wait for flush

        // 2026-03-30 00:30 CEST, DST started at 02:00 on 2026-03-29
        timeProvider.SetUtcNow(new DateTimeOffset(2026, 3, 29, 22, 30, 0, TimeSpan.Zero));
        logger.LogDebug("b");
        await Task.Delay(100); // wait for flush

        loggerFactory.Dispose();

        Assert.Equal(new[] { "a" }, ReadLines("dst-start-2026-03-29-0.log"));
        Assert.Equal(new[] { "b" }, ReadLines("dst-start-2026-03-30-0.log"));
    }

    [Fact]
    public async Task RollingIntervalRollsAtLocalMidnightWhenDaylightSavingTimeEnds()
    {
        var timeZone = CreateCentralEuropeanTimeZone();
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 24, 22, 30, 0, TimeSpan.Zero)); // 2026-10-25 00:30 CEST
        timeProvider.SetLocalTimeZone(timeZone);

        using var loggerFactory = CreateLocalDateFactory("dst-end", timeProvider, timeZone);
        var logger = loggerFactory.CreateLogger("dst-end");
        logger.LogDebug("a");
        await Task.Delay(100); // wait for flush

        // 2026-10-25 23:30 CET, DST ended at 03:00 on 2026-10-25
        timeProvider.SetUtcNow(new DateTimeOffset(2026, 10, 25, 22, 30, 0, TimeSpan.Zero));
        logger.LogDebug("b");
        await Task.Delay(100); // wait for flush

        // 2026-10-26 00:30 CET
        timeProvider.Advance(TimeSpan.FromHours(1));
        logger.LogDebug("c");
        await Task.Delay(100); // wait for flush

        loggerFactory.Dispose();

        Assert.Equal(new[] { "a", "b" }, ReadLines("dst-end-2026-10-25-0.log"));
        Assert.False(File.Exists(Path.Join(directory, "dst-end-2026-10-25-1.log")));
        Assert.Equal(new[] { "c" }, ReadLines("dst-end-2026-10-26-0.log"));
    }

    [Fact]
    public void RollingIntervalUsesSameInstantForCheckpointAndFileName()
    {
        var timeProvider = new SteppingTimeProvider(new DateTimeOffset(2000, 1, 1, 9, 30, 0, TimeSpan.Zero));
        var stream = new RollingFileStream(
            (timestamp, sequence) => Path.Join(directory, $"same-instant-{timestamp:yyyy-MM-dd-HH}-{sequence}.log"),
            RollingInterval.Hour, 5, timeProvider, false);
        stream.Write(Encoding.UTF8.GetBytes("a"), 0, 1);

        // A second clock read during this write would see the next hour
        timeProvider.SetUtcNow(new DateTimeOffset(2000, 1, 1, 9, 59, 59, TimeSpan.Zero), new DateTimeOffset(2000, 1, 1, 10, 0, 0, TimeSpan.Zero));
        stream.Write(Encoding.UTF8.GetBytes("b"), 0, 1);
        stream.Write(Encoding.UTF8.GetBytes("c"), 0, 1);
        stream.Dispose();

        File.ReadAllText(Path.Join(directory, "same-instant-2000-01-01-09-0.log")).Should().Be("ab");
        File.ReadAllText(Path.Join(directory, "same-instant-2000-01-01-10-0.log")).Should().Be("c");
        File.Exists(Path.Join(directory, "same-instant-2000-01-01-09-1.log")).Should().BeFalse();
    }

    [Fact]
    public void SharedFileStreamDisposesFileStream()
    {
        var path = Path.Join(directory, "shared-dispose-0.log");
        var stream = new SharedFileStream(path);

        stream.Write(Encoding.UTF8.GetBytes("foo"), 0, 3);
        stream.Dispose();

        File.Delete(path);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void SharedRollingFileCanBeWrittenByMultipleLoggerFactories()
    {
        var path = Path.Join(directory, "shared-factory-0.log");

        using (var firstFactory = CreateSharedFactory())
        {
            firstFactory.CreateLogger("first").LogInformation("first logger message");
        }

        using (var secondFactory = CreateSharedFactory())
        {
            secondFactory.CreateLogger("second").LogInformation("second logger message");
        }

        var logText = File.ReadAllText(path);
        Assert.Contains("first logger message", logText, StringComparison.Ordinal);
        Assert.Contains("second logger message", logText, StringComparison.Ordinal);

        ILoggerFactory CreateSharedFactory()
        {
            return LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Information);
                x.AddZLoggerRollingFile(options =>
                {
                    options.FilePathSelector = (_, sequence) => Path.Join(directory, $"shared-factory-{sequence}.log");
                    options.RollingInterval = RollingInterval.Day;
                    options.RollingSizeKB = 5;
                    options.FileShared = true;
                });
            });
        }
    }

    static StreamReader OpenFile(string path)
    {
        return new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8);
    }

    string[] ReadLines(string fileName)
    {
        return File.ReadAllLines(Path.Join(directory, fileName));
    }

    ILoggerFactory CreateLocalDateFactory(string prefix, TimeProvider timeProvider, TimeZoneInfo timeZone)
    {
        return LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerRollingFile(options =>
            {
                options.FilePathSelector = (timestamp, sequence) =>
                    Path.Join(directory, $"{prefix}-{TimeZoneInfo.ConvertTime(timestamp, timeZone):yyyy-MM-dd}-{sequence}.log");
                options.RollingInterval = RollingInterval.Day;
                options.RollingSizeKB = 5;
                options.TimeProvider = timeProvider;
            });
        });
    }

    // CET/CEST: DST from the last Sunday of March 02:00 to the last Sunday of October 03:00
    static TimeZoneInfo CreateCentralEuropeanTimeZone()
    {
        var rule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
            DateTime.MinValue.Date,
            DateTime.MaxValue.Date,
            TimeSpan.FromHours(1),
            TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 5, DayOfWeek.Sunday),
            TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 3, 0, 0), 10, 5, DayOfWeek.Sunday));
        return TimeZoneInfo.CreateCustomTimeZone("CET-Test", TimeSpan.FromHours(1), "CET-Test", "CET", "CEST", [rule]);
    }

    // Returns the given time once, then the next time for every later read
    private class SteppingTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        private DateTimeOffset utcNow = utcNow;
        private DateTimeOffset? nextUtcNow;

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

        public void SetUtcNow(DateTimeOffset utcNow, DateTimeOffset nextUtcNow)
        {
            this.utcNow = utcNow;
            this.nextUtcNow = nextUtcNow;
        }

        public override DateTimeOffset GetUtcNow()
        {
            var result = utcNow;
            if (nextUtcNow is { } next)
            {
                utcNow = next;
                nextUtcNow = null;
            }
            return result;
        }
    }
}
