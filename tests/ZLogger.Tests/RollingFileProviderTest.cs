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
    public void SharedFileStreamDisposesFileStream()
    {
        var path = Path.Join(directory, "shared-dispose-0.log");
        var stream = new SharedFileStream(path);

        stream.Write(Encoding.UTF8.GetBytes("foo"), 0, 3);
        stream.Dispose();

        File.Delete(path);
        File.Exists(path).Should().BeFalse();
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
        logText.Should().Contain("first logger message");
        logText.Should().Contain("second logger message");

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
}
