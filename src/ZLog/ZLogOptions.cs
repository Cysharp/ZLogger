using System;
using System.Buffers;
using System.Text.Json;
using System.Threading;

namespace ZLog
{
    public class ZLogOptions
    {
        public Action<IBufferWriter<byte>, LogInfo>? PrefixFormatter { get; set; }
        public Action<IBufferWriter<byte>, LogInfo>? SuffixFormatter { get; set; }
        public bool RequireJavaScriptEncode { get; set; }
        public Action<Exception>? ErrorLogger { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public void UseDefaultStructuredLogFormatter()
        {
            RequireJavaScriptEncode = true;
            PrefixFormatter = DefaultStructuredLogPrefixFormatter;
            SuffixFormatter = DefaultStructuredLogSuffixFormatter;
        }

        [ThreadStatic]
        static Utf8JsonWriter? jsonWriter;

        static void DefaultStructuredLogPrefixFormatter(IBufferWriter<byte> buffer, LogInfo info)
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

            writer.WriteStartObject();
            info.WriteToJsonWriter(ref writer);

            writer.WritePropertyName("Payload");
            writer.Flush();
            if (!info.IsJson)
            {
                // payload is string, add quotation
                var span = buffer.GetSpan(1);
                span[0] = (byte)'\"';
                buffer.Advance(1);
            }

            writer.Reset();
        }

        static void DefaultStructuredLogSuffixFormatter(IBufferWriter<byte> buffer, LogInfo info)
        {
            if (!info.IsJson)
            {
                var span = buffer.GetSpan(2);
                span[0] = (byte)'\"';
                span[1] = (byte)'}'; // WriteEndObject
                buffer.Advance(2);
            }
            else
            {
                var span = buffer.GetSpan(1);
                span[0] = (byte)'}'; // WriteEndObject
                buffer.Advance(1);
            }
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
