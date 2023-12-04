using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZLogger.Internal;

namespace ZLogger.Tests
{
    class TestProcessor : IAsyncLogProcessor
    {
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
            EntryMessages.Enqueue(log.FormatToString(formatter));
        }
    }

    class TestState
    {
        public int X { get; set; }
    }

    file static class ZLoggerEntryExtensions
    {
        public static string FormatToString(this IZLoggerEntry entry, IZLoggerFormatter formatter)
        {
            var buffer = ArrayBufferWriterPool.Rent();
            try
            {
                formatter.FormatLogEntry(buffer, entry);
                return Encoding.UTF8.GetString(buffer.WrittenSpan);
            }
            finally
            {
                ArrayBufferWriterPool.Return(buffer);
            }
        }
    }
}
