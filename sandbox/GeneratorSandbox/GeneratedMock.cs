using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ZLogger;
using ZLogger.Internal;

namespace GeneratorSandbox;



public static class Log22
{
    // [ZLoggerMessage(Information, "Could not open socket to {hostName} {ipAddress}.")]
    public static void CouldNotOpenSocket(this ILogger<FooBarBaz> logger, string hostName,  int ipAddress)
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
    const int Count = 2;
    static readonly JsonEncodedText jsonParameter1 = JsonEncodedText.Encode("hostName");
    static readonly JsonEncodedText jsonParameter2 = JsonEncodedText.Encode("ipAddress");

    readonly string hostName;
    readonly int ipAddress;

    public CouldNotOpenSocketState(string hostName, int ipAddress)
    {
        this.hostName = hostName;
        this.ipAddress = ipAddress;
    }

    public int ParameterCount => Count;

    public void ToString(IBufferWriter<byte> writer)
    {
        var chunk1 = "Could not open socket to "u8;
        var chunk2 = " "u8;
        var chunk3 = "."u8;

        var dest = writer.GetSpan(chunk1.Length + chunk2.Length + chunk3.Length + CodeGeneratorUtil.GuessedParameterByteCount(Count - 1) + CodeGeneratorUtil.GetStringMaxByteCount(hostName));
        var count = 0;

        CodeGeneratorUtil.WriteValue(writer, chunk1, ref dest, ref count);
        CodeGeneratorUtil.WriteValue(writer, hostName, ref dest, ref count);
        CodeGeneratorUtil.WriteValue(writer, chunk2, ref dest, ref count);
        CodeGeneratorUtil.WriteValue(writer, ipAddress, ref dest, ref count);
        CodeGeneratorUtil.WriteValue(writer, chunk3, ref dest, ref count);

        if (count != 0)
        {
            writer.Advance(count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void WriteValues(IBufferWriter<byte> writer, int[] src, ref Span<byte> dest, ref int count)
    {
        if (src == null) return;

        var first = true;
        foreach (var item in src)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                CodeGeneratorUtil.WriteValue(writer, ", "u8, ref dest, ref count);
            }
            CodeGeneratorUtil.WriteValue(writer, item, ref dest, ref count);
        }
    }

    public override string ToString() => $"Could not open socket to {hostName} {ipAddress}.";

    public void WriteJsonMessage(Utf8JsonWriter writer)
    {
        var bufferWriter = CodeGeneratorUtil.GetThreadStaticArrayBufferWriter();
        ToString(bufferWriter);
        writer.WriteString(CodeGeneratorUtil.JsonEncodedMessage, bufferWriter.WrittenSpan);
    }

    public void WriteJsonParameterKeyValues(Utf8JsonWriter writer)
    {
        writer.WriteString(jsonParameter1, hostName);
        writer.WriteNumber(jsonParameter2, ipAddress);
    }

    public ReadOnlySpan<byte> GetParameterKey(int index)
    {
        switch (index)
        {
            case 0: return "hostName"u8;
            case 1: return "ipAddress"u8;
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return default;
    }

    public object GetParameterValue(int index)
    {
        switch (index)
        {
            case 0: return hostName;
            case 1: return ipAddress;
        }
        CodeGeneratorUtil.ThrowArgumentOutOfRangeException();
        return null!;
    }

    public T GetParameterValue<T>(int index)
    {
        switch (index)
        {
            case 0: return Unsafe.As<string, T>(ref Unsafe.AsRef(hostName));
            case 1: return Unsafe.As<int, T>(ref Unsafe.AsRef(ipAddress));
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

