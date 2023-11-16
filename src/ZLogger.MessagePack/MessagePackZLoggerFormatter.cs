using System;
using System.Buffers;
using System.Numerics;
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
                var formatter = new MessagePackZLoggerFormatter(options);
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
        public LogInfoProperties IncludeProperties { get; set; } = LogInfoProperties.Timestamp | LogInfoProperties.LogLevel | LogInfoProperties.CategoryName;

        readonly ZLoggerOptions options;

        public MessagePackZLoggerFormatter(ZLoggerOptions options)
        {
            this.options = options;
        }

        public void FormatLogEntry<TEntry>(IBufferWriter<byte> writer, TEntry entry) where TEntry : IZLoggerEntry
        {
            var messagePackWriter = new MessagePackWriter(writer);
            var propCount = BitOperations.PopCount((uint)IncludeProperties) + entry.ParameterCount + 1;
            if (entry.LogInfo.Exception != null) 
                propCount++;

            if (entry.ScopeState != null)
            {
                var scopeProperties = entry.ScopeState.Properties;
                for (var i = 0; i < scopeProperties.Length; i++)
                {
                    if (scopeProperties[i].Key != "{OriginalFormat}")
                    {
                        propCount++;
                    }
                }
            }

            messagePackWriter.WriteMapHeader(propCount);

            if ((IncludeProperties & LogInfoProperties.CategoryName) != 0)
            {
                messagePackWriter.WriteRaw(CategoryNameKey);
                messagePackWriter.WriteString(entry.LogInfo.Category.Utf8Span);
            }
            if ((IncludeProperties & LogInfoProperties.LogLevel) != 0)
            {
                messagePackWriter.WriteRaw(LogLevelKey);
                messagePackWriter.WriteRaw(EncodedLogLevel(entry.LogInfo.LogLevel));
            }
            if ((IncludeProperties & LogInfoProperties.EventIdValue) != 0)
            {
                messagePackWriter.WriteRaw(EventIdKey);
                messagePackWriter.WriteInt32(entry.LogInfo.EventId.Id);
            }
            if ((IncludeProperties & LogInfoProperties.EventIdName) != 0)
            {
                messagePackWriter.WriteRaw(EventIdNameKey);
                messagePackWriter.Write(entry.LogInfo.EventId.Name);
            }
            if ((IncludeProperties & LogInfoProperties.Timestamp) != 0)
            {
                messagePackWriter.WriteRaw(TimestampKey);
                MessagePackSerializerOptions.Resolver.GetFormatterWithVerify<DateTime>()
                    .Serialize(ref messagePackWriter, entry.LogInfo.Timestamp.Utc.DateTime, MessagePackSerializerOptions);
            }

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
                    WriteKeyName(ref messagePackWriter, entry, i);
                }

                WriteParameterValue(ref messagePackWriter, entry, entry.GetParameterType(i), i);
            }

            if (entry.ScopeState != null)
            {
                var scopeProperties = entry.ScopeState.Properties;
                for (var i = 0; i < scopeProperties.Length; i++)
                {
                    var (key, value) = scopeProperties[i];
                    // If `BeginScope(format, arg1, arg2)` style is used, the first argument `format` string is passed with this name
                    if (key == "{OriginalFormat}") continue;

                    WriteKeyName(ref messagePackWriter, key);
                    if (value == null)
                    {
                        messagePackWriter.WriteNil();
                    }
                    else
                    {
                        MessagePackSerializer.Serialize(value.GetType(), ref messagePackWriter, value, MessagePackSerializerOptions);
                    }
                }
            }
            messagePackWriter.Flush();
        }

        void WriteKeyName<TEntry>(ref MessagePackWriter messagePackWriter, TEntry entry, int parameterIndex)
            where TEntry : IZLoggerEntry
        {
            if (entry.IsSupportUtf8ParameterKey)
            {
                var key = entry.GetParameterKey(parameterIndex);
                messagePackWriter.Write(key);
            }
            else
            {
                var key = entry.GetParameterKeyAsString(parameterIndex);
                WriteKeyName(ref messagePackWriter, key);
            }
        }

        void WriteKeyName(ref MessagePackWriter messagePackWriter, ReadOnlySpan<char> keyName)
        {
            if (options.KeyNameMutator is { } mutator)
            {
                var bufferSize = keyName.Length * 2;
                while (!TryMutate(ref messagePackWriter, keyName, bufferSize))
                {
                    bufferSize *= 2;
                }
            }
            else
            {
                messagePackWriter.Write(keyName);
            }
            return;

            bool TryMutate(ref MessagePackWriter messagePackWriter, ReadOnlySpan<char> source, int bufferSize)
            {
                var buffer = ArrayPool<char>.Shared.Rent(bufferSize);
                try
                {
                    if (mutator.TryMutate(source, buffer, out var written))
                    {
                        messagePackWriter.Write(buffer.AsSpan(0, written));
                        return true;
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
                return false;
            }
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

        void WriteParameterValue<TEntry>(ref MessagePackWriter messagePackWriter, TEntry entry, Type type, int index)
            where TEntry : IZLoggerEntry
        {
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Boolean:
                    messagePackWriter.Write(entry.GetParameterValue<bool>(index));
                    return;
                case TypeCode.Char:
                    messagePackWriter.Write(entry.GetParameterValue<char>(index));
                    return;
                case TypeCode.SByte:
                    messagePackWriter.Write(entry.GetParameterValue<sbyte>(index));
                    return;
                case TypeCode.Byte:
                    messagePackWriter.Write(entry.GetParameterValue<byte>(index));
                    return;
                case TypeCode.Int16:
                    messagePackWriter.Write(entry.GetParameterValue<Int16>(index));
                    return;
                case TypeCode.UInt16:
                    messagePackWriter.Write(entry.GetParameterValue<UInt16>(index));
                    return;
                case TypeCode.Int32:
                    messagePackWriter.Write(entry.GetParameterValue<Int32>(index));
                    return;
                case TypeCode.UInt32:
                    messagePackWriter.Write(entry.GetParameterValue<UInt32>(index));
                    return;
                case TypeCode.Int64:
                    messagePackWriter.Write(entry.GetParameterValue<Int64>(index));
                    return;
                case TypeCode.UInt64:
                    messagePackWriter.Write(entry.GetParameterValue<UInt64>(index));
                    return;
                case TypeCode.Single:
                    messagePackWriter.Write(entry.GetParameterValue<Single>(index));
                    return;
                case TypeCode.Double:
                    messagePackWriter.Write(entry.GetParameterValue<double>(index));
                    return;
                case TypeCode.DateTime:
                    return;
                case TypeCode.String:
                    messagePackWriter.Write(entry.GetParameterValue<string>(index));
                    return;
            }

            if (type.IsValueType)
            {
                if (type == typeof(DateTime))
                {
                    var value = entry.GetParameterValue<DateTime>(index);
                    MessagePackSerializer.Serialize(type, ref messagePackWriter, value, MessagePackSerializerOptions);
                }
                else if (type == typeof(DateTimeOffset))
                {
                    var value = entry.GetParameterValue<DateTimeOffset>(index);
                    MessagePackSerializer.Serialize(type, ref messagePackWriter, value, MessagePackSerializerOptions);
                }
                else if (type == typeof(TimeSpan))
                {
                    var value = entry.GetParameterValue<TimeSpan>(index);
                    MessagePackSerializer.Serialize(type, ref messagePackWriter, value, MessagePackSerializerOptions);
                }
#if NET6_OR_GRATER                
                else if (type == typeof(TimeOnly))
                {
                    var value = entry.GetParameterValue<TimeOnly>(index);
                    MessagePackSerializer.Serialize(type, ref messagePackWriter, value, MessagePackSerializerOptions);
                }
                else if (type == typeof(DateOnly))
                {
                    var value = entry.GetParameterValue<DateOnly>(index);
                    MessagePackSerializer.Serialize(type, ref messagePackWriter, value, MessagePackSerializerOptions);
                }
#endif
                else if (type == typeof(Guid))
                {
                    var value = entry.GetParameterValue<Guid>(index);
                    MessagePackSerializer.Serialize(type, ref messagePackWriter, value, MessagePackSerializerOptions);
                }
                else if (Nullable.GetUnderlyingType(type) is { } underlyingType)
                {
                    code = Type.GetTypeCode(underlyingType);
                    switch (code)
                    {
                        case TypeCode.Boolean:
                            {
                                var nullableValue = entry.GetParameterValue<bool?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.Char:
                            {
                                var nullableValue = entry.GetParameterValue<char?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.SByte:
                            {
                                var nullableValue = entry.GetParameterValue<sbyte?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.Byte:
                            {
                                var nullableValue = entry.GetParameterValue<byte?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.Int16:
                            {
                                var nullableValue = entry.GetParameterValue<Int16?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.UInt16:
                            {
                                var nullableValue = entry.GetParameterValue<UInt16?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.Int32:
                            {
                                var nullableValue = entry.GetParameterValue<Int32?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.UInt32:
                            {
                                var nullableValue = entry.GetParameterValue<UInt32?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.Int64:
                            {
                                var nullableValue = entry.GetParameterValue<Int64?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.UInt64:
                            {
                                var nullableValue = entry.GetParameterValue<UInt64?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.Single:
                            {
                                var nullableValue = entry.GetParameterValue<Single?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.Double:
                            {
                                var nullableValue = entry.GetParameterValue<double?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                        case TypeCode.DateTime:
                            {
                                var nullableValue = entry.GetParameterValue<DateTime?>(index);
                                if (nullableValue.HasValue)
                                    messagePackWriter.Write(nullableValue.Value);
                                else
                                    messagePackWriter.WriteNil();
                                return;
                            }
                    }
                }
            }

            var boxedValue = entry.GetParameterValue(index);
            if (boxedValue == null)
            {
                messagePackWriter.WriteNil();
            }
            else
            {
                MessagePackSerializer.Serialize(type, ref messagePackWriter, boxedValue, MessagePackSerializerOptions);
            }
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
