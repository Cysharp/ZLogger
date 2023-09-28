#pragma warning disable CS8601
#pragma warning disable CS8618

using Cysharp.Text;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ZLogger.Entries
{
    public struct MessageLogState<TPayload> : IZLoggerState
    {
        public static readonly Func<MessageLogState<TPayload>, LogInfo, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Message;

        public MessageLogState([AllowNull]TPayload payload, string message)
        {
            Payload = payload;
            Message = message;
        }

        static IZLoggerEntry factory(MessageLogState<TPayload> self, LogInfo logInfo)
        {
            return self.CreateLogEntry(logInfo);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return MessageLogEntry<TPayload>.Create(logInfo, this);
        }
    }

    public class MessageLogEntry<TPayload> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<MessageLogEntry<TPayload>> cache = new ConcurrentQueue<MessageLogEntry<TPayload>>();

        MessageLogState<TPayload> state;

        public LogInfo LogInfo { get; private set; }

        MessageLogEntry()
        {
        }

        public static MessageLogEntry<TPayload> Create(in LogInfo logInfo, in MessageLogState<TPayload> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new MessageLogEntry<TPayload>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.Append(state.Message);
                formatter.FormatLogEntry(writer,this, (string)null, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }
}
