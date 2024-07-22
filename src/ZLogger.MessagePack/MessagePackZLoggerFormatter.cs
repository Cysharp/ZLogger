using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace ZLogger.MessagePack;

public record MessagePackPropertyNames(
    MessagePackEncodedText Category,
    MessagePackEncodedText Timestamp,
    MessagePackEncodedText LogLevel,
    MessagePackEncodedText EventId,
    MessagePackEncodedText EventIdName,
    MessagePackEncodedText Exception,
    MessagePackEncodedText Message,
    
    MessagePackEncodedText ExceptionName,
    MessagePackEncodedText ExceptionMessage,
    MessagePackEncodedText ExceptionStackTrace,
    MessagePackEncodedText ExceptionInnerException,
    
    MessagePackEncodedText LogLevelTrace,
    MessagePackEncodedText LogLevelDebug,
    MessagePackEncodedText LogLevelInformation,
    MessagePackEncodedText LogLevelWarning,
    MessagePackEncodedText LogLevelError,
    MessagePackEncodedText LogLevelCritical,
    MessagePackEncodedText LogLevelNone,
    
    MessagePackEncodedText? ParameterKeyValues = null,
    MessagePackEncodedText? ScopeKeyValues = null)
{
    public static readonly MessagePackPropertyNames Default = new(
        Category: MessagePackEncodedText.Encode("Category"),
        Timestamp: MessagePackEncodedText.Encode("Timestamp"),
        LogLevel: MessagePackEncodedText.Encode("LogLevel"),
        EventId: MessagePackEncodedText.Encode("EventId"),
        EventIdName: MessagePackEncodedText.Encode("EventIdName"),
        Exception: MessagePackEncodedText.Encode("Exception"),
        Message: MessagePackEncodedText.Encode("Message"),

        ExceptionName: MessagePackEncodedText.Encode("Name"),
        ExceptionMessage: MessagePackEncodedText.Encode("Message"),
        ExceptionStackTrace: MessagePackEncodedText.Encode("StackTrace"),
        ExceptionInnerException: MessagePackEncodedText.Encode("InnerException"),

        LogLevelTrace: MessagePackEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Trace)),
        LogLevelDebug: MessagePackEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Debug)),
        LogLevelInformation: MessagePackEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Information)),
        LogLevelWarning: MessagePackEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Warning)),
        LogLevelError: MessagePackEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Error)),
        LogLevelCritical: MessagePackEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.Critical)),
        LogLevelNone: MessagePackEncodedText.Encode(nameof(Microsoft.Extensions.Logging.LogLevel.None)));

    public MessagePackEncodedText GetEncodedLogLevel(LogLevel logLevel) => logLevel switch
    {
        Microsoft.Extensions.Logging.LogLevel.Trace => LogLevelTrace,
        Microsoft.Extensions.Logging.LogLevel.Debug => LogLevelDebug,
        Microsoft.Extensions.Logging.LogLevel.Information => LogLevelInformation,
        Microsoft.Extensions.Logging.LogLevel.Warning => LogLevelWarning,
        Microsoft.Extensions.Logging.LogLevel.Error => LogLevelError,
        Microsoft.Extensions.Logging.LogLevel.Critical => LogLevelCritical,
        Microsoft.Extensions.Logging.LogLevel.None => LogLevelNone,
        _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
    };
}

public class MessagePackZLoggerFormatter : IZLoggerFormatter
{
    [ThreadStatic]
    static ArrayBufferWriter<byte>? threadStaticBufferWriter;

    bool IZLoggerFormatter.WithLineBreak => false;

    public MessagePackSerializerOptions MessagePackSerializerOptions { get; set; } = MessagePackSerializer.DefaultOptions;
    public IncludeProperties IncludeProperties { get; set; } = IncludeProperties.Default;
    public MessagePackPropertyNames PropertyNames { get; set; } = MessagePackPropertyNames.Default;
    public IKeyNameMutator? KeyNameMutator { get; set; }

    public void FormatLogEntry(IBufferWriter<byte> writer, IZLoggerEntry entry)
    {
        var messagePackWriter = new MessagePackWriter(writer);
        var propCount = BitOperations.PopCount((uint)IncludeProperties);
        var scopePropCount = 0;

        if ((IncludeProperties & IncludeProperties.ParameterKeyValues) != 0 &&
            // flatten
            PropertyNames.ParameterKeyValues == null)
        {
            propCount--;
            propCount += entry.ParameterCount;
        }
        if (entry.LogInfo.Exception == null && ((IncludeProperties & IncludeProperties.Exception) != 0))
        {
            propCount--;
        }
        if ((IncludeProperties & IncludeProperties.ScopeKeyValues) != 0)
        {
            if (entry.LogInfo.ScopeState == null)
            {
                propCount--;
            }
            else
            {
                var scopeProperties = entry.LogInfo.ScopeState.Properties;
                foreach (var t in scopeProperties)
                {
                    if (t.Key != "{OriginalFormat}") scopePropCount++;
                }

                // flatten
                if (PropertyNames.ScopeKeyValues == null)
                {
                    propCount--;
                    propCount += scopePropCount;
                }
            }
        }

        messagePackWriter.WriteMapHeader(propCount);

        // LogInfo
        var flag = IncludeProperties;
        if ((flag & IncludeProperties.CategoryName) != 0)
        {
            messagePackWriter.WriteRaw(PropertyNames.Category.Utf8EncodedValue);
            messagePackWriter.WriteString(entry.LogInfo.Category.Utf8Span);
        }
        if ((flag & IncludeProperties.LogLevel) != 0)
        {
            messagePackWriter.WriteRaw(PropertyNames.LogLevel.Utf8EncodedValue);
            messagePackWriter.WriteRaw(PropertyNames.GetEncodedLogLevel(entry.LogInfo.LogLevel).Utf8EncodedValue);
        }
        if ((flag & IncludeProperties.EventIdValue) != 0)
        {
            messagePackWriter.WriteRaw(PropertyNames.EventId.Utf8EncodedValue);
            messagePackWriter.WriteInt32(entry.LogInfo.EventId.Id);
        }
        if ((flag & IncludeProperties.EventIdName) != 0)
        {
            messagePackWriter.WriteRaw(PropertyNames.EventIdName.Utf8EncodedValue);
            messagePackWriter.Write(entry.LogInfo.EventId.Name);
        }
        if ((flag & IncludeProperties.Timestamp) != 0)
        {
            messagePackWriter.WriteRaw(PropertyNames.Timestamp.Utf8EncodedValue);
            MessagePackSerializerOptions.Resolver.GetFormatterWithVerify<DateTime>()
                .Serialize(ref messagePackWriter, entry.LogInfo.Timestamp.Utc.DateTime, MessagePackSerializerOptions);
        }
        if ((flag & IncludeProperties.Exception) != 0)
        {
            if (entry.LogInfo.Exception is { } ex)
            {
                messagePackWriter.WriteRaw(PropertyNames.Exception.Utf8EncodedValue);
                WriteException(ref messagePackWriter, ex);
            }
        }

        // Message
        if ((flag & IncludeProperties.Message) != 0)
        {
            messagePackWriter.WriteRaw(PropertyNames.Message.Utf8EncodedValue);
            var buffer = GetThreadStaticBufferWriter();
            entry.ToString(buffer);
            messagePackWriter.WriteString(buffer.WrittenSpan);
        }

        // Scope
        if ((flag & IncludeProperties.ScopeKeyValues) != 0)
        {
            if (entry.LogInfo.ScopeState is { Properties: var scopeProperties })
            {
                // nested
                if (PropertyNames.ScopeKeyValues != null)
                {
                    messagePackWriter.WriteRaw(PropertyNames.ScopeKeyValues.Value.Utf8EncodedValue);
                    messagePackWriter.WriteMapHeader(scopePropCount);
                }
                
                foreach (var t in scopeProperties)
                {
                    var (key, value) = t;
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
        }

        // Params
        if ((flag & IncludeProperties.ParameterKeyValues) != 0)
        {
            // nested
            if (PropertyNames.ParameterKeyValues != null)
            {
                messagePackWriter.WriteRaw(PropertyNames.ParameterKeyValues.Value.Utf8EncodedValue);
                messagePackWriter.WriteMapHeader(entry.ParameterCount);
            }
            for (var i = 0; i < entry.ParameterCount; i++)
            {
                WriteKeyName(ref messagePackWriter, entry, i);
                WriteParameterValue(ref messagePackWriter, entry, entry.GetParameterType(i), i);
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
            messagePackWriter.WriteString(key);
        }
        else
        {
            var key = entry.GetParameterKeyAsString(parameterIndex);
            WriteKeyName(ref messagePackWriter, key);
        }
    }

    void WriteKeyName(ref MessagePackWriter messagePackWriter, ReadOnlySpan<char> keyName)
    {
        if (KeyNameMutator is { } mutator)
        {
            if (mutator.IsSupportSlice)
            {
                messagePackWriter.Write(mutator.Slice(keyName));
            }
            else
            {
                var bufferSize = keyName.Length * 2;
                while (!TryMutate(ref messagePackWriter, keyName, bufferSize))
                {
                    bufferSize *= 2;
                }
            }
        }
        else
        {
            messagePackWriter.Write(keyName);
        }

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

    void WriteException(ref MessagePackWriter messagePackWriter, Exception? ex)
    {
        while (true)
        {
            if (ex == null)
            {
                messagePackWriter.WriteNil();
                return;
            }

            messagePackWriter.WriteMapHeader(4);

            messagePackWriter.WriteRaw(PropertyNames.ExceptionName.Utf8EncodedValue);
            messagePackWriter.Write(ex.GetType().FullName);

            messagePackWriter.WriteRaw(PropertyNames.ExceptionMessage.Utf8EncodedValue);
            messagePackWriter.Write(ex.Message);

            messagePackWriter.WriteRaw(PropertyNames.ExceptionStackTrace.Utf8EncodedValue);
            messagePackWriter.Write(ex.StackTrace);

            messagePackWriter.WriteRaw(PropertyNames.ExceptionInnerException.Utf8EncodedValue);
            ex = ex.InnerException;
        }
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
