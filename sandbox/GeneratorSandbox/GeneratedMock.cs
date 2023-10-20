using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Utf8StringInterpolation;
using ZLogger;
using ZLogger.Internal;

namespace GeneratorSandbox;



public static class Log22
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







}

file readonly struct CouldNotOpenSocketState : IZLoggerFormattable
{
    const int _parameterCount = 2;

    // TODO:
    const int _utf8LiteralLength = 33;
    const int _guessedParameterLength = 2 * 11;

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
    public int ParameterCount => _parameterCount;
    public bool IsSupportUtf8ParameterKey => true;
    public override string ToString() => "Could not open socket to {hostName} {ipAddress}."; // TODO:alignment, format

    public void ToString(IBufferWriter<byte> writer)
    {
        // chunk0.Length + chunk2.Length + chunk4.Length

        var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(_utf8LiteralLength, _guessedParameterLength, writer);

        stringWriter.AppendUtf8("Could not open socket to "u8);
        stringWriter.AppendFormatted(hostName); // TODO:alignment, format
        stringWriter.AppendUtf8(" "u8);
        stringWriter.AppendFormatted(ipAddress);
        stringWriter.AppendUtf8("."u8);

        stringWriter.Flush();
    }

    public void WriteJsonParameterKeyValues(Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions)
    {
        writer.WriteString(_jsonParameter_hostName, this.hostName);
        writer.WriteNumber(_jsonParameter_ipAddress, this.ipAddress);
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
            case 0: return Unsafe.As<string, T>(ref Unsafe.AsRef(this.hostName));
            case 1: return Unsafe.As<int, T>(ref Unsafe.AsRef(this.ipAddress));
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

