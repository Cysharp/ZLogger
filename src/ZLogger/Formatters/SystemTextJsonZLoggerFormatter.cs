using System.Buffers;
using System.Diagnostics.CodeAnalysis;
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
        public static ZLoggerOptions UseJsonFormatter(this ZLoggerOptions options, Action<SystemTextJsonZLoggerFormatter>? configure = null)
        {
            return options.UseFormatter(() =>
            {
                var formatter = new SystemTextJsonZLoggerFormatter();
                configure?.Invoke(formatter);
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
        ScopeKeyValues = 1 << 7,
        ParameterKeyValues = 1 << 8,
        MemberName = 1 << 9, // added in 2.2.0
        FilePath = 1 << 10, // added in 2.2.0
        LineNumber = 1 << 11, // added in 2.2.0

        Default = Timestamp | LogLevel | CategoryName | Message | Exception | ScopeKeyValues | ParameterKeyValues,
        All = Timestamp | LogLevel | CategoryName | EventIdValue | EventIdName | Message | Exception | ScopeKeyValues | ParameterKeyValues | MemberName | FilePath | LineNumber
    }
}

namespace ZLogger.Formatters
{
    public record JsonPropertyNames(
        JsonEncodedText Category,
        JsonEncodedText Timestamp,
        JsonEncodedText LogLevel,
        JsonEncodedText EventId,
        JsonEncodedText EventIdName,
        JsonEncodedText Exception,
        JsonEncodedText Message,

        JsonEncodedText ExceptionName,
        JsonEncodedText ExceptionMessage,
        JsonEncodedText ExceptionStackTrace,
        JsonEncodedText ExceptionInnerException,

        JsonEncodedText LogLevelTrace,
        JsonEncodedText LogLevelDebug,
        JsonEncodedText LogLevelInformation,
        JsonEncodedText LogLevelWarning,
        JsonEncodedText LogLevelError,
        JsonEncodedText LogLevelCritical,
        JsonEncodedText LogLevelNone,

        JsonEncodedText MemberName,
        JsonEncodedText FilePath,
        JsonEncodedText LineNumber
    )
    {
        public static readonly JsonPropertyNames Default = new(
            Category: JsonEncodedText.Encode(nameof(LogInfo.Category)),
            Timestamp: JsonEncodedText.Encode(nameof(LogInfo.Timestamp)),
            LogLevel: JsonEncodedText.Encode(nameof(LogInfo.LogLevel)),
            EventId: JsonEncodedText.Encode(nameof(LogInfo.EventId)),
            EventIdName: JsonEncodedText.Encode("EventIdName"),
            Exception: JsonEncodedText.Encode(nameof(LogInfo.Exception)),
            Message: JsonEncodedText.Encode("Message"),

            ExceptionName: JsonEncodedText.Encode("Name"),
            ExceptionMessage: JsonEncodedText.Encode("Message"),
            ExceptionStackTrace: JsonEncodedText.Encode("StackTrace"),
            ExceptionInnerException: JsonEncodedText.Encode("InnerException"),

            LogLevelTrace: JsonEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Trace)),
            LogLevelDebug: JsonEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Debug)),
            LogLevelInformation: JsonEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Information)),
            LogLevelWarning: JsonEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Warning)),
            LogLevelError: JsonEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Error)),
            LogLevelCritical: JsonEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Critical)),
            LogLevelNone: JsonEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.None)),

            MemberName: JsonEncodedText.Encode(nameof(LogInfo.MemberName)),
            FilePath: JsonEncodedText.Encode(nameof(LogInfo.FilePath)),
            LineNumber: JsonEncodedText.Encode(nameof(LogInfo.LineNumber))
        );
    }

    public delegate void JsonLogInfoFormatter(Utf8JsonWriter jsonWriter, in LogInfo info);

    public class SystemTextJsonZLoggerFormatter : IZLoggerFormatter
    {
        public JsonPropertyNames JsonPropertyNames { get; set; } = JsonPropertyNames.Default;

        bool IZLoggerFormatter.WithLineBreak => true;

        public IncludeProperties IncludeProperties { get; set; } = IncludeProperties.Default;

        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public JsonLogInfoFormatter? AdditionalFormatter { get; set; }
        public JsonEncodedText? PropertyKeyValuesObjectName { get; set; } // if null(default), non nested.
        public IKeyNameMutator? KeyNameMutator { get; set; }
        public bool UseUtcTimestamp { get; set; } // default is false, use Local.

        Utf8JsonWriter? jsonWriter;

        [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode")]
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode")]
        public void FormatLogEntry(IBufferWriter<byte> writer, IZLoggerEntry entry)
        {
            jsonWriter?.Reset(writer);
            jsonWriter ??= new Utf8JsonWriter(writer);

            jsonWriter.WriteStartObject();

            // LogInfo
            FormatLogInfo(jsonWriter, entry.LogInfo);

            // Message
            if ((IncludeProperties & IncludeProperties.Message) != 0)
            {
                var bufferWriter = ArrayBufferWriterPool.GetThreadStaticInstance();
                entry.ToString(bufferWriter);
                jsonWriter.WriteString(JsonPropertyNames.Message, bufferWriter.WrittenSpan);
            }

            // Scope
            if ((IncludeProperties & IncludeProperties.ScopeKeyValues) != 0)
            {
                if (entry.LogInfo.ScopeState is { IsEmpty: false } scopeState)
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
            }

            // Additional
            AdditionalFormatter?.Invoke(jsonWriter, entry.LogInfo);

            // Params
            if ((IncludeProperties & IncludeProperties.ParameterKeyValues) != 0)
            {
                if (PropertyKeyValuesObjectName == null)
                {
                    entry.WriteJsonParameterKeyValues(jsonWriter, JsonSerializerOptions, KeyNameMutator);
                }
                else
                {
                    jsonWriter.WriteStartObject(PropertyKeyValuesObjectName.Value);
                    entry.WriteJsonParameterKeyValues(jsonWriter, JsonSerializerOptions, KeyNameMutator);
                    jsonWriter.WriteEndObject();
                }
            }

            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
        }

        JsonEncodedText LogLevelToEncodedText(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return JsonPropertyNames.LogLevelTrace;
                case LogLevel.Debug:
                    return JsonPropertyNames.LogLevelDebug;
                case LogLevel.Information:
                    return JsonPropertyNames.LogLevelInformation;
                case LogLevel.Warning:
                    return JsonPropertyNames.LogLevelWarning;
                case LogLevel.Error:
                    return JsonPropertyNames.LogLevelError;
                case LogLevel.Critical:
                    return JsonPropertyNames.LogLevelCritical;
                case LogLevel.None:
                    return JsonPropertyNames.LogLevelNone;
                default:
                    return JsonEncodedText.Encode(((int)logLevel).ToString());
            }
        }

        void FormatLogInfo(Utf8JsonWriter jsonWriter, in LogInfo info)
        {
            var flag = IncludeProperties;
            if ((flag & IncludeProperties.Timestamp) != 0)
            {
                jsonWriter.WriteString(JsonPropertyNames.Timestamp, UseUtcTimestamp ? info.Timestamp.Utc : info.Timestamp.Local);
            }
            if ((flag & IncludeProperties.LogLevel) != 0)
            {
                jsonWriter.WriteString(JsonPropertyNames.LogLevel, LogLevelToEncodedText(info.LogLevel));
            }
            if ((flag & IncludeProperties.CategoryName) != 0)
            {
                jsonWriter.WriteString(JsonPropertyNames.Category, info.Category.JsonEncoded);
            }
            if ((flag & IncludeProperties.EventIdValue) != 0)
            {
                jsonWriter.WriteNumber(JsonPropertyNames.EventId, info.EventId.Id);
            }
            if ((flag & IncludeProperties.EventIdName) != 0)
            {
                jsonWriter.WriteString(JsonPropertyNames.EventIdName, info.EventId.Name);
            }
            if ((flag & IncludeProperties.Exception) != 0)
            {
                if (info.Exception is { } ex)
                {
                    jsonWriter.WritePropertyName(JsonPropertyNames.Exception);
                    WriteException(jsonWriter, ex);
                }
            }
            if ((flag & IncludeProperties.MemberName) != 0)
            {
                jsonWriter.WriteString(JsonPropertyNames.MemberName, info.MemberName);
            }
            if ((flag & IncludeProperties.FilePath) != 0)
            {
                jsonWriter.WriteString(JsonPropertyNames.FilePath, info.FilePath);
            }
            if ((flag & IncludeProperties.LineNumber) != 0)
            {
                jsonWriter.WriteNumber(JsonPropertyNames.LineNumber, info.LineNumber);
            }
        }

        void WriteException(Utf8JsonWriter jsonWriter, Exception? ex)
        {
            if (ex == null)
            {
                jsonWriter.WriteNullValue();
            }
            else
            {
                jsonWriter.WriteStartObject();
                {
                    jsonWriter.WriteString(JsonPropertyNames.ExceptionName, ex.GetType().FullName);
                    jsonWriter.WriteString(JsonPropertyNames.ExceptionMessage, ex.Message);
                    jsonWriter.WriteString(JsonPropertyNames.ExceptionStackTrace, ex.StackTrace);
                    jsonWriter.WritePropertyName(JsonPropertyNames.ExceptionInnerException);
                    {
                        WriteException(jsonWriter, ex.InnerException);
                    }
                }
                jsonWriter.WriteEndObject();
            }
        }

        internal static void WriteMutatedJsonKeyName(ReadOnlySpan<char> keyName, Utf8JsonWriter jsonWriter, IKeyNameMutator? mutator = null)
        {
            if (mutator == null)
            {
                jsonWriter.WritePropertyName(keyName);
            }
            else if (mutator.IsSupportSlice)
            {
                jsonWriter.WritePropertyName(mutator.Slice(keyName));
            }
            else
            {
                var bufferSize = keyName.Length;
                while (!TryMutate(keyName, bufferSize, jsonWriter, mutator))
                {
                    bufferSize *= 2;
                }
            }

            static bool TryMutate(ReadOnlySpan<char> source, int bufferSize, Utf8JsonWriter jsonWriter, IKeyNameMutator mutator)
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
