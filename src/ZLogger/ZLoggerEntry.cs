using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
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

    public sealed class ZLoggerEntry<TState> : IZLoggerEntry, IObjectPoolNode<ZLoggerEntry<TState>>
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
        public bool IsSupportUtf8ParameterKey => state.IsSupportUtf8ParameterKey;
        
        public ReadOnlySpan<byte> GetParameterKey(int index) => state.GetParameterKey(index);
        public string GetParameterKeyAsString(int index) => state.GetParameterKeyAsString(index);

        public Type GetParameterType(int index) => state.GetParameterType(index);

        public object? GetParameterValue(int index) => state.GetParameterValue(index);

        public T? GetParameterValue<T>(int index) => state.GetParameterValue<T>(index);

        public void ToString(IBufferWriter<byte> writer) => state.ToString(writer);

        public void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions) 
            => state.WriteJsonParameterKeyValues(jsonWriter, jsonSerializerOptions);

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
            var buffer = ArrayBufferWriterPool.GetThreadStaticInstance();
            formatter.FormatLogEntry(buffer, entry);
            return Encoding.UTF8.GetString(buffer.WrittenSpan);
        }
    }
}
