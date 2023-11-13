using System.Buffers;
using System.Text;

namespace ZLogger.Formatters
{
    public class PlainTextZLoggerFormatter : IZLoggerFormatter
    {
        public Action<IBufferWriter<byte>, LogInfo>? PrefixFormatter { get; set; }
        public Action<IBufferWriter<byte>, LogInfo>? SuffixFormatter { get; set; }
        public Action<IBufferWriter<byte>, Exception> ExceptionFormatter { get; set; } = DefaultExceptionLoggingFormatter;

        static byte[] newLine = Encoding.UTF8.GetBytes(Environment.NewLine);

        public void FormatLogEntry<TEntry>(IBufferWriter<byte> writer, TEntry entry) where TEntry : IZLoggerEntry
        {
            PrefixFormatter?.Invoke(writer, entry.LogInfo);
            entry.ToString(writer);
            SuffixFormatter?.Invoke(writer, entry.LogInfo);

            if (entry.LogInfo.Exception is { } ex)
            {
                ExceptionFormatter(writer, ex);
            }
        }

        static void DefaultExceptionLoggingFormatter(IBufferWriter<byte> writer, Exception exception)
        {
            // \n + exception
            Write(writer, newLine);
            WriteExceptionLoggingCore(writer, exception);
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
                Write(writer, newLine, " ---> ");
                WriteExceptionLoggingCore(writer, innerException);
                Write(writer, newLine, "   --- End of inner exception stack trace ---");
            }

            if (stackTrace != null)
            {
                Write(writer, newLine, stackTrace);
            }
        }

        static void Write(IBufferWriter<byte> writer, ReadOnlySpan<byte> value)
        {
            var span = writer.GetSpan(value.Length);
            value.CopyTo(span);
            writer.Advance(value.Length);
        }

        static void Write(IBufferWriter<byte> writer, ReadOnlySpan<byte> message1, string message2)
        {
            var span = writer.GetSpan(message1.Length + Encoding.UTF8.GetMaxByteCount(message2.Length));
            message1.CopyTo(span);
            var written2 = Encoding.UTF8.GetBytes(message2, span.Slice(message1.Length));
            writer.Advance(message1.Length + written2);
        }

        static void Write(IBufferWriter<byte> writer, string message1, string message2, string message3)
        {
            var span = writer.GetSpan(Encoding.UTF8.GetMaxByteCount(message1.Length + message2.Length + message3.Length));

            var written1 = Encoding.UTF8.GetBytes(message1, span);
            var written2 = Encoding.UTF8.GetBytes(message2, span.Slice(written1));
            var written3 = Encoding.UTF8.GetBytes(message3, span.Slice(written1 + written2));
            writer.Advance(written1 + written2 + written3);
        }
    }
}
