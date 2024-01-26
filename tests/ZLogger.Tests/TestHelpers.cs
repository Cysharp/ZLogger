using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZLogger.Internal;

namespace ZLogger.Tests
{
    class TestProcessor : IAsyncLogProcessor
    {
        public readonly Queue<IZLoggerEntry> Entries = new();
        readonly IZLoggerFormatter formatter;

        public string DequeueAsString() => Entries.Dequeue().FormatToString(formatter);

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
            Entries.Enqueue(log);
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
