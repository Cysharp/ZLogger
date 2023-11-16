using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

    public ReadOnlySpan<byte> AsSpan() => storage.AsSpan(0, written);

    public bool TryWrite<T>(T value, out int offset)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
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

        // NOTE: use in ZLoggerInterpolatedStringHandler, alwayus called TryWrite before Read operations so make Read operation in write.
        ReaderCache<T>.Register();

        Unsafe.WriteUnaligned(ref storage[written], value);
        offset = written;
        written += Unsafe.SizeOf<T>();
        return true;
    }

    public bool TryRead<T>(int offset, out T value)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()
            || offset < 0
            || (storage.Length < offset + Unsafe.SizeOf<T>()))
        {
            value = default!;
            return false;
        }

        value = Unsafe.ReadUnaligned<T>(ref storage[offset]);
        return true;
    }

    public T Read<T>(int offset)
    {
        if (!TryRead<T>(offset, out var value))
        {
            ThrowArgumentOutOfRangeException();
        }
        return value;
    }

    public object? Read(Type type, int offset)
    {
        if (offset < 0) return null;
        return ReaderCache.GetReadMethod(type)?.Invoke(this, offset);
    }

    public ReadOnlySpan<byte> ReadRawEnumValue(Type type, int offset)
    {
        var (_, size, _) = ReaderCache.GetEnumDictionaryAndValueSize(type);
        return MemoryMarshal.CreateReadOnlySpan(ref storage[offset], size);
    }

    public bool TryReadTo(Type type, int offset, int alignment, string? format, ref Utf8StringWriter<IBufferWriter<byte>> handler)
    {
        if (offset < 0) return false;

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

        if (type.IsEnum)
        {
            var (dict, size, converter) = ReaderCache.GetEnumDictionaryAndValueSize(type);
            var rawValue = MemoryMarshal.CreateReadOnlySpan(ref storage[offset], size);
            var name = dict.GetUtf8Name(rawValue);
            if (name == null)
            {
                handler.AppendLiteral(converter(name));
            }
            else
            {
                handler.AppendUtf8(name);
            }
        }

        if (type == typeof(Guid))
        {
            handler.AppendFormatted(Read<Guid>(offset), alignment, format);
        }
        else if (type == typeof(DateTime))
        {
            handler.AppendFormatted(Read<DateTime>(offset), alignment, format);
        }
        else if (type == typeof(DateTimeOffset))
        {
            handler.AppendFormatted(Read<DateTimeOffset>(offset), alignment, format);
        }
        else if (type == typeof(TimeSpan))
        {
            handler.AppendFormatted(Read<TimeSpan>(offset), alignment, format);
        }
#if NET6_0_OR_GREATER
        else if (type == typeof(TimeOnly))
        {
            handler.AppendFormatted(Read<TimeOnly>(offset), alignment, format);
        }
        else if (type == typeof(DateOnly))
        {
            handler.AppendFormatted(Read<DateOnly>(offset), alignment, format);
        }
#endif

        return true;
    }

    public bool TryReadTo(Type type, int offset, int alignment, string? format, ref DefaultInterpolatedStringHandler handler)
    {
        if (offset < 0) return false;

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

        if (type.IsEnum)
        {
            var (dict, size, converter) = ReaderCache.GetEnumDictionaryAndValueSize(type);
            var rawValue = MemoryMarshal.CreateReadOnlySpan(ref storage[offset], size);
            var name = dict.GetStringName(rawValue);
            if (name == null)
            {
                handler.AppendLiteral(converter(rawValue));
            }
            else
            {
                handler.AppendLiteral(name);
            }
        }

        if (type == typeof(Guid))
        {
            handler.AppendFormatted(Read<Guid>(offset), alignment, format);
        }
        else if (type == typeof(DateTime))
        {
            handler.AppendFormatted(Read<DateTime>(offset), alignment, format);
        }
        else if (type == typeof(DateTimeOffset))
        {
            handler.AppendFormatted(Read<DateTimeOffset>(offset), alignment, format);
        }
        else if (type == typeof(TimeSpan))
        {
            handler.AppendFormatted(Read<TimeSpan>(offset), alignment, format);
        }
#if NET6_0_OR_GREATER        
        else if (type == typeof(TimeOnly))
        {
            handler.AppendFormatted(Read<TimeOnly>(offset), alignment, format);
        }
        else if (type == typeof(DateOnly))
        {
            handler.AppendFormatted(Read<DateOnly>(offset), alignment, format);
        }
#endif
        return true;
    }

    public bool TryReadTo(Type type, int offset, Utf8JsonWriter jsonWriter)
    {
        if (offset < 0) return false;

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
                break;
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

        if (type.IsEnum)
        {
            var (dict, size, converter) = ReaderCache.GetEnumDictionaryAndValueSize(type);
            var rawValue = MemoryMarshal.CreateReadOnlySpan(ref storage[offset], size);
            var name = dict.GetJsonEncodedName(rawValue);
            if (name == null)
            {
                jsonWriter.WriteStringValue(converter(rawValue));
            }
            else
            {
                jsonWriter.WriteStringValue(name.Value);
            }
        }

        if (type == typeof(Guid))
        {
            jsonWriter.WriteStringValue(Read<Guid>(offset));
        }
        else if (type == typeof(DateTime))
        {
            jsonWriter.WriteStringValue(Read<DateTime>(offset));
        }
        else if (type == typeof(DateTimeOffset))
        {
            jsonWriter.WriteStringValue(Read<DateTimeOffset>(offset));
        }

        return true;
    }

    static void ThrowArgumentOutOfRangeException()
    {
        throw new ArgumentOutOfRangeException();
    }

    delegate string RawEnumValueToString(ReadOnlySpan<byte> rawEnumValue);

    static class ReaderCache
    {
        internal static readonly ConcurrentDictionary<Type, Func<MagicalBox, int, object?>?> readCache = new();
        internal static readonly ConcurrentDictionary<Type, (EnumDictionary, int, RawEnumValueToString)> enumCache = new();

        // Enum

        public static Func<MagicalBox, int, object?>? GetReadMethod(Type type)
        {
            return readCache.TryGetValue(type, out var value) ? value : null;
        }

        public static (EnumDictionary, int, RawEnumValueToString) GetEnumDictionaryAndValueSize(Type type)
        {
            return enumCache.TryGetValue(type, out var value) ? value : default!;
        }
    }

    static class ReaderCache<T>
    {
        static ReaderCache()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                ReaderCache.readCache.TryAdd(typeof(T), null);
                return;
            }

            ReaderCache.readCache.TryAdd(typeof(T), static (box, offset) =>
            {
                return box.Read<T>(offset);
            });

            if (typeof(T).IsEnum)
            {
                RawEnumValueToString converter = static x =>
                {
                    var v = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(x));
                    return v?.ToString() ?? "";
                };
                ReaderCache.enumCache.TryAdd(typeof(T), (EnumDictionary.Create<T>(), Unsafe.SizeOf<T>(), converter));
            }
        }

        public static void Register()
        {
            // do nothing(call cctor)
        }
    }
}
