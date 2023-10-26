using System;
using System.Buffers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;
using ZLogger.Internal;

namespace ZLogger.Formatters
{
    public static class ZLoggerOptionsSystemTextJsonExtensions
    {
        public static ZLoggerOptions UseJsonFormatter(this ZLoggerOptions options, Action<SystemTextJsonZLoggerFormatter>? jsonConfigure = null)
        {
            return options.UseFormatter(() =>
            {
                var formatter = new SystemTextJsonZLoggerFormatter(options);
                jsonConfigure?.Invoke(formatter);
                return formatter;
            });
        }
    }

    public class SystemTextJsonZLoggerFormatter : IZLoggerFormatter
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
        public Action<Utf8JsonWriter, LogInfo> MetadataFormatter { get; set; } = DefaultMetadataFormatter;

        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        readonly ZLoggerOptions options;
        Utf8JsonWriter? jsonWriter;

        public SystemTextJsonZLoggerFormatter(ZLoggerOptions options)
        {
            this.options = options;
        }
        
        public void FormatLogEntry<TEntry>(IBufferWriter<byte> writer, TEntry entry) where TEntry : IZLoggerEntry
        {
            jsonWriter?.Reset(writer);
            jsonWriter ??= new Utf8JsonWriter(writer);

            jsonWriter.WriteStartObject();
            
            MetadataFormatter.Invoke(jsonWriter, entry.LogInfo);

            var bufferWriter = ArrayBufferWriterPool.GetThreadStaticInstance();
            entry.ToString(bufferWriter);
            jsonWriter.WriteString(MessagePropertyName, bufferWriter.WrittenSpan);
            
            entry.WriteJsonParameterKeyValues(jsonWriter, JsonSerializerOptions, options);
            
            if (entry.ScopeState is { IsEmpty: false } scopeState)
            {
                for (var i = 0; i < scopeState.Properties.Count; i++)
                {
                    var x = scopeState.Properties[i];
                    // If `BeginScope(format, arg1, arg2)` style is used, the first argument `format` string is passed with this name
                    if (x.Key == "{OriginalFormat}")
                        continue;

                    WriteKeyName(x.Key);
                    
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

            jsonWriter.WriteEndObject();
            jsonWriter.Flush();
        }

        void WriteKeyName(ReadOnlySpan<char> keyName)
        {
            if (options.KeyNameMutator is { } mutator)
            {
                for (var i = 0; i < 2; i++)
                {
                    if (TryMutate(mutator, keyName, keyName.Length * (i + 1)))
                    {
                        return;
                    }
                }
            }
            jsonWriter!.WritePropertyName(keyName);
            return;

            bool TryMutate(IKeyNameMutator mutator, ReadOnlySpan<char> source, int bufferSize)
            {
                Span<char> buffer = stackalloc char[bufferSize];
                if (mutator.TryMutate(source, buffer, out var written))
                {
                    jsonWriter!.WritePropertyName(buffer[..written]);
                    return true;
                }
                return false;
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

        public static void DefaultMetadataFormatter(Utf8JsonWriter jsonWriter, LogInfo info)
        {
            jsonWriter.WriteString(CategoryNameText, info.CategoryName);
            jsonWriter.WriteString(LogLevelText, LogLevelToEncodedText(info.LogLevel));
            jsonWriter.WriteNumber(EventIdText, info.EventId.Id);
            jsonWriter.WriteString(EventIdNameText, info.EventId.Name);
            jsonWriter.WriteString(TimestampText, info.Timestamp);

            // Write Exception
            if (info.Exception is { } ex)
            {
                jsonWriter.WritePropertyName(ExceptionText);
                WriteException(jsonWriter, ex);
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
    }
}
