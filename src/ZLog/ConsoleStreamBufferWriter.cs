using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;


namespace ZLog
{
    internal class ConsoleStreamBufferWriter : IBufferWriter<byte>
    {
        readonly Stream consoleStream;

        byte[] buffer;
        byte[] defaultBuffer;
        int written;

        public ConsoleStreamBufferWriter(Stream consoleStream)
        {
            this.consoleStream = consoleStream;
            this.defaultBuffer = this.buffer = new byte[65536];
            this.written = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count)
        {
            written += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (buffer != defaultBuffer)
            {
                Flush();
            }

            if (written + sizeHint > buffer.Length)
            {
                Flush();
            }

            if (buffer.Length > sizeHint)
            {
                ArrayPool<byte>.Shared.Rent(sizeHint);
            }

            return new Memory<byte>(buffer, written, buffer.Length - written);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            return GetMemory(sizeHint).Span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetForNewLine([NotNullWhen(true)]out byte[]? rawBuffer, out int rawWritten)
        {
            if (buffer.Length - written > 2)
            {
                rawBuffer = buffer;
                rawWritten = written;
                return true;
            }
            else
            {
                rawBuffer = null;
                rawWritten = 0;
                return false;
            }
        }

        public void Flush()
        {
            if (written != 0)
            {
                // ConsolePal does not support async write, use Write(byte[]) API is most primitive.
                consoleStream.Write(buffer, 0, written);
                written = 0;

                if (buffer != defaultBuffer)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = defaultBuffer;
                }
            }
        }
    }
}