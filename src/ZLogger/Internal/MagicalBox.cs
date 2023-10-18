using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Utf8StringInterpolation;

namespace ZLogger.Internal;

internal unsafe struct MagicalBox
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
        return ReaderCache.GetReadMethod(type)?.Invoke(this, offset);
    }

    public bool TryReadTo(Type type, int offset, int alignment, string? format, ref Utf8StringWriter<IBufferWriter<byte>> handler)
    {
        if (offset < 0) return false;

        // TODO: many types...?
        if (type == typeof(int))
        {
            handler.AppendFormatted(Read<int>(offset), alignment, format);
        }

        return true;
    }

    public bool TryReadTo(Type type, int offset, int alignment, string? format, ref DefaultInterpolatedStringHandler handler)
    {
        if (offset < 0) return false;

        if (type == typeof(int))
        {
            handler.AppendFormatted(Read<int>(offset), alignment, format);
        }
        return true;
    }

    public bool TryReadTo(Type type, int offset, string propertyName, Utf8JsonWriter jsonWriter)
    {
        if (offset < 0) return false;




        if (type == typeof(int))
        {
            //jsonWriter.WriteNumberValue(

            //handler.AppendFormatted(Read<int>(offset), alignment, format);
        }
        return true;
    }

    static void ThrowArgumentOutOfRangeException()
    {
        throw new ArgumentOutOfRangeException();
    }

    static class ReaderCache
    {
        internal static readonly ConcurrentDictionary<Type, Func<MagicalBox, int, object?>?> cache = new ConcurrentDictionary<Type, Func<MagicalBox, int, object?>?>();

        public static Func<MagicalBox, int, object?>? GetReadMethod(Type type)
        {
            return cache.GetValueOrDefault(type);
        }
    }

    static class ReaderCache<T>
    {
        static ReaderCache()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                ReaderCache.cache.TryAdd(typeof(T), null);
                return;
            }

            ReaderCache.cache.TryAdd(typeof(T), static (box, offset) =>
            {
                return box.Read<T>(offset);
            });
        }

        public static void Register()
        {
            // do nothing(call cctor)
        }
    }
}
