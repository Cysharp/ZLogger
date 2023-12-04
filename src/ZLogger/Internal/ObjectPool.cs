using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ZLogger.Internal
{
    internal interface IObjectPoolNode<T>
    {
        ref T? NextNode { get; }
    }

    internal sealed class ObjectPool<T>
        where T : class, IObjectPoolNode<T>
    {
        int gate;
        T? root;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPop([NotNullWhen(true)] out T? result)
        {
        // similar as ImmutableInterlocked
        TRY_AGAIN:
            if (Interlocked.CompareExchange(ref gate, 1, 0) == 0)
            {
                var v = root;
                if (!(v is null))
                {
                    ref var nextNode = ref v.NextNode;
                    root = nextNode;
                    nextNode = null;
                    result = v;
                    Volatile.Write(ref gate, 0);
                    return true;
                }

                Volatile.Write(ref gate, 0);
            }
            else
            {
                goto TRY_AGAIN;
            }
            result = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPush(T item)
        {
        TRY_AGAIN:
            if (Interlocked.CompareExchange(ref gate, 1, 0) == 0)
            {
                item.NextNode = root;
                root = item;
                Volatile.Write(ref gate, 0);
                return true;
            }
            else
            {
                goto TRY_AGAIN;
            }
        }
    }
}
