using System.Buffers;

namespace ZLogger
{
    // Define how write log entry(text, strctured-json, structured-msgpack, etc...)
    public interface IZLoggerFormatter
    {
        bool WithLineBreak { get; }
        void FormatLogEntry(IBufferWriter<byte> writer, IZLoggerEntry entry);
    }
}
