using System.Buffers;
using System.Text.Json;

namespace ZLogger;
// Implement for log state.

public interface IZLoggerEntryCreatable
{
    IZLoggerEntry CreateEntry(in LogInfo info);
}

public interface IZLoggerFormattable : IZLoggerEntryCreatable
{
    int ParameterCount { get; }
    bool IsSupportUtf8ParameterKey { get; }
    string ToString();
    void ToString(IBufferWriter<byte> writer);

    string GetOriginalFormat();
    void WriteOriginalFormat(IBufferWriter<byte> writer);

    void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions, IKeyNameMutator? keyNameMutator = null);

    ReadOnlySpan<byte> GetParameterKey(int index);
    string GetParameterKeyAsString(int index);
    object? GetParameterValue(int index);
    T? GetParameterValue<T>(int index);
    Type GetParameterType(int index);
}

public interface IReferenceCountable
{
    void Retain();
    void Release();
}

public interface IZLoggerAdditionalInfo
{
    (object? Context, string? MemberName, string? FilePath, int LineNumber) GetAdditionalInfo();
}
