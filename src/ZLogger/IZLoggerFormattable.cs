using System;
using System.Buffers;
using System.Text.Json;
using ZLogger.Internal;

namespace ZLogger
{
    // Implement for log state.
    public interface IZLoggerFormattable
    {
        int ParameterCount { get; }
        string ToString();
        void ToString(IBufferWriter<byte> writer);
        void WriteJsonMessage(Utf8JsonWriter writer);

        // when true, can use GetParameter***, WriteJsonParameterKeyValues.
        bool IsSupportStructuredLogging { get; }

        void WriteJsonParameterKeyValues(Utf8JsonWriter writer);
        ReadOnlySpan<byte> GetParameterKey(int index);
        object GetParameterValue(int index);
        T GetParameterValue<T>(int index);
        Type GetParameterType(int index);
    }

    // Cachable(Returnable) state holder, store struct state to heap for AsyncLogProcessor.
    public interface IZLoggerEntry2 : IZLoggerFormattable
    {
        LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter2 formatter); // TODO: 2 -> 1
        void Return();
    }

    // Define how write log entry(text, strctured-json, structured-msgpack, etc...)
    public interface IZLoggerFormatter2
    {
        void FormatLogEntry<TEntry>(IBufferWriter<byte> writer, TEntry entry)
            where TEntry : IZLoggerEntry2;
    }

    // Create LogEntry from struct state without T constraints 
    public static class LogEntryFactory<T>
    {
        public static CreateLogEntry<T>? Create; // Register from each state static constructor
    }

    public delegate IZLoggerEntry2 CreateLogEntry<TState>(in LogInfo info, in TState state);

    public sealed class ZLoggerEntry<TState> : IZLoggerEntry2, IObjectPoolNode<ZLoggerEntry<TState>>
        where TState : IZLoggerFormattable
    {
        static readonly ObjectPool<ZLoggerEntry<TState>> cache = new();

        ZLoggerEntry<TState>? next;
        ref ZLoggerEntry<TState>? IObjectPoolNode<ZLoggerEntry<TState>>.NextNode => ref next;

        LogInfo logInfo;
        TState state;

        ZLoggerEntry(in LogInfo logInfo, in TState state)
        {
            this.logInfo = logInfo;
            this.state = state;
        }

        public static ZLoggerEntry<TState> Create(in LogInfo logInfo, in TState state)
        {
            if (cache.TryPop(out var entry))
            {
                entry.logInfo = logInfo;
                entry.state = state;
            }
            else
            {
                entry = new ZLoggerEntry<TState>(logInfo, state);
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

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter2 formatter)
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
}
