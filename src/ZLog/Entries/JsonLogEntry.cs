using System.Buffers;
using System.Text.Json;

namespace ZLog.Entries
{
    internal struct PayloadState<T> : IZLogState
    {
        readonly T payload;

        public PayloadState(T payload)
        {
            this.payload = payload;
        }

        public IUtf8LogEntry CreateLogEntry()
        {
            // TODO:cache
            // TODO:only JsonLog???
            return new JsonLogEntry<T>(payload);
        }
    }

    internal class JsonLogEntry<T> : IUtf8LogEntry
    {
        public T State;

        public JsonLogEntry(T state)
        {
            this.State = state;
        }

        public void FormatUtf8(IBufferWriter<byte> writer)
        {
            using (var jsonWriter = new Utf8JsonWriter(writer))
            {
                JsonSerializer.Serialize(jsonWriter, State);
            }
        }

        public void Return()
        {
            State = default!;
        }
    }
}
