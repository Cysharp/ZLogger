using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Logging;
using Xunit;
using ZLogger.MessagePack;

namespace ZLogger.Tests
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class TestPayload
    {
        public int X { get; set; }
    }
    
    class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
        
        public TestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    
    class TestProcessor : IAsyncLogProcessor
    {
        public Queue<dynamic> EntryMessages = new Queue<dynamic>();
        readonly ZLoggerOptions options;
        readonly IZLoggerFormatter formatter;
        readonly ArrayBufferWriter<byte> bufferWriter = new ArrayBufferWriter<byte>();

        public dynamic Dequeue() => EntryMessages.Dequeue();

        public TestProcessor(ZLoggerOptions options)
        {
            this.options = options;
            formatter = options.CreateFormatter();
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public void Post(IZLoggerEntry log)
        {
            log.FormatUtf8(bufferWriter, formatter);
            var entry = MessagePackSerializer.Deserialize<dynamic>(bufferWriter.WrittenMemory, ContractlessStandardResolver.Options);
            bufferWriter.Clear();
            EntryMessages.Enqueue(entry);
        }
    }
    
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
            logger.ZLogInformation(new EventId(1, "HOGE"), "AAA {0} BBB {1}", 111, "Hello");
            
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
                logger.ZLogError(new EventId(1, "NG"), ex, "DAMEDA {0}", 111);                
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
                logger.ZLogError(new EventId(1, "NG"), ex, "DAMEDA {0}", 111);                
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
            logger.ZLogInformationWithPayload(new EventId(1, "HOGE"), new TestPayload { X = 999}, "UMU {0}", 111);
        
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