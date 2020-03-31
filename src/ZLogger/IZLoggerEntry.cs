using System;
using System.Buffers;
using System.Text.Json;

namespace ZLogger
{
    public interface IZLoggerEntry
    {
        public LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter);
        void SwitchCasePayload<TPayload>(Action<IZLoggerEntry, TPayload, object?> payloadCallback, object? state);
        object? GetPayload();
        void Return();
    }
}