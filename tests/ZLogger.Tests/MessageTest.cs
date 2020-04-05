using Cysharp.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ZLogger.Tests
{
    public class TestProcessor : IAsyncLogProcessor
    {
        public Queue<string> EntryMessages = new Queue<string>();
        readonly ZLoggerOptions options;

        public string Dequeue() => EntryMessages.Dequeue();

        public TestProcessor(ZLoggerOptions options)
        {
            this.options = options;
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public void Post(IZLoggerEntry log)
        {
            EntryMessages.Enqueue(log.FormatToString(options, null));
        }
    }


    public class MessageTest
    {
        [Fact]
        public void OverloadCheck()
        {
            var options = new ZLoggerOptions()
            {
                ExceptionFormatter = (writer, ex) => { }
            };

            var processsor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processsor);
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogDebug("foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebug(new EventId(10), "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebug(new Exception(), "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebug(new EventId(10), new Exception(), "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebug("foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebug(new EventId(10), "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebug(new Exception(), "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebug(new EventId(10), new Exception(), "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebugWithPayload(new { Foo = 999 }, "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebugWithPayload(new EventId(10), new { Foo = 999 }, "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebugWithPayload(new Exception(), new { Foo = 999 }, "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebugWithPayload(new EventId(10), new Exception(), new { Foo = 999 }, "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogDebugWithPayload(new { Foo = 999 }, "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebugWithPayload(new EventId(10), new { Foo = 999 }, "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebugWithPayload(new Exception(), new { Foo = 999 }, "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogDebugWithPayload(new EventId(10), new Exception(), new { Foo = 999 }, "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, new EventId(10), "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, new Exception(), "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, new EventId(10), new Exception(), "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLog(LogLevel.Debug, "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, new EventId(10), "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, new Exception(), "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLog(LogLevel.Debug, new EventId(10), new Exception(), "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogWithPayload(LogLevel.Debug, new { Foo = 999 }, "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogWithPayload(LogLevel.Debug, new EventId(10), new { Foo = 999 }, "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogWithPayload(LogLevel.Debug, new Exception(), new { Foo = 999 }, "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogWithPayload(LogLevel.Debug, new EventId(10), new Exception(), new { Foo = 999 }, "foo");
            processsor.EntryMessages.Dequeue().Should().Be("foo");

            logger.ZLogWithPayload(LogLevel.Debug, new { Foo = 999 }, "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogWithPayload(LogLevel.Debug, new EventId(10), new { Foo = 999 }, "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogWithPayload(LogLevel.Debug, new Exception(), new { Foo = 999 }, "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");

            logger.ZLogWithPayload(LogLevel.Debug, new EventId(10), new Exception(), new { Foo = 999 }, "foo {0}", "bar");
            processsor.EntryMessages.Dequeue().Should().Be("foo bar");
        }

        [Fact]
        public void TextOptions()
        {
            var options = new ZLoggerOptions()
            {
                PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "[Pre:{0}]", info.LogLevel),
                SuffixFormatter = (writer, info) => ZString.Utf8Format(writer, "[Suf:{0}]", info.CategoryName),
                ExceptionFormatter = (writer, ex) => ZString.Utf8Format(writer, "{0}", ex.Message)
            };
            var processsor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processsor);
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogDebug("FooBar{0}-NanoNano{1}", 100, 200);
            processsor.Dequeue().Should().Be("[Pre:Debug]FooBar100-NanoNano200[Suf:test]");

            logger.ZLogInformation("FooBar{0}-NanoNano{1}", 100, 300);
            processsor.Dequeue().Should().Be("[Pre:Information]FooBar100-NanoNano300[Suf:test]");

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
                    option.IsStructuredLogging = true;
                    option.StructuredLoggingFormatter = (writer, info) =>
                    {
                        // Use default and add custom metadata
                        info.WriteToJsonWriter(writer);
                        writer.WriteString("Hash", sourceCodeHash);
                    };
                });
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogDebugWithPayload(new { tako = 100, yaki = "‚ ‚¢‚¤‚¦‚¨" }, "FooBar{0} NanoNano{1}", 100, 200);

            loggerFactory.Dispose();

            using var sr = new StreamReader(new MemoryStream(ms.ToArray()), Encoding.UTF8);
            var json = sr.ReadLine();

            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("Message").GetString().Should().Be("FooBar100 NanoNano200");
            var payload = doc.GetProperty("Payload");
            payload.GetProperty("tako").GetInt32().Should().Be(100);
            payload.GetProperty("yaki").GetString().Should().Be("‚ ‚¢‚¤‚¦‚¨");

            doc.GetProperty("Hash").GetString().Should().Be(sourceCodeHash);
            doc.GetProperty("LogLevel").GetString().Should().Be("Debug");
        }

    }
}
