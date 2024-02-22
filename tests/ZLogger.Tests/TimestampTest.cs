using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLogger.Formatters;

namespace ZLogger.Tests;

public class TimestampTest
{
    class FakeTime : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return new DateTimeOffset(1999, 12, 30, 11, 12, 33, TimeSpan.Zero);
        }

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    string GetLogString(MessageTemplateHandler prefixTemplate)
    {
        LogProcessor processor = new();
        var factory = LoggerFactory.Create(builder =>
        {
            builder.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = new FakeTime();
                options.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter(prefixTemplate, (in MessageTemplate template, in LogInfo info) => template.Format(info.Timestamp));
                });
                processor.SetOptions(options);

                return processor;
            });
        });

        var logger = factory.CreateLogger("foo");
        logger.ZLogInformation($"");

        factory.Dispose();
        return processor.WrittenData;
    }

    [Fact]
    public void TimestampFormatTest()
    {
        GetLogString($"{0}").Should().Be("1999-12-30 11:12:33.000");
        GetLogString($"{0:datetime}").Should().Be("1999-12-30 11:12:33");
        GetLogString($"{0:dateonly}").Should().Be("1999-12-30");
        GetLogString($"{0:timeonly}").Should().Be("11:12:33");
    }
}

file class LogProcessor : IAsyncLogProcessor
{
    public string WrittenData = null;
    IZLoggerFormatter formatter;

    public void SetOptions(ZLoggerOptions options)
    {
        formatter = options.CreateFormatter();
    }

    public ValueTask DisposeAsync()
    {


        return default;
    }

    public void Post(IZLoggerEntry log)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        log.FormatUtf8(bufferWriter, formatter);
        WrittenData = Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        log.Return();
    }
}