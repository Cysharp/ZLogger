using System;
using System.Buffers;
using System.Text;
using MessagePack;

namespace SerializerFoundation;

internal static class WriteBufferExtensions
{
    public static void WriteString<TWriteBuffer>(
        this ref TWriteBuffer writeBuffer,
        ReadOnlySpan<char> value)
        where TWriteBuffer : struct, IWriteBuffer

#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        var encoding = Encoding.UTF8;
        var maxLength = encoding.GetMaxByteCount(value.Length);

        var pool = ArrayPool<byte>.Shared;
        byte[]? rentedArray = null;

        try
        {
            Span<byte> buffer = maxLength >= 256 ? rentedArray = pool.Rent(maxLength) : stackalloc byte[maxLength];
            var actualLength = encoding.GetBytes(value, buffer);

            writeBuffer.WriteString(buffer[..actualLength]);
        }
        finally
        {
            if (rentedArray is not null)
            {
                pool.Return(rentedArray);
            }
        }
    }
}
