using System.Buffers;
using System.Text.Json;

namespace ZLogger
{
    public interface IZLoggerEntry
    {
        public LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter);
        void Return();
    }
} 