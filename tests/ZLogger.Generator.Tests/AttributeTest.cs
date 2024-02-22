#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLogger.Generator.Tests
{
    public partial class AttributeTest
    {
#pragma warning disable xUnit1013

        [ZLoggerMessage(Message = "Hello {x} {y}")]
        public partial void ZeroCtor(ILogger logger, LogLevel level, int x, int y);

        [ZLoggerMessage("Hello {x} {y}")]
        public partial void Msg(ILogger logger, LogLevel level, int x, int y);

        [ZLoggerMessage(LogLevel.Debug, Message = "Hello {x} {y}")]
        public partial void Level(ILogger logger, int x, int y);

        [ZLoggerMessage(11, LogLevel.Warning, "Hello {x} {y}")]
        public partial void Three(ILogger logger, int x, int y);

        [ZLoggerMessage(LogLevel.Trace, "Hello {x} {y}", SkipEnabledCheck = true)]
        public partial void Skip(ILogger logger, int x, int y);

        [ZLoggerMessage(LogLevel.Information, "Hello {x} {y}")]
        public partial void Err(ILogger logger, Exception exception, int x, int y);

        [ZLoggerMessage(LogLevel.Error, "Hello {str} {x}")]
        public partial void ErrNullable(ILogger logger, string? str, int? x, Exception? exception = null);

        [ZLoggerMessage(EventId = 12, EventName = "EventName", Level = LogLevel.Warning, Message = "Hello {x} {y}")]
        public partial void EventName(ILogger logger, int x, int y);

#pragma warning restore xUnit1013

        [Fact]
        public void Log()
        {
            using var _ = TestHelper.CreateLogger<AttributeTest>(LogLevel.Debug, options => options.UsePlainTextFormatter(formatter =>
            {
                formatter.SetPrefixFormatter($"{0}:", (in MessageTemplate template, in LogInfo info) => template.Format(info.LogLevel));
            }), out var logger, out var list);

            ZeroCtor(logger, LogLevel.Debug, 10, 20);
            list[0].Should().Be("Debug:Hello 10 20");

            ZeroCtor(logger, LogLevel.Warning, 10, 20);
            list[1].Should().Be("Warning:Hello 10 20");

            Msg(logger, LogLevel.Critical, 10, 20);
            list[2].Should().Be("Critical:Hello 10 20");

            Level(logger, 30, 20);
            list[3].Should().Be("Debug:Hello 30 20");

            Three(logger, 30, 20);
            list[4].Should().Be("Warning:Hello 30 20");

            EventName(logger, 40, 50);
            list[5].Should().Be("Warning:Hello 40 50");
        }

        [Fact]
        public void Error()
        {
            using var _ = TestHelper.CreateLogger<AttributeTest>(LogLevel.Debug, options => options.UsePlainTextFormatter(formatter =>
            {
                formatter.SetPrefixFormatter($"{0}:", (in MessageTemplate template, in LogInfo info) => template.Format(info.Exception?.Message));
                formatter.SetExceptionFormatter((_, _) => { });
            }), out var logger, out var list);

            Err(logger, new Exception("MY EXCEPTION!"), 10, 20);
            list[0].Should().Be("MY EXCEPTION!:Hello 10 20");
        }
    }

}
