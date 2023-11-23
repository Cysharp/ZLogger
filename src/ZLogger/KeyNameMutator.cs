using System.Data.Common;

namespace ZLogger;

public interface IKeyNameMutator
{
    bool IsSupportSlice { get; }
    ReadOnlySpan<char> Slice(ReadOnlySpan<char> source);
    bool TryMutate(ReadOnlySpan<char> source, scoped Span<char> destination, out int written);
}

public static class KeyNameMutator
{
    /// <summary>
    /// Returns the last member name of the source.
    /// </summary>
    public static readonly IKeyNameMutator LastMemberName = new LastMemberNameMutator();
    /// <summary>
    /// The first character converted to lowercase.
    /// </summary>
    public static readonly IKeyNameMutator LowerFirstCharacter = new LowerFirstCharacterMutator();
    /// <summary>
    /// The first character converted to uppercase.
    /// </summary>
    public static readonly IKeyNameMutator UpperFirstCharacter = new UpperFirstCharacterMutator();
    /// <summary>
    /// Returns the last member name of the source with the first character converted to lowercase.
    /// </summary>
    public static readonly IKeyNameMutator LastMemberNameLowerFirstCharacter = new CombineMutator(LastMemberName, LowerFirstCharacter);
    /// <summary>
    /// Returns the last member name of the source with the first character converted to uppercase.
    /// </summary>
    public static readonly IKeyNameMutator LastMemberNameUpperFirstCharacter = new CombineMutator(LastMemberName, UpperFirstCharacter);
}

internal sealed class CombineMutator(IKeyNameMutator sliceSupported, IKeyNameMutator tryMutateSupported) : IKeyNameMutator
{
    public bool IsSupportSlice => false;

    public ReadOnlySpan<char> Slice(ReadOnlySpan<char> source)
    {
        throw new NotSupportedException();
    }

    public bool TryMutate(ReadOnlySpan<char> source, Span<char> destination, out int written)
    {
        source = sliceSupported.Slice(source);
        return tryMutateSupported.TryMutate(source, destination, out written);
    }
}

internal sealed class LastMemberNameMutator : IKeyNameMutator
{
    public bool IsSupportSlice => true;

    public ReadOnlySpan<char> Slice(ReadOnlySpan<char> source)
    {
        var firstOpenParentheses = source.IndexOf('(');
        if (firstOpenParentheses != -1)
        {
            source = source.Slice(0, firstOpenParentheses);
        }

        var lastDot = source.LastIndexOf('.');
        if (lastDot != -1)
        {
            return source.Slice(lastDot + 1);
        }

        return source;
    }

    public bool TryMutate(ReadOnlySpan<char> source, Span<char> destination, out int written)
    {
        throw new NotSupportedException();
    }
}

internal sealed class LowerFirstCharacterMutator : IKeyNameMutator
{
    public bool IsSupportSlice => false;
    public ReadOnlySpan<char> Slice(ReadOnlySpan<char> source) => throw new NotSupportedException();

    public bool TryMutate(ReadOnlySpan<char> source, scoped Span<char> destination, out int written)
    {
        if (source.Length > destination.Length)
        {
            written = default;
            return false;
        }

        if (source.Length <= 0)
        {
            written = 0;
            return true;
        }

        destination[0] = char.ToLowerInvariant(source[0]);
        if (source.Length > 1)
        {
            source[1..].CopyTo(destination[1..]);
        }
        written = source.Length;
        return true;
    }
}

internal sealed class UpperFirstCharacterMutator : IKeyNameMutator
{
    public bool IsSupportSlice => false;
    public ReadOnlySpan<char> Slice(ReadOnlySpan<char> source) => throw new NotSupportedException();

    public bool TryMutate(ReadOnlySpan<char> source, scoped Span<char> destination, out int written)
    {
        if (source.Length > destination.Length)
        {
            written = default;
            return false;
        }

        if (source.Length <= 0)
        {
            written = 0;
            return true;
        }

        destination[0] = char.ToUpperInvariant(source[0]);
        if (source.Length > 1)
        {
            source[1..].CopyTo(destination[1..]);
        }
        written = source.Length;
        return true;
    }
}
