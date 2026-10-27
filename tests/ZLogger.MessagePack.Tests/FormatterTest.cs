using System;
using MessagePack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace ZLogger.MessagePack.Tests;

public class FormatterTest
{
    TestProcessor processor;
    ILogger logger;
    readonly FakeTimeProvider timeProvider = new();
    readonly DateTimeOffset now = DateTimeOffset.UtcNow;

    public FormatterTest()
    {
        timeProvider.SetUtcNow(now);
        
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            IncludeProperties = IncludeProperties.Default
        });

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");
    }

    [Fact]
    public void PlainMessage()
    {
        logger.ZLogInformation(new EventId(1, "HOGE"), $"AAA {111} BBB {"Hello"}");
            
        var msgpack = processor.Dequeue();
        Assert.Equal("test", (string)msgpack["Category"]);
        Assert.Equal("Information", (string)msgpack["LogLevel"]);
        Assert.Equal("AAA 111 BBB Hello", (string)msgpack["Message"]);
        Assert.Equal(now, (DateTimeOffset)msgpack["Timestamp"]);
        Assert.False((bool)msgpack.ContainsKey("Exception"));
        Assert.False((bool)msgpack.ContainsKey("Payload"));
    }

    [Fact]
    public void WithException()
    {
        try
        {
            throw new TestException("DAME");
        }
        catch (Exception ex)
        {
            logger.ZLogError(new EventId(1, "NG"), ex, $"DAMEDA 111");                
        }
        
        var msgpack = processor.Dequeue();
        Assert.Equal("test", (string)msgpack["Category"]);
        Assert.Equal("Error", (string)msgpack["LogLevel"]);
        Assert.Equal("DAMEDA 111", (string)msgpack["Message"]);
        Assert.Equal(now, (DateTimeOffset)msgpack["Timestamp"]);
        Assert.Equal("ZLogger.MessagePack.Tests.TestException", (string)msgpack["Exception"]["Name"]);
        Assert.Equal("DAME", (string)msgpack["Exception"]["Message"]);
        Assert.NotEmpty((string)msgpack["Exception"]["StackTrace"]);
        Assert.Null((string)msgpack["Exception"]["InnerException"]);
    }

    [Fact]
    public void WithExceptionWithInnerException()
    {
        try
        {
            throw new TestException("DAME!", new TestException("INNER!"));
        }
        catch (Exception ex)
        {
            logger.ZLogError(new EventId(1, "NG"), ex, $"DAMEDA 111");                
        }
        
        var msgpack = processor.Dequeue();
        Assert.Equal("test", (string)msgpack["Category"]);
        Assert.Equal("Error", (string)msgpack["LogLevel"]);
        Assert.Equal("DAMEDA 111", (string)msgpack["Message"]);
        Assert.Equal(now, (DateTimeOffset)msgpack["Timestamp"]);
        Assert.Equal("ZLogger.MessagePack.Tests.TestException", (string)msgpack["Exception"]["Name"]);
        Assert.Equal("DAME!", (string)msgpack["Exception"]["Message"]);
        Assert.NotEmpty((string)msgpack["Exception"]["StackTrace"]);
        Assert.Equal("ZLogger.MessagePack.Tests.TestException", (string)msgpack["Exception"]["InnerException"]["Name"]);
        Assert.Equal("INNER!", (string)msgpack["Exception"]["InnerException"]["Message"]);
    }
    
    [Fact]
    public void WithParameters()
    {
        var payload = new TestPayload { X = 999 };
        var x = 100;
        int? y = null;
        logger.ZLogInformation(new EventId(1, "HOGE"), $"UMU {payload} {x} {y}");
        
        var msgpack = processor.Dequeue();
        Assert.Equal("test", (string)msgpack["Category"]);
        Assert.Equal("Information", (string)msgpack["LogLevel"]);
        Assert.Equal(now, (DateTimeOffset)msgpack["Timestamp"]);
        Assert.Equal(100, (int?)msgpack["x"]);
        Assert.Null((int?)msgpack["y"]);
        Assert.Equal(999, (int)msgpack["payload"]["x"]);
        Assert.False((bool)msgpack.ContainsKey("Exception"));
    }

    [Fact]
    public void LowercaseMutator()
    {
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            KeyNameMutator = KeyNameMutator.LowerFirstCharacter
        });
            
        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");
            
        var XyzAbc = 100;
        var fOo = 200;
        logger.ZLogInformation($"AAA {XyzAbc} {fOo}");

        var msgpack = processor.Dequeue();
        Assert.Equal("test", (string)msgpack["Category"]);
        Assert.Equal("Information", (string)msgpack["LogLevel"]);
        Assert.Equal("AAA 100 200", (string)msgpack["Message"]);
        Assert.Equal(100, (int)msgpack["xyzAbc"]);
        Assert.Equal(200, (int)msgpack["fOo"]);
    }

    [Fact]
    public void ExcludeLogInfoProperties()
    {
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            IncludeProperties = IncludeProperties.LogLevel |
                                IncludeProperties.Timestamp |
                                IncludeProperties.EventIdValue
        });

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");
                
        logger.ZLogInformation(new EventId(1, "TEST"), $"HELLO!");
        
        var msgpack = processor.Dequeue();
        Assert.Equal("Information", (string)msgpack["LogLevel"]);
        Assert.Equal(1, (int)msgpack["EventId"]);
        Assert.Equal(now, (DateTimeOffset)msgpack["Timestamp"]);
        Assert.False((bool)msgpack.ContainsKey("Exception"));
        Assert.False((bool)msgpack.ContainsKey("Category"));
        Assert.False((bool)msgpack.ContainsKey("EventIdName"));
    }

    [Fact]
    public void ExcludeAllLogInfo()
    {
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            IncludeProperties = IncludeProperties.None
        });

        var loggerFactory = LoggerFactory.Create(x => x
            .SetMinimumLevel(LogLevel.Debug)
            .AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                return processor;
            }));
        logger = loggerFactory.CreateLogger("test");
                
        logger.ZLogInformation(new EventId(1, "TEST"), $"HELLO!");
        
        var msgpack = processor.Dequeue();
        Assert.False((bool)msgpack.ContainsKey("LogLevel"));
        Assert.False((bool)msgpack.ContainsKey("Timestamp"));
        Assert.False((bool)msgpack.ContainsKey("EventId"));
        Assert.False((bool)msgpack.ContainsKey("EventIdName"));
        Assert.False((bool)msgpack.ContainsKey("Exception"));
        Assert.False((bool)msgpack.ContainsKey("Category"));
        Assert.False((bool)msgpack.ContainsKey("EventIdName"));
    }
    
    [Fact]
    public void CustomPropertyNames()
    {
        var formatter = new MessagePackZLoggerFormatter
        {
            PropertyNames = MessagePackPropertyNames.Default with
            {
                Timestamp = MessagePackEncodedText.Encode("time"),
                LogLevel = MessagePackEncodedText.Encode("level"),
                LogLevelInformation = MessagePackEncodedText.Encode("INFO"),
            }
        };

        processor = new TestProcessor(formatter);

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");
                
        logger.ZLogInformation($"HELLO!");

        var msg = processor.Dequeue();
        Assert.Equal("INFO", (string)msg["level"]);
    }

    [Fact]
    public void NestedParameterKeyValues()
    {
        var formatter = new MessagePackZLoggerFormatter
        {
            PropertyNames = MessagePackPropertyNames.Default with
            {
                ParameterKeyValues = MessagePackEncodedText.Encode("attributes"),
                ScopeKeyValues = MessagePackEncodedText.Encode("scope")
            }
        };

        processor = new TestProcessor(formatter);

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");


        logger.ZLogInformation($"{456:@y}");

        var msg = processor.Dequeue();
        Assert.Equal("456", (string)msg["Message"]);
        Assert.Equal(456, (int)msg["attributes"]["y"]);
    }

    [Fact]
    public void NestedScopeKeyValues()
    {
        var formatter = new MessagePackZLoggerFormatter
        {
            PropertyNames = MessagePackPropertyNames.Default with
            {
                ParameterKeyValues = MessagePackEncodedText.Encode("attributes"),
                ScopeKeyValues = MessagePackEncodedText.Encode("scope")
            }
        };

        processor = new TestProcessor(formatter);

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.IncludeScopes = true;
                options.TimeProvider = timeProvider;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");

        using (logger.BeginScope("{X}", 123))
        {
            logger.ZLogInformation($"{456:@y}");
        }

        var msg = processor.Dequeue();
        Assert.Equal("456", (string)msg["Message"]);
        Assert.Equal(123, (int)msg["scope"]["X"]);
        Assert.Equal(456, (int)msg["attributes"]["y"]);
    }
    
    /// <summary>
    /// Ensures that serializer options with a MessageProcessor are rejected when they are set,
    /// because a whole-message processor (such as framing or compression) cannot apply to the values inside a log entry.
    /// FramingProcessor is used because it is built into MessagePack and needs no extra package.
    /// </summary>
    [Fact]
    public void MessageProcessorIsRejected()
    {
        var formatter = new MessagePackZLoggerFormatter();
        var setOptions = () => formatter.MessagePackSerializerOptions = MessagePackSerializerOptions.Default.WithFraming();
        Assert.Throws<ArgumentException>(setOptions);
    }

    /// <summary>
    /// Ensures that setting null serializer options throws ArgumentNullException instead of failing later.
    /// </summary>
    [Fact]
    public void NullOptionsAreRejected()
    {
        var formatter = new MessagePackZLoggerFormatter();
        var setOptions = () => formatter.MessagePackSerializerOptions = null!;
        Assert.Throws<ArgumentNullException>(setOptions);
    }

    /// <summary>
    /// Ensures that a key name mutator that needs more room than the key name gets it: an empty key name is mutated to "p_"
    /// (the buffer has a 16-character minimum; without it, the buffer would start and stay at zero characters, and the formatter
    /// would throw once the retry limit is reached), and a 200-character key name, whose initial buffer of
    /// 400 characters is past the 256-character stack threshold, is mutated correctly through a pooled array.
    /// </summary>
    [Fact(Timeout = 10000)]
    public async System.Threading.Tasks.Task KeyNameMutatorThatNeedsMoreRoom()
    {
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            IncludeProperties = IncludeProperties.ScopeKeyValues,
            KeyNameMutator = new PrefixMutator()
        });

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                options.IncludeScopes = true;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");

        var longKey = new string('k', 200);
        await System.Threading.Tasks.Task.Run(() =>
        {
            using (logger.BeginScope(new[]
            {
                new System.Collections.Generic.KeyValuePair<string, object?>("", 1),
                new System.Collections.Generic.KeyValuePair<string, object?>(longKey, 2),
            }))
            {
                logger.ZLogInformation($"AAA");
            }
        }, TestContext.Current.CancellationToken);

        var msgpack = processor.Dequeue();
        Assert.Equal(1, (int)msgpack["p_"]);
        Assert.Equal(2, (int)msgpack["p_" + longKey]);
    }

    /// <summary>
    /// Ensures that a key name mutator that never succeeds makes logging fail with an InvalidOperationException naming the mutator,
    /// the number of attempts and the last buffer length,
    /// after a bounded number of retries (1 + 10 calls, the last with a buffer of 1024 times the initial 16 characters),
    /// instead of growing the buffer until it runs out of memory, and that an entry logged afterwards is written intact.
    /// The mutator stops the test early if it is called more often, so that a regression fails fast instead of allocating gigabytes.
    /// </summary>
    [Fact(Timeout = 10000)]
    public async System.Threading.Tasks.Task KeyNameMutatorThatNeverSucceedsThrows()
    {
        var mutator = new FailingMutator();
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            IncludeProperties = IncludeProperties.Message | IncludeProperties.ParameterKeyValues,
            KeyNameMutator = mutator
        });

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");

        var x = 1;
        var failing = () => System.Threading.Tasks.Task.Run(() => logger.ZLogInformation($"{x}"), TestContext.Current.CancellationToken);
        var thrown = await Assert.ThrowsAsync<AggregateException>(failing);
        var inner = Assert.IsType<InvalidOperationException>(Assert.Single(thrown.InnerExceptions));
        Assert.Contains(nameof(FailingMutator), inner.Message, StringComparison.Ordinal);
        Assert.Contains("11 times", inner.Message, StringComparison.Ordinal);
        Assert.Contains("16384 characters", inner.Message, StringComparison.Ordinal);

        Assert.Equal(11, mutator.Calls);
        Assert.Equal(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, 11), i => 16 << i), mutator.DestinationLengths);
        Assert.Empty(processor.EntryMessages);

        // An entry without parameters does not call the mutator; it must not carry anything left from the failed one.
        logger.ZLogInformation($"TAIL");
        var msgpack = processor.Dequeue();
        Assert.Equal("TAIL", (string)msgpack["Message"]);
        Assert.Equal(1, (int)((System.Collections.IDictionary)msgpack).Count);
    }

    /// <summary>
    /// Ensures that a key name mutator that never succeeds receives buffers of exactly the requested lengths, even when they
    /// come from the pool, whose arrays may be larger, so that the exception reports the length the mutator actually received.
    /// A 150-character key name starts at 300 characters, which the pool rounds up to 512.
    /// </summary>
    [Fact(Timeout = 10000)]
    public async System.Threading.Tasks.Task KeyNameMutatorGetsExactlyTheRequestedBufferLength()
    {
        var mutator = new FailingMutator();
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            IncludeProperties = IncludeProperties.ScopeKeyValues,
            KeyNameMutator = mutator
        });

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                options.IncludeScopes = true;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");

        var longKey = new string('k', 150);
        var failing = () => System.Threading.Tasks.Task.Run(() =>
        {
            using (logger.BeginScope(new[] { new System.Collections.Generic.KeyValuePair<string, object?>(longKey, 1) }))
            {
                logger.ZLogInformation($"AAA");
            }
        }, TestContext.Current.CancellationToken);
        var thrown = await Assert.ThrowsAsync<AggregateException>(failing);
        var inner = Assert.IsType<InvalidOperationException>(Assert.Single(thrown.InnerExceptions));
        Assert.Contains($"{300 * 1024} characters", inner.Message, StringComparison.Ordinal);

        Assert.Equal(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, 11), i => 300 << i), mutator.DestinationLengths);
    }

    // Always returns false, and records how it was called. Throws once called far more often than the retry limit allows.
    sealed class FailingMutator : IKeyNameMutator
    {
        public int Calls;
        public readonly System.Collections.Generic.List<int> DestinationLengths = new();

        public bool IsSupportSlice => false;

        public ReadOnlySpan<char> Slice(ReadOnlySpan<char> source) => throw new NotSupportedException();

        public bool TryMutate(ReadOnlySpan<char> source, Span<char> destination, out int written)
        {
            if (++Calls > 20)
            {
                throw new NotSupportedException("TryMutate was called more often than the retry limit allows.");
            }
            DestinationLengths.Add(destination.Length);
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

    /// <summary>
    /// Ensures that IncludeProperties.All writes the caller information (MemberName, FilePath and LineNumber),
    /// and that the map header count matches the entries written, so the other properties are still read correctly.
    /// </summary>
    [Fact]
    public void IncludeAllProperties()
    {
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            IncludeProperties = IncludeProperties.All
        });

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");

        var x = 100;
        logger.ZLogInformation(new EventId(1, "HOGE"), $"AAA {x}");

        var msgpack = processor.Dequeue();
        Assert.Equal("test", (string)msgpack["Category"]);
        Assert.Equal("Information", (string)msgpack["LogLevel"]);
        Assert.Equal(1, (int)msgpack["EventId"]);
        Assert.Equal("HOGE", (string)msgpack["EventIdName"]);
        Assert.Equal("AAA 100", (string)msgpack["Message"]);
        Assert.Equal(100, (int)msgpack["x"]);
        Assert.Equal(nameof(IncludeAllProperties), (string)msgpack["MemberName"]);
        Assert.EndsWith("FormatterTest.cs", (string)msgpack["FilePath"], StringComparison.Ordinal);
        Assert.InRange((int)msgpack["LineNumber"], 1, int.MaxValue);
    }

    /// <summary>
    /// Ensures that the values written through the serializer options follow them: with DotNetOptimized options the timestamp
    /// is the int64 of DateTime.ToBinary, DateTimeOffset is [ticks, offset minutes], and Guid and decimal (a boxed value)
    /// are 16-byte binaries, both as parameters and as scope values. The default options write them differently.
    /// </summary>
    [Fact]
    public void ValuesFollowOptions()
    {
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            MessagePackSerializerOptions = MessagePackSerializerOptions.DotNetOptimized
        });

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                options.IncludeScopes = true;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");

        var dto = new DateTimeOffset(2026, 10, 23, 10, 2, 3, TimeSpan.FromHours(9));
        var guid = new Guid("0f8fad5b-d9cb-469f-a165-70867728950e");
        var amount = 123.456m;
        using (logger.BeginScope("{ScopeGuid}", guid))
        {
            logger.ZLogInformation($"{dto} {guid} {amount}");
        }

        var msgpack = processor.Dequeue();
        Assert.Equal(now.DateTime.ToBinary(), (long)msgpack["Timestamp"]); // the formatter writes LogInfo.Timestamp.Utc.DateTime, whose Kind is Unspecified
        Assert.Equal(dto.Ticks, (long)msgpack["dto"][0]);
        Assert.Equal(9 * 60, (short)msgpack["dto"][1]);
        Assert.Equal(guid.ToByteArray(), (byte[])msgpack["guid"]);
        Assert.Equal(16, ((byte[])msgpack["amount"]).Length);
        Assert.Equal(guid.ToByteArray(), (byte[])msgpack["ScopeGuid"]);
    }

    /// <summary>
    /// Ensures that DateTime and DateTime? parameters both follow the serializer options, so they are encoded the same way:
    /// with DotNetOptimized options both are written as the int64 of DateTime.ToBinary, not as the timestamp extension.
    /// The source-generated method keeps the declared type DateTime?; an interpolated string unwraps it to DateTime.
    /// </summary>
    [Fact]
    public void DateTimeParametersFollowOptions()
    {
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            MessagePackSerializerOptions = MessagePackSerializerOptions.DotNetOptimized
        });

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                return processor;
            });
        });
        logger = loggerFactory.CreateLogger("test");

        var dt = new DateTime(2026, 10, 23, 10, 2, 3, DateTimeKind.Local);
        DateTime? ndt = new DateTime(2026, 10, 23, 1, 2, 3, DateTimeKind.Utc);
        logger.LogDateTimes(dt, ndt);

        var msgpack = processor.Dequeue();
        Assert.Equal(dt.ToBinary(), (long)msgpack["dt"]);
        Assert.Equal(ndt.Value.ToBinary(), (long)msgpack["ndt"]);
    }

    /// <summary>
    /// Ensures that formatting which re-enters the formatter on the same thread does not corrupt the outer entry:
    /// a parameter's ToString logs through the same logger while the outer entry is being formatted, and TestProcessor
    /// formats synchronously in Post, like the in-memory provider. Both entries must be read back intact.
    /// </summary>
    [Fact]
    public void ReentrantLoggingKeepsBothEntriesIntact()
    {
        var payload = new ReentrantPayload { X = 5 };
        ReentrantPayload.Logger = logger;
        try
        {
            logger.ZLogInformation($"outer {payload}");
        }
        finally
        {
            ReentrantPayload.Logger = null;
        }

        var inner = processor.Dequeue();
        Assert.Equal("inner", (string)inner["Message"]);

        var outer = processor.Dequeue();
        Assert.Equal("outer outer-value", (string)outer["Message"]);
        Assert.Equal("test", (string)outer["Category"]);
        Assert.Equal(5, (int)outer["payload"]["x"]);
        Assert.Empty(processor.EntryMessages);
    }

    /// <summary>
    /// Ensures that enum and nullable enum parameters are written as their underlying integer, or as nil when null.
    /// The source-generated method keeps the declared type LogLevel?, which takes the nullable branch of the formatter;
    /// an interpolated string unwraps it to LogLevel.
    /// </summary>
    [Fact]
    public void EnumParametersAreWrittenAsIntegers()
    {
        LogLevel? interpolated = LogLevel.Warning;
        logger.ZLogInformation($"{interpolated}");
        logger.LogNullableEnums(LogLevel.Error, null);

        Assert.Equal((int)LogLevel.Warning, (int)processor.Dequeue()["interpolated"]);

        var generated = processor.Dequeue();
        Assert.Equal((int)LogLevel.Error, (int)generated["level"]);
        Assert.Null((object)generated["none"]);
    }

    /// <summary>
    /// Ensures that when serializing a value fails, nothing of that entry is written to the output,
    /// so that the entries written after it are still read correctly.
    /// TestProcessor keeps its buffer when formatting throws, like a stream writer shared by all entries.
    /// Without the staging buffer, the netstandard2.1 build leaves a partial entry even for a short message, because it
    /// commits before each value serialized through the options; the net9.0+ build commits only when its span runs out,
    /// which the long message forces before the failing value is written.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(100_000)]
    public void FailedEntryLeavesNoPartialOutput(int messageLength)
    {
        var padding = new string('a', messageLength);
        var unserializable = new UnserializableValue();
        var failing = () => logger.ZLogInformation($"{padding}{unserializable}");
        Assert.ThrowsAny<Exception>(failing);

        var tail = "TAIL";
        logger.ZLogInformation($"{tail}");

        var msgpack = processor.Dequeue();
        Assert.Equal("TAIL", (string)msgpack["tail"]);
        Assert.Empty(processor.EntryMessages);
    }

    /// <summary>
    /// Ensures that parameters of the types with dedicated branches (DateTimeOffset, TimeSpan and Guid)
    /// are each written exactly once, so that the map stays consistent and the parameter that follows them is read correctly.
    /// </summary>
    [Fact]
    public void ValueTypeParametersAreWrittenOnce()
    {
        var dto = new DateTimeOffset(2026, 10, 23, 10, 2, 3, TimeSpan.FromHours(9));
        var ts = TimeSpan.FromMilliseconds(123456.789);
        var guid = new Guid("0f8fad5b-d9cb-469f-a165-70867728950e");
        var tail = "TAIL";
        logger.ZLogInformation($"{dto} {ts} {guid} {tail}");

        var msgpack = processor.Dequeue();
        Assert.Equal(4 + 4, (int)((System.Collections.IDictionary)msgpack).Count); // Timestamp, LogLevel, Category, Message and the parameters (no exception and no scope, so those keys are absent)
        Assert.Equal(9 * 60, (short)msgpack["dto"][1]);
        Assert.Equal(ts.Ticks, (long)msgpack["ts"]);
        Assert.Equal("0f8fad5b-d9cb-469f-a165-70867728950e", (string)msgpack["guid"]);
        Assert.Equal("TAIL", (string)msgpack["tail"]);
    }

    // MessagePack v4 has formatters for DateOnly and TimeOnly only in its net9.0+ build; on net8.0 its netstandard build throws for them.
