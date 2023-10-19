using System.Buffers;
using System.Text;

namespace ZLogger.Internal
{
    internal static class BufferWriterUtils
    {
        static readonly byte[] newLine = Encoding.UTF8.GetBytes(Environment.NewLine);
        static readonly byte newLineBytes1 = newLine[0];
        static readonly byte newLineBytes2 = newLine.Length >= 2 ? newLine[1] : default;

        public static void WriteNewLine(IBufferWriter<byte> writer)
        {
            var span = writer.GetSpan(newLine.Length);
            span[0] = newLineBytes1;
            if (newLine.Length > 1)
            {
                span[1] = newLineBytes2;
            }
            writer.Advance(newLine.Length);
        }
    }
}
