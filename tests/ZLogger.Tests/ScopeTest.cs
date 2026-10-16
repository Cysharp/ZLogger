using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZLogger.Tests;

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

        var doc = JsonDocument.Parse(processor.DequeueAsString()).RootElement;
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

        var doc = JsonDocument.Parse(processor.DequeueAsString()).RootElement;

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

        var json = processor.DequeueAsString();
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

        var log1 = JsonDocument.Parse(processor.DequeueAsString()).RootElement;
        var log2 = JsonDocument.Parse(processor.DequeueAsString()).RootElement;

        log1.GetProperty("Message").GetString().Should().Be("Message 1");
        log1.GetProperty("X").GetInt32().Should().Be(111);
        log1.TryGetProperty("Y", out _).Should().BeFalse();

        log2.GetProperty("Message").GetString().Should().Be("Message 2");
        log2.GetProperty("X").GetInt32().Should().Be(111);
        log2.GetProperty("Y").GetInt32().Should().Be(222);
    }

    /// <summary>
    /// Verifies that all properties of an ICollection scope state (a dictionary) are written to JSON,
    /// even when their count exceeds the default capacity of the scope property list.
    /// </summary>
    [Fact]
    public void BeginScope_DictionaryToJson()
    {
        var scope = new Dictionary<string, object>
        {
            ["A"] = 1,
            ["B"] = 2,
            ["C"] = 3,
            ["D"] = 4,
            ["E"] = 5,
            ["F"] = 6,
        };

        using (logger.BeginScope(scope))
        {
            logger.ZLogInformation($"Message");
        }

        var doc = JsonDocument.Parse(processor.DequeueAsString()).RootElement;
        doc.GetProperty("Message").GetString().Should().Be("Message");
        foreach (var (key, value) in scope)
        {
            doc.GetProperty(key).GetInt32().Should().Be((int)value);
        }
    }

    /// <summary>
    /// Verifies that properties captured by a scope state are not carried over to a later log entry
    /// after the scope state has been returned to the pool and reused.
    /// </summary>
    [Fact]
    public void BeginScope_ReturnedStateDoesNotKeepPreviousProperties()
    {
        using (logger.BeginScope(new Dictionary<string, object> { ["A"] = 1, ["B"] = 2, ["C"] = 3, ["D"] = 4, ["E"] = 5 }))
        {
            logger.ZLogInformation($"Message 1");
        }

        var entry1 = processor.Entries.Peek();
        var log1 = JsonDocument.Parse(processor.DequeueAsString()).RootElement;
        entry1.Return();

        using (logger.BeginScope("X={X}", 111))
        {
            logger.ZLogInformation($"Message 2");
        }

        var log2 = JsonDocument.Parse(processor.DequeueAsString()).RootElement;

        log1.GetProperty("E").GetInt32().Should().Be(5);

        log2.GetProperty("Message").GetString().Should().Be("Message 2");
        log2.GetProperty("X").GetInt32().Should().Be(111);
        foreach (var key in new[] { "A", "B", "C", "D", "E" })
        {
            log2.TryGetProperty(key, out _).Should().BeFalse();
        }
    }

    [Fact]
    public void VersionMismatch()
    {
        var invalidScopeStateOwner = new InvalidScopeStateOwner();
        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.AddZLoggerLogProcessor(options =>
            {
                options.IncludeScopes = true;
                return invalidScopeStateOwner;
            });
        });
        logger = loggerFactory.CreateLogger("test");

        using (logger.BeginScope("X={X}", 123))
        {
            logger.LogInformation($"AAAAAAA");
        }
        using (logger.BeginScope("X={X}", 456))
        {
            logger.LogInformation($"BBBBBBBB");
        }

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = invalidScopeStateOwner.LogInfos[0].ScopeState!.Properties;
        });
    }

    class InvalidScopeStateOwner : IAsyncLogProcessor
    {
        public List<LogInfo> LogInfos { get; private set; } = [];

        public ValueTask DisposeAsync() => default;

        public void Post(IZLoggerEntry entry)
        {
            LogInfos.Add(entry.LogInfo);
            entry.Return();
        }
    }
}
