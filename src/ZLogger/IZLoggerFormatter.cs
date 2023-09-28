using System;
using System.Buffers;

namespace ZLogger
{
    public interface IZLoggerFormatter
    {
        void FormatLogEntry<TEntry, TPayload>(
            IBufferWriter<byte> writer,
            TEntry entry,
            TPayload payload,
            ReadOnlySpan<byte> utf8Message)
            where TEntry : IZLoggerEntry;
    }
}
