using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;

namespace ZLogger.MessagePack.Tests
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
        public Queue<dynamic> EntryMessages = new();
        readonly ZLoggerOptions options;
        readonly IZLoggerFormatter formatter;
        readonly ArrayBufferWriter<byte> bufferWriter = new();

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
}
