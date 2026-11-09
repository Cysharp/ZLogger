using System.Collections.Generic;
using System.Linq;
using ZLogger.Internal;

namespace ZLogger.Tests
{
    public class SpanListTest
    {
        /// <summary>
        /// Verifies that elements added one by one across multiple growths of the backing array are all preserved in order
        /// and exposed through AsSpan with the exact count.
        /// </summary>
        [Fact]
        public void AddGrowsAndPreservesOrder()
        {
            var list = new SpanList<int>();
            list.Count.Should().Be(0);
            list.AsSpan().Length.Should().Be(0);

            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }

            list.Count.Should().Be(100);
            list.AsSpan().ToArray().Should().Equal(Enumerable.Range(0, 100));
        }

        /// <summary>
        /// Verifies that AddRange appends elements from both ICollection and non-collection sources
        /// after the existing elements.
        /// </summary>
        [Fact]
        public void AddRangeAppendsFromCollectionAndEnumerable()
        {
            var list = new SpanList<int>();
            list.Add(-1);

            list.AddRange(new List<int> { 0, 1, 2, 3, 4, 5 });
            list.AddRange(new List<int>());
            list.AddRange(Enumerate(6, 10));

            list.AsSpan().ToArray().Should().Equal(Enumerable.Range(-1, 17));

            // An iterator method never implements ICollection<T>, so this source always exercises the enumeration path of AddRange.
            static IEnumerable<int> Enumerate(int start, int count)
            {
                for (var i = 0; i < count; i++)
                {
                    yield return start + i;
                }
            }
        }

        /// <summary>
        /// Verifies that AddRange with an ICollection source correctly grows the backing array
        /// both when the list is empty and the source exceeds the default capacity,
        /// and when the doubled capacity is sufficient for the appended elements.
        /// </summary>
        [Fact]
        public void AddRangeFromCollectionGrowsBeyondDefaultCapacity()
        {
            var list = new SpanList<int>();

            list.AddRange(new List<int>(Enumerable.Range(0, 10)));
            list.Count.Should().Be(10);
            list.AsSpan().ToArray().Should().Equal(Enumerable.Range(0, 10));

            list.AddRange(new List<int>(Enumerable.Range(10, 3)));
            list.Count.Should().Be(13);
            list.AsSpan().ToArray().Should().Equal(Enumerable.Range(0, 13));
        }

        /// <summary>
        /// Verifies that a list constructed from a span holds an independent copy of the source elements.
        /// </summary>
        [Fact]
        public void ConstructFromSpanCopiesElements()
        {
            var source = new SpanList<string>();
            source.Add("x");
            source.Add(null);
            source.Add("y");

            var copy = new SpanList<string>(source.AsSpan());
            source.Clear();

            copy.Count.Should().Be(3);
            copy.AsSpan().ToArray().Should().Equal("x", null, "y");
        }
    }
}
