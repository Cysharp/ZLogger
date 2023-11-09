using System.Buffers;
using System.Text.Json;

namespace ZLogger
{
    // Implement for log state.
    public interface IZLoggerFormattable
    {
        IZLoggerEntry CreateEntry(LogInfo info);

        int ParameterCount { get; }
        bool IsSupportUtf8ParameterKey { get; }
        string ToString();
        void ToString(IBufferWriter<byte> writer);

        void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions, ZLoggerOptions options);

        ReadOnlySpan<byte> GetParameterKey(int index);
        ReadOnlySpan<char> GetParameterKeyAsString(int index);
        object? GetParameterValue(int index);
        T? GetParameterValue<T>(int index);
        Type GetParameterType(int index);
    }
    
    public interface IReferenceCountZLoggerFormattable : IZLoggerFormattable
    {
        void Retain();
        void Release();
    }
}
