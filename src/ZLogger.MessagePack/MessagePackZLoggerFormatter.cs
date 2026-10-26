using System;
using System.Buffers;
using System.Numerics;

using MessagePack;

using Microsoft.Extensions.Logging;

using SerializerFoundation;

using ZLogger.MessagePack.Internal;

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

    // Declared as init properties rather than positional parameters, so that the existing constructor stays compatible.
    public MessagePackEncodedText MemberName { get; init; } = MessagePackEncodedText.Encode(nameof(LogInfo.MemberName));
    public MessagePackEncodedText FilePath { get; init; } = MessagePackEncodedText.Encode(nameof(LogInfo.FilePath));
    public MessagePackEncodedText LineNumber { get; init; } = MessagePackEncodedText.Encode(nameof(LogInfo.LineNumber));

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

    [ThreadStatic]
    static ArrayBufferWriter<byte>? threadStaticEntryBufferWriter;

    // Doubling the buffer this many times allows a mutated key name up to 1024 times the initial buffer size.
    // A cleaner solution would be for the mutator to report the maximum length of its output, so that the buffer could be
    // sized once and no retry limit would be needed. Adding that member to IKeyNameMutator would break existing implementations
    // (default interface methods are not available on netstandard2.0, which ZLogger targets); a separate optional interface
    // would avoid the break but add API surface, so a fixed limit is used for now.
    const int MaxKeyNameMutatorRetries = 10;

    bool IZLoggerFormatter.WithLineBreak => false;

    /// <summary>
    /// Options used to serialize the timestamp, every non-null scope value, and parameter values whose parameter type is not
    /// bool, char, sbyte, byte, short, ushort, int, uint, long, ulong, float, double, string or an enum (or the nullable form of
    /// one of these), for example DateTime, DateTimeOffset, TimeSpan, Guid, decimal, nint, Int128 and other objects.
    /// Parameters of the excluded types (enums as their underlying integer, so an enum setting in the options does not apply),
    /// null values, and the other log properties (such as the category, the message and the exception) are always written
    /// directly, regardless of the options.
    /// </summary>
    /// <remarks>
    /// Options with a <see cref="MessagePackSerializerOptions.MessageProcessor"/> are rejected.
    /// A processor such as compression applies to a whole message, while this formatter serializes each value
    /// into a part of a log entry. Compress log output at the destination instead.
    /// </remarks>
    /// <exception cref="ArgumentNullException">The value is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The options have a <see cref="MessagePackSerializerOptions.MessageProcessor"/>.</exception>
    public MessagePackSerializerOptions MessagePackSerializerOptions
    {
        get;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value.MessageProcessor != null)
            {
                throw new ArgumentException("MessageProcessor is not supported, because it applies to a whole message while the formatter serializes each value into a part of a log entry. Compress log output at the destination instead.", nameof(value));
            }
            field = value;
        }
    } = MessagePackSerializerOptions.Default;

    public IncludeProperties IncludeProperties { get; set; } = IncludeProperties.Default;
    public MessagePackPropertyNames PropertyNames { get; set; } = MessagePackPropertyNames.Default;
    public IKeyNameMutator? KeyNameMutator { get; set; }

    public void FormatLogEntry(IBufferWriter<byte> writer, IZLoggerEntry entry)
    {
        // Format the entry into a staging buffer and copy it to writer only when it is complete.
        // Writing values commits the bytes written so far, so if serializing a value throws, writing straight into
        // writer would leave a partial map there, and every entry after it would be read incorrectly.
        var staging = TakeThreadStaticBufferWriter(ref threadStaticEntryBufferWriter);
        try
        {
            FormatLogEntryCore(staging, entry);

            // Request the whole entry at once instead of using writer.Write. Write fills whatever span GetSpan() returns,
            // commits it with Advance, and throws if the next GetSpan() returns an empty span (as StreamBufferWriter does when
            // its buffer is exactly full), leaving a partial entry in writer. GetSpan(sizeHint) returns at least sizeHint bytes,
            // so one copy suffices, and a writer that fails does so before any byte of the entry is committed.
            var staged = staging.WrittenSpan;
            staged.CopyTo(writer.GetSpan(staged.Length));
            writer.Advance(staged.Length);
        }
        finally
        {
            threadStaticEntryBufferWriter = staging;
        }
    }

    void FormatLogEntryCore(IBufferWriter<byte> writer, IZLoggerEntry entry)
    {
#if NET9_0_OR_GREATER
        var writeBuffer = new BufferWriterWriteBuffer(writer);
#else
        var writeBuffer = new CompatibleBufferWriterWriteBuffer(writer);
#endif

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

        writeBuffer.WriteMapHeader(propCount);

        // LogInfo
        var flag = IncludeProperties;
        if ((flag & IncludeProperties.CategoryName) != 0)
        {
            writeBuffer.WriteRaw(PropertyNames.Category.Utf8EncodedValue);
            writeBuffer.WriteString(entry.LogInfo.Category.Utf8Span);
        }

        if ((flag & IncludeProperties.LogLevel) != 0)
        {
            writeBuffer.WriteRaw(PropertyNames.LogLevel.Utf8EncodedValue);
            writeBuffer.WriteRaw(PropertyNames.GetEncodedLogLevel(entry.LogInfo.LogLevel).Utf8EncodedValue);
        }

        if ((flag & IncludeProperties.EventIdValue) != 0)
        {
            writeBuffer.WriteRaw(PropertyNames.EventId.Utf8EncodedValue);
            writeBuffer.WriteInt32(entry.LogInfo.EventId.Id);
        }

        if ((flag & IncludeProperties.EventIdName) != 0)
        {
            writeBuffer.WriteRaw(PropertyNames.EventIdName.Utf8EncodedValue);
            writeBuffer.WriteString(entry.LogInfo.EventId.Name);
        }

        if ((flag & IncludeProperties.Timestamp) != 0)
        {
            writeBuffer.WriteRaw(PropertyNames.Timestamp.Utf8EncodedValue);
            SerializeValue(ref writeBuffer, writer, entry.LogInfo.Timestamp.Utc.DateTime);
        }

        if ((flag & IncludeProperties.Exception) != 0)
        {
            if (entry.LogInfo.Exception is { } ex)
            {
                writeBuffer.WriteRaw(PropertyNames.Exception.Utf8EncodedValue);
                WriteException(ref writeBuffer, ex);
            }
        }

        if ((flag & IncludeProperties.MemberName) != 0)
        {
            writeBuffer.WriteRaw(PropertyNames.MemberName.Utf8EncodedValue);
            writeBuffer.WriteString(entry.LogInfo.MemberName);
        }

        if ((flag & IncludeProperties.FilePath) != 0)
        {
            writeBuffer.WriteRaw(PropertyNames.FilePath.Utf8EncodedValue);
            writeBuffer.WriteString(entry.LogInfo.FilePath);
        }

        if ((flag & IncludeProperties.LineNumber) != 0)
        {
            writeBuffer.WriteRaw(PropertyNames.LineNumber.Utf8EncodedValue);
            writeBuffer.WriteInt32(entry.LogInfo.LineNumber);
        }

        // Message
        if ((flag & IncludeProperties.Message) != 0)
        {
            writeBuffer.WriteRaw(PropertyNames.Message.Utf8EncodedValue);
            var buffer = TakeThreadStaticBufferWriter(ref threadStaticBufferWriter);
            try
            {
                entry.ToString(buffer);
                writeBuffer.WriteString(buffer.WrittenSpan);
            }
            finally
            {
                threadStaticBufferWriter = buffer;
            }
        }

        // Scope
        if ((flag & IncludeProperties.ScopeKeyValues) != 0)
        {
            if (entry.LogInfo.ScopeState is { Properties: var scopeProperties })
            {
                // nested
                if (PropertyNames.ScopeKeyValues != null)
                {
                    writeBuffer.WriteRaw(PropertyNames.ScopeKeyValues.Value.Utf8EncodedValue);
                    writeBuffer.WriteMapHeader(scopePropCount);
                }

                foreach (var t in scopeProperties)
                {
                    var (key, value) = t;

                    // If `BeginScope(format, arg1, arg2)` style is used, the first argument `format` string is passed with this name
                    if (key == "{OriginalFormat}") continue;

                    WriteKeyName(ref writeBuffer, key);
                    if (value == null)
                    {
                        writeBuffer.WriteNil();
                    }
                    else
                    {
                        SerializeValue(ref writeBuffer, writer, value.GetType(), value);
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
                writeBuffer.WriteRaw(PropertyNames.ParameterKeyValues.Value.Utf8EncodedValue);
                writeBuffer.WriteMapHeader(entry.ParameterCount);
            }

            for (var i = 0; i < entry.ParameterCount; i++)
            {
                WriteKeyName(ref writeBuffer, entry, i);
                WriteParameterValue(ref writeBuffer, writer, entry, entry.GetParameterType(i), i);
            }
        }

        writeBuffer.Flush();
    }

    void WriteKeyName<TEntry, TWriteBuffer>(ref TWriteBuffer messagePackWriter, TEntry entry, int parameterIndex)
        where TEntry : IZLoggerEntry
        where TWriteBuffer : struct, IWriteBuffer

#if NET9_0_OR_GREATER
        , allows ref struct
#endif
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

    void WriteKeyName<TWriteBuffer>(ref TWriteBuffer writeBuffer, ReadOnlySpan<char> keyName)
        where TWriteBuffer : struct, IWriteBuffer

#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        if (KeyNameMutator is { } mutator)
        {
            if (mutator.IsSupportSlice)
            {
                writeBuffer.WriteString(mutator.Slice(keyName));
            }
            else
            {
                // Start from at least 16 characters: for an empty key name keyName.Length * 2 is 0, and doubling 0 never grows the buffer.
                var bufferSize = Math.Max(checked(keyName.Length * 2), 16);
                var retries = 0;
                while (!TryMutate(ref writeBuffer, keyName, bufferSize))
                {
                    // After MaxKeyNameMutatorRetries doublings, a mutator that still returns false is treated as broken.
                    if (++retries > MaxKeyNameMutatorRetries)
                    {
                        throw new InvalidOperationException($"{mutator.GetType().FullName}.TryMutate returned false {retries} times for the key name '{keyName.ToString()}', the last time with a buffer of {bufferSize} characters. TryMutate should return false only when the destination is too small.");
                    }
                    bufferSize = checked(bufferSize * 2);
                }
            }
        }
        else
        {
            writeBuffer.WriteString(keyName);
        }

        // Small buffers on the stack and larger ones from the pool, as in SystemTextJsonZLoggerFormatter.
        bool TryMutate(ref TWriteBuffer writeBuffer, ReadOnlySpan<char> source, int bufferSize)
        {
            if (bufferSize > 256)
            {
                // Rent may return a larger array; pass exactly bufferSize so that the size in the exception is what the mutator received.
                var buffer = ArrayPool<char>.Shared.Rent(bufferSize);
                try
                {
                    if (mutator.TryMutate(source, buffer.AsSpan(0, bufferSize), out var written))
                    {
                        writeBuffer.WriteString(buffer.AsSpan(0, written));
                        return true;
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
            }
            else
            {
                Span<char> buffer = stackalloc char[bufferSize];
                if (mutator.TryMutate(source, buffer, out var written))
                {
                    writeBuffer.WriteString(buffer[..written]);
                    return true;
                }
            }
            return false;
        }
    }

    void WriteException<TWriteBuffer>(ref TWriteBuffer writeBuffer, Exception? ex)
        where TWriteBuffer : struct, IWriteBuffer

#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        while (true)
        {
            if (ex == null)
            {
                writeBuffer.WriteNil();
                return;
            }

            writeBuffer.WriteMapHeader(4);

            writeBuffer.WriteRaw(PropertyNames.ExceptionName.Utf8EncodedValue);
            writeBuffer.WriteString(ex.GetType().FullName);

            writeBuffer.WriteRaw(PropertyNames.ExceptionMessage.Utf8EncodedValue);
            writeBuffer.WriteString(ex.Message);

            writeBuffer.WriteRaw(PropertyNames.ExceptionStackTrace.Utf8EncodedValue);
            writeBuffer.WriteString(ex.StackTrace);

            writeBuffer.WriteRaw(PropertyNames.ExceptionInnerException.Utf8EncodedValue);
            ex = ex.InnerException;
        }
    }

    void WriteParameterValue<TEntry, TWriteBuffer>(
        ref TWriteBuffer writeBuffer,
        IBufferWriter<byte> underlyingWriter,
        TEntry entry,
        Type type,
        int index)
        where TEntry : IZLoggerEntry
        where TWriteBuffer : struct, IWriteBuffer

#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        var code = Type.GetTypeCode(type);
        switch (code)
        {
            case TypeCode.Boolean:
                writeBuffer.WriteBoolean(entry.GetParameterValue<bool>(index));
                return;
            case TypeCode.Char:
                writeBuffer.WriteChar(entry.GetParameterValue<char>(index));
                return;
            case TypeCode.SByte:
                writeBuffer.WriteSByte(entry.GetParameterValue<sbyte>(index));
                return;
            case TypeCode.Byte:
                writeBuffer.WriteByte(entry.GetParameterValue<byte>(index));
                return;
            case TypeCode.Int16:
                writeBuffer.WriteInt16(entry.GetParameterValue<Int16>(index));
                return;
            case TypeCode.UInt16:
                writeBuffer.WriteUInt16(entry.GetParameterValue<UInt16>(index));
                return;
            case TypeCode.Int32:
                writeBuffer.WriteInt32(entry.GetParameterValue<Int32>(index));
                return;
            case TypeCode.UInt32:
                writeBuffer.WriteUInt32(entry.GetParameterValue<UInt32>(index));
                return;
            case TypeCode.Int64:
                writeBuffer.WriteInt64(entry.GetParameterValue<Int64>(index));
                return;
            case TypeCode.UInt64:
                writeBuffer.WriteUInt64(entry.GetParameterValue<UInt64>(index));
                return;
            case TypeCode.Single:
                writeBuffer.WriteSingle(entry.GetParameterValue<Single>(index));
                return;
            case TypeCode.Double:
                writeBuffer.WriteDouble(entry.GetParameterValue<double>(index));
                return;
            case TypeCode.DateTime:
                SerializeValue(ref writeBuffer, underlyingWriter, entry.GetParameterValue<DateTime>(index));
                return;
            case TypeCode.String:
                writeBuffer.WriteString(entry.GetParameterValue<string>(index));
                return;
        }

        if (type.IsValueType)
        {
            if (type == typeof(DateTimeOffset))
            {
                var value = entry.GetParameterValue<DateTimeOffset>(index);
                SerializeValue(ref writeBuffer, underlyingWriter, value);
                return;
            }

            if (type == typeof(TimeSpan))
            {
                var value = entry.GetParameterValue<TimeSpan>(index);
                SerializeValue(ref writeBuffer, underlyingWriter, value);
                return;
            }

#if NET6_0_OR_GREATER
            if (type == typeof(TimeOnly))
            {
                var value = entry.GetParameterValue<TimeOnly>(index);
                SerializeValue(ref writeBuffer, underlyingWriter, value);
                return;
            }

            if (type == typeof(DateOnly))
            {
                var value = entry.GetParameterValue<DateOnly>(index);
                SerializeValue(ref writeBuffer, underlyingWriter, value);
                return;
            }
#endif

            if (type == typeof(Guid))
            {
                var value = entry.GetParameterValue<Guid>(index);
                SerializeValue(ref writeBuffer, underlyingWriter, value);
                return;
            }

            if (Nullable.GetUnderlyingType(type) is { } underlyingType)
            {
                code = Type.GetTypeCode(underlyingType);
                switch (code)
                {
                    case TypeCode.Boolean:
                    {
                        var nullableValue = entry.GetParameterValue<bool?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteBoolean(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.Char:
                    {
                        var nullableValue = entry.GetParameterValue<char?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteChar(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.SByte:
                    {
                        var nullableValue = entry.GetParameterValue<sbyte?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteSByte(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.Byte:
                    {
                        var nullableValue = entry.GetParameterValue<byte?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteByte(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.Int16:
                    {
                        var nullableValue = entry.GetParameterValue<short?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteInt16(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.UInt16:
                    {
                        var nullableValue = entry.GetParameterValue<ushort?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteUInt16(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.Int32:
                    {
                        var nullableValue = entry.GetParameterValue<int?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteInt32(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.UInt32:
                    {
                        var nullableValue = entry.GetParameterValue<uint?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteUInt32(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.Int64:
                    {
                        var nullableValue = entry.GetParameterValue<long?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteInt64(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.UInt64:
                    {
                        var nullableValue = entry.GetParameterValue<ulong?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteUInt64(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.Single:
                    {
                        var nullableValue = entry.GetParameterValue<Single?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteSingle(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.Double:
                    {
                        var nullableValue = entry.GetParameterValue<double?>(index);
                        if (nullableValue.HasValue)
                            writeBuffer.WriteDouble(nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                    case TypeCode.DateTime:
                    {
                        var nullableValue = entry.GetParameterValue<DateTime?>(index);
                        if (nullableValue.HasValue)
                            SerializeValue(ref writeBuffer, underlyingWriter, nullableValue.Value);
                        else
                            writeBuffer.WriteNil();
                        return;
                    }
                }
            }
        }

        var boxedValue = entry.GetParameterValue(index);
        if (boxedValue == null)
        {
            writeBuffer.WriteNil();
        }
        else
        {
            SerializeValue(ref writeBuffer, underlyingWriter, type, boxedValue);
        }
    }

    // Values are serialized through the IBufferWriter<byte> entry points, never through the ones that take a ref TWriteBuffer.
    // A formatter from a library compiled against a netstandard build of MessagePack (a [MessagePackObject] type or a custom
    // formatter in the options) can serve only the Compatible (non-ref-struct) buffers. The IBufferWriter<byte> entry points
    // reroute such values to those buffers, while the ref TWriteBuffer entry points cannot change the caller's buffer type and
    // throw instead. Other values still go through the ref struct buffers inside MessagePack.
    void SerializeValue<TWriteBuffer, T>(
        ref TWriteBuffer writeBuffer,
        IBufferWriter<byte> underlyingWriter,
        T value)
        where TWriteBuffer : struct, IWriteBuffer
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        writeBuffer.Flush();
        MessagePackSerializer.Serialize(underlyingWriter, value, MessagePackSerializerOptions);
    }

    void SerializeValue<TWriteBuffer>(
        ref TWriteBuffer writeBuffer,
        IBufferWriter<byte> underlyingWriter,
        Type type,
        object value)
        where TWriteBuffer : struct, IWriteBuffer
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        writeBuffer.Flush();
        MessagePackSerializer.Serialize(type, underlyingWriter, value, MessagePackSerializerOptions);
    }

    // Takes the buffer out of its thread-static slot, which all instances share. Formatting can re-enter a
    // MessagePackZLoggerFormatter on the same thread when a value's ToString or MessagePack formatter logs to a provider that
    // formats in Post (such as the in-memory provider); the re-entrant call then gets a buffer of its own instead of
    // overwriting this one. The caller puts the buffer back when it is done.
    static ArrayBufferWriter<byte> TakeThreadStaticBufferWriter(ref ArrayBufferWriter<byte>? slot)
    {
        var bufferWriter = slot ?? new ArrayBufferWriter<byte>();
        slot = null;
#if NET8_0_OR_GREATER
        bufferWriter.ResetWrittenCount();
#else
        bufferWriter.Clear();
#endif
        return bufferWriter;
    }
}
