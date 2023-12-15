using System;
using System.Collections.Generic;
using System.Text;
using System.Buffers;
using System.Threading.Tasks;
using ZLogger;

namespace Tests
{
    class TestProcessor : IAsyncLogProcessor
    {
        [ThreadStatic]
        static ArrayBufferWriter<byte>? bufferWriter;
        
        public Queue<string> EntryMessages = new();
        readonly IZLoggerFormatter formatter;

        public string Dequeue() => EntryMessages.Dequeue();

        public TestProcessor(ZLoggerOptions options)
        {
            formatter = options.CreateFormatter();
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public void Post(IZLoggerEntry log)
        {
            EntryMessages.Enqueue(FormatToString(log, formatter));
        }

        static string FormatToString(IZLoggerEntry entry, IZLoggerFormatter formatter)
        {
            bufferWriter ??= new ArrayBufferWriter<byte>();
            bufferWriter.Clear();
            
            formatter.FormatLogEntry(bufferWriter, entry);
            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }
    }

    class TestState
    {
        public int X { get; set; }
    }
}
