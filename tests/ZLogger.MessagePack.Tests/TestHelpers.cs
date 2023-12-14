using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;

namespace ZLogger.MessagePack.Tests
{
    [MessagePackObject]
    public class TestPayload
    {
        [Key("x")]
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
    
    class TestProcessor(IZLoggerFormatter formatter) : IAsyncLogProcessor
    {
        public readonly Queue<byte[]> EntryMessages = new();
        readonly ArrayBufferWriter<byte> bufferWriter = new();

        public byte[] DequeueAsRaw() => EntryMessages.Dequeue();

        public dynamic Dequeue()
        {
            var data = EntryMessages.Dequeue();
            return MessagePackSerializer.Deserialize<dynamic>(data, ContractlessStandardResolver.Options);
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public void Post(IZLoggerEntry log)
        {
            log.FormatUtf8(bufferWriter, formatter);
            EntryMessages.Enqueue(bufferWriter.WrittenMemory.ToArray());
            bufferWriter.Clear();
        }
    }    
}
