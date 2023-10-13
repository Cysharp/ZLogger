using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Utf8StringInterpolation;
using Xunit;
using ZLogger.Formatters;

namespace ZLogger.Tests
{
    public class MessageTest
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
        public void StructuredLoggingOptions()
        {
            using var ms = new MemoryStream();

            var sourceCodeHash = Guid.NewGuid().ToString();


            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerStream(ms, option =>
                {
                    var hashProp = JsonEncodedText.Encode("Hash");

                    option.UseJsonFormatter(formatter =>
                    {
                        formatter.MetadataFormatter = (writer, info) =>
                        {
                            // Use default and add custom metadata
                            SystemTextJsonZLoggerFormatter.DefaultMetadataFormatter(writer, info);
                            writer.WriteString(hashProp, sourceCodeHash);
                        };
                    });
                });
            });
            var logger = loggerFactory.CreateLogger("test");

            var tako = 100;
            var yaki = "あいうえお";
            logger.ZLogDebug($"FooBar{tako} NanoNano{yaki}");
            
            logger.LogInformation("");

            loggerFactory.Dispose();

            using var sr = new StreamReader(new MemoryStream(ms.ToArray()), Encoding.UTF8);
            var json = sr.ReadLine();

            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("Message").GetString().Should().Be("FooBar100 NanoNanoあいうえお");
            doc.GetProperty("tako").GetInt32().Should().Be(100);
            doc.GetProperty("yaki").GetString().Should().Be("あいうえお");

            doc.GetProperty("Hash").GetString().Should().Be(sourceCodeHash);
            doc.GetProperty("LogLevel").GetString().Should().Be("Debug");
        }

    }
}
