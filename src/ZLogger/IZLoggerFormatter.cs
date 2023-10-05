using System;
using System.Buffers;

namespace ZLogger
{
    public interface IZLoggerFormatter
    {
        /// <summary>
        /// Defines the final format of each log entry.
        /// </summary>
        /// <param name="writer">Write a utf8 byte sequence to this buffer.</param>
        /// <param name="entry">Logged entry</param>
        /// <param name="payload">Logged payload</param>
        /// <param name="utf8Message">Logged message already rendered into a single string.</param>
        void FormatLogEntry<TEntry, TPayload>(
            IBufferWriter<byte> writer,
            TEntry entry,
            TPayload payload,
            ReadOnlySpan<byte> utf8Message)
            where TEntry : IZLoggerEntry;
    }
}
