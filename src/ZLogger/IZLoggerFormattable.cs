using System;
using System.Buffers;
using System.Text.Json;

namespace ZLogger
{
    // Implement for log state.
    public interface IZLoggerFormattable
    {
        int ParameterCount { get; }
        string ToString();
        void ToString(IBufferWriter<byte> writer);

        void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions);
        
        ReadOnlySpan<byte> GetParameterKey(int index);
        object? GetParameterValue(int index);
        T? GetParameterValue<T>(int index);
        Type GetParameterType(int index);
    }
}
