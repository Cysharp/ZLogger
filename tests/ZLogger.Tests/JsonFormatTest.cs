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


            var loggerFactory = LoggerFactory.Create(x => x
                .SetMinimumLevel(LogLevel.Debug)
                .AddZLoggerStream(ms, options =>
                {
                    var hashProp = JsonEncodedText.Encode("Hash");

                    options.UseJsonFormatter(formatter =>
                    {
                        formatter.AdditionalFormatter = (Utf8JsonWriter writer, in LogInfo _) =>
                        {
                            writer.WriteString(hashProp, sourceCodeHash);
                        };
                    });
                }));

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
        public void FormatException_Custom()
        {
            const string exceptionProperty = "ex";

            using var ms = new MemoryStream();

            var loggerFactory = LoggerFactory.Create(x => x
                .SetMinimumLevel(LogLevel.Debug)
                .AddZLoggerStream(ms, options =>
                {
                    var hashProp = JsonEncodedText.Encode("Hash");

                    options.UseJsonFormatter(formatter =>
                    {
                        formatter.ExceptionFormatter = (Utf8JsonWriter writer, Exception exception) =>
                        {
                            writer.WriteString(exceptionProperty, exception.ToString());
                        };
                    });
                }));
            var logger = loggerFactory.CreateLogger("test");

            var exception = new Exception("ZLogger test exception");
            logger.LogDebug(exception, exception.Message);

            loggerFactory.Dispose();

            using var sr = new StreamReader(new MemoryStream(ms.ToArray()), Encoding.UTF8);
            var json = sr.ReadLine();

            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty(exceptionProperty).GetString().Should().Be(exception.ToString());
        }

        [Fact]
        public void FormatPropertyKeyValues_Custom()
        {
            using var ms = new MemoryStream();

            var loggerFactory = LoggerFactory.Create(x => x
                .SetMinimumLevel(LogLevel.Debug)
                .AddZLoggerStream(ms, options =>
                {
                    var hashProp = JsonEncodedText.Encode("Hash");

                    options.UseJsonFormatter(formatter =>
                    {
                        formatter.PropertyKeyValuesFormatter = (Utf8JsonWriter writer, IZLoggerEntry entry) =>
                        {
                            for (var i = 0; i < entry.ParameterCount; i++)
                            {
                                if (entry.IsSupportUtf8ParameterKey)
                                {
                                    var key = entry.GetParameterKey(i);
                                    writer.WritePropertyName(key);
                                }
                                else
                                {
                                    var key = entry.GetParameterKeyAsString(i);
                                    writer.WritePropertyName(key);
                                }

                                var value = entry.GetParameterValue(i);
                                writer.WriteStringValue(value?.ToString());
                            }
                        };
                    });
                }));
            var logger = loggerFactory.CreateLogger("test");

            var fooValue = new StringWrapper("foo");
            var barValue = new StringWrapper("bar");
            logger.LogDebug("{foo}{bar}", fooValue, barValue);

            loggerFactory.Dispose();

            using var sr = new StreamReader(new MemoryStream(ms.ToArray()), Encoding.UTF8);
            var json = sr.ReadLine();

            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("foo").GetString().Should().Be(fooValue.Value);
            doc.GetProperty("bar").GetString().Should().Be(barValue.Value);
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
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var now = DateTime.UtcNow;
            logger.ZLogInformation(new EventId(1, "TEST"), $"HELLO!");

            var json = processor.DequeueAsString();
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
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogInformation(new EventId(1, "TEST"), $"HELLO!");

            var json = processor.DequeueAsString();
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
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var Tako = 100;
            var Yaki = "あいうえお";
            logger.ZLogDebug($"tako: {Tako} yaki: {Yaki}");

            var json = processor.DequeueAsString();
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
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var zzz = new { Tako = 100, Yaki = "あいうえお" };

            logger.ZLogDebug($"tako: {zzz.Tako} yaki: {zzz.Yaki}");

            var json = processor.DequeueAsString();
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
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var zzz = new { tako = 100, yaki = "あいうえお" };

            logger.ZLogDebug($"tako: {zzz.tako} yaki: {zzz.yaki}");

            var json = processor.DequeueAsString();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("Message").GetString().Should().Be("tako: 100 yaki: あいうえお");
            doc.GetProperty("Tako").GetInt32().Should().Be(100);
            doc.GetProperty("Yaki").GetString().Should().Be("あいうえお");
        }

        [Fact]
        public void NestPayload()
        {
            var options = new ZLoggerOptions
            {
                IncludeScopes = true
            };
            options.UseJsonFormatter(formatter =>
            {
                formatter.KeyNameMutator = KeyNameMutator.LastMemberName;
                formatter.PropertyKeyValuesObjectName = JsonEncodedText.Encode("Payload");
            });

            var processor = new TestProcessor(options);

            using var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var zzz = new { Tako = 100, Yaki = "あいうえお" };

            logger.ZLogDebug($"tako: {zzz.Tako} yaki: {zzz.Yaki}");

            var json = processor.DequeueAsString();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("Message").GetString().Should().Be("tako: 100 yaki: あいうえお");
            doc.GetProperty("Payload").GetProperty("Tako").GetInt32().Should().Be(100);
            doc.GetProperty("Payload").GetProperty("Yaki").GetString().Should().Be("あいうえお");
        }

        [Fact]
        public void ConfigureName()
        {
            var options = new ZLoggerOptions().UseJsonFormatter(formatter =>
            {
                formatter.JsonPropertyNames = JsonPropertyNames.Default with
                {
                    Message = JsonEncodedText.Encode("MMMMMMMMMMMOsage"),
                    LogLevel = JsonEncodedText.Encode("LeVeL5"),
                    Category = JsonEncodedText.Encode("CAT"),
                    Timestamp = JsonEncodedText.Encode("TST"),
                    EventId = JsonEncodedText.Encode("EventIddd"),
                    EventIdName = JsonEncodedText.Encode("EventNAME"),
                };
                formatter.IncludeProperties = IncludeProperties.All;
            });

            var processor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogInformation(new EventId(1, "TEST"), $"HELLO!");

            var json = processor.DequeueAsString();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.TryGetProperty("MMMMMMMMMMMOsage", out _).Should().BeTrue();
            doc.TryGetProperty("LeVeL5", out _).Should().BeTrue();
            doc.TryGetProperty("TST", out _).Should().BeTrue();
            doc.TryGetProperty("EventIddd", out _).Should().BeTrue();
            doc.TryGetProperty("EventNAME", out _).Should().BeTrue();
            doc.TryGetProperty("CAT", out _).Should().BeTrue();
        }


        [Fact]
        public void KeyNameMutator_CallMethod()
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
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var Tako = 100;
            var Yaki = "あいうえお";
            var anon = new { Tako, Yaki };
            var mc = new MyClass();
            logger.ZLogDebug($"tako: {mc.Call(10, anon.Tako)} yaki: {(((mc.Call2(1, 2))))}");

            var json = processor.DequeueAsString();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("Call").GetInt32().Should().Be(110);
            doc.GetProperty("Call2").GetInt32().Should().Be(3);
        }

        [Fact]
        public void FormatLogEntry_DontEscapeCharacters()
        {
            using var ms = new MemoryStream();

            var loggerFactory = LoggerFactory.Create(x => x
                .SetMinimumLevel(LogLevel.Debug)
                .AddZLoggerStream(ms, options =>
                {
                    options.UseJsonFormatter();
                }));

            var logger = loggerFactory.CreateLogger("test");

            using var scope = logger.BeginScope("{tako}", "たこ");

            var yaki = "やき";
            logger.ZLogDebug($"{yaki} <>+");

            loggerFactory.Dispose();

            using var sr = new StreamReader(new MemoryStream(ms.ToArray()), Encoding.UTF8);
            var json = sr.ReadLine();

            json.Should().NotContainAny("\\u");
        }

        class MyClass
        {
            public int Call(int x, int y) => x + y;
            public int Call2(int x, int y) => x + y;
        }

        class StringWrapper(string value)
        {
            public string Value => value;

            public override string ToString() => value;
        }
    }
}
