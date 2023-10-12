using System;
using System.Buffers;
using System.Text.Json;
using Cysharp.Text;
using ZLogger.Internal;

namespace ZLogger
{
    // Cachable(Returnable) state holder, store struct state to heap for AsyncLogProcessor.
    public interface IZLoggerEntry : IZLoggerFormattable
    {
        LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter); // TODO: 2 -> 1
        void Return();
    }

    // Create LogEntry from struct state without T constraints 
    public static class LogEntryFactory<T>
    {
        public static CreateLogEntry<T>? Create; // Register from each state static constructor
    }

    public delegate IZLoggerEntry CreateLogEntry<TState>(in LogInfo info, in TState state);

    public sealed class IzLoggerEntry<TState> : IZLoggerEntry, IObjectPoolNode<IzLoggerEntry<TState>>
        where TState : IZLoggerFormattable
    {
        static readonly ObjectPool<IzLoggerEntry<TState>> cache = new();

        IzLoggerEntry<TState>? next;
        ref IzLoggerEntry<TState>? IObjectPoolNode<IzLoggerEntry<TState>>.NextNode => ref next;

        LogInfo logInfo;
        TState state;

        IzLoggerEntry(in LogInfo logInfo, in TState state)
        {
            this.logInfo = logInfo;
            this.state = state;
        }

        public static IzLoggerEntry<TState> Create(in LogInfo logInfo, in TState state)
        {
            if (cache.TryPop(out var entry))
            {
                entry.logInfo = logInfo;
                entry.state = state;
            }
            else
            {
                entry = new IzLoggerEntry<TState>(logInfo, state);
            }
            return entry;
        }

        public override string ToString() => state.ToString();

        public int ParameterCount => state.ParameterCount;
        public bool IsSupportStructuredLogging => state.IsSupportStructuredLogging;
        public ReadOnlySpan<byte> GetParameterKey(int index) => state.GetParameterKey(index);

        public Type GetParameterType(int index) => state.GetParameterType(index);

        public object GetParameterValue(int index) => state.GetParameterValue(index);

        public T GetParameterValue<T>(int index) => state.GetParameterValue<T>(index);

        public void ToString(IBufferWriter<byte> writer) => state.ToString(writer);

        public void WriteJsonMessage(Utf8JsonWriter writer) => state.WriteJsonMessage(writer);

        public void WriteJsonParameterKeyValues(Utf8JsonWriter writer) => state.WriteJsonParameterKeyValues(writer);

        public LogInfo LogInfo => logInfo;

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            formatter.FormatLogEntry(writer, this);
        }

        public void Return()
        {
            state = default!;
            logInfo = default!;
            cache.TryPush(this);
        }
    }
    
    public static class ZLoggerEntryExtensions
    {
        public static string FormatToString(this IZLoggerEntry entry, IZLoggerFormatter formatter)
        {
            var boxedBuilder = (IBufferWriter<byte>)ZString.CreateUtf8StringBuilder();
            try
            {
                entry.FormatUtf8(boxedBuilder, formatter);
                return boxedBuilder.ToString()!;
            }
            finally
            {
                ((Utf8ValueStringBuilder)boxedBuilder).Dispose();
            }
        }
    }
}