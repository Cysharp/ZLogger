using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using ZLogger.Providers;

namespace ZLogger.Tests;

public class RollingFileProviderTest
{
    readonly string directory = Path.Join(Path.GetTempPath(), "zlogger-test"); 
    
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
        
        File.Exists(path1).Should().Be(true);
        
        var logger = loggerFactory.CreateLogger("mytest");
        logger.LogDebug("foo");
        logger.LogDebug("bar");
        logger.LogDebug("baz");
   
        await Task.Delay(100); // wait for flush
        File.Exists(path1).Should().BeTrue();
        
        // Next day
        timeProvider.Advance(TimeSpan.FromDays(1));
        var path2 = Path.Join(directory, $"ZLoggerRollingTest_{timeProvider.GetUtcNow():yyyy-MM-dd}-0.log");
        logger.LogDebug("a");
        logger.LogDebug("v");
        logger.LogDebug("c");

        await Task.Delay(100); // wait for flush
        File.Exists(path2).Should().BeTrue();
        

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
        
        File.Exists(path1).Should().Be(true);
        
        var logger = loggerFactory.CreateLogger("mytest");
        logger.LogDebug(new string('a', 10000));
        await Task.Delay(TimeSpan.FromSeconds(1)); // wait for flush
        logger.LogDebug("tako");
        await Task.Delay(TimeSpan.FromSeconds(1)); // wait for flush

        File.Exists(path2).Should().BeTrue();
    }

    [Fact]
    public void RollingIntervalUsesLocalTimeForCheckpoint()
    {
        var localDirectory = Path.Join(Path.GetTempPath(), $"zlogger-local-time-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(localDirectory);

        var timeProvider = new FixedTimeProvider(
            new DateTimeOffset(2000, 1, 1, 14, 59, 0, TimeSpan.Zero),
            TimeZoneInfo.CreateCustomTimeZone("UTC+09", TimeSpan.FromHours(9), "UTC+09", "UTC+09"));
        var path1 = Path.Join(localDirectory, "local-time-2000-01-01-0.log");
        var path2 = Path.Join(localDirectory, "local-time-2000-01-01-1.log");

        try
        {
            using (var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerRollingFile(options =>
                {
                    options.FilePathSelector = (timestamp, sequence) =>
                        Path.Join(localDirectory, $"local-time-{timestamp:yyyy-MM-dd}-{sequence}.log");
                    options.RollingInterval = RollingInterval.Day;
                    options.RollingSizeKB = 5;
                    options.TimeProvider = timeProvider;
                });
            }))
            {
                var logger = loggerFactory.CreateLogger("local-time");
                logger.LogDebug("before local midnight");

                timeProvider.Advance(TimeSpan.FromMinutes(2));
                logger.LogDebug("after local midnight");
            }

            File.Exists(path1).Should().BeTrue();
            File.Exists(path2).Should().BeTrue();
        }
        finally
        {
            Directory.Delete(localDirectory, true);
        }
    }
    
    static StreamReader OpenFile(string path)
    {
        return new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.UTF8);
    }

    sealed class FixedTimeProvider(DateTimeOffset initialUtcNow, TimeZoneInfo localTimeZone) : TimeProvider
    {
        DateTimeOffset utcNow = initialUtcNow;

        public override DateTimeOffset GetUtcNow() => utcNow;

        public override TimeZoneInfo LocalTimeZone { get; } = localTimeZone;

        public void Advance(TimeSpan amount) => utcNow += amount;
    }
}
