using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Utf8StringInterpolation;
using ZLogger;
using ZLogger.Internal;

namespace GeneratorSandbox;



public static partial class Log22
{
    // [ZLoggerMessage(Information, "Could not open socket to {hostName} {ipAddress}.")]
    public static void CouldNotOpenSocket(this ILogger<FooBarBaz> logger, string hostName, int ipAddress)
    {
        if (!logger.IsEnabled(LogLevel.Information)) return;
        logger.Log(LogLevel.Information, -1, new CouldNotOpenSocketState(hostName, ipAddress), null, (state, ex) => state.ToString());
    }

    public static void CouldNotOpenSocket2(this ILogger<FooBarBaz> logger, string hostName, int ipAddress, Exception exception)
    {
        if (!logger.IsEnabled(LogLevel.Information)) return;
        logger.Log(LogLevel.Information, -1, new CouldNotOpenSocketState(hostName, ipAddress), exception, (state, ex) => state.ToString());
    }

    public static void CouldNotOpenSocketWithCaller(this ILogger<FooBarBaz> logger, string hostName, int ipAddress, [CallerLineNumber] int lineNumber = 0)
    {
        if (!logger.IsEnabled(LogLevel.Information)) return;
        logger.Log(LogLevel.Information, -1, new CouldNotOpenSocketState(hostName, ipAddress, lineNumber), null, (state, ex) => state.ToString());
    }

    [ZLoggerMessage(LogLevel.Information, "Could not open socket to {hostName} {ipAddress}.")]
    public static partial void CouldNotOpenSocket3(this ILogger<FooBarBaz> logger, string hostName, int ipAddress);

}

