using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using ZLogger.Formatters;

namespace ZLogger.Tests
{
    public class JsonFormatTest
    {
        [Fact]
        public void FormatLogEntry_CustomMetadata()
        {
            using var ms = new MemoryStream();

            var sourceCodeHash = Guid.NewGuid().ToString();


            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLogger(zLogger =>
                {
                    zLogger.AddStream(ms, options =>
                    {
                        var hashProp = JsonEncodedText.Encode("Hash");

                        options.UseJsonFormatter(formatter =>
                        {
                            formatter.LogInfoFormatter = (writer, info) =>
                            {
                                // Use default and add custom metadata
                                formatter.DefaultLogInfoFormatter(writer, info);
                                writer.WriteString(hashProp, sourceCodeHash);
                            };
                        });
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

        [Fact]
        public void FormatLogEntry_ExcludeLogInfoProperties()
        {
            var options = new ZLoggerOptions().UseJsonFormatter(formatter =>
            {
                formatter.IncludeProperties = IncludeProperties.LogLevel |
                                              IncludeProperties.Timestamp |
                                              IncludeProperties.EventIdValue;
            });

            var processor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLogger(zLogger => zLogger.AddLogProcessor(processor));
            });
            var logger = loggerFactory.CreateLogger("test");

            var now = DateTime.UtcNow;
            logger.ZLogInformation(new EventId(1, "TEST"), $"HELLO!");

            var json = processor.Dequeue();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.TryGetProperty("Message", out _).Should().BeFalse();
            doc.GetProperty("LogLevel").GetString().Should().Be("Information");
            doc.GetProperty("Timestamp").GetDateTime().Should().BeOnOrAfter(now);
            doc.GetProperty("EventId").GetInt32().Should().Be(1);
            doc.TryGetProperty("EventIdName", out _).Should().BeFalse();
            doc.TryGetProperty("CategoryName", out _).Should().BeFalse();
        }

        [Fact]
        public void FormatLogEntry_ExcludeAllLogInfo()
        {
            var options = new ZLoggerOptions().UseJsonFormatter(formatter =>
            {
                formatter.IncludeProperties = IncludeProperties.None;
            });

            var processor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLogger(zLogger => zLogger.AddLogProcessor(processor));
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogInformation(new EventId(1, "TEST"), $"HELLO!");

            var json = processor.Dequeue();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.TryGetProperty("Message", out _).Should().BeFalse();
            doc.TryGetProperty("LogLevel", out _).Should().BeFalse();
            doc.TryGetProperty("Timestamp", out _).Should().BeFalse();
            doc.TryGetProperty("EventId", out _).Should().BeFalse();
            doc.TryGetProperty("EventIdName", out _).Should().BeFalse();
            doc.TryGetProperty("EventIdName", out _).Should().BeFalse();
            doc.TryGetProperty("CategoryName", out _).Should().BeFalse();
        }

        [Fact]
        public void KeyNameMutator_Lower()
        {
            var options = new ZLoggerOptions
            {
                IncludeScopes = true
            };
            options.UseJsonFormatter(formatter =>
            {
                formatter.KeyNameMutator = KeyNameMutator.LowerFirstCharacter;
            });

            var processor = new TestProcessor(options);

            using var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLogger(zLogger => zLogger.AddLogProcessor(processor));
            });
            var logger = loggerFactory.CreateLogger("test");

            var Tako = 100;
            var Yaki = "あいうえお";
            logger.ZLogDebug($"tako: {Tako} yaki: {Yaki}");

            var json = processor.Dequeue();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("Message").GetString().Should().Be("tako: 100 yaki: あいうえお");
            doc.GetProperty("tako").GetInt32().Should().Be(100);
            doc.GetProperty("yaki").GetString().Should().Be("あいうえお");
        }

        [Fact]
        public void KeyNameMutatorProp1()
        {
            var options = new ZLoggerOptions
            {
                IncludeScopes = true
            };
            options.UseJsonFormatter(formatter =>
            {
                formatter.KeyNameMutator = KeyNameMutator.LastMemberName;
            });

            var processor = new TestProcessor(options);

            using var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLogger(zLogger => zLogger.AddLogProcessor(processor));
            });
            var logger = loggerFactory.CreateLogger("test");

            var zzz = new { Tako = 100, Yaki = "あいうえお" };

            logger.ZLogDebug($"tako: {zzz.Tako} yaki: {zzz.Yaki}");

            var json = processor.Dequeue();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("Message").GetString().Should().Be("tako: 100 yaki: あいうえお");
            doc.GetProperty("Tako").GetInt32().Should().Be(100);
            doc.GetProperty("Yaki").GetString().Should().Be("あいうえお");
        }

        [Fact]
        public void KeyNameMutatorProp2()
        {
            var options = new ZLoggerOptions
            {
                IncludeScopes = true
            };
            options.UseJsonFormatter(formatter =>
            {
                formatter.KeyNameMutator = KeyNameMutator.LastMemberNameUpperFirstCharacter;
            });

            var processor = new TestProcessor(options);

            using var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLogger(zLogger => zLogger.AddLogProcessor(processor));
            });
            var logger = loggerFactory.CreateLogger("test");

            var zzz = new { tako = 100, yaki = "あいうえお" };

            logger.ZLogDebug($"tako: {zzz.tako} yaki: {zzz.yaki}");

            var json = processor.Dequeue();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("Message").GetString().Should().Be("tako: 100 yaki: あいうえお");
            doc.GetProperty("Tako").GetInt32().Should().Be(100);
            doc.GetProperty("Yaki").GetString().Should().Be("あいうえお");
        }
    }
}
