namespace ZLogger
{
    public interface IKeyNameMutator
    {
        bool TryMutate(ReadOnlySpan<char> source, scoped Span<char> destination, out int written);
    }
    
    public class LowerCamelCaseMutator : IKeyNameMutator
    {
        public static readonly IKeyNameMutator Instance = new LowerCamelCaseMutator();
        
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

    public class UpperCamelCaseMutator : IKeyNameMutator
    {
        public static readonly IKeyNameMutator Instance = new UpperCamelCaseMutator();
        
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
}
