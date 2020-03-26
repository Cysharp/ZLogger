using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace ZLog.Entries
{
    internal class StringFormatterEntry<TState> : IZLogEntry
    {
        static ConcurrentQueue<StringFormatterEntry<TState>> cache = new ConcurrentQueue<StringFormatterEntry<TState>>();
#pragma warning disable CS8618
        TState state;
        Exception exception;
        Func<TState, Exception, string> formatter;

        public LogInfo LogInfo { get; private set; }

#pragma warning restore CS8618

        public static StringFormatterEntry<TState> Create(LogInfo info, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!cache.TryDequeue(out var entry))
            {
                entry = new StringFormatterEntry<TState>();
            }

            entry.LogInfo = info;
            entry.state = state;
            entry.exception = exception;
            entry.formatter = formatter;

            return entry;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLogOptions options, Utf8JsonWriter? jsonWriter)
        {
            var str = formatter(state, exception);

            if (options.IsStructuredLogging && jsonWriter != null)
            {
                jsonWriter.WriteString(options.MessagePropertyName, str);
            }
            else
            {
                var memory = writer.GetMemory(Encoding.UTF8.GetMaxByteCount(str.Length));
                MemoryMarshal.TryGetArray<byte>(memory, out var segment);
                var written = Encoding.UTF8.GetBytes(str, 0, str.Length, segment.Array, segment.Offset);
                writer.Advance(written);
            }
        }

        public void Return()
        {
            this.state = default!;
            this.LogInfo = default!;
            this.exception = default!;
            this.formatter = default!;
        }
    }
}
