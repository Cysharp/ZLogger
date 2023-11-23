using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace ZLogger.Internal;

internal sealed class EnumDictionary
{
    readonly Entry[][] buckets; // immutable array.
    readonly int indexFor;
    const float LoadFactor = 0.75f;

    EnumDictionary(Entry[][] buckets)
    {
        this.buckets = buckets;
        this.indexFor = buckets.Length - 1;
    }

    internal static EnumDictionary Create<T>()
    {
        var type = typeof(T);

        if (!type.IsEnum) throw new ArgumentException();

        // load Enum value and name pair
        var values = new List<(string, object)>();
        foreach (var value in Enum.GetValues(type))
        {
            var name = Enum.GetName(type, value);
            values.Add((name!, value));
        }

        // create dictionary
        var tableSize = CalculateCapacity(values.Count, LoadFactor);
        var buckets = new Entry[tableSize][];
        var dict = new EnumDictionary(buckets);

        foreach (var (name, enumValue) in values)
        {
            // dictionary key is enumValue, value is name.
            var unboxedEnumValue = (T)enumValue;

            var keySpan =
#if NETSTANDARD2_0                
                Shims.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref unboxedEnumValue), Unsafe.SizeOf<T>());
#else            
                MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref unboxedEnumValue), Unsafe.SizeOf<T>());
#endif            
            var key = keySpan.ToArray();

            var value1 = Encoding.UTF8.GetBytes(name);
            var value2 = JsonEncodedText.Encode(value1);

            var entry = new Entry(key, name, value1, value2);
            dict.TryAddInternal(entry);
        }

        return dict;
    }

    ref Entry GetValueRefOrNullRef(ReadOnlySpan<byte> key)
    {
        var table = buckets;
        var hash = GetBytesHashCode(key);
        var entry = table[hash & indexFor];

        if (entry == null) goto NOT_FOUND;

        for (int i = 0; i < entry.Length; i++)
        {
            ref var v = ref entry[i];

            if (v.Key.AsSpan().SequenceEqual(key))
            {
                return ref v;
            }
        }

    NOT_FOUND:
        return ref Unsafe.NullRef<Entry>();
    }

    public string? GetStringName(ReadOnlySpan<byte> value)
    {
        ref var entry = ref GetValueRefOrNullRef(value);
        if (Unsafe.IsNullRef(ref entry)) return null;
        return entry.Name;
    }

    public ReadOnlySpan<byte> GetUtf8Name(ReadOnlySpan<byte> value)
    {
        ref var entry = ref GetValueRefOrNullRef(value);
        if (Unsafe.IsNullRef(ref entry)) return Array.Empty<byte>();
        return entry.Utf8Name;
    }

    public JsonEncodedText? GetJsonEncodedName(ReadOnlySpan<byte> value)
    {
        ref var entry = ref GetValueRefOrNullRef(value);
        if (Unsafe.IsNullRef(ref entry)) return default;
        return entry.JsonEncoded;
    }

    bool TryAddInternal(Entry entry)
    {
        var h = GetBytesHashCode(entry.Key);

        var array = buckets[h & (indexFor)];
        if (array == null)
        {
            buckets[h & (indexFor)] = new[] { entry };
        }
        else
        {
            // check duplicate
            for (int i = 0; i < array.Length; i++)
            {
                var e = array[i].Key;
                if (entry.Key.AsSpan().SequenceEqual(e))
                {
                    return false;
                }
            }

            var newArray = new Entry[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            array = newArray;
            array[array.Length - 1] = entry;
            buckets[h & (indexFor)] = array;
        }

        return true;
    }

    static int CalculateCapacity(int collectionSize, float loadFactor)
    {
        var size = (int)(((float)collectionSize) / loadFactor);

        size--;
        size |= size >> 1;
        size |= size >> 2;
        size |= size >> 4;
        size |= size >> 8;
        size |= size >> 16;
        size += 1;

        if (size < 8)
        {
            size = 8;
        }
        return size;
    }

    static int GetBytesHashCode(ReadOnlySpan<byte> bytes)
    {
        unchecked
        {
            return (int)XxHash3.HashToUInt64(bytes);
        }
    }

    readonly struct Entry(byte[] key, string name, byte[] utf8Name, JsonEncodedText jsonEncoded)
    {
        public readonly byte[] Key = key;
        public readonly string Name = name;
        public readonly byte[] Utf8Name = utf8Name;
        public readonly JsonEncodedText JsonEncoded = jsonEncoded;

        // for debugging
        public override string ToString()
        {
            return Name;
        }
    }
}

internal static class EnumLookup<T>
{
    static readonly EnumDictionary instance = EnumDictionary.Create<T>();

    public static string? GetStringName(ReadOnlySpan<byte> value) => instance.GetStringName(value);

    public static ReadOnlySpan<byte> GetUtf8Name(ReadOnlySpan<byte> value) => instance.GetUtf8Name(value);
    public static JsonEncodedText? GetJsonEncodedName(ReadOnlySpan<byte> value) => instance.GetJsonEncodedName(value);

    public static JsonEncodedText? GetJsonEncodedName(T value)
    {
#if NETSTANDARD2_0
        var key = Shims.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>());
#else
        var key = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>());
#endif
        return GetJsonEncodedName(key);
    }
}
