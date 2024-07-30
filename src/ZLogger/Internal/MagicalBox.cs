using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Utf8StringInterpolation;

namespace ZLogger.Internal;

internal unsafe partial struct MagicalBox
{
    byte[] storage;
    int written;

    public MagicalBox(byte[] storage)
    {
        this.storage = storage;
    }

    public int Written => written;

    public bool TryWrite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(T value, out int offset)
    {
        if (!IsSupportedType<T>())
        {
            offset = 0;
            return false;
        }

        var size = Unsafe.SizeOf<T>();
        if (storage.Length < written + size)
        {
            offset = 0;
            return false;
        }

        // NOTE: use in ZLoggerInterpolatedStringHandler, always called TryWrite before Read operations so make Read operation in write.
        ReaderCache.Register<T>();

        Unsafe.WriteUnaligned(ref storage[written], value);
        offset = written;
        written += Unsafe.SizeOf<T>();
        return true;
    }

    public bool TryRead<T>(int offset, out T value)
    {
        if (!IsSupportedType<T>()
            || offset < 0
            || (storage.Length < offset + Unsafe.SizeOf<T>()))
        {
            value = default!;
            return false;
        }

        value = Unsafe.ReadUnaligned<T>(ref storage[offset]);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    T Read<T>(int offset)
    {
        if (!TryRead<T>(offset, out var value))
        {
            ThrowArgumentOutOfRangeException();
        }
        return value;
    }

    public object? ReadBoxed(Type type, int offset)
    {
        return ReaderCache.ReadBoxed(type, this, offset);
    }

    public bool TryReadTo(Type type, int offset, int alignment, string? format, ref Utf8StringWriter<IBufferWriter<byte>> handler)
    {
        if (offset < 0) return false;
        if (type.IsEnum) goto USE_READER;

        var code = Type.GetTypeCode(type);
        switch (code)
        {
            case TypeCode.Boolean:
                handler.AppendFormatted(Read<Boolean>(offset), alignment, format);
                return true;
            case TypeCode.Char:
                handler.AppendFormatted(Read<Char>(offset), alignment, format);
                return true;
            case TypeCode.SByte:
                handler.AppendFormatted(Read<SByte>(offset), alignment, format);
                return true;
            case TypeCode.Byte:
                handler.AppendFormatted(Read<Byte>(offset), alignment, format);
                return true;
            case TypeCode.Int16:
                handler.AppendFormatted(Read<Int16>(offset), alignment, format);
                return true;
            case TypeCode.UInt16:
                handler.AppendFormatted(Read<UInt16>(offset), alignment, format);
                return true;
            case TypeCode.Int32:
                handler.AppendFormatted(Read<Int32>(offset), alignment, format);
                return true;
            case TypeCode.UInt32:
                handler.AppendFormatted(Read<UInt32>(offset), alignment, format);
                return true;
            case TypeCode.Int64:
                handler.AppendFormatted(Read<Int64>(offset), alignment, format);
                return true;
            case TypeCode.UInt64:
                handler.AppendFormatted(Read<UInt64>(offset), alignment, format);
                return true;
            case TypeCode.Single:
                handler.AppendFormatted(Read<Single>(offset), alignment, format);
                return true;
            case TypeCode.Double:
                handler.AppendFormatted(Read<Double>(offset), alignment, format);
                return true;
            case TypeCode.Decimal:
                handler.AppendFormatted(Read<Decimal>(offset), alignment, format);
                return true;
            case TypeCode.DateTime:
                handler.AppendFormatted(Read<DateTime>(offset), alignment, format);
                return true;
            default:
                break;
        }

        USE_READER:
        return ReaderCache.TryReadTo(type, this, offset, ref handler, alignment, format);
    }

    public bool TryReadTo(Type type, int offset, int alignment, string? format, ref DefaultInterpolatedStringHandler handler)
    {
        if (offset < 0) return false;
        if (type.IsEnum) goto USE_READER;

        var code = Type.GetTypeCode(type);
        switch (code)
        {
            case TypeCode.Boolean:
                handler.AppendFormatted(Read<Boolean>(offset), alignment, format);
                return true;
            case TypeCode.Char:
                handler.AppendFormatted(Read<Char>(offset), alignment, format);
                return true;
            case TypeCode.SByte:
                handler.AppendFormatted(Read<SByte>(offset), alignment, format);
                return true;
            case TypeCode.Byte:
                handler.AppendFormatted(Read<Byte>(offset), alignment, format);
                return true;
            case TypeCode.Int16:
                handler.AppendFormatted(Read<Int16>(offset), alignment, format);
                return true;
            case TypeCode.UInt16:
                handler.AppendFormatted(Read<UInt16>(offset), alignment, format);
                return true;
            case TypeCode.Int32:
                handler.AppendFormatted(Read<Int32>(offset), alignment, format);
                return true;
            case TypeCode.UInt32:
                handler.AppendFormatted(Read<UInt32>(offset), alignment, format);
                return true;
            case TypeCode.Int64:
                handler.AppendFormatted(Read<Int64>(offset), alignment, format);
                return true;
            case TypeCode.UInt64:
                handler.AppendFormatted(Read<UInt64>(offset), alignment, format);
                return true;
            case TypeCode.Single:
                handler.AppendFormatted(Read<Single>(offset), alignment, format);
                return true;
            case TypeCode.Double:
                handler.AppendFormatted(Read<Double>(offset), alignment, format);
                return true;
            case TypeCode.Decimal:
                handler.AppendFormatted(Read<Decimal>(offset), alignment, format);
                return true;
            case TypeCode.DateTime:
                handler.AppendFormatted(Read<DateTime>(offset), alignment, format);
                return true;
            default:
                break;
        }

        USE_READER:
        return ReaderCache.TryReadTo(type, this, offset, ref handler, alignment, format);
    }

    public bool TryReadTo(Type type, int offset, Utf8JsonWriter jsonWriter, JsonSerializerOptions? options)
    {
        if (offset < 0) return false;
        if (type.IsEnum) goto USE_READER;

        var code = Type.GetTypeCode(type);
        switch (code)
        {
            case TypeCode.Boolean:
                jsonWriter.WriteBooleanValue(Read<Boolean>(offset));
                return true;
            case TypeCode.Char:
                var c = Read<char>(offset);
                Span<char> cs = stackalloc char[1];
                cs[0] = c;
                jsonWriter.WriteStringValue(cs);
                return true;
            case TypeCode.SByte:
                jsonWriter.WriteNumberValue(Read<SByte>(offset));
                return true;
            case TypeCode.Byte:
                jsonWriter.WriteNumberValue(Read<Byte>(offset));
                return true;
            case TypeCode.Int16:
                jsonWriter.WriteNumberValue(Read<Int16>(offset));
                return true;
            case TypeCode.UInt16:
                jsonWriter.WriteNumberValue(Read<UInt16>(offset));
                return true;
            case TypeCode.Int32:
                jsonWriter.WriteNumberValue(Read<Int32>(offset));
                return true;
            case TypeCode.UInt32:
                jsonWriter.WriteNumberValue(Read<UInt32>(offset));
                return true;
            case TypeCode.Int64:
                jsonWriter.WriteNumberValue(Read<Int64>(offset));
                return true;
            case TypeCode.UInt64:
                jsonWriter.WriteNumberValue(Read<UInt64>(offset));
                return true;
            case TypeCode.Single:
                jsonWriter.WriteNumberValue(Read<Single>(offset));
                return true;
            case TypeCode.Double:
                jsonWriter.WriteNumberValue(Read<Double>(offset));
                return true;
            case TypeCode.Decimal:
                jsonWriter.WriteNumberValue(Read<Decimal>(offset));
                return true;
            case TypeCode.DateTime:
                jsonWriter.WriteStringValue(Read<DateTime>(offset));
                return true;
            default:
                break;
        }

        USE_READER:
        return ReaderCache.TryReadTo(type, this, offset, jsonWriter, options);
    }

    static bool IsSupportedType<T>()
    {
#if NETSTANDARD2_0
        var type = typeof(T);
        var code = Type.GetTypeCode(type);
        switch (code)
        {
            case TypeCode.Boolean:
            case TypeCode.Byte:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
        }
        if (type.IsEnum || type == typeof(Guid) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan))
        {
            return true;
        }
        return false;
#else
        return !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#endif
    }

