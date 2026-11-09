namespace ZLogger.Internal
{
    // A minimal growable list that owns its backing array, so that the stored elements can be exposed as a span
    // without relying on the private field layout of List<T>.
    internal sealed class SpanList<T>
    {
        const int DefaultCapacity = 4;

        T[] items;
        int count;

        public SpanList()
        {
            items = Array.Empty<T>();
        }

        public SpanList(ReadOnlySpan<T> source)
        {
            items = source.ToArray();
            count = items.Length;
        }

        public int Count => count;

        // The returned span aliases the backing array. It is invalidated by Add/AddRange (which may reallocate the array)
        // and by Clear (which zeroes the elements), so do not keep it across mutations.
        public ReadOnlySpan<T> AsSpan() => items.AsSpan(0, count);

        public void Add(T item)
        {
            if (count == items.Length)
            {
                Grow(count + 1);
            }

            items[count++] = item;
        }

        public void AddRange(IEnumerable<T> source)
        {
            if (source is ICollection<T> collection)
            {
                var sourceCount = collection.Count;
                if (sourceCount == 0) return;

                if (items.Length - count < sourceCount)
                {
                    Grow(count + sourceCount);
                }

                collection.CopyTo(items, count);
                count += sourceCount;
                return;
            }

            // Enumerable.TryGetNonEnumeratedCount is intentionally not used to pre-size the array here.
            // It does not recognize the state created by BeginScope(format, args) (an IReadOnlyList<T> but not an ICollection<T>), which is the most common source,
            // and knowing the count would only save a few reallocations because the elements must be enumerated one by one anyway.
            foreach (var item in source)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            // Clear the stored elements so that the list does not keep references alive.
            Array.Clear(items, 0, count);
            count = 0;
        }

        void Grow(int minimumCapacity)
        {
            var newCapacity = items.Length == 0 ? DefaultCapacity : items.Length * 2;
            if (newCapacity < minimumCapacity)
            {
                newCapacity = minimumCapacity;
            }

            Array.Resize(ref items, newCapacity);
        }
    }
}
