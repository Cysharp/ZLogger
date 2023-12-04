using System;
using System.Buffers;
using System.Collections.Concurrent;

namespace ZLogger.Internal
{
    internal static class ArrayBufferWriterPool
    {
        [ThreadStatic]
        static ArrayBufferWriter<byte>? bufferWriter;

        static ConcurrentQueue<ArrayBufferWriter<byte>> cache = new();

        public static ArrayBufferWriter<byte> GetThreadStaticInstance()
        {
            var writer = bufferWriter;
            if (writer == null)
            {
                writer = bufferWriter = new ArrayBufferWriter<byte>();
            }
#if NET8_0_OR_GREATER
            writer.ResetWrittenCount();
#else
            writer.Clear();
#endif
            return writer;
        }

        public static ArrayBufferWriter<byte> Rent()
        {
            if (cache.TryDequeue(out var writer))
            {
                return writer;
            }
            return new ArrayBufferWriter<byte>(256);
        }

        public static void Return(ArrayBufferWriter<byte> writer)
        {
#if NET8_0_OR_GREATER
        writer.ResetWrittenCount();
#else
            writer.Clear();
#endif
            cache.Enqueue(writer);
        }
    }
}