file readonly struct CouldNotOpenSocketState : IZLoggerFormattable,  IReadOnlyList<KeyValuePair<string, object?>>
{
    static readonly JsonEncodedText _jsonParameter_hostName = JsonEncodedText.Encode("hostName");
    static readonly JsonEncodedText _jsonParameter_ipAddress = JsonEncodedText.Encode("ipAddress");

    public string? CallerMemberName { get; }
    public string? CallerFilePath { get; }
    public int CallerLineNumber { get; }

    readonly string hostName;
    readonly int ipAddress;

    public CouldNotOpenSocketState(string hostName, int ipAddress, int callerLineNumber = 0)
    {
        this.hostName = hostName;
        this.ipAddress = ipAddress;
        CallerLineNumber = callerLineNumber;
    }

    public IZLoggerEntry CreateEntry(in LogInfo info)
    {
        return ZLoggerEntry<CouldNotOpenSocketState>.Create(info, this);
    }

    public int ParameterCount => 2;
    public bool IsSupportUtf8ParameterKey => true;

    public override string ToString() => $"Could not open socket to {hostName} {ipAddress}.";

    public void ToString(IBufferWriter<byte> writer)
    {
        var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(literalLength: 33, formattedCount: 2, bufferWriter: writer);

        stringWriter.AppendUtf8("Could not open socket to "u8);
        stringWriter.AppendFormatted(this.hostName, 0, null);
        stringWriter.AppendUtf8(" "u8);
        stringWriter.AppendFormatted(this.ipAddress, 0, null);

        // CodeGeneratorUtil.AppendAsJson(ref stringWriter, this.ipAddress);

        stringWriter.AppendUtf8("."u8);

        stringWriter.Flush();
    }

    // NOTE: keyNameMutator is only affects Interpolated String(perf reason).
    public void WriteJsonParameterKeyValues(Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions, IKeyNameMutator? keyNameMutator = null)
    {
        writer.WriteString(_jsonParameter_hostName, this.hostName);
        writer.WriteNumber(_jsonParameter_ipAddress, this.ipAddress);

        var datetime = DateTime.Now;
        
        // writer.WriteString(_jsonParameter_hostName, EnumLookup<LogLevel>.GetJsonEncodedName(LogLevel.Information));

        // writer.WritePropertyName(_jsonParameter_ipAddress); JsonSerializer.Serialize(writer, this.ipAddress, jsonSerializerOptions);
    }

    public ReadOnlySpan<byte> GetParameterKey(int index)
    {
        switch (index)
        {
            case 0: return "hostName"u8;
            case 1: return "ipAddress"u8;
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public string GetParameterKeyAsString(int index)
    {
        switch (index)
        {
            case 0: return "hostName";
            case 1: return "ipAddress";
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public object GetParameterValue(int index)
    {
        switch (index)
        {
            case 0: return this.hostName;
            case 1: return this.ipAddress;
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public T GetParameterValue<T>(int index)
    {
        switch (index)
        {
            case 0: return Unsafe.As<string, T>(ref Unsafe.AsRef(in this.hostName));
            case 1: return Unsafe.As<int, T>(ref Unsafe.AsRef(in this.ipAddress));
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public Type GetParameterType(int index)
    {
        switch (index)
        {
            case 0: return typeof(string);
            case 1: return typeof(int);
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public object? GetContext() => null;

    public int Count => 2;

    public KeyValuePair<string, object?> this[int index] => index switch
    {
        0 => new KeyValuePair<string, object?>("hostName", hostName),
        1 => new KeyValuePair<string, object?>("ipAddress", ipAddress),
        _ => throw new ArgumentOutOfRangeException()
    };
    
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public string GetOriginalFormat()
    {
        return "Could not open socket to {hostName} {ipAddress}.";
    }

    public void WriteOriginalFormat(IBufferWriter<byte> writer)
    {
        var format = "Could not open socket to {hostName} {ipAddress}."u8;
        writer.GetSpan(format.Length);
        writer.Write(format);
        writer.Advance(format.Length);
    }

    public (string? MemberName, string? FilePath, int LineNumber) GetCallerInfo()
    {
        throw new NotImplementedException();
    }

    struct Enumerator : IEnumerator<KeyValuePair<string, object?>>
    {
        int currentIndex;
        CouldNotOpenSocketState state;
        
        public Enumerator(CouldNotOpenSocketState state)
        {
            this.state = state;
            currentIndex = -1;
        }

        object IEnumerator.Current => Current;
        public bool MoveNext() => ++currentIndex < 2;
        public void Reset() => currentIndex = -1;

        public KeyValuePair<string, object?> Current => state[currentIndex];


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

file readonly struct SamplerState2 : IZLoggerFormattable
{
    static readonly JsonEncodedText _jsonParameter_banana = JsonEncodedText.Encode("banana");
    static readonly JsonEncodedText _jsonParameter_dt = JsonEncodedText.Encode("dt");

    readonly global::System.Guid? banana;
    readonly global::System.DateTime? dt;

    public SamplerState2(global::System.Guid? banana, global::System.DateTime? dt)
    {
        this.banana = banana;
        this.dt = dt;
    }

    public IZLoggerEntry CreateEntry(in LogInfo info)
    {
        return ZLoggerEntry<SamplerState2>.Create(info, this);
    }

    public int ParameterCount => 2;
    public bool IsSupportUtf8ParameterKey => true;
    public override string ToString() => $"Could not open socket to {banana} {dt}.";

    public void ToString(IBufferWriter<byte> writer)
    {
        var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(literalLength: 27, formattedCount: 2, bufferWriter: writer);

        stringWriter.AppendUtf8("Could not open socket to "u8);
        stringWriter.AppendFormatted(banana, 0, null);
        stringWriter.AppendUtf8(" "u8);
        stringWriter.AppendFormatted(dt, 0, null);
        stringWriter.AppendUtf8("."u8);

        stringWriter.Flush();
    }

    public void WriteJsonParameterKeyValues(Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions, IKeyNameMutator? keyNameMutator = null)
    {
        if (this.banana == null) { writer.WriteNull(_jsonParameter_banana); } else { writer.WriteString(_jsonParameter_banana, this.banana.Value); }
        if (this.dt == null) { writer.WriteNull(_jsonParameter_dt); } else { writer.WriteString(_jsonParameter_dt, this.dt.Value); }
    }

    public ReadOnlySpan<byte> GetParameterKey(int index)
    {
        switch (index)
        {
            case 0: return "banana"u8;
            case 1: return "dt"u8;
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public string GetParameterKeyAsString(int index)
    {
        switch (index)
        {
            case 0: return "banana";
            case 1: return "dt";
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public object GetParameterValue(int index)
    {
        switch (index)
        {
            case 0: return this.banana!;
            case 1: return this.dt!;
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public T GetParameterValue<T>(int index)
    {
        switch (index)
        {
            case 0: return Unsafe.As<global::System.Guid?, T>(ref Unsafe.AsRef(in this.banana));
            case 1: return Unsafe.As<global::System.DateTime?, T>(ref Unsafe.AsRef(in this.dt));
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public Type GetParameterType(int index)
    {
        switch (index)
        {
            case 0: return typeof(global::System.Guid?);
            case 1: return typeof(global::System.DateTime?);
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default!;
    }

    public object? GetContext() => null;

    public string GetOriginalFormat()
    {
        throw new NotImplementedException();
    }

    public void WriteOriginalFormat(IBufferWriter<byte> writer)
    {
        throw new NotImplementedException();
    }
}