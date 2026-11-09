using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using ZLogger.Internal;

namespace ZLogger.Tests
{
    public class AsyncStreamLineMessageWriterTest
    {
        // AsyncStreamLineMessageWriter.AppendLine's fallback path (taken when StreamBufferWriter
        // has 2 bytes or fewer left in its 65536-byte buffer) used to call writer.Advance(2)
        // unconditionally, even though only newLine.Length bytes (1 on LF platforms such as
        // Linux/macOS) were actually copied into the span. That silently drifted the `written`
        // counter past the real buffer contents, so the very next Flush() attempts
        // stream.Write(buffer, 0, written) with written > buffer.Length, throwing
        // ArgumentOutOfRangeException - and once `written` is out of sync it never recovers,
        // i.e. permanent log loss (#237, root cause of #208).
        //
        // AppendLine is a private implementation detail, and reproducing the exact buffer
        // boundary through the public logging API is not deterministic: StreamBufferWriter gets
        // Flush()'d - resetting its position - at unpredictable points depending on how the
        // background write loop happens to batch-drain the channel relative to the producer. To
        // make the boundary deterministic, this test drives a real StreamBufferWriter to exactly
        // 1 byte short of full (the documented trigger condition for the fallback path) and
        // invokes the real (private) AppendLine method via reflection.
        [Fact]
        public void AppendLine_AtBufferBoundary_DoesNotCorruptWriterState_OnLFPlatforms()
        {
            if (Environment.NewLine.Length != 1)
            {
                return; // this regression is specific to single-byte-newline (LF) platforms; CI runs on ubuntu
            }

            const int BufferCapacity = 65536; // StreamBufferWriter's hardcoded default buffer size

            using var ms = new MemoryStream();
            using var idle = new MemoryStream(); // backs an otherwise-unused writer instance, just to borrow its private AppendLine

            var target = new AsyncStreamLineMessageWriter(idle, new ZLoggerOptions());
            try
            {
                var appendLine = typeof(AsyncStreamLineMessageWriter)
                    .GetMethod("AppendLine", BindingFlags.NonPublic | BindingFlags.Instance)!;

                var writer = new StreamBufferWriter(ms);

                // Prime the writer to exactly 1 byte short of full.
                var filler = writer.GetSpan(BufferCapacity - 1);
                filler.Slice(0, BufferCapacity - 1).Fill((byte)'x');
                writer.Advance(BufferCapacity - 1);

                // Exercises the real (fixed or unfixed) AppendLine implementation.
                appendLine.Invoke(target, new object[] { writer });

                // A subsequent ordinary write must still land correctly. Under the bug,
                // `written` silently exceeded the buffer's capacity above, so the very next
                // Flush() throws ArgumentOutOfRangeException and the whole batch - the 65535
                // filler bytes and the newline - is permanently lost.
                Action act = () =>
                {
                    var span = writer.GetSpan(1);
                    span[0] = (byte)'y';
                    writer.Advance(1);
                    writer.Flush();
                };

                act.Should().NotThrow<ArgumentOutOfRangeException>(
                    "a newline landing exactly at the buffer boundary must not corrupt StreamBufferWriter's internal position");

                var bytes = ms.ToArray();
                bytes.Length.Should().Be(BufferCapacity + 1, "65535 filler bytes + 1 newline byte + 1 trailing byte, none lost or duplicated");
                bytes[BufferCapacity - 1].Should().Be((byte)'\n', "AppendLine must add exactly one newline byte");
                bytes[BufferCapacity].Should().Be((byte)'y', "the write following the newline must not be shifted or corrupted");
            }
            finally
            {
                target.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
        }
    }
}
