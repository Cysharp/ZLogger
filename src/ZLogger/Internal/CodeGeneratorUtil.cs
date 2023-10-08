using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace ZLogger.Internal
{
    // Used for Code Generator so "public".
    public static class CodeGeneratorUtil
    {
        public static readonly JsonEncodedText JsonEncodedMessage = JsonEncodedText.Encode("Message");

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        [ThreadStatic]
        static ArrayBufferWriter<byte>? bufferWriter;

        public static ArrayBufferWriter<byte> GetThreadStaticArrayBufferWriter()
        {
            var writer = bufferWriter;
            if (writer == null)
            {
                writer = bufferWriter = new ArrayBufferWriter<byte>();
            }
            writer.Clear();
            return writer;
        }
#endif

        public static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException();
        }

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
        public static void WriteValue(IBufferWriter<byte> writer, ReadOnlySpan<byte> src, ref Span<byte> dest, ref int destWritten)
        {
            if (src.Length == 0) return;

            if (dest.Length < src.Length)
            {
                if (destWritten != 0)
                {
                    writer.Advance(destWritten);
                    destWritten = 0;
                }
                dest = writer.GetSpan(src.Length);
            }

            src.CopyTo(dest);
            var written = src.Length;
            dest = dest.Slice(written);
            destWritten += written;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteValue(IBufferWriter<byte> writer, string? src, ref Span<byte> dest, ref int destWritten)
        {
            if (src == null || src.Length == 0) return;

            var max = Encoding.UTF8.GetMaxByteCount(src.Length);
            if (dest.Length < max)
            {
                if (destWritten != 0)
                {
                    writer.Advance(destWritten);
                    destWritten = 0;
                }
                dest = writer.GetSpan(max);
            }

            var written = Encoding.UTF8.GetBytes(src, dest);
            dest = dest.Slice(written);
            destWritten += written;
        }

        public static void WriteValue(IBufferWriter<byte> writer, int src, ref Span<byte> dest, ref int destWritten)
        {
            var written = 0;
            while (!Utf8Formatter.TryFormat(src, dest, out written))
            {
                if (destWritten != 0)
                {
                    writer.Advance(destWritten);
                    destWritten = 0;
                }
                dest = writer.GetSpan(dest.Length * 2);
            }

            dest = dest.Slice(written);
            destWritten += written;
        }

        public static void WriteValue<T>(IBufferWriter<byte> writer, T src, ref Span<byte> dest, ref int destWritten)
        {
            var str = src?.ToString();
            WriteValue(writer, str, ref dest, ref destWritten);
        }
    }
}