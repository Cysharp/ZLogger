using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Time.Testing;
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

            Assert.Equal("FooBar100 NanoNanoあいうえお", doc.GetProperty("Message").GetString());
            Assert.Equal(100, doc.GetProperty("tako").GetInt32());
            Assert.Equal("あいうえお", doc.GetProperty("yaki").GetString());

            Assert.Equal(sourceCodeHash, doc.GetProperty("Hash").GetString());
            Assert.Equal("Debug", doc.GetProperty("LogLevel").GetString());
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

            var now = new DateTimeOffset(2026, 10, 23, 1, 2, 3, 456, TimeSpan.Zero);
            var timeProvider = new FakeTimeProvider(now);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                // TimeProvider takes effect on the provider's options, not on the ones the processor formats with.
                x.AddZLoggerLogProcessor(providerOptions =>
                {
                    providerOptions.TimeProvider = timeProvider;
                    return processor;
                });
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogInformation(new EventId(1, "TEST"), $"HELLO!");

            var json = processor.DequeueAsString();
            var doc = JsonDocument.Parse(json).RootElement;

            Assert.False(doc.TryGetProperty("Message", out _));
            Assert.Equal("Information", doc.GetProperty("LogLevel").GetString());
            Assert.Equal(now, doc.GetProperty("Timestamp").GetDateTimeOffset());
            Assert.Equal(1, doc.GetProperty("EventId").GetInt32());
            Assert.False(doc.TryGetProperty("EventIdName", out _));
            Assert.False(doc.TryGetProperty("Category", out _));
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

            Assert.False(doc.TryGetProperty("Message", out _));
            Assert.False(doc.TryGetProperty("LogLevel", out _));
            Assert.False(doc.TryGetProperty("Timestamp", out _));
            Assert.False(doc.TryGetProperty("EventId", out _));
            Assert.False(doc.TryGetProperty("EventIdName", out _));
            Assert.False(doc.TryGetProperty("EventIdName", out _));
            Assert.False(doc.TryGetProperty("Category", out _));
        }

        /// <summary>
        /// Ensures that a key name mutator that needs more room than the key name gets it: an empty key name does not
        /// loop forever (the buffer has a minimum size), and a long key name grows the buffer past the stack threshold
        /// into a pooled array.
        /// </summary>
        [Fact(Timeout = 10000)]
        public async Task KeyNameMutatorThatNeedsMoreRoom()
        {
            var options = new ZLoggerOptions
            {
                IncludeScopes = true
            };
            options.UseJsonFormatter(formatter =>
            {
                formatter.KeyNameMutator = new PrefixMutator();
            });

            var processor = new TestProcessor(options);

            using var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                // IncludeScopes takes effect on the provider's options, not on the ones the processor formats with.
                x.AddZLoggerLogProcessor(providerOptions =>
                {
                    providerOptions.IncludeScopes = true;
                    return processor;
                });
            });
            var logger = loggerFactory.CreateLogger("test");

            var longKey = new string('k', 200);
            using (logger.BeginScope(new[] { new KeyValuePair<string, object>("", 1), new KeyValuePair<string, object>(longKey, 2) }))
            {
                logger.ZLogDebug($"AAA");
            }

            var json = await Task.Run(() => processor.DequeueAsString(), TestContext.Current.CancellationToken);
            var doc = JsonDocument.Parse(json).RootElement;

            Assert.Equal(1, doc.GetProperty("p_").GetInt32());
            Assert.Equal(2, doc.GetProperty("p_" + longKey).GetInt32());
        }

        /// <summary>
        /// Ensures that a key name mutator that never succeeds makes formatting fail with an InvalidOperationException naming the
        /// mutator, after a bounded number of retries, instead of growing the buffer until it runs out of memory.
        /// </summary>
        [Fact(Timeout = 10000)]
        public async Task KeyNameMutatorThatNeverSucceedsThrows()
        {
            var options = new ZLoggerOptions();
            options.UseJsonFormatter(formatter =>
            {
                formatter.KeyNameMutator = new FailingMutator();
            });

            var processor = new TestProcessor(options);

            using var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var x = 1;
            logger.ZLogDebug($"{x}");

            var format = () => Task.Run(() => processor.DequeueAsString(), TestContext.Current.CancellationToken);
            var thrown = await Assert.ThrowsAsync<InvalidOperationException>(format);
            Assert.Contains(nameof(FailingMutator), thrown.Message, StringComparison.Ordinal);
        }

        sealed class FailingMutator : IKeyNameMutator
        {
            public bool IsSupportSlice => false;

            public ReadOnlySpan<char> Slice(ReadOnlySpan<char> source) => throw new NotSupportedException();

            public bool TryMutate(ReadOnlySpan<char> source, Span<char> destination, out int written)
            {
                written = 0;
                return false;
            }
        }

        // Prepends "p_", so the destination must hold at least two characters more than the source.
        sealed class PrefixMutator : IKeyNameMutator
        {
            public bool IsSupportSlice => false;

            public ReadOnlySpan<char> Slice(ReadOnlySpan<char> source) => throw new NotSupportedException();

            public bool TryMutate(ReadOnlySpan<char> source, Span<char> destination, out int written)
            {
                if (destination.Length < source.Length + 2)
                {
                    written = 0;
                    return false;
                }
                "p_".AsSpan().CopyTo(destination);
                source.CopyTo(destination[2..]);
                written = source.Length + 2;
                return true;
            }
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

            Assert.Equal("tako: 100 yaki: あいうえお", doc.GetProperty("Message").GetString());
            Assert.Equal(100, doc.GetProperty("tako").GetInt32());
            Assert.Equal("あいうえお", doc.GetProperty("yaki").GetString());
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

            Assert.Equal("tako: 100 yaki: あいうえお", doc.GetProperty("Message").GetString());
            Assert.Equal(100, doc.GetProperty("Tako").GetInt32());
            Assert.Equal("あいうえお", doc.GetProperty("Yaki").GetString());
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

            Assert.Equal("tako: 100 yaki: あいうえお", doc.GetProperty("Message").GetString());
            Assert.Equal(100, doc.GetProperty("Tako").GetInt32());
            Assert.Equal("あいうえお", doc.GetProperty("Yaki").GetString());
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

            Assert.Equal("tako: 100 yaki: あいうえお", doc.GetProperty("Message").GetString());
            Assert.Equal(100, doc.GetProperty("Payload").GetProperty("Tako").GetInt32());
            Assert.Equal("あいうえお", doc.GetProperty("Payload").GetProperty("Yaki").GetString());
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

            Assert.True(doc.TryGetProperty("MMMMMMMMMMMOsage", out _));
            Assert.True(doc.TryGetProperty("LeVeL5", out _));
            Assert.True(doc.TryGetProperty("TST", out _));
            Assert.True(doc.TryGetProperty("EventIddd", out _));
            Assert.True(doc.TryGetProperty("EventNAME", out _));
            Assert.True(doc.TryGetProperty("CAT", out _));
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

            Assert.Equal(110, doc.GetProperty("Call").GetInt32());
            Assert.Equal(3, doc.GetProperty("Call2").GetInt32());
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

            Assert.DoesNotContain("\\u", json, StringComparison.Ordinal);
        }

        class MyClass
        {
            public int Call(int x, int y) => x + y;
            public int Call2(int x, int y) => x + y;
        }

    }
}
