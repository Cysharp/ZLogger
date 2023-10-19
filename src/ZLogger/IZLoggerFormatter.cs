using System.Buffers;

namespace ZLogger
{
    // Define how write log entry(text, strctured-json, structured-msgpack, etc...)
    public interface IZLoggerFormatter
    {
        void FormatLogEntry<TEntry>(IBufferWriter<byte> writer, TEntry entry, bool withLineBreak = true)
            where TEntry : IZLoggerEntry;
    }
}
