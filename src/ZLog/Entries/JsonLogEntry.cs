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

        public IZLogEntry CreateLogEntry(LogInfo logInfo)
        {
            // TODO:cache
            // TODO:only JsonLog???
            return new JsonLogEntry<T>(logInfo, payload);
        }
    }

    internal class JsonLogEntry<T> : IZLogEntry
    {
        T state;

        public LogInfo LogInfo { get; private set; }

        public JsonLogEntry(LogInfo logInfo, T state)
        {
            this.LogInfo = logInfo;
            this.state = state;
        }

        public void FormatUtf8(IBufferWriter<byte> writer)
        {
            using (var jsonWriter = new Utf8JsonWriter(writer))
            {
                JsonSerializer.Serialize(jsonWriter, state);
            }
        }

        public void Return()
        {
            LogInfo = default!;
            state = default!;
        }
    }
}
