using System.Buffers;
using System.Text.Json;

namespace ZLog
{
    public interface IZLogEntry
    {
        public LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer, ZLogOptions options, Utf8JsonWriter? jsonWriter);
        void Return();
    }
} 