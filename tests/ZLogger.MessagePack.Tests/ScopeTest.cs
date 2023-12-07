using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using FluentAssertions;
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
        ((string)msgpack["Message"]).Should().Be("FooBar333 NanoNano444");
        ((int)msgpack["X"]).Should().Be(111);
        ((string?)msgpack["Y"]).Should().Be(null);

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
        ((string)msgpack["Message"]).Should().Be("FooBar100 NanoNano200");
        ((string)msgpack["Hoge"]).Should().Be("AAA");
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
        ((string)msgpack["Message"]).Should().Be("FooBar100 NanoNano200");
        ((int)msgpack["Scope"]["x"]).Should().Be(999);
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
        ((string)msgpack1["Message"]).Should().Be("Message 1");
        ((int)msgpack1["A"]).Should().Be(100);
        ((bool)msgpack1.ContainsKey("B")).Should().BeFalse();

        ((string)msgpack2["Message"]).Should().Be("Message 2");
        ((int)msgpack2["A"]).Should().Be(100);
        ((int)msgpack2["B"]).Should().Be(200);
    }
}