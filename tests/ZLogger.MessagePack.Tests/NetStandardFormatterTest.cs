using System;
using FluentAssertions;
using MessagePack;
using Microsoft.Extensions.Logging;
using Xunit;
using ZLogger.MessagePack.Tests.NetStandardTypes;

namespace ZLogger.MessagePack.Tests;

// Values whose formatters come from a library compiled against a netstandard build of MessagePack. On net9.0 and later those
// formatters serve only the Compatible (non-ref-struct) buffers, so the formatter has to let MessagePack reroute them.
public class NetStandardFormatterTest
{
    static ILogger CreateLogger(TestProcessor processor)
    {
        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.IncludeScopes = true;
                return processor;
            });
        });
        return loggerFactory.CreateLogger("test");
    }

    // A map of one entry: the key as fixstr, then the value as MessagePack itself serializes it with the given options.
    static string ExpectedSingleEntry<T>(string key, T value, MessagePackSerializerOptions options)
    {
        return "81" + Convert.ToHexString([(byte)(0xa0 | key.Length)]) + Convert.ToHexString(System.Text.Encoding.UTF8.GetBytes(key)) +
            Convert.ToHexString(MessagePackSerializer.Serialize(value, options));
    }

    /// <summary>
    /// Ensures that a parameter of a [MessagePackObject] type declared in a netstandard-only library is written as MessagePack
    /// serializes it, instead of the entry being lost to the exception that the ref struct buffers raise for such a type.
    /// </summary>
    [Fact]
    public void ParameterOfNetStandardTypeIsWritten()
    {
        var formatter = new MessagePackZLoggerFormatter { IncludeProperties = IncludeProperties.ParameterKeyValues };
        var processor = new TestProcessor(formatter);
        var logger = CreateLogger(processor);

        var payload = new NetStandardPayload { X = 42 };
        logger.ZLogInformation($"{payload}");

        Convert.ToHexString(processor.DequeueAsRaw()).Should().Be(ExpectedSingleEntry("payload", payload, formatter.MessagePackSerializerOptions));
    }

    /// <summary>
    /// Ensures that a scope value of a [MessagePackObject] type declared in a netstandard-only library is written as
    /// MessagePack serializes it. Scope values are serialized through their runtime type, a different path from parameters.
    /// </summary>
    [Fact]
    public void ScopeValueOfNetStandardTypeIsWritten()
    {
        var formatter = new MessagePackZLoggerFormatter { IncludeProperties = IncludeProperties.ScopeKeyValues };
        var processor = new TestProcessor(formatter);
        var logger = CreateLogger(processor);

        var payload = new NetStandardPayload { X = 42 };
        using (logger.BeginScope("{Payload}", payload))
        {
            logger.ZLogInformation($"message");
        }

        Convert.ToHexString(processor.DequeueAsRaw()).Should().Be(ExpectedSingleEntry("Payload", payload, formatter.MessagePackSerializerOptions));
    }

    /// <summary>
    /// Ensures that a formatter from a netstandard-only library, put in the options for a type that the formatter serializes
    /// through the options without boxing (DateTimeOffset), is used for the parameter instead of making the entry fail.
    /// </summary>
    [Fact]
    public void FormatterInOptionsFromNetStandardLibraryIsUsed()
    {
        var options = new MessagePackSerializerOptions(new MessagePackFormatterResolver(
            MessagePackFormatterFactory.Combine(new UtcTicksDateTimeOffsetFormatterFactory(), MessagePackFormatterFactory.Default)));
        var formatter = new MessagePackZLoggerFormatter
        {
            IncludeProperties = IncludeProperties.ParameterKeyValues,
            MessagePackSerializerOptions = options,
        };
        var processor = new TestProcessor(formatter);
        var logger = CreateLogger(processor);

        var dto = new DateTimeOffset(2026, 10, 26, 1, 2, 3, TimeSpan.FromHours(9));
        logger.ZLogInformation($"{dto}");

        var expected = ExpectedSingleEntry("dto", dto, options);
        Convert.ToHexString(processor.DequeueAsRaw()).Should().Be(expected);

        // The custom formatter writes the UTC ticks as an integer (uint64, the smallest form for this positive value),
        // not the array that the built-in formatter writes.
        expected.Should().EndWith("CF" + dto.UtcTicks.ToString("X16"));
    }
}
