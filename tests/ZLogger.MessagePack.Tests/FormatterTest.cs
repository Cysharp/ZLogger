using System;
using FluentAssertions;
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
        ((string)msgpack["Category"]).Should().Be("test");
        ((string)msgpack["LogLevel"]).Should().Be("Information");
        ((string)msgpack["Message"]).Should().Be("AAA 111 BBB Hello");
        ((DateTimeOffset)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
        ((bool)msgpack.ContainsKey("Exception")).Should().BeFalse();
        ((bool)msgpack.ContainsKey("Payload")).Should().BeFalse();
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
        ((string)msgpack["Category"]).Should().Be("test");
        ((string)msgpack["LogLevel"]).Should().Be("Error");
        ((string)msgpack["Message"]).Should().Be("DAMEDA 111");
        ((DateTimeOffset)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
        ((string)msgpack["Exception"]["Name"]).Should().Be("ZLogger.MessagePack.Tests.TestException");
        ((string)msgpack["Exception"]["Message"]).Should().Be("DAME");
        ((string)msgpack["Exception"]["StackTrace"]).Should().NotBeEmpty();
        ((string)msgpack["Exception"]["InnerException"]).Should().BeNull();
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
        ((string)msgpack["Category"]).Should().Be("test");
        ((string)msgpack["LogLevel"]).Should().Be("Error");
        ((string)msgpack["Message"]).Should().Be("DAMEDA 111");
        ((DateTimeOffset)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
        ((string)msgpack["Exception"]["Name"]).Should().Be("ZLogger.MessagePack.Tests.TestException");
        ((string)msgpack["Exception"]["Message"]).Should().Be("DAME!");
        ((string)msgpack["Exception"]["StackTrace"]).Should().NotBeEmpty();
        ((string)msgpack["Exception"]["InnerException"]["Name"]).Should().Be("ZLogger.MessagePack.Tests.TestException");
        ((string)msgpack["Exception"]["InnerException"]["Message"]).Should().Be("INNER!");
    }
    
    [Fact]
    public void WithParameters()
    {
        var payload = new TestPayload { X = 999 };
        var x = 100;
        int? y = null;
        logger.ZLogInformation(new EventId(1, "HOGE"), $"UMU {payload} {x} {y}");
        
        var msgpack = processor.Dequeue();
        ((string)msgpack["Category"]).Should().Be("test");
        ((string)msgpack["LogLevel"]).Should().Be("Information");
        ((DateTimeOffset)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
        ((int?)msgpack["x"]).Should().Be(100);
        ((int?)msgpack["y"]).Should().Be(null);
        ((int)msgpack["payload"]["x"]).Should().Be(999);
        ((bool)msgpack.ContainsKey("Exception")).Should().BeFalse();            
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
        ((string)msgpack["Category"]).Should().Be("test");
        ((string)msgpack["LogLevel"]).Should().Be("Information");
        ((string)msgpack["Message"]).Should().Be("AAA 100 200");
        ((int)msgpack["xyzAbc"]).Should().Be(100);
        ((int)msgpack["fOo"]).Should().Be(200);
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
        ((string)msgpack["LogLevel"]).Should().Be("Information");
        ((int)msgpack["EventId"]).Should().Be(1);
        ((DateTimeOffset)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
        ((bool)msgpack.ContainsKey("Exception")).Should().BeFalse();
        ((bool)msgpack.ContainsKey("CategoryName")).Should().BeFalse();
        ((bool)msgpack.ContainsKey("EventIdName")).Should().BeFalse();
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
        ((bool)msgpack.ContainsKey("LogLevel")).Should().BeFalse();
        ((bool)msgpack.ContainsKey("Timestamp")).Should().BeFalse();
        ((bool)msgpack.ContainsKey("EventId")).Should().BeFalse();
        ((bool)msgpack.ContainsKey("EventIdName")).Should().BeFalse();
        ((bool)msgpack.ContainsKey("Exception")).Should().BeFalse();
        ((bool)msgpack.ContainsKey("CategoryName")).Should().BeFalse();
        ((bool)msgpack.ContainsKey("EventIdName")).Should().BeFalse();
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
        ((string)msg["level"]).Should().Be("INFO");
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
        ((string)msg["Message"]).Should().Be("456");
        ((int)msg["attributes"]["y"]).Should().Be(456);
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
        ((string)msg["Message"]).Should().Be("456");
        ((int)msg["scope"]["X"]).Should().Be(123);
        ((int)msg["attributes"]["y"]).Should().Be(456);
    }
    
    /// <summary>
    /// Ensures that a MessageProcessor in the serializer options (such as compression) is ignored:
    /// logging does not fail, and the timestamp, parameters and scope values are written as plain values,
    /// not wrapped in per-value envelopes.
    /// </summary>
    [Fact]
    public void MessageProcessorIsIgnored()
    {
        processor = new TestProcessor(new MessagePackZLoggerFormatter
        {
            MessagePackSerializerOptions = MessagePackSerializerOptions.Default.WithFraming()
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

        var guid = new Guid("0f8fad5b-d9cb-469f-a165-70867728950e");
        var amount = 123.456m;
        using (logger.BeginScope("{Scope}", 789L))
        {
            logger.ZLogInformation($"AAA {guid} {amount}");
        }

        var msgpack = processor.Dequeue();
        ((DateTimeOffset)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
        ((string)msgpack["guid"]).Should().Be("0f8fad5b-d9cb-469f-a165-70867728950e");
        ((string)msgpack["amount"]).Should().Be("123.456");
        ((long)msgpack["Scope"]).Should().Be(789L);
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
        ((string)msgpack["Category"]).Should().Be("test");
        ((string)msgpack["LogLevel"]).Should().Be("Information");
        ((int)msgpack["EventId"]).Should().Be(1);
        ((string)msgpack["EventIdName"]).Should().Be("HOGE");
        ((string)msgpack["Message"]).Should().Be("AAA 100");
        ((int)msgpack["x"]).Should().Be(100);
        ((string)msgpack["MemberName"]).Should().Be(nameof(IncludeAllProperties));
        ((string)msgpack["FilePath"]).Should().EndWith("FormatterTest.cs");
        ((int)msgpack["LineNumber"]).Should().BePositive();
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
        ((int)((System.Collections.IDictionary)msgpack).Count).Should().Be(4 + 4); // Timestamp, LogLevel, Category, Message and the parameters
        ((short)msgpack["dto"][1]).Should().Be(9 * 60);
        ((long)msgpack["ts"]).Should().Be(ts.Ticks);
        ((string)msgpack["guid"]).Should().Be("0f8fad5b-d9cb-469f-a165-70867728950e");
        ((string)msgpack["tail"]).Should().Be("TAIL");
    }

    // MessagePack v4 serves DateOnly and TimeOnly only in its net9.0+ build; on net8.0 its netstandard build throws for them.
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
        ((int)((System.Collections.IDictionary)msgpack).Count).Should().Be(4 + 3); // Timestamp, LogLevel, Category, Message and the parameters
        ((int)msgpack["date"]).Should().Be(date.DayNumber);
        ((long)msgpack["time"]).Should().Be(time.Ticks);
        ((string)msgpack["tail"]).Should().Be("TAIL");
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

        msg1.AsSpan().SequenceEqual(msg2).Should().BeTrue();
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
} 
