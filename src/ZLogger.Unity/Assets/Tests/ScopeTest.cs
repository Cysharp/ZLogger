using Xunit;
using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ZLogger;

namespace Tests;

public class ScopeTest
{
    TestProcessor processor;
    ILogger logger;

    public ScopeTest()
    {
        var options = new ZLoggerOptions
        {
            IncludeScopes = true
        };
        options.UseJsonFormatter();
            
        processor = new TestProcessor(options);

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
    public void BeginScope_FormattedLogValuesToJson()
    {
        using (logger.BeginScope("({X}, {Y})", 111, null))
        {
            var x = 333;
            var y = 444;
            logger.ZLogInformation($"FooBar{x} NanoNano{y}");
        }

        var doc = JsonDocument.Parse(processor.Dequeue()).RootElement;
        doc.GetProperty("Message").GetString().Should().Be("FooBar333 NanoNano444");
        doc.GetProperty("X").GetInt32().Should().Be(111);
        doc.GetProperty("Y").ValueKind.Should().Be(JsonValueKind.Null);
    }
        
    [Fact]
    public void BeginScope_KeyValuePairToJson()
    {
        using (logger.BeginScope(new KeyValuePair<string, object>("Hoge", "AAA")))
        {
            var x = 100;
            var y = 200;
            logger.ZLogInformation($"FooBar{x} NanoNano{y}");
        }

        var doc = JsonDocument.Parse(processor.Dequeue()).RootElement;

        doc.GetProperty("Message").GetString().Should().Be("FooBar100 NanoNano200");
        doc.GetProperty("Hoge").GetString().Should().Be("AAA");
    }
        
    [Fact]
    public void BeginScope_AnyScopeValueToJson()
    {
        using (logger.BeginScope(new TestState { X = 999 }))
        {
            var x = 100;
            var y = 200;
            logger.ZLogInformation($"FooBar{x} NanoNano{y}");
        }

        var json = processor.Dequeue();
        var doc = JsonDocument.Parse(json).RootElement;
        doc.GetProperty("Message").GetString().Should().Be("FooBar100 NanoNano200");
            
        var scope = doc.GetProperty("Scope");
        scope.GetProperty("X").GetInt32().Should().Be(999);
    }
        
    [Fact]
    public void BeginScope_NestedToJson()
    {
        using (logger.BeginScope("X={X}", 111))
        {
            logger.ZLogInformation($"Message 1");

            using (logger.BeginScope("Y={Y}", 222))
            {
                logger.ZLogInformation($"Message 2");
            }
        }

        var log1 = JsonDocument.Parse(processor.Dequeue()).RootElement;
        var log2 = JsonDocument.Parse(processor.Dequeue()).RootElement;

        log1.GetProperty("Message").GetString().Should().Be("Message 1");
        log1.GetProperty("X").GetInt32().Should().Be(111);
        log1.TryGetProperty("Y", out _).Should().BeFalse();

        log2.GetProperty("Message").GetString().Should().Be("Message 2");
        log2.GetProperty("X").GetInt32().Should().Be(111);
        log2.GetProperty("Y").GetInt32().Should().Be(222);
    }
}
