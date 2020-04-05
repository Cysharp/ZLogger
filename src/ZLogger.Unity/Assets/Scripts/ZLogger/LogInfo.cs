using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace ZLogger
{
    public readonly struct LogInfo
    {
        public readonly string CategoryName;
        public readonly DateTimeOffset Timestamp;
        public readonly LogLevel LogLevel;
        public readonly EventId EventId;
        public readonly Exception Exception;

        public LogInfo(string categoryName, DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, Exception exception)
        {
            EventId = eventId;
            CategoryName = categoryName;
            Timestamp = timestamp;
            LogLevel = logLevel;
            Exception = exception;
        }

        static readonly JsonEncodedText CategoryNameText = JsonEncodedText.Encode(nameof(CategoryName));
        static readonly JsonEncodedText TimestampText = JsonEncodedText.Encode(nameof(Timestamp));
        static readonly JsonEncodedText LogLevelText = JsonEncodedText.Encode(nameof(LogLevel));
        static readonly JsonEncodedText EventIdText = JsonEncodedText.Encode(nameof(EventId));
        static readonly JsonEncodedText EventIdNameText = JsonEncodedText.Encode("EventIdName");
        static readonly JsonEncodedText ExceptionText = JsonEncodedText.Encode(nameof(Exception));

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

        public void WriteToJsonWriter(Utf8JsonWriter writer)
        {
            writer.WriteString(CategoryNameText, CategoryName);
            writer.WriteString(LogLevelText, LogLevelToEncodedText(LogLevel));
            writer.WriteNumber(EventIdText, EventId.Id);
            writer.WriteString(EventIdNameText, EventId.Name);
            writer.WriteString(TimestampText, Timestamp);
            writer.WritePropertyName(ExceptionText);
            WriteException(writer, Exception);
        }

        static void WriteException(Utf8JsonWriter writer, Exception ex)
        {
            if (ex == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                {
                    writer.WriteString(NameText, ex.GetType().FullName);
                    writer.WriteString(MessageText, ex.Message);
                    writer.WriteString(StackTraceText, ex.StackTrace);
                    writer.WritePropertyName(InnerExceptionText);
                    {
                        WriteException(writer, ex.InnerException);
                    }
                }
                writer.WriteEndObject();
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