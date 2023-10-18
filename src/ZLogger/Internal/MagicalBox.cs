using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        // TODO: return boxed.
        throw new NotImplementedException();
    }

    public bool TryReadTo(Type type, int offset, int alignment, string? format, ref Utf8StringWriter<IBufferWriter<byte>> handler)
    {
        if (offset < 0) return false;
        // if (storage.Length < offset + Unsafe.SizeOf<T>())


        //if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        //{
        //    return false;
        //}

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

        //if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        //{
        //    return false;
        //}

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
}
