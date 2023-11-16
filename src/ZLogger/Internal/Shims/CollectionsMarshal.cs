using System.Runtime.CompilerServices;

#if !NET6_0_OR_GREATER
namespace System.Runtime.InteropServices;

internal static class CollectionsMarshal
{
    internal static Span<T> AsSpan<T>(List<T> list)
    {
        return Unsafe.As<StrongBox<T[]>>(list).Value.AsSpan(0, list.Count);
    }
}
#endif