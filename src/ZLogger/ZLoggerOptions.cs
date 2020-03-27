using System;
using System.Buffers;
using System.Text.Json;
using System.Threading;

namespace ZLogger
{
    public class ZLoggerOptions
    {
        public Action<Exception>? ErrorLogger { get; set; }
        public CancellationToken CancellationToken { get; set; }

        // Options for Text logging
        public Action<IBufferWriter<byte>, LogInfo>? PrefixFormatter { get; set; }
        public Action<IBufferWriter<byte>, LogInfo>? SuffixFormatter { get; set; }

        // Options for Structured Logging
        public bool IsStructuredLogging { get; set; }
        public Action<Utf8JsonWriter, LogInfo>? StructuredLoggingFormatter { get; set; }
        public JsonEncodedText MessagePropertyName { get; set; } = JsonEncodedText.Encode("Message");
        public JsonEncodedText PayloadPropertyName { get; set; } = JsonEncodedText.Encode("Payload");

        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            WriteIndented = false,
            IgnoreNullValues = false,
        };

        public void UseDefaultStructuredLogFormatter()
        {
            IsStructuredLogging = true;
            StructuredLoggingFormatter = DefaultStructuredLoggingFormatter;
        }

        [ThreadStatic]
        static Utf8JsonWriter? jsonWriter;

        internal Utf8JsonWriter GetThradStaticUtf8JsonWriter(IBufferWriter<byte> buffer)
        {
            Utf8JsonWriter writer;
            if (jsonWriter == null)
            {
                writer = jsonWriter = new Utf8JsonWriter(buffer, new JsonWriterOptions { Indented = false, SkipValidation = true });
            }
            else
            {
                writer = jsonWriter;
                writer.Reset(buffer);
            }

            return writer;
        }

        static void DefaultStructuredLoggingFormatter(Utf8JsonWriter writer, LogInfo info)
        {
            info.WriteToJsonWriter(ref writer);
        }

        internal void LogException(Exception ex)
        {
            if (ErrorLogger == null)
            {
                Console.WriteLine(ex);
            }
            else
            {
                ErrorLogger(ex);
            }
        }
    }
}
