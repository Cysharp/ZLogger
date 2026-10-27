using Microsoft.Extensions.Logging;
using System;
using Utf8StringInterpolation;
using System.Linq;

namespace ZLogger.Tests
{
    public class PlainTextFormatTest
    {
        [Fact]
        public void OverloadCheck()
        {
            var options = new ZLoggerOptions();
            options.UsePlainTextFormatter(plainText =>
            {
                plainText.SetExceptionFormatter((writer, ex) => { });
            });

            var processor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogDebug($"foo");
            Assert.Equal("foo", processor.DequeueAsString());

            logger.ZLogDebug(new EventId(10), $"foo");
            Assert.Equal("foo", processor.DequeueAsString());

            logger.ZLogDebug(new Exception(), $"foo");
            Assert.Equal("foo", processor.DequeueAsString());

            logger.ZLogDebug(new EventId(10), new Exception(), $"foo");
            Assert.Equal("foo", processor.DequeueAsString());

            var bar = "bar";
            logger.ZLogDebug($"foo {bar}");
            Assert.Equal("foo bar", processor.DequeueAsString());

            logger.ZLogDebug(new EventId(10), $"foo {bar}");
            Assert.Equal("foo bar", processor.DequeueAsString());

            logger.ZLogDebug(new Exception(), $"foo {bar}");
            Assert.Equal("foo bar", processor.DequeueAsString());

            logger.ZLogDebug(new EventId(10), new Exception(), $"foo {bar}");
            Assert.Equal("foo bar", processor.DequeueAsString());

            logger.ZLog(LogLevel.Debug, $"foo");
            Assert.Equal("foo", processor.DequeueAsString());

            logger.ZLog(LogLevel.Debug, new EventId(10), $"foo");
            Assert.Equal("foo", processor.DequeueAsString());

            logger.ZLog(LogLevel.Debug, new Exception(), $"foo");
            Assert.Equal("foo", processor.DequeueAsString());

            logger.ZLog(LogLevel.Debug, new EventId(10), new Exception(), $"foo");
            Assert.Equal("foo", processor.DequeueAsString());

            logger.ZLog(LogLevel.Debug, $"foo {bar}");
            Assert.Equal("foo bar", processor.DequeueAsString());

            logger.ZLog(LogLevel.Debug, new EventId(10), $"foo {bar}");
            Assert.Equal("foo bar", processor.DequeueAsString());

            logger.ZLog(LogLevel.Debug, new Exception(), $"foo {bar}");
            Assert.Equal("foo bar", processor.DequeueAsString());

            logger.ZLog(LogLevel.Debug, new EventId(10), new Exception(), $"foo {bar}");
            Assert.Equal("foo bar", processor.DequeueAsString());
        }

        [Fact]
        public void TextOptions()
        {
            var options = new ZLoggerOptions();
            options.UsePlainTextFormatter(formatter =>
            {
                formatter.SetPrefixFormatter($"[Pre:{0}]", (in MessageTemplate formatter, in LogInfo info) => formatter.Format(info.LogLevel));
                formatter.SetSuffixFormatter($"[Suf:{0}]", (in MessageTemplate formatter, in LogInfo info) => formatter.Format(info.Category));
                formatter.SetExceptionFormatter((writer, ex) => Utf8String.Format(writer, $"{ex.Message}"));
            });
            var processsor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processsor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var x = 100;
            var y = 200;
            logger.ZLogDebug($"FooBar{x}-NanoNano{y}");
            Assert.Equal("[Pre:Debug]FooBar100-NanoNano200[Suf:test]", processsor.DequeueAsString());

            logger.ZLogInformation($"FooBar{x}-NanoNano{y}");
            Assert.Equal("[Pre:Information]FooBar100-NanoNano200[Suf:test]", processsor.DequeueAsString());

            // fallback case
            logger.LogDebug("FooBar{0}-NanoNano{1}", 100, 200);
            Assert.Equal("[Pre:Debug]FooBar100-NanoNano200[Suf:test]", processsor.DequeueAsString());

            logger.LogInformation("FooBar{0}-NanoNano{1}", 100, 300);
            Assert.Equal("[Pre:Information]FooBar100-NanoNano300[Suf:test]", processsor.DequeueAsString());
        }

        [Fact]
        public void CollectionDestructuring()
        {
            var options = new ZLoggerOptions();
            options.UsePlainTextFormatter();
            var processor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var array = new[] { 111, 222, 333 };
            var enumerable = new[] { "a", "i", "u", "e" }.Where(_ => true);

            logger.ZLogDebug($"array: {array} enumerable: {enumerable}");

            var message = processor.DequeueAsString();
            Assert.Equal("array: [111,222,333] enumerable: [\"a\",\"i\",\"u\",\"e\"]", message);
        }
    }
}
