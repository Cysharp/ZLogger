using System;
using System.Buffers;
using System.Text;
using MessagePack;

using SerializerFoundation;

namespace ZLogger.MessagePack.Internal;

// MessagePack v4 writes strings only from string or from UTF-8 bytes, while key names reach the formatter as
// ReadOnlySpan<char> (for example, from IKeyNameMutator), so this encodes them to UTF-8 first.
internal static class WriteBufferExtensions
{
    public static void WriteString<TWriteBuffer>(
        this ref TWriteBuffer writeBuffer,
        scoped ReadOnlySpan<char> value)
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
            // Small names are encoded on the stack and larger ones in a pooled array, keeping stack usage small;
            // the 256-byte threshold follows SystemTextJsonZLoggerFormatter, which uses 256 characters for mutated key names.
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
