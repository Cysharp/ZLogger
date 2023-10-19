using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ZLogger.MessagePack.Tests
{
    public class FormatterTest
    {
        TestProcessor processor;
        ILogger logger;

        public FormatterTest()
        {
            var options = new ZLoggerOptions();

            options.UseMessagePackFormatter();

            processor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            logger = loggerFactory.CreateLogger("test");
        }

        [Fact]
        public void PlainMessage()
        {
            var now = DateTime.UtcNow;
            logger.ZLogInformation(new EventId(1, "HOGE"), $"AAA {111} BBB {"Hello"}");
            
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Information");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("HOGE");
            ((string)msgpack["Message"]).Should().Be("AAA 111 BBB Hello");
            ((DateTime)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
            ((bool)msgpack.ContainsKey("Exception")).Should().BeFalse();
            ((bool)msgpack.ContainsKey("Payload")).Should().BeFalse();
        }

        [Fact]
        public void WithException()
        {
            var now = DateTime.UtcNow;
            try
            {
                throw new TestException("DAME");
            }
            catch (Exception ex)
            {
                logger.ZLogError(new EventId(1, "NG"), ex, $"DAMEDA 111");                
            }
        
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Error");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("NG");
            ((string)msgpack["Message"]).Should().Be("DAMEDA 111");
            ((DateTime)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
            ((string)msgpack["Exception"]["Name"]).Should().Be("ZLogger.MessagePack.Tests.TestException");
            ((string)msgpack["Exception"]["Message"]).Should().Be("DAME");
            ((string)msgpack["Exception"]["StackTrace"]).Should().NotBeEmpty();
            ((string)msgpack["Exception"]["InnerException"]).Should().BeNull();
        }

        [Fact]
        public void WithExceptionWithInnerException()
        {
            var now = DateTime.UtcNow;
            try
            {
                throw new TestException("DAME!", new TestException("INNER!"));
            }
            catch (Exception ex)
            {
                logger.ZLogError(new EventId(1, "NG"), ex, $"DAMEDA 111");                
            }
        
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Error");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("NG");
            ((string)msgpack["Message"]).Should().Be("DAMEDA 111");
            ((DateTime)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
            ((string)msgpack["Exception"]["Name"]).Should().Be("ZLogger.MessagePack.Tests.TestException");
            ((string)msgpack["Exception"]["Message"]).Should().Be("DAME!");
            ((string)msgpack["Exception"]["StackTrace"]).Should().NotBeEmpty();
            ((string)msgpack["Exception"]["InnerException"]["Name"]).Should().Be("ZLogger.MessagePack.Tests.TestException");
            ((string)msgpack["Exception"]["InnerException"]["Message"]).Should().Be("INNER!");
        }
    
        [Fact]
        public void WithParameters()
        {
            var now = DateTime.UtcNow;
            var payload = new TestPayload { X = 999 };
            var x = 100;
            var y = 200;
            logger.ZLogInformation(new EventId(1, "HOGE"), $"UMU {payload} {x} {y}");
        
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Information");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("HOGE");
            ((DateTime)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
            ((int?)msgpack["x"]).Should().Be(100);
            ((int?)msgpack["y"]).Should().Be(null);
            ((int)msgpack["payload"]["x"]).Should().Be(999);
            ((int)msgpack["x"]).Should().Be(100);
            ((int)msgpack["y"]).Should().Be(200);
            ((bool)msgpack.ContainsKey("Exception")).Should().BeFalse();            
        }

        [Fact]
        public void LowercaseMutator()
        {
            var options = new ZLoggerOptions
            {
                KeyNameMutator = KeyNameMutator.LowercaseInitial
            };

            options.UseMessagePackFormatter();

            processor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            logger = loggerFactory.CreateLogger("test");


            var XyzAbc = 100;
            var fOo = 200;
            logger.ZLogInformation($"AAA {XyzAbc} {fOo}");
 
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Information");
            ((string)msgpack["Message"]).Should().Be("AAA 100 200");
            ((int)msgpack["xyzAbc"]).Should().Be(100);
            ((int)msgpack["fOo"]).Should().Be(200);
        }
   }
}
