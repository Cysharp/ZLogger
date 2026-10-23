using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagePack;

namespace ZLogger.MessagePack.Tests
{
    [MessagePackObject]
    public class TestPayload
    {
        [Key("x")]
        public int X { get; set; }
    }

    // Logs from its ToString, which the formatter calls while it builds the message of the entry that contains it.
    [MessagePackObject]
    public class ReentrantPayload
    {
        [ThreadStatic]
        public static Microsoft.Extensions.Logging.ILogger? Logger;

        [Key("x")]
        public int X { get; set; }

        public override string ToString()
        {
            Logger?.ZLogInformation($"inner");
            return "outer-value";
        }
    }

    // No MessagePack attributes and not contractless-resolvable by default, so serializing it throws.
    public class UnserializableValue
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

    class TestProcessor(IZLoggerFormatter formatter) : IAsyncLogProcessor
    {
        static readonly MessagePackSerializerOptions ContractlessOptions = new(MessagePackFormatterFactory.Default.WithContractless());

        public readonly Queue<byte[]> EntryMessages = new();
        readonly ArrayBufferWriter<byte> bufferWriter = new();

        public byte[] DequeueAsRaw() => EntryMessages.Dequeue();

        public dynamic Dequeue()
        {
            var data = EntryMessages.Dequeue();
            return MessagePackSerializer.Deserialize<dynamic>(data, ContractlessOptions);
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
