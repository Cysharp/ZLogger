using Cysharp.Text;
using MessagePack;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZLogger;
using ZLogger.Internal;

namespace GeneratorSandbox;


public interface IStructuredKeyValueWriter
{
    void WriteKey(ReadOnlySpan<byte> key);
    void WriteValue<T>(T value);
}


public interface IZLoggerFormattable
{
    int ParameterCount { get; }
    string ToString();
    void ToString(IBufferWriter<byte> writer);
    void WriteJsonMessageString(Utf8JsonWriter writer);
    void WriteJsonParameterKeyValues(Utf8JsonWriter writer);
    ReadOnlySpan<byte> GetParameterKey(int index);
    object GetParameterValue(int index);
    T GetParameterValue<T>(int index);
    Type GetParameterType(int index);
}

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

        var dest = writer.GetSpan(chunk1.Length + chunk2.Length + chunk3.Length + Util.GuessedParameterByteCount(Count - 1) + Util.GetStringMaxByteCount(hostName));
        var count = 0;

        Util.WriteValue(writer, chunk1, ref dest, ref count);
        Util.WriteValue(writer, hostName, ref dest, ref count);
        Util.WriteValue(writer, chunk2, ref dest, ref count);
        Util.WriteValue(writer, ipAddress, ref dest, ref count);
        Util.WriteValue(writer, chunk3, ref dest, ref count);

        if (count != 0)
        {
            writer.Advance(count);
        }
    }

    public override string ToString() => $"Could not open socket to {hostName} {ipAddress}.";

    public void WriteJsonMessageString(Utf8JsonWriter writer)
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

file static class Util
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GuessedParameterByteCount(int parameterCount)
    {
        return parameterCount * 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetStringMaxByteCount(string? str)
    {
        return (str == null) ? 0 : Encoding.UTF8.GetMaxByteCount(str.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteValue(IBufferWriter<byte> writer, ReadOnlySpan<byte> src, ref Span<byte> dest, ref int count)
    {
        if (src.Length == 0) return;

        if (dest.Length < src.Length)
        {
            if (count != 0)
            {
                writer.Advance(count);
                count = 0;
            }
            dest = writer.GetSpan(src.Length);
        }

        src.CopyTo(dest);
        var written = src.Length;
        dest = dest.Slice(written);
        count += written;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteValue(IBufferWriter<byte> writer, string? src, ref Span<byte> dest, ref int count)
    {
        if (src == null || src.Length == 0) return;

        var max = Encoding.UTF8.GetMaxByteCount(src.Length);
        if (dest.Length < max)
        {
            if (count != 0)
            {
                writer.Advance(count);
                count = 0;
            }
            dest = writer.GetSpan(max);
        }

        var written = Encoding.UTF8.GetBytes(src, dest);
        dest = dest.Slice(written);
        count += written;
    }

    public static void WriteValue(IBufferWriter<byte> writer, int src, ref Span<byte> dest, ref int count)
    {
        var written = 0;
        while (!Utf8Formatter.TryFormat(src, dest, out written))
        {
            if (count != 0)
            {
                writer.Advance(count);
                count = 0;
            }
            dest = writer.GetSpan(dest.Length * 2);
        }

        dest = dest.Slice(written);
        count += written;
    }

    public static void WriteValue<T>(IBufferWriter<byte> writer, T src, ref Span<byte> dest, ref int count)
    {
        var str = src?.ToString();
        WriteValue(writer, str, ref dest, ref count);
    }
}