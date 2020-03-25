using Cysharp.Text;
using System.Buffers;
using System.Collections.Concurrent;

namespace ZLog.Entries
{
    internal struct FormatLogState<T1, T2> : IZLogState
    {
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;

        public FormatLogState(string format, T1 arg1, T2 arg2)
        {
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public IZLogEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<T1, T2>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<T1, T2> : IZLogEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<T1, T2>> cache = new ConcurrentQueue<FormatLogEntry<T1, T2>>();

        FormatLogState<T1, T2> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<T1, T2> Create(in LogInfo logInfo, in FormatLogState<T1, T2> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<T1, T2>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer)
        {
            // TODO: ZString.FormatUtf8(writer);
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2);
                var dest = writer.GetSpan(sb.Length);
                sb.TryCopyTo(dest, out var written);
                writer.Advance(written);
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }
}
