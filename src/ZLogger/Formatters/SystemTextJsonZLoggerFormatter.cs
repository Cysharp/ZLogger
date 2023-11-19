using System.Buffers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;
using ZLogger.Formatters;
using ZLogger.Internal;

namespace ZLogger
{
    public static class ZLoggerOptionsSystemTextJsonExtensions
    {
        public static ZLoggerOptions UseJsonFormatter(this ZLoggerOptions options, Action<SystemTextJsonZLoggerFormatter>? jsonConfigure = null)
        {
            return options.UseFormatter(() =>
            {
                var formatter = new SystemTextJsonZLoggerFormatter();
                jsonConfigure?.Invoke(formatter);
                return formatter;
            });
        }
    }

    // use in JsonFormatter and MessagePackFormatter
    [Flags]
    public enum IncludeProperties
    {
        None = 0,
        Timestamp = 1 << 0,
        LogLevel = 1 << 1,
        CategoryName = 1 << 2,
        EventIdValue = 1 << 3,
        EventIdName = 1 << 4,
        Message = 1 << 5,
        Exception = 1 << 6,
        All = Timestamp | LogLevel | CategoryName | EventIdValue | EventIdName | Message | Exception
    }
}

namespace ZLogger.Formatters
{
    public class SystemTextJsonZLoggerFormatter : IZLoggerFormatter
    {
        static readonly JsonEncodedText CategoryNameText = JsonEncodedText.Encode(nameof(LogInfo.Category));
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

        public bool WithLineBreak => true;

        public JsonEncodedText MessagePropertyName { get; set; } = JsonEncodedText.Encode("Message");
        public Action<Utf8JsonWriter, LogInfo> LogInfoFormatter { get; set; }
        public IncludeProperties IncludeProperties { get; set; } = IncludeProperties.Timestamp | IncludeProperties.LogLevel | IncludeProperties.CategoryName | IncludeProperties.Message | IncludeProperties.Exception;

        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public IKeyNameMutator? KeyNameMutator { get; set; }

        Utf8JsonWriter? jsonWriter;

        public SystemTextJsonZLoggerFormatter()
        {
            LogInfoFormatter = DefaultLogInfoFormatter;
        }

        public void FormatLogEntry<TEntry>(IBufferWriter<byte> writer, TEntry entry) where TEntry : IZLoggerEntry
        {
            jsonWriter?.Reset(writer);
            jsonWriter ??= new Utf8JsonWriter(writer);

            jsonWriter.WriteStartObject();

            // LogInfo
            LogInfoFormatter.Invoke(jsonWriter, entry.LogInfo);

            // Message
            if ((IncludeProperties & IncludeProperties.Message) != 0)
            {
                var bufferWriter = ArrayBufferWriterPool.GetThreadStaticInstance();
                entry.ToString(bufferWriter);
                jsonWriter.WriteString(MessagePropertyName, bufferWriter.WrittenSpan);
            }

            // Scope
            if (entry.ScopeState is { IsEmpty: false } scopeState)
            {
                var properties = scopeState.Properties;
                for (var i = 0; i < properties.Length; i++)
                {
                    var x = properties[i];
                    // If `BeginScope(format, arg1, arg2)` style is used, the first argument `format` string is passed with this name
                    if (x.Key == "{OriginalFormat}") continue;

                    WriteMutatedJsonKeyName(x.Key.AsSpan(), jsonWriter, KeyNameMutator);

                    if (x.Value is { } value)
                    {
                        JsonSerializer.Serialize(jsonWriter, value, JsonSerializerOptions);
                    }
                    else
                    {
                        jsonWriter.WriteNullValue();
                    }
                }
            }

            // Params
            entry.WriteJsonParameterKeyValues(jsonWriter, JsonSerializerOptions, KeyNameMutator);

            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
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

        public void DefaultLogInfoFormatter(Utf8JsonWriter jsonWriter, LogInfo info)
        {
            var flag = IncludeProperties;
            if ((flag & IncludeProperties.Timestamp) != 0)
            {
                jsonWriter.WriteString(TimestampText, info.Timestamp.Local); // use Local
            }
            if ((flag & IncludeProperties.LogLevel) != 0)
            {
                jsonWriter.WriteString(LogLevelText, LogLevelToEncodedText(info.LogLevel));
            }
            if ((flag & IncludeProperties.CategoryName) != 0)
            {
                jsonWriter.WriteString(CategoryNameText, info.Category.JsonEncoded);
            }
            if ((flag & IncludeProperties.EventIdValue) != 0)
            {
                jsonWriter.WriteNumber(EventIdText, info.EventId.Id);
            }
            if ((flag & IncludeProperties.EventIdName) != 0)
            {
                jsonWriter.WriteString(EventIdNameText, info.EventId.Name);
            }
            if ((flag & IncludeProperties.Exception) != 0)
            {
                if (info.Exception is { } ex)
                {
                    jsonWriter.WritePropertyName(ExceptionText);
                    WriteException(jsonWriter, ex);
                }
            }
        }

        public static void WriteException(Utf8JsonWriter jsonWriter, Exception? ex)
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
                        WriteException(jsonWriter, ex.InnerException);
                    }
                }
                jsonWriter.WriteEndObject();
            }
        }

        public static void WriteMutatedJsonKeyName(ReadOnlySpan<char> keyName, Utf8JsonWriter jsonWriter, IKeyNameMutator? mutator = null)
        {
            if (mutator == null)
            {
                jsonWriter.WritePropertyName(keyName);
                return;
            }

            var bufferSize = keyName.Length;
            while (!TryMutate(keyName, bufferSize))
            {
                bufferSize *= 2;
            }
            return;

            bool TryMutate(ReadOnlySpan<char> source, int bufferSize)
            {
                if (bufferSize > 256)
                {
                    var buffer = new char[bufferSize];
                    if (mutator.TryMutate(source, buffer, out var written))
                    {
                        jsonWriter.WritePropertyName(buffer.AsSpan(0, written));
                        return true;
                    }
                }
                else
                {
                    Span<char> buffer = stackalloc char[bufferSize];
                    if (mutator.TryMutate(source, buffer, out var written))
                    {
                        jsonWriter.WritePropertyName(buffer[..written]);
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
