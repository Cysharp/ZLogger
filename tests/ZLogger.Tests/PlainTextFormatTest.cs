using FluentAssertions;
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
            processor.DequeueAsString().Should().Be("foo");

            logger.ZLogDebug(new EventId(10), $"foo");
            processor.DequeueAsString().Should().Be("foo");

            logger.ZLogDebug(new Exception(), $"foo");
            processor.DequeueAsString().Should().Be("foo");

            logger.ZLogDebug(new EventId(10), new Exception(), $"foo");
            processor.DequeueAsString().Should().Be("foo");

            var bar = "bar";
            logger.ZLogDebug($"foo {bar}");
            processor.DequeueAsString().Should().Be("foo bar");

            logger.ZLogDebug(new EventId(10), $"foo {bar}");
            processor.DequeueAsString().Should().Be("foo bar");

            logger.ZLogDebug(new Exception(), $"foo {bar}");
            processor.DequeueAsString().Should().Be("foo bar");

            logger.ZLogDebug(new EventId(10), new Exception(), $"foo {bar}");
            processor.DequeueAsString().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, $"foo");
            processor.DequeueAsString().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, new EventId(10), $"foo");
            processor.DequeueAsString().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, new Exception(), $"foo");
            processor.DequeueAsString().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, new EventId(10), new Exception(), $"foo");
            processor.DequeueAsString().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, $"foo {bar}");
            processor.DequeueAsString().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, new EventId(10), $"foo {bar}");
            processor.DequeueAsString().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, new Exception(), $"foo {bar}");
            processor.DequeueAsString().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, new EventId(10), new Exception(), $"foo {bar}");
            processor.DequeueAsString().Should().Be("foo bar");
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
            processsor.DequeueAsString().Should().Be("[Pre:Debug]FooBar100-NanoNano200[Suf:test]");

            logger.ZLogInformation($"FooBar{x}-NanoNano{y}");
            processsor.DequeueAsString().Should().Be("[Pre:Information]FooBar100-NanoNano200[Suf:test]");

            // fallback case
            logger.LogDebug("FooBar{0}-NanoNano{1}", 100, 200);
            processsor.DequeueAsString().Should().Be("[Pre:Debug]FooBar100-NanoNano200[Suf:test]");

            logger.LogInformation("FooBar{0}-NanoNano{1}", 100, 300);
            processsor.DequeueAsString().Should().Be("[Pre:Information]FooBar100-NanoNano300[Suf:test]");
        }

        [Fact]
        public void GetUtf8StringWriter()
        {
            var options = new ZLoggerOptions();
            options.UsePlainTextFormatter(formatter =>
            {
                formatter.SetPrefixFormatter($"", (in MessageTemplate template, in LogInfo info) =>
                {
                    if (info.ScopeState is not { IsEmpty: false } scope) return;

                    using var writer = template.GetUtf8StringWriter();
                    foreach (var (key, value) in scope.Properties)
                    {
                        if (key == "{OriginalFormat}") continue;
                        writer.AppendLiteral("[");
                        writer.AppendFormatted(key);
                        writer.AppendLiteral("=");
                        writer.AppendFormatted(value);
                        writer.AppendLiteral("]");
                    }
                });
                formatter.SetSuffixFormatter($" ({0}", (in MessageTemplate template, in LogInfo info) =>
                {
                    template.Format(info.LogLevel);
                    var writer = template.GetUtf8StringWriter();
                    writer.AppendLiteral(")");
                    writer.Flush();
                });
            });
            var processor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(options =>
                {
                    options.IncludeScopes = true;
                    return processor;
                });
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogDebug($"no scope");
            Assert.Equal("no scope (Debug)", processor.DequeueAsString());

            using (logger.BeginScope("Type {Type}", "Foo"))
            using (logger.BeginScope("Property {Property} {Id}", "Bar", 17))
            {
                logger.ZLogInformation($"message");
            }
            Assert.Equal("[Type=Foo][Property=Bar][Id=17]message (Information)", processor.DequeueAsString());
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
            message.Should().Be("array: [111,222,333] enumerable: [\"a\",\"i\",\"u\",\"e\"]");
        }
    }
}
