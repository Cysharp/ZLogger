using System.Buffers;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ZLog.Entries
{
    internal struct JsonLogState<T> : IZLogState
    {
        readonly T payload;
        
        public bool IsJson => true;

        public JsonLogState(T payload)
        {
            this.payload = payload;
        }

        public IZLogEntry CreateLogEntry(LogInfo logInfo)
        {
            return JsonLogEntry<T>.Create(logInfo, payload);
        }
    }

    internal class JsonLogEntry<T> : IZLogEntry
    {
        static ConcurrentQueue<JsonLogEntry<T>> cache = new ConcurrentQueue<JsonLogEntry<T>>();

#pragma warning disable CS8618

        T state;

        public LogInfo LogInfo { get; private set; }

        JsonLogEntry()
        {
        }

#pragma warning restore CS8618

        public static JsonLogEntry<T> Create(in LogInfo logInfo, T state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new JsonLogEntry<T>();
            }

            result.LogInfo = logInfo;
            result.state = state;

            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, bool requireJavaScriptEncode)
        {
            // TODO:handle bool requireJavaScriptEncode

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
