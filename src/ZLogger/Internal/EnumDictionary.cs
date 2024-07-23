#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace ZLogger.Internal;

internal static unsafe class EnumDictionary<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
{
    static object dictionary;

    public static readonly delegate* managed<T, string?> GetStringName;
    public static readonly delegate* managed<T, ReadOnlySpan<byte>> GetUtf8Name; // be careful, zero is not found.
    public static readonly delegate* managed<T, JsonEncodedText?> GetJsonEncodedName;

    [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode")]
    static EnumDictionary()
    {
        var type = typeof(T);
        var source = new List<(T, EnumName)>();
        foreach (var key in Enum.GetValues(type))
        {
            var name = Enum.GetName(type, key);
            var utf8Name = Encoding.UTF8.GetBytes(name!);
            var jsonName = JsonEncodedText.Encode(utf8Name);

            if (name != null)
            {
                source.Add(((T)key, new EnumName(name, utf8Name, jsonName)));
            }
        }

        var e = Enum.GetUnderlyingType(type);
        var code = Type.GetTypeCode(e);
        switch (code)
        {
            case TypeCode.SByte:
                dictionary = source.Select(x => (Unsafe.As<T, SByte>(ref x.Item1), x.Item2)).Distinct(new DistinctEqualityComparer<SByte>()).ToFrozenDictionary(x => x.Item1, x => x.Item2);
                GetStringName = &GetStringNameCore<SByte>;
                GetUtf8Name = &GetUtf8NameCore<SByte>;
                GetJsonEncodedName = &GetJsonEncodedTextCore<SByte>;
                break;
            case TypeCode.Byte:
                dictionary = source.Select(x => (Unsafe.As<T, Byte>(ref x.Item1), x.Item2)).Distinct(new DistinctEqualityComparer<Byte>()).ToFrozenDictionary(x => x.Item1, x => x.Item2);
                GetStringName = &GetStringNameCore<Byte>;
                GetUtf8Name = &GetUtf8NameCore<Byte>;
                GetJsonEncodedName = &GetJsonEncodedTextCore<Byte>;
                break;
            case TypeCode.Int16:
                dictionary = source.Select(x => (Unsafe.As<T, Int16>(ref x.Item1), x.Item2)).Distinct(new DistinctEqualityComparer<Int16>()).ToFrozenDictionary(x => x.Item1, x => x.Item2);
                GetStringName = &GetStringNameCore<Int16>;
                GetUtf8Name = &GetUtf8NameCore<Int16>;
                GetJsonEncodedName = &GetJsonEncodedTextCore<Int16>;
                break;
            case TypeCode.UInt16:
                dictionary = source.Select(x => (Unsafe.As<T, UInt16>(ref x.Item1), x.Item2)).Distinct(new DistinctEqualityComparer<UInt16>()).ToFrozenDictionary(x => x.Item1, x => x.Item2);
                GetStringName = &GetStringNameCore<UInt16>;
                GetUtf8Name = &GetUtf8NameCore<UInt16>;
                GetJsonEncodedName = &GetJsonEncodedTextCore<UInt16>;
                break;
            case TypeCode.Int32:
                dictionary = source.Select(x => (Unsafe.As<T, Int32>(ref x.Item1), x.Item2)).Distinct(new DistinctEqualityComparer<Int32>()).ToFrozenDictionary(x => x.Item1, x => x.Item2);
                GetStringName = &GetStringNameCore<Int32>;
                GetUtf8Name = &GetUtf8NameCore<Int32>;
                GetJsonEncodedName = &GetJsonEncodedTextCore<Int32>;
                break;
            case TypeCode.UInt32:
                dictionary = source.Select(x => (Unsafe.As<T, UInt32>(ref x.Item1), x.Item2)).Distinct(new DistinctEqualityComparer<UInt32>()).ToFrozenDictionary(x => x.Item1, x => x.Item2);
                GetStringName = &GetStringNameCore<UInt32>;
                GetUtf8Name = &GetUtf8NameCore<UInt32>;
                GetJsonEncodedName = &GetJsonEncodedTextCore<UInt32>;
                break;
            case TypeCode.Int64:
                dictionary = source.Select(x => (Unsafe.As<T, Int64>(ref x.Item1), x.Item2)).Distinct(new DistinctEqualityComparer<Int64>()).ToFrozenDictionary(x => x.Item1, x => x.Item2);
                GetStringName = &GetStringNameCore<Int64>;
                GetUtf8Name = &GetUtf8NameCore<Int64>;
                GetJsonEncodedName = &GetJsonEncodedTextCore<Int64>;
                break;
            case TypeCode.UInt64:
                dictionary = source.Select(x => (Unsafe.As<T, UInt64>(ref x.Item1), x.Item2)).Distinct(new DistinctEqualityComparer<UInt64>()).ToFrozenDictionary(x => x.Item1, x => x.Item2);
                GetStringName = &GetStringNameCore<UInt64>;
                GetUtf8Name = &GetUtf8NameCore<UInt64>;
                GetJsonEncodedName = &GetJsonEncodedTextCore<UInt64>;
                break;
            default:
                throw new ArgumentException();
        }
    }

    static string? GetStringNameCore<TUnderlying>(T value)
        where TUnderlying : notnull
    {
#if NET8_0_OR_GREATER
        if (Unsafe.As<object, FrozenDictionary<TUnderlying, EnumName>>(ref dictionary).TryGetValue(Unsafe.As<T, TUnderlying>(ref value), out var entry))
#else
        if (Unsafe.As<object, Dictionary<TUnderlying, EnumName>>(ref dictionary).TryGetValue(Unsafe.As<T, TUnderlying>(ref value), out var entry))
#endif
        {
            return entry.Name;
        }
        else
        {
            return null;
        }
    }

    static ReadOnlySpan<byte> GetUtf8NameCore<TUnderlying>(T value)
        where TUnderlying : notnull
    {
#if NET8_0_OR_GREATER
        if (Unsafe.As<object, FrozenDictionary<TUnderlying, EnumName>>(ref dictionary).TryGetValue(Unsafe.As<T, TUnderlying>(ref value), out var entry))
#else
        if (Unsafe.As<object, Dictionary<TUnderlying, EnumName>>(ref dictionary).TryGetValue(Unsafe.As<T, TUnderlying>(ref value), out var entry))
#endif
        {
            return entry.Utf8Name;
        }
        else
        {
            return null;
        }
    }

    static JsonEncodedText? GetJsonEncodedTextCore<TUnderlying>(T value)
        where TUnderlying : notnull
    {
#if NET8_0_OR_GREATER
        if (Unsafe.As<object, FrozenDictionary<TUnderlying, EnumName>>(ref dictionary).TryGetValue(Unsafe.As<T, TUnderlying>(ref value), out var entry))
#else
        if (Unsafe.As<object, Dictionary<TUnderlying, EnumName>>(ref dictionary).TryGetValue(Unsafe.As<T, TUnderlying>(ref value), out var entry))
#endif
        {
            return entry.JsonEncoded;
        }
        else
        {
            return null;
        }
    }
}

file sealed class DistinctEqualityComparer<T> : IEqualityComparer<(T, EnumName)>
        where T : IEquatable<T>
{
    public bool Equals((T, EnumName) x, (T, EnumName) y)
    {
        return x.Item1.Equals(y.Item1);
    }

    public int GetHashCode([DisallowNull] (T, EnumName) obj)
    {
        return obj.Item1.GetHashCode();
    }
}

file sealed class EnumName(string name, byte[] utf8Name, JsonEncodedText jsonEncoded)
{
    public readonly string Name = name;
    public readonly byte[] Utf8Name = utf8Name;
    public readonly JsonEncodedText JsonEncoded = jsonEncoded;

    // for debugging
    public override string ToString()
    {
        return Name;
    }
}

#if !NET8_0_OR_GREATER
file static class ShimsExtensions
{
    public static Dictionary<TKey, TValue> ToFrozenDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        where TKey : notnull
    {
        return source.ToDictionary(keySelector, valueSelector);
    }
}
#endif