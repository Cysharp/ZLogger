using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Buffers;
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



    [ZLoggerMessage(LogLevel.Information, "Could not open socket to {hostName} {ipAddress}.")]
    public static partial void CouldNotOpenSocket3(this ILogger<FooBarBaz> logger, string hostName, int ipAddress);




}

file readonly struct CouldNotOpenSocketState : IZLoggerFormattable
{
    static readonly JsonEncodedText _jsonParameter_hostName = JsonEncodedText.Encode("hostName");
    static readonly JsonEncodedText _jsonParameter_ipAddress = JsonEncodedText.Encode("ipAddress");

    readonly string hostName;
    readonly int ipAddress;

    public CouldNotOpenSocketState(string hostName, int ipAddress)
    {
        this.hostName = hostName;
        this.ipAddress = ipAddress;
    }

    public IZLoggerEntry CreateEntry(LogInfo info)
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

    public ReadOnlySpan<char> GetParameterKeyAsString(int index)
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

}

