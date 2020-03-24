using System.Buffers;

namespace ZLog
{
    public interface IUtf8LogEntry
    {
        void FormatUtf8(IBufferWriter<byte> writer);
        void Return();
    }
}
