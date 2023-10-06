using MessagePack;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSandbox;


public interface IStructuredKeyValueWriter<T>
{
    void WriteKey(ReadOnlySpan<byte> key);
    void WriteValue<T>(T value);
}


readonly struct CouldNotOpenSocketState
{
    const int ParameterCount = 2;

    readonly string hostName;
    readonly int ipAddress;


    public CouldNotOpenSocketState(string hostName, int ipAddress)
    {
        this.hostName = hostName;
        this.ipAddress = ipAddress;
    }

    public void ToString(IBufferWriter<byte> writer)
    {
        var chunk1 = "Could not open socket to "u8;
        var chunk2 = " "u8;
        var chunk3 = "."u8;

        var dest = writer.GetSpan(chunk1.Length + chunk2.Length + chunk3.Length + Util.GuessedParameterByteCount(ParameterCount - 1) + Util.GetStringMaxByteCount(hostName));
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

    public override string ToString()
    {
        return $"Could not open socket to {hostName} {ipAddress}.";
    }



    //public void WriteKeyValue<T>(T writer)
    //{
    //    writer.WriteKey("hostName"u8);
    //    writer.WriteValue(hostName);
    //    writer.WriteKey("ipAddress"u8);
    //    writer.WriteValue(ipAddress);
    //}


    //public void WriteKeyValue(ref MessagePackWriter writer)
    //{
    //    writer.WriteKey("hostName"u8);
    //    writer.WriteValue(hostName);
    //    writer.WriteKey("ipAddress"u8);
    //    writer.WriteValue(ipAddress);
    //}
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