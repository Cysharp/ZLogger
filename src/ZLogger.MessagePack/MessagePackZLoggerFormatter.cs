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

        [ThreadStatic]
        static ArrayBufferWriter<byte>? threadStaticBufferWriter;

        public MessagePackSerializerOptions MessagePackSerializerOptions { get; set; } = MessagePackSerializer.DefaultOptions;
        public string MessagePropertyName { get; set; } = "Message";
        
        public void FormatLogEntry<TEntry>(IBufferWriter<byte> writer, TEntry entry, bool withLineBreak = true) 
            where TEntry : IZLoggerEntry
        {
            var messagePackWriter = new MessagePackWriter(writer);

            var propCount = 6 + entry.ParameterCount;
            
            if (entry.LogInfo.Exception != null) 
                propCount++;

            if (entry.ScopeState != null)
            {
                for (var i = 0; i < entry.ScopeState.Properties.Count; i++)
                {
                    if (entry.ScopeState.Properties[i].Key != "{OriginalFormat}")
                    {
                        propCount++;
                    }
                }
            }

            messagePackWriter.WriteMapHeader(propCount);

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
            var buffer = GetThreadStaticBufferWriter();
            entry.ToString(buffer);
            messagePackWriter.WriteString(buffer.WrittenSpan);
            
            if (entry.LogInfo.Exception is { } ex)
            {
                messagePackWriter.WriteRaw(ExceptionKey);
                WriteException(ref messagePackWriter, ex);
            }

            for (var i = 0; i < entry.ParameterCount; i++)
            {
                if (entry.IsSupportUtf8ParameterKey)
                {
                    var key = entry.GetParameterKey(i);
                    messagePackWriter.Write(key);
                }
                else
                {
                    var key = entry.GetParameterKeyAsString(i);
                    messagePackWriter.Write(key);
                }
                
                var valueType = entry.GetParameterType(i);
                if (valueType == typeof(string))
                {
                    messagePackWriter.Write(entry.GetParameterValue<string>(i));
                }
                else if (valueType == typeof(bool))
                {
                    messagePackWriter.Write(entry.GetParameterValue<bool>(i));
                }
                else if (valueType == typeof(bool?))
                {
                    var nullableValue = entry.GetParameterValue<bool?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(byte))
                {
                    messagePackWriter.Write(entry.GetParameterValue<byte>(i));
                }
                else if (valueType == typeof(byte?))
                {
                    var nullableValue = entry.GetParameterValue<byte?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(Int16))
                {
                    messagePackWriter.Write(entry.GetParameterValue<Int16>(i));
                }
                else if (valueType == typeof(Int16?))
                {
                    var nullableValue = entry.GetParameterValue<Int16?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(UInt16))
                {
                    messagePackWriter.Write(entry.GetParameterValue<UInt16>(i));
                }
                else if (valueType == typeof(UInt16?))
                {
                    var nullableValue = entry.GetParameterValue<UInt16?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(Int32))
                {
                    messagePackWriter.Write(entry.GetParameterValue<Int32>(i));
                }
                else if (valueType == typeof(Int32?))
                {
                    var nullableValue = entry.GetParameterValue<Int32?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(UInt32))
                {
                    messagePackWriter.Write(entry.GetParameterValue<UInt32>(i));
                }
                else if (valueType == typeof(UInt32?))
                {
                    var nullableValue = entry.GetParameterValue<UInt32?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(Int64))
                {
                    messagePackWriter.Write(entry.GetParameterValue<Int64>(i));
                }
                else if (valueType == typeof(Int64?))
                {
                    var nullableValue = entry.GetParameterValue<Int64?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(UInt64))
                {
                    messagePackWriter.Write(entry.GetParameterValue<UInt64>(i));
                }
                else if (valueType == typeof(UInt16?))
                {
                    var nullableValue = entry.GetParameterValue<UInt16?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(float))
                {
                    messagePackWriter.Write(entry.GetParameterValue<float>(i));
                }
                else if (valueType == typeof(float?))
                {
                    var nullableValue = entry.GetParameterValue<float?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(double))
                {
                    messagePackWriter.Write(entry.GetParameterValue<double>(i));
                }
                else if (valueType == typeof(double?))
                {
                    var nullableValue = entry.GetParameterValue<double?>(i);
                    if (nullableValue.HasValue)
                    {
                        messagePackWriter.Write(nullableValue.Value);
                    }
                    else
                    {
                        messagePackWriter.WriteNil();
                    }
                }
                else if (valueType == typeof(DateTime))
                {
                    var value = entry.GetParameterValue<DateTime>(i);
                    MessagePackSerializer.Serialize(valueType, ref messagePackWriter, value, MessagePackSerializerOptions);
                }
                else if (valueType == typeof(DateTime?))
                {
                    var value = entry.GetParameterValue<DateTime?>(i);
                    MessagePackSerializer.Serialize(valueType, ref messagePackWriter, value, MessagePackSerializerOptions);                    
                }
                else if (valueType == typeof(DateTimeOffset))
                {
                    var value = entry.GetParameterValue<DateTimeOffset>(i);
                    MessagePackSerializer.Serialize(valueType, ref messagePackWriter, value, MessagePackSerializerOptions);
                }
                else if (valueType == typeof(DateTimeOffset?))
                {
                    var value = entry.GetParameterValue<DateTimeOffset?>(i);
                    MessagePackSerializer.Serialize(valueType, ref messagePackWriter, value, MessagePackSerializerOptions);
                }
                else // TODO: GUID, TimeSpan
                {
                    var boxedValue = entry.GetParameterValue(i);
                    MessagePackSerializer.Serialize(valueType, ref messagePackWriter, boxedValue, MessagePackSerializerOptions);
                }
            }
            
            if (entry.ScopeState != null)
            {
                for (var i = 0; i < entry.ScopeState.Properties.Count; i++)
                {
                    var (key, value) = entry.ScopeState.Properties[i];
                    // If `BeginScope(format, arg1, arg2)` style is used, the first argument `format` string is passed with this name
                    if (key == "{OriginalFormat}")
                        continue;
                    
                    messagePackWriter.Write(key);
                    if (value == null)
                    {
                        messagePackWriter.WriteNil();
                    }
                    else
                    {
                        MessagePackSerializer.Serialize(value.GetType(), ref messagePackWriter, value,
                            MessagePackSerializerOptions);
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


        static ArrayBufferWriter<byte> GetThreadStaticBufferWriter()
        {
            threadStaticBufferWriter ??= new ArrayBufferWriter<byte>();
#if NET8_0_OR_GREATER
            threadStaticBufferWriter.ResetWrittenCount();
#else
            threadStaticBufferWriter.Clear();
#endif
            return threadStaticBufferWriter;
        }
    }
}
