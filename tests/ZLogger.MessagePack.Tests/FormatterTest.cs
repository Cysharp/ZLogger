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
    class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
    }

    class TestPayload
    {
        public int X { get; set; }
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
            logger.ZLogInformation(new EventId(1, "HOGE"), "AAA {0} BBB {1}", 111, "Hello");
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Information");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("HOGE");
            ((string)msgpack["Message"]).Should().Be("AAA 111 BBB Hello");
            ((string)msgpack["Timestamp"]).Should().NotBeEmpty();
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
                logger.ZLogError(new EventId(1, "NG"), ex, "DAMEDA {0}", 111);                
            }
        
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Error");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("NG");
            ((string)msgpack["Message"]).Should().Be("DAMEDA 111");
            ((string)msgpack["Timestamp"]).Should().NotBeEmpty();
            ((string)msgpack["Exception"]["Name"]).Should().Be("ZLogger.Tests.TestException");
            ((string)msgpack["Exception"]["Message"]).Should().Be("DAME");
            ((string)msgpack["Exception"]["StackTrace"]).Should().NotBeEmpty();
            ((bool)msgpack.ContainsKey("Payload")).Should().BeFalse();
        }
    
        [Fact]
        public void WithPayload()
        {
            logger.ZLogInformationWithPayload(new EventId(1, "HOGE"), new TestPayload { X = 999}, "UMU {0}", 111);
        
            var msgpack = processor.Dequeue();
            ((string)msgpack["CategoryName"]).Should().Be("test");
            ((string)msgpack["LogLevel"]).Should().Be("Error");
            ((int)msgpack["EventId"]).Should().Be(1);
            ((string)msgpack["EventIdName"]).Should().Be("NG");
            ((string)msgpack["Timestamp"]).Should().NotBeEmpty();
            ((int)msgpack["Payload"]["X"]).Should().Be(999);
            ((bool)msgpack.ContainsKey("Exception")).Should().BeFalse();            
        }
   }
}