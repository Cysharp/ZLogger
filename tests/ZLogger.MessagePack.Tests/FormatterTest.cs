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
                var x = 100;
                logger.ZLogError(new EventId(1, "NG"), ex, $"DAMEDA {x}");                
            }
        
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Error");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("NG");
            ((string)msgpack["Message"]).Should().Be("DAMEDA 111");
            ((DateTime)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
            ((string)msgpack["Exception"]["Name"]).Should().Be("ZLogger.Tests.TestException");
            ((string)msgpack["Exception"]["Message"]).Should().Be("DAME");
            ((string)msgpack["Exception"]["StackTrace"]).Should().NotBeEmpty();
            ((string)msgpack["Exception"]["InnerException"]).Should().BeNull();
            
            ((bool)msgpack.ContainsKey("Payload")).Should().BeFalse();
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
                var x = 111;
                logger.ZLogError(new EventId(1, "NG"), ex, $"DAMEDA {x}");                
            }
        
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Error");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("NG");
            ((string)msgpack["Message"]).Should().Be("DAMEDA 111");
            ((DateTime)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
            ((string)msgpack["Exception"]["Name"]).Should().Be("ZLogger.Tests.TestException");
            ((string)msgpack["Exception"]["Message"]).Should().Be("DAME!");
            ((string)msgpack["Exception"]["StackTrace"]).Should().NotBeEmpty();
            ((string)msgpack["Exception"]["InnerException"]["Name"]).Should().Be("ZLogger.Tests.TestException");
            ((string)msgpack["Exception"]["InnerException"]["Message"]).Should().Be("INNER!");
            ((bool)msgpack.ContainsKey("Payload")).Should().BeFalse();
        }
    
        [Fact]
        public void WithPayload()
        {
            var now = DateTime.UtcNow;
            var payload = new TestPayload { X = 999 };
            logger.ZLogInformation(new EventId(1, "HOGE"), $"UMU {payload}");
        
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Information");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("HOGE");
            ((DateTime)msgpack["Timestamp"]).Should().BeOnOrAfter(now);
            ((int)msgpack["Payload"]["X"]).Should().Be(999);
            ((bool)msgpack.ContainsKey("Exception")).Should().BeFalse();            
        }
   }
}