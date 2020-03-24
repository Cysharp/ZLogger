using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;

namespace ZLog.Entries
{
    internal class StringFormatterEntry<TState> : IUtf8LogEntry
    {
        static ConcurrentQueue<StringFormatterEntry<TState>> cache = new ConcurrentQueue<StringFormatterEntry<TState>>();

        LogLevel logLevel;
        EventId eventId;
#pragma warning disable CS8618
        TState state;
        Exception exception;
        Func<TState, Exception, string> formatter;
#pragma warning restore CS8618

        public static StringFormatterEntry<TState> Create(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!cache.TryDequeue(out var entry))
            {
                entry = new StringFormatterEntry<TState>();
            }

            entry.Set(logLevel, eventId, state, exception, formatter);

            return entry;
        }

        void Set(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.logLevel = logLevel;
            this.eventId = eventId;
            this.state = state;
            this.exception = exception;
            this.formatter = formatter;
        }

        public void FormatUtf8(IBufferWriter<byte> writer)
        {
            var str = formatter(state, exception);
            var memory = writer.GetMemory(Encoding.UTF8.GetMaxByteCount(str.Length));
            MemoryMarshal.TryGetArray<byte>(memory, out var segment);
            var written = Encoding.UTF8.GetBytes(str, 0, str.Length, segment.Array, segment.Offset);
            writer.Advance(written);
        }

        public void Return()
        {
            this.state = default!;
            this.exception = default!;
            this.formatter = default!;
        }
    }
}
