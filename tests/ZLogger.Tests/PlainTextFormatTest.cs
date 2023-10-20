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
                plainText.ExceptionFormatter = (writer, ex) => { };
            });

            var processsor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processsor);
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogDebug($"foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebug(new EventId(10), $"foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebug(new Exception(), $"foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebug(new EventId(10), new Exception(), $"foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            var bar = "bar";
            logger.ZLogDebug($"foo {bar}");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebug(new EventId(10), $"foo {bar}");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebug(new Exception(), $"foo {bar}");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebug(new EventId(10), new Exception(), $"foo {bar}");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, $"foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, new EventId(10), $"foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, new Exception(), $"foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, new EventId(10), new Exception(), $"foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, $"foo {bar}");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, new EventId(10), $"foo {bar}");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, new Exception(), $"foo {bar}");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, new EventId(10), new Exception(), $"foo {bar}");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");
        }

        [Fact]
        public void TextOptions()
        {
            var options = new ZLoggerOptions();
            options.UsePlainTextFormatter(formatter =>
            {
                formatter.PrefixFormatter = (writer, info) => Utf8String.Format(writer, $"[Pre:{info.LogLevel}]");
                formatter.SuffixFormatter = (writer, info) => Utf8String.Format(writer, $"[Suf:{info.CategoryName}]");
                formatter.ExceptionFormatter = (writer, ex) => Utf8String.Format(writer, $"{ex.Message}");
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
            processsor.Dequeue().Should().Be("[Pre:Debug]FooBar100-NanoNano200[Suf:test]");

            logger.ZLogInformation($"FooBar{x}-NanoNano{y}");
            processsor.Dequeue().Should().Be("[Pre:Information]FooBar100-NanoNano200[Suf:test]");

            // fallback case
            logger.LogDebug("FooBar{0}-NanoNano{1}", 100, 200);
            processsor.Dequeue().Should().Be("[Pre:Debug]FooBar100-NanoNano200[Suf:test]");

            logger.LogInformation("FooBar{0}-NanoNano{1}", 100, 300);
            processsor.Dequeue().Should().Be("[Pre:Information]FooBar100-NanoNano300[Suf:test]");
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

            var message = processor.Dequeue();
            message.Should().Be("array: 111, 222, 333 enumerable: a, i, u, e");
        }
    }
}
