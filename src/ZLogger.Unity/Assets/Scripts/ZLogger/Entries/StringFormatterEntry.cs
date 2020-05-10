using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace ZLogger.Entries
{
    public class StringFormatterEntry<TState> : IZLoggerEntry
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

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter jsonWriter)
        {
            var str = formatter(state, exception);

            if (options.EnableStructuredLogging && jsonWriter != null)
            {
                options.StructuredLoggingFormatter.Invoke(jsonWriter, this.LogInfo);
                jsonWriter.WriteString(options.MessagePropertyName, str);
                jsonWriter.WriteNull(options.PayloadPropertyName);
            }
            else
            {
                options.PrefixFormatter?.Invoke(writer, this.LogInfo);

                if (str != null)
                {
                    var memory = writer.GetMemory(Encoding.UTF8.GetMaxByteCount(str.Length));
                    if (MemoryMarshal.TryGetArray<byte>(memory, out var segment) && segment.Array != null)
                    {
                        var written1 = Encoding.UTF8.GetBytes(str, 0, str.Length, segment.Array, segment.Offset);
                        writer.Advance(written1);
                    }
                }

                options.SuffixFormatter?.Invoke(writer, this.LogInfo);
                if (this.LogInfo.Exception != null)
                {
                    options.ExceptionFormatter(writer, this.LogInfo.Exception);
                }
            }
        }

        public object GetPayload()
        {
            return null;
        }

        public void SwitchCasePayload<TPayload>(Action<IZLoggerEntry, TPayload, object> payloadCallback, object state)
        {
        }

        public void Return()
        {
            this.state = default;
            this.LogInfo = default;
            this.exception = default;
            this.formatter = default;
        }
    }
}
