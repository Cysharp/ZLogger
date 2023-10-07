using System;
using System.Buffers;
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
    }
}