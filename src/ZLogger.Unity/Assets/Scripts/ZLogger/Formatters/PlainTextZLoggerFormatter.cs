using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace ZLogger.Formatters
{
    public class PlainTextZLoggerFormatter : IZLoggerFormatter
    {
        public Action<IBufferWriter<byte>, LogInfo>? PrefixFormatter { get; set; }
        public Action<IBufferWriter<byte>, LogInfo>? SuffixFormatter { get; set; }
        public Action<IBufferWriter<byte>, Exception> ExceptionFormatter { get; set; } = DefaultExceptionLoggingFormatter;

        static byte[] newLine = Encoding.UTF8.GetBytes(Environment.NewLine);

        public void FormatLogEntry<TEntry, TPayload>(
            IBufferWriter<byte> writer,
            TEntry entry,
            TPayload? payload,
            ReadOnlySpan<byte> utf8Message)
            where TEntry : IZLoggerEntry
        {
            PrefixFormatter?.Invoke(writer, entry.LogInfo);
            Write(writer, utf8Message);
            SuffixFormatter?.Invoke(writer, entry.LogInfo);

            if (entry.LogInfo.Exception is { } ex)
            {
                ExceptionFormatter(writer, ex);
            }
        }
        
        static void DefaultExceptionLoggingFormatter(IBufferWriter<byte> writer, Exception exception)
        {
            // \n + exception
            Write(writer, Environment.NewLine);
            WriteExceptionLoggingCore(writer, exception);
        }

        static void Write(IBufferWriter<byte> writer, ReadOnlySpan<byte> value)
        {
            var span = writer.GetSpan(value.Length);
            value.CopyTo(span);
            writer.Advance(value.Length);
        }

        static void WriteExceptionLoggingCore(IBufferWriter<byte> writer, Exception exception)
        {
            // className: message
            //  ---> InnerException
            // InnerException StackTrace
            // --- End of inner exception stack trace ---
            // StackTrace

            var className = exception.GetType().FullName;
            var message = exception.Message;
            var innerException = exception.InnerException;
            var stackTrace = exception.StackTrace; // allocating stacktrace string but okay(shoganai).

            Write(writer, className!, ": ", message ?? "");
            if (innerException != null)
            {
                Write(writer, Environment.NewLine, " ---> ");
                WriteExceptionLoggingCore(writer, innerException);
                Write(writer, Environment.NewLine, "   --- End of inner exception stack trace ---");
            }

            if (stackTrace != null)
            {
                Write(writer, Environment.NewLine, stackTrace);
            }
        }

        static void Write(IBufferWriter<byte> writer, string message)
        {
            var memory = writer.GetMemory(Encoding.UTF8.GetMaxByteCount(message.Length));
            if (MemoryMarshal.TryGetArray<byte>(memory, out var array) && array.Array != null)
            {
                var written = Encoding.UTF8.GetBytes(message, 0, message.Length, array.Array, array.Offset);
                writer.Advance(written);
            }
        }

        static void Write(IBufferWriter<byte> writer, string message1, string message2)
        {
            var memory = writer.GetMemory(Encoding.UTF8.GetMaxByteCount(message1.Length + message2.Length));
            if (MemoryMarshal.TryGetArray<byte>(memory, out var array) && array.Array != null)
            {
                var written1 = Encoding.UTF8.GetBytes(message1, 0, message1.Length, array.Array, array.Offset);
                var written2 = Encoding.UTF8.GetBytes(message2, 0, message2.Length, array.Array, array.Offset + written1);
                writer.Advance(written1 + written2);
            }
        }

        static void Write(IBufferWriter<byte> writer, string message1, string message2, string message3)
        {
            var memory = writer.GetMemory(Encoding.UTF8.GetMaxByteCount(message1.Length + message2.Length + message3.Length));
            if (MemoryMarshal.TryGetArray<byte>(memory, out var array) && array.Array != null)
            {
                var written1 = Encoding.UTF8.GetBytes(message1, 0, message1.Length, array.Array, array.Offset);
                var written2 = Encoding.UTF8.GetBytes(message2, 0, message2.Length, array.Array, array.Offset + written1);
                var written3 = Encoding.UTF8.GetBytes(message3, 0, message3.Length, array.Array, array.Offset + written1 + written2);
                writer.Advance(written1 + written2 + written3);
            }
        }
    }
}
