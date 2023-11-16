using System.Buffers;

namespace ZLogger
{
    // Define how write log entry(text, strctured-json, structured-msgpack, etc...)
    public interface IZLoggerFormatter
    {
        bool WithLineBreak { get; }
        void FormatLogEntry<TEntry>(IBufferWriter<byte> writer, TEntry entry)
            where TEntry : IZLoggerEntry;
    }
}
