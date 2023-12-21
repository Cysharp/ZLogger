using System;
using System.Buffers;
using System.Text.Json;
using ZLogger.LogStates;

namespace ZLogger.Unity;

struct UnityDebugLogState : IZLoggerFormattable, IReferenceCountable
{
    public int ParameterCount => innerState.ParameterCount;
    public bool IsSupportUtf8ParameterKey => false;
    public int Version => version;
    
    readonly InterpolatedStringLogState innerState;
    readonly UnityEngine.Object? context;
    readonly int version;

    public UnityDebugLogState(InterpolatedStringLogState state, UnityEngine.Object? context)
    {
        this.innerState = state;
        this.context = context;
        version = state.Version;
    }

    public IZLoggerEntry CreateEntry(LogInfo info)
    {
        return ZLoggerEntry<UnityDebugLogState>.Create(info, this);
    }

    public void Release()
    {
        innerState.Release();
    }

    public void Retain()
    {
        innerState.Retain();
    }

    public override string ToString()
    {
        // with validate
        if (innerState.Version != version)
        {
            throw new InvalidOperationException("ZLogger log state version is unmatched. The reason is that the external log provider is not generating strings immediately, ZLog does not support such providers.");
        }

        return innerState.ToString();
    }
    
    public void ToString(IBufferWriter<byte> writer) => innerState.ToString(writer);

    public void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions, IKeyNameMutator? keyNameMutator = null)
    {
        innerState.WriteJsonParameterKeyValues(jsonWriter, jsonSerializerOptions, keyNameMutator);
    }

    public ReadOnlySpan<byte> GetParameterKey(int index) => innerState.GetParameterKey(index);
    public ReadOnlySpan<char> GetParameterKeyAsString(int index) => innerState.GetParameterKeyAsString(index);

    public T? GetParameterValue<T>(int index) => innerState.GetParameterValue<T>(index);
    public Type GetParameterType(int index) => innerState.GetParameterType(index);

    public object? GetParameterValue(int index)
    {
        // Special treatment for the provider of UnityEngine.Debug.
        if (index == -1)
        {
            return context;
        }
        return innerState.GetParameterValue(index);
    }
}
