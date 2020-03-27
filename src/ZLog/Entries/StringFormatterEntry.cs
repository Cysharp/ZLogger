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
        static readonly ConcurrentQueue<StringFormatterEntry<TState>> cache = new ConcurrentQueue<StringFormatterEntry<TState>>();
        static readonly byte[] newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);

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
                string? exceptionMessage = default;
                int exceptionMessageLength = 0;
                if (exception != null)
                {
                    exceptionMessage = exception.ToString();
                    exceptionMessageLength = exceptionMessage.Length + newLineBytes.Length;
                }

                var memory = writer.GetMemory(Encoding.UTF8.GetMaxByteCount(str.Length + exceptionMessageLength));
                if (MemoryMarshal.TryGetArray<byte>(memory, out var segment) && segment.Array != null)
                {
                    var written1 = Encoding.UTF8.GetBytes(str, 0, str.Length, segment.Array, segment.Offset);
                    var written2 = 0;
                    var written3 = 0;
                    if (exceptionMessage != null)
                    {
                        newLineBytes.CopyTo(segment.Array, segment.Offset + written1);
                        written2 = newLineBytes.Length;
                        written3 = Encoding.UTF8.GetBytes(exceptionMessage, 0, exceptionMessage.Length, segment.Array, segment.Offset + written1 + written2);
                    }

                    writer.Advance(written1 + written2 + written3);
                }
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