    static void ThrowArgumentOutOfRangeException()
    {
        throw new ArgumentOutOfRangeException();
    }

    static class ReaderCache
    {
        sealed class Handlers(
            delegate* managed<MagicalBox, int, Utf8JsonWriter, JsonSerializerOptions?, bool> utf8JsonWriter,
            delegate* managed<MagicalBox, int, ref DefaultInterpolatedStringHandler, int, string?, bool> stringHandler, // box, offset, handler, alignment, format
            delegate* managed<MagicalBox, int, ref Utf8StringWriter<IBufferWriter<byte>>, int, string?, bool> utf8StringWriter,
            delegate* managed<MagicalBox, int, object?> readBoxed)
        {
            public readonly delegate* managed<MagicalBox, int, Utf8JsonWriter, JsonSerializerOptions?, bool> Utf8JsonWriter = utf8JsonWriter;
            public readonly delegate* managed<MagicalBox, int, ref DefaultInterpolatedStringHandler, int, string?, bool> StringHandler = stringHandler;
            public readonly delegate* managed<MagicalBox, int, ref Utf8StringWriter<IBufferWriter<byte>>, int, string?, bool> Utf8StringWriter = utf8StringWriter;
            public readonly delegate* managed<MagicalBox, int, object?> ReadBoxed = readBoxed;
        }

