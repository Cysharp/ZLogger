using System.Runtime.CompilerServices;

namespace ZLogger.Internal;

internal static class UnsafeListHelper
{
    internal static Span<T> AsSpan<T>(List<T> list)
    {
        return Unsafe.As<StrongBox<T[]>>(list).Value.AsSpan(0, list.Count);
    }
}