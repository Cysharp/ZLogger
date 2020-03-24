using Cysharp.Text;
using System.Buffers;
using System.Collections.Concurrent;

namespace ZLog.Entries
{
    internal static class LogEntryCache
    {
        internal static class Queue<T1, T2>
        {
            static readonly ConcurrentQueue<LogEntry<T1, T2>> q = new ConcurrentQueue<LogEntry<T1, T2>>();

            public static LogEntry<T1, T2> Rent()
            {
                if (q.TryDequeue(out var result))
                {
                    return result;
                }
                else
                {
                    return new LogEntry<T1, T2>(q);
                }
            }
        }
    }

    internal struct LogState<T1, T2> : IZLogState
    {
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;

        public LogState(string format, T1 arg1, T2 arg2)
        {
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public IUtf8LogEntry CreateLogEntry()
        {
            var entry = LogEntryCache.Queue<T1, T2>.Rent();
            entry.State = this;
            return entry;
        }
    }

    internal class LogEntry<T1, T2> : IUtf8LogEntry
    {
        readonly ConcurrentQueue<LogEntry<T1, T2>> globalQueue;

        public LogState<T1, T2> State;

        public LogEntry(ConcurrentQueue<LogEntry<T1, T2>> globalQueue)
        {
            this.globalQueue = globalQueue;
        }

        public void FormatUtf8(IBufferWriter<byte> writer)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(State.Format, State.Arg1, State.Arg2);
                var dest = writer.GetSpan(sb.Length);
                sb.TryCopyTo(dest, out var written);
                writer.Advance(written);
            }
        }

        public void Return()
        {
            State = default;
            globalQueue.Enqueue(this);
        }
    }
}