        static readonly ConcurrentDictionary<Type, Handlers> cache = new();

        public static bool TryReadTo(Type type, MagicalBox box, int offset, Utf8JsonWriter jsonWriter, JsonSerializerOptions? options) => cache.TryGetValue(type, out var value) ? value.Utf8JsonWriter(box, offset, jsonWriter, options) : false;
        public static bool TryReadTo(Type type, MagicalBox box, int offset, ref DefaultInterpolatedStringHandler handler, int alignment, string? format) => cache.TryGetValue(type, out var value) ? value.StringHandler(box, offset, ref handler, alignment, format) : false;
        public static bool TryReadTo(Type type, MagicalBox box, int offset, ref Utf8StringWriter<IBufferWriter<byte>> writer, int alignment, string? format) => cache.TryGetValue(type, out var value) ? value.Utf8StringWriter(box, offset, ref writer, alignment, format) : false;
        public static object? ReadBoxed(Type type, MagicalBox box, int offset) => cache.TryGetValue(type, out var value) ? value.ReadBoxed(box, offset) : null;

        public static void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
        {
            if (IsRegistered<T>.Value)
            {
                return;
            }

            if (!IsSupportedType<T>())
            {
                return;
            }

            if (typeof(T).IsEnum)
            {
                cache.TryAdd(typeof(T), new Handlers(&EnumJsonWrite<T>, &EnumStringWrite<T>, &EnumUtf8Write<T>, &ReadBoxed<T>));
            }
            else if (typeof(T) == typeof(Guid))
            {
                cache.TryAdd(typeof(Guid), new Handlers(&GuidJsonWrite, &StringAppendFormatted<Guid>, &Utf8AppendFormatted<Guid>, &ReadBoxed<Guid>));
            }
            else if (typeof(T) == typeof(DateTimeOffset))
            {
                cache.TryAdd(typeof(DateTimeOffset), new Handlers(&DateTimeOffsetJsonWrite, &StringAppendFormatted<DateTimeOffset>, &Utf8AppendFormatted<DateTimeOffset>, &ReadBoxed<DateTimeOffset>));
            }
            else
            {
                cache.TryAdd(typeof(T), new Handlers(&JsonSerialize<T>, &StringAppendFormatted<T>, &Utf8AppendFormatted<T>, &ReadBoxed<T>));
            }

            IsRegistered<T>.Value = true;
        }

