using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ZLogger.Internal;
using ZLogger.LogStates;

namespace ZLogger
{
    public interface INonReturnableZLoggerEntry : IZLoggerFormattable
    {
        LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter);
    }

    // Cachable(Returnable) state holder, store struct state to heap for AsyncLogProcessor.
    public interface IZLoggerEntry : INonReturnableZLoggerEntry
    {
        void Return();
    }

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

        public IZLoggerEntry CreateEntry(in LogInfo info) => state.CreateEntry(info);
        public int ParameterCount => state.ParameterCount;
        public bool IsSupportUtf8ParameterKey => state.IsSupportUtf8ParameterKey;

        public ReadOnlySpan<byte> GetParameterKey(int index) => state.GetParameterKey(index);
        public string GetParameterKeyAsString(int index) => state.GetParameterKeyAsString(index);

        public Type GetParameterType(int index) => state.GetParameterType(index);

        public object? GetParameterValue(int index) => state.GetParameterValue(index);

        public T? GetParameterValue<T>(int index) => state.GetParameterValue<T>(index);

        public void ToString(IBufferWriter<byte> writer) => state.ToString(writer);

        public void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions, IKeyNameMutator? keyNameMutator = null)
            => state.WriteJsonParameterKeyValues(jsonWriter, jsonSerializerOptions, keyNameMutator);

        public LogInfo LogInfo => logInfo;

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            formatter.FormatLogEntry(writer, this);
        }

        public string GetOriginalFormat() => state.GetOriginalFormat();

        public void WriteOriginalFormat(IBufferWriter<byte> writer) => state.WriteOriginalFormat(writer);

        public void Return()
        {
            if (state is VersionedLogState)
            {
                Unsafe.As<TState, VersionedLogState>(ref state).Release();
            }
            else if (state is IReferenceCountable)
            {
                ((IReferenceCountable)state).Release();
            }
            state = default!;

            logInfo.ScopeState?.Return();
            logInfo = default!;

            cache.TryPush(this);
        }
    }
}
