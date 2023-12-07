using System;
using FluentAssertions;
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