        static object? ReadBoxed<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(MagicalBox box, int offset)
        {
            if (box.TryRead<T>(offset, out var v))
            {
                return (object?)v;
            }
            return null;
        }

        static bool EnumJsonWrite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(MagicalBox box, int offset, Utf8JsonWriter writer, JsonSerializerOptions? options)
        {
            if (box.TryRead<T>(offset, out var v))
            {
                unsafe
                {
                    var name = EnumDictionary<T>.GetJsonEncodedName(v);
                    if (name != null)
                    {
                        writer.WriteStringValue(name.Value);
                    }
                    else
                    {
                        writer.WriteStringValue(v!.ToString());
                    }
                }
                return true;
            }

            return false;
        }

        static bool EnumStringWrite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(MagicalBox box, int offset, ref DefaultInterpolatedStringHandler handler, int alignment, string? format)
        {
            if (box.TryRead<T>(offset, out var v))
            {
                unsafe
                {
                    var name = EnumDictionary<T>.GetStringName(v);
                    if (name != null)
                    {
                        handler.AppendFormatted(name, alignment, format);
                    }
                    else
                    {
                        handler.AppendFormatted(v, alignment, format);
                    }
                }
                return true;
            }

            return false;
        }

        static bool EnumUtf8Write<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(MagicalBox box, int offset, ref Utf8StringWriter<IBufferWriter<byte>> writer, int alignment, string? format)
        {
            if (box.TryRead<T>(offset, out var v))
            {
                unsafe
                {
                    var name = EnumDictionary<T>.GetUtf8Name(v);
                    if (name.Length != 0)
                    {
                        writer.AppendUtf8(name);
                    }
                    else
                    {
                        writer.AppendFormatted(v, alignment, format);
                    }
                }
                return true;
            }

            return false;
        }

        static bool GuidJsonWrite(MagicalBox box, int offset, Utf8JsonWriter writer, JsonSerializerOptions? options)
        {
            if (box.TryRead<Guid>(offset, out var v))
            {
                writer.WriteStringValue(v);
                return true;
            }

            return false;
        }

        static bool DateTimeOffsetJsonWrite(MagicalBox box, int offset, Utf8JsonWriter writer, JsonSerializerOptions? options)
        {
            if (box.TryRead<DateTimeOffset>(offset, out var v))
            {
                writer.WriteStringValue(v);
                return true;
            }

            return false;
        }

        [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode")]
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode")]
        static bool JsonSerialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(MagicalBox box, int offset, Utf8JsonWriter writer, JsonSerializerOptions? options)
        {
            if (box.TryRead<T>(offset, out var v))
            {
                JsonSerializer.Serialize(writer, v, options);
                return true;
            }

            return false;
        }

        static bool StringAppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(MagicalBox box, int offset, ref DefaultInterpolatedStringHandler handler, int alignment, string? format)
        {
            if (box.TryRead<T>(offset, out var v))
            {
                handler.AppendFormatted(v, alignment, format);
                return true;
            }

            return false;
        }

        static bool Utf8AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(MagicalBox box, int offset, ref Utf8StringWriter<IBufferWriter<byte>> writer, int alignment, string? format)
        {
            if (box.TryRead<T>(offset, out var v))
            {
                writer.AppendFormatted(v, alignment, format);
                return true;
            }

            return false;
        }

        static class IsRegistered<T>
        {
            public static bool Value;
        }
    }
}