#if NET9_0_OR_GREATER
    /// <summary>
    /// Ensures that DateOnly and TimeOnly parameters are each written exactly once, in the same formats as MessagePack v2,
    /// so that the map stays consistent and the parameter that follows them is read correctly.
    /// </summary>
    [Fact]
    public void DateOnlyAndTimeOnlyParametersAreWrittenOnce()
    {
        var date = new DateOnly(2026, 10, 23);
        var time = new TimeOnly(10, 2, 3, 456);
        var tail = "TAIL";
        logger.ZLogInformation($"{date} {time} {tail}");

        var msgpack = processor.Dequeue();
        Assert.Equal(4 + 3, (int)((System.Collections.IDictionary)msgpack).Count); // Timestamp, LogLevel, Category, Message and the parameters (no exception and no scope, so those keys are absent)
        Assert.Equal(date.DayNumber, (int)msgpack["date"]);
        Assert.Equal(time.Ticks, (long)msgpack["time"]);
        Assert.Equal("TAIL", (string)msgpack["tail"]);
    }
#endif

    [Fact]
    public void WithSourceGenerator()
    {
        int IntValue = 123;
        int? IntNullableValue = 123;
        int? IntNull = null; 

        float FloatValue = 123.456f;
        float? FloatNullableValue = 123.456f;
        float? FloatNull = null;

        var StringValue = "Hello Hello Hello";
        
        logger.ZLogInformation($"{IntValue} {IntNullableValue} {IntNull} {FloatValue} {FloatNullableValue} {FloatNull} {StringValue}");
        var msg1 = processor.DequeueAsRaw();
        
        logger.Log1(IntValue, IntNullableValue, IntNull, FloatValue, FloatNullableValue, FloatNull, StringValue);
        var msg2 = processor.DequeueAsRaw();

        Assert.Equal(msg1, msg2);
    }
}

static partial class GeneratedLog
{
    [ZLoggerMessage(LogLevel.Information, "{IntValue} {IntNullableValue} {IntNull} {FloatValue} {FloatNullableValue} {FloatNull} {StringValue}")]
    public static partial void Log1(
        this ILogger logger, 
        int intValue,
        int? intNullableValue,
        int? intNull,
        float floatValue,
        float? floatNullableValue,
        float? floatNull,
        string stringValue);

    [ZLoggerMessage(LogLevel.Information, "{level} {none}")]
    public static partial void LogNullableEnums(this ILogger logger, LogLevel? level, LogLevel? none);

    [ZLoggerMessage(LogLevel.Information, "{dt} {ndt}")]
    public static partial void LogDateTimes(this ILogger logger, DateTime dt, DateTime? ndt);
} 
