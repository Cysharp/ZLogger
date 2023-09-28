using System;
using System.Buffers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;

namespace ZLogger.Formatters
{
    public static class ZLoggerOptionsSystemTextJsonExtensions
    {
        public static ZLoggerOptions UseJsonFormatter(this ZLoggerOptions options, Action<SystemTextJsonZLogFormatter> jsonConfigure = null)
        {
            return options.UseFormatter(writer =>
            {
                var formatter = new SystemTextJsonZLogFormatter(writer);
                jsonConfigure?.Invoke(formatter);
                return formatter;
            });
        }
    }
    
    public class SystemTextJsonZLogFormatter : IZLoggerFormatter
    {
        static readonly JsonEncodedText CategoryNameText = JsonEncodedText.Encode(nameof(LogInfo.CategoryName));
        static readonly JsonEncodedText TimestampText = JsonEncodedText.Encode(nameof(LogInfo.Timestamp));
        static readonly JsonEncodedText LogLevelText = JsonEncodedText.Encode(nameof(LogInfo.LogLevel));
        static readonly JsonEncodedText EventIdText = JsonEncodedText.Encode(nameof(LogInfo.EventId));
        static readonly JsonEncodedText EventIdNameText = JsonEncodedText.Encode("EventIdName");
        static readonly JsonEncodedText ExceptionText = JsonEncodedText.Encode(nameof(LogInfo.Exception));

        static readonly JsonEncodedText NameText = JsonEncodedText.Encode("Name");
        static readonly JsonEncodedText MessageText = JsonEncodedText.Encode("Message");
        static readonly JsonEncodedText StackTraceText = JsonEncodedText.Encode("StackTrace");
        static readonly JsonEncodedText InnerExceptionText = JsonEncodedText.Encode("InnerException");

        static readonly JsonEncodedText Trace = JsonEncodedText.Encode(nameof(LogLevel.Trace));
        static readonly JsonEncodedText Debug = JsonEncodedText.Encode(nameof(LogLevel.Debug));
        static readonly JsonEncodedText Information = JsonEncodedText.Encode(nameof(LogLevel.Information));
        static readonly JsonEncodedText Warning = JsonEncodedText.Encode(nameof(LogLevel.Warning));
        static readonly JsonEncodedText Error = JsonEncodedText.Encode(nameof(LogLevel.Error));
        static readonly JsonEncodedText Critical = JsonEncodedText.Encode(nameof(LogLevel.Critical));
        static readonly JsonEncodedText None = JsonEncodedText.Encode(nameof(LogLevel.None));
        
        public JsonEncodedText MessagePropertyName { get; set; } = JsonEncodedText.Encode("Message");
        public JsonEncodedText PayloadPropertyName { get; set; } = JsonEncodedText.Encode("Payload");

        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public SystemTextJsonZLogFormatter(IBufferWriter<byte> writer)
        {
            jsonWriter = new Utf8JsonWriter(writer);
        }

        readonly Utf8JsonWriter jsonWriter;
        
        
        public void WriteLogEntry<TEntry, TPayload>(TEntry entry, TPayload payload, ReadOnlySpan<byte> utf8Message, ZLoggerOptions options) where TEntry : IZLoggerEntry
        {
            try
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString(CategoryNameText, entry.LogInfo.CategoryName);
                jsonWriter.WriteString(LogLevelText, LogLevelToEncodedText(entry.LogInfo.LogLevel));
                jsonWriter.WriteNumber(EventIdText, entry.LogInfo.EventId.Id);
                jsonWriter.WriteString(EventIdNameText, entry.LogInfo.EventId.Name);
                jsonWriter.WriteString(TimestampText, entry.LogInfo.Timestamp);

                jsonWriter.WriteString(MessagePropertyName, utf8Message);

                if (payload != null)
                {
                    jsonWriter.WritePropertyName(PayloadPropertyName);
                    JsonSerializer.Serialize(jsonWriter, payload, JsonSerializerOptions);
                }

                // Write Exception
                if (entry.LogInfo.Exception is { } ex)
                {
                    WriteException(ex);
                }
                jsonWriter.WriteEndObject();
                jsonWriter.Flush();
            }
            finally
            {
                jsonWriter.Reset();
            }
        }
        
        void WriteException(Exception ex)
        {
            if (ex == null)
            {
                jsonWriter.WriteNullValue();
            }
            else
            {
                jsonWriter.WriteStartObject();
                {
                    jsonWriter.WriteString(NameText, ex.GetType().FullName);
                    jsonWriter.WriteString(MessageText, ex.Message);
                    jsonWriter.WriteString(StackTraceText, ex.StackTrace);
                    jsonWriter.WritePropertyName(InnerExceptionText);
                    {
                        WriteException(ex.InnerException);
                    }
                }
                jsonWriter.WriteEndObject();
            }
        }
        
        static JsonEncodedText LogLevelToEncodedText(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return Trace;
                case LogLevel.Debug:
                    return Debug;
                case LogLevel.Information:
                    return Information;
                case LogLevel.Warning:
                    return Warning;
                case LogLevel.Error:
                    return Error;
                case LogLevel.Critical:
                    return Critical;
                case LogLevel.None:
                    return None;
                default:
                    return JsonEncodedText.Encode(((int)logLevel).ToString());
            }
        }
   }
}
