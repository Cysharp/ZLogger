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

#if USE_LIMIT_AND_SIZE
        int size;
        readonly int limit;

        public ObjectPool(int limit)
        {
            this.limit = limit;
        }

        public int Size => size;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPop([NotNullWhen(true)] out T? result)
        {
            // Instead of lock, use CompareExchange gate.
            // In a worst case, missed cached object(create new one) but it's not a big deal.
            if (Interlocked.CompareExchange(ref gate, 1, 0) == 0)
            {
                var v = root;
                if (!(v is null))
                {
                    ref var nextNode = ref v.NextNode;
                    root = nextNode;
                    nextNode = null;
#if USE_LIMIT_AND_SIZE
                    size--;
#endif
                    result = v;
                    Volatile.Write(ref gate, 0);
                    return true;
                }

                Volatile.Write(ref gate, 0);
            }
            result = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPush(T item)
        {
            if (Interlocked.CompareExchange(ref gate, 1, 0) == 0)
            {
                item.NextNode = root;
                root = item;
                Volatile.Write(ref gate, 0);
                return true;

#if USE_LIMIT_AND_SIZE
                if (size < limit)
                {
                    item.NextNode = root;
                    root = item;
                    size++;
                    Volatile.Write(ref gate, 0);
                    return true;
                }
                else
                {
                    Volatile.Write(ref gate, 0);
                }
#endif
            }
            return false;
        }
    }
}
