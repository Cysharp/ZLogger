using ZLogger.Internal;

namespace ZLogger.Tests
{
    public class MessageSequenceTest
    {
        /// <summary>
        /// Verifies that the cache key registered by GetOrCreate is independent of the list passed in,
        /// so that clearing and refilling that list (as the pooled literal list is) does not corrupt the cached entry.
        /// </summary>
        [Fact]
        public void GetOrCreateCacheKeyIsIndependentOfPassedList()
        {
            // Use literals unique to this test because the cache is static and shared with other tests.
            var pooled = new SpanList<string>();
            pooled.Add("MessageSequenceTest-key-independence-a");
            pooled.Add(null);
            var sequenceA = MessageSequence.GetOrCreate(38, 1, pooled);

            pooled.Clear();
            pooled.Add("MessageSequenceTest-key-independence-b");
            pooled.Add(null);
            var sequenceB = MessageSequence.GetOrCreate(38, 1, pooled);

            sequenceB.Should().NotBeSameAs(sequenceA);

            // Look up with a different list instance; this would miss if the registered key aliased the pooled list,
            // because that list now holds the literals of B.
            var other = new SpanList<string>();
            other.Add("MessageSequenceTest-key-independence-a");
            other.Add(null);
            MessageSequence.GetOrCreate(38, 1, other).Should().BeSameAs(sequenceA);
        }
    }
}
