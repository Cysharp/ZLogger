using System.Collections.Generic;
using System.Threading.Tasks;

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
}
