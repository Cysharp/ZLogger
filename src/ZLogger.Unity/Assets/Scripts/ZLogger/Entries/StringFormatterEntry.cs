using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Text;

namespace ZLogger.Entries
{
    public class StringFormatterEntry<TState> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<StringFormatterEntry<TState>> cache = new ConcurrentQueue<StringFormatterEntry<TState>>();
        static readonly byte[] newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);

#pragma warning disable CS8618
        TState state;
        Exception? exception;
        Func<TState, Exception?, string> formatter;

        public LogInfo LogInfo { get; private set; }

#pragma warning restore CS8618

        public static StringFormatterEntry<TState> Create(LogInfo info, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
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

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var str = this.formatter(state, exception);
            if (str != null)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(str.Length));
                try
                {
                    var bytesWritten = Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, 0);
                    formatter.FormatLogEntry(writer, this, (string)null, buffer.AsSpan(0, bytesWritten));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        public object? GetPayload()
        {
            return null;
        }

        public void SwitchCasePayload<TPayload>(Action<IZLoggerEntry, TPayload, object?> payloadCallback, object? state)
        {
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
