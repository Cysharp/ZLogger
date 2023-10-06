using System;
using System.Linq;
using System.Buffers;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace ZLogger.MessagePack
{
    public static class ZLoggerOptionsMessagePackExtensions
    {
        public static ZLoggerOptions UseMessagePackFormatter(this ZLoggerOptions options, Action<MessagePackZLoggerFormatter>? messagePackConfigure = null)
        {
            return options.UseFormatter(() =>
            {
                var formatter = new MessagePackZLoggerFormatter();
                messagePackConfigure?.Invoke(formatter);
                return formatter;
            });
        }
    }

    public class MessagePackZLoggerFormatter : IZLoggerFormatter
    {
        // "CategoryName"
        static readonly byte[] CategoryNameKey = { 0b10100000 | 12, 67, 97, 116, 101, 103, 111, 114, 121, 78, 97, 109, 101 };
        // "Timestamp"
        static readonly byte[] TimestampKey = { 0b10100000 | 9, 84, 105, 109, 101, 115, 116, 97, 109, 112 };
        // "LogLevel"
        static readonly byte[] LogLevelKey = { 0b10100000 | 8, 76, 111, 103, 76, 101, 118, 101, 108 };
        // "EventId"
        static readonly byte[] EventIdKey = { 0b10100000 | 7, 69, 118, 101, 110, 116, 73, 100 };
        // "EventIdName"
        static readonly byte[] EventIdNameKey = { 0b10100000 | 11, 69, 118, 101, 110, 116, 73, 100, 78, 97, 109, 101 };
        // "Exception"
        static readonly byte[] ExceptionKey = { 0b10100000 | 9, 69, 120, 99, 101, 112, 116, 105, 111, 110 };

        // "Name"
        static readonly byte[] NameKey = { 0b10100000 | 4, 78, 97, 109, 101 };
        // "Message"
        static readonly byte[] MessageKey = { 0b10100000 | 7, 77, 101, 115, 115, 97, 103, 101 };
        // "StackTrace"
        static readonly byte[] StackTraceKey = { 0b10100000 | 10, 83, 116, 97, 99, 107, 84, 114, 97, 99, 101 };
        // "InnerException"
        static readonly byte[] InnerExceptionKey = { 0b10100000 | 14, 73, 110, 110, 101, 114, 69, 120, 99, 101, 112, 116, 105, 111, 110 };

        // "Trace"
        static readonly byte[] Trace = { 0b10100000 | 5, 84, 114, 97, 99, 101 };
        // "Debug"
        static readonly byte[] Debug = { 0b10100000 | 5, 68, 101, 98, 117, 103 };
        // "Information"
        static readonly byte[] Information = { 0b10100000 | 11, 73, 110, 102, 111, 114, 109, 97, 116, 105, 111, 110 };
        // "Warning"
        static readonly byte[] Warning = { 0b10100000 | 7, 87, 97, 114, 110, 105, 110, 103 };
        // "Error"
        static readonly byte[] Error = { 0b10100000 | 5, 69, 114, 114, 111, 114 };
        // "Critical"
        static readonly byte[] Critical = { 0b10100000 | 8, 67, 114, 105, 116, 105, 99, 97, 108 };
        // "None"
        static readonly byte[] None = { 0b10100000 | 4, 78, 111, 110, 101 };

        public MessagePackSerializerOptions MessagePackSerializerOptions { get; set; } = MessagePackSerializer.DefaultOptions;
        public string MessagePropertyName { get; set; } = "Message";
        public string PayloadPropertyName { get; set; } = "Payload";

        public void FormatLogEntry<TEntry, TPayload>(
            IBufferWriter<byte> writer,
            TEntry entry,
            TPayload? payload,
            ReadOnlySpan<byte> utf8Message)
            where TEntry : IZLoggerEntry
        {
            var messagePackWriter = new MessagePackWriter(writer);

            messagePackWriter.WriteMapHeader(6 + 
                                             (entry.LogInfo.Exception != null ? 1 : 0) + 
                                             (payload != null ? 1 : 0) +
                                             (entry.ScopeState?.Properties.Count(x => x.Key != "{OriginalFormat}") ?? 0));

            messagePackWriter.WriteRaw(CategoryNameKey);
            messagePackWriter.Write(entry.LogInfo.CategoryName);

            messagePackWriter.WriteRaw(LogLevelKey);
            messagePackWriter.WriteRaw(EncodedLogLevel(entry.LogInfo.LogLevel));

            messagePackWriter.WriteRaw(EventIdKey);
            messagePackWriter.WriteInt32(entry.LogInfo.EventId.Id);

            messagePackWriter.WriteRaw(EventIdNameKey);
            messagePackWriter.Write(entry.LogInfo.EventId.Name);

            messagePackWriter.WriteRaw(TimestampKey);
            MessagePackSerializerOptions.Resolver.GetFormatterWithVerify<DateTime>()
                .Serialize(ref messagePackWriter, entry.LogInfo.Timestamp.UtcDateTime, MessagePackSerializerOptions);

            messagePackWriter.Write(MessagePropertyName);
            messagePackWriter.WriteString(utf8Message);

            if (entry.LogInfo.Exception is { } ex)
            {
                messagePackWriter.WriteRaw(ExceptionKey);
                WriteException(ref messagePackWriter, ex);
            }
            if (payload is { })
            {
                messagePackWriter.Write(PayloadPropertyName);
                MessagePackSerializerOptions.Resolver.GetFormatterWithVerify<TPayload>()
                    .Serialize(ref messagePackWriter, payload, MessagePackSerializerOptions);
            }
            if (entry.ScopeState is { IsEmpty: false } scopeState)
            {
                for (var i = 0; i < scopeState.Properties.Count; i++)
                {
                    var x = scopeState.Properties[i];
                    // If `BeginScope(format, arg1, arg2)` style is used, the first argument `format` string is passed with this name
                    if (x.Key == "{OriginalFormat}")
                        continue;
                    
                    messagePackWriter.Write(x.Key);
                    if (x.Value is { } value)
                    {
                        MessagePackSerializer.Serialize(value.GetType(), ref messagePackWriter, value,
                            MessagePackSerializerOptions);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
            }
            messagePackWriter.Flush();
        }

        static void WriteException(ref MessagePackWriter messagePackWriter, Exception? ex)
        {
            if (ex == null)
            {
                messagePackWriter.WriteNil();
                return;
            }

            messagePackWriter.WriteMapHeader(4);

            messagePackWriter.WriteRaw(NameKey);
            messagePackWriter.Write(ex.GetType().FullName);

            messagePackWriter.WriteRaw(MessageKey);
            messagePackWriter.Write(ex.Message);

            messagePackWriter.WriteRaw(StackTraceKey);
            messagePackWriter.Write(ex.StackTrace);

            messagePackWriter.WriteRaw(InnerExceptionKey);
            WriteException(ref messagePackWriter, ex.InnerException);
        }

        static byte[] EncodedLogLevel(LogLevel logLevel)
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
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}
