using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Xunit;

namespace ZLogger.MessagePack.Tests;

public class ScopeTest
{
    TestProcessor processor;
    ILogger logger;

    public ScopeTest()
    {
        processor = new TestProcessor(new MessagePackZLoggerFormatter());

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.IncludeScopes = true; 
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");
    }

    [Fact]
    public void BeginScope_FormattedLogValuesToMessagePack()
    {
        using (logger.BeginScope("({X}, {Y})", 111, null))
        {
            var a = 333;
            var b = 444;
            logger.ZLogInformation($"FooBar{a} NanoNano{b}");
        }

        var msgpack = processor.Dequeue();
        Assert.Equal("FooBar333 NanoNano444", (string)msgpack["Message"]);
        Assert.Equal(111, (int)msgpack["X"]);
        Assert.Null((string?)msgpack["Y"]);

    }

    [Fact]
    public void BeginScope_KeyValuePair()
    {
        using (logger.BeginScope(new KeyValuePair<string, object?>("Hoge", "AAA")))
        {
            var a = 100;
            var b = 200;
            logger.ZLogInformation($"FooBar{a} NanoNano{b}");
        }

        var msgpack = processor.Dequeue();
        Assert.Equal("FooBar100 NanoNano200", (string)msgpack["Message"]);
        Assert.Equal("AAA", (string)msgpack["Hoge"]);
    }

    [Fact]
    public void BeginScope_AnyScopeValue()
    {
        using (logger.BeginScope(new TestPayload { X = 999 }))
        {
            var a = 100;
            var b = 200;
            logger.ZLogInformation($"FooBar{a} NanoNano{b}");
        }

        var msgpack = processor.Dequeue();
        Assert.Equal("FooBar100 NanoNano200", (string)msgpack["Message"]);
        Assert.Equal(999, (int)msgpack["Scope"]["x"]);
    }

    [Fact]
    public void BeginScope_Nested()
    {
        using (logger.BeginScope("A={A}", 100))
        {
            logger.ZLogInformation($"Message 1");

            using (logger.BeginScope("B={B}", 200))
            {
                logger.ZLogInformation($"Message 2");
            }
        }

        var msgpack1 = processor.Dequeue();
        var msgpack2 = processor.Dequeue();
        Assert.Equal("Message 1", (string)msgpack1["Message"]);
        Assert.Equal(100, (int)msgpack1["A"]);
        Assert.False((bool)msgpack1.ContainsKey("B"));

        Assert.Equal("Message 2", (string)msgpack2["Message"]);
        Assert.Equal(100, (int)msgpack2["A"]);
        Assert.Equal(200, (int)msgpack2["B"]);
    }
}