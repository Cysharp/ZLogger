using System.Buffers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;
using ZLogger.Formatters;
using ZLogger.Internal;

namespace ZLogger.LogStates;

public sealed class InterpolatedStringLogState :
    IZLoggerFormattable,
    IReferenceCountable,
    IZLoggerAdditionalInfo,
    IObjectPoolNode<InterpolatedStringLogState>,
    IEnumerable<KeyValuePair<string, object?>>
{
    static readonly ObjectPool<InterpolatedStringLogState> cache = new();

    public ref InterpolatedStringLogState? NextNode => ref next;
    InterpolatedStringLogState? next;

    public bool IsSupportUtf8ParameterKey => false;
    public int ParameterCount { get; private set; }

    internal (object? context, string? MemberName, string? FilePath, int LineNumber) additionalInfo; // set from ZLog method

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // pooling values.
    byte[] magicalBoxStorage = default!;
    internal InterpolatedStringParameter[] parameters = default!;

    int refCount;
    internal MessageSequence messageSequence = default!;
    internal MagicalBox magicalBox;

    // pool safety token
    short version;

    public short Version => version;

    InterpolatedStringLogState()
    {
        this.magicalBoxStorage = new byte[256];
        this.parameters = new InterpolatedStringParameter[16];
    }

    public static InterpolatedStringLogState Create(int formattedCount)
    {
        if (!cache.TryPop(out var state))
        {
            state = new InterpolatedStringLogState();
        }

        state.magicalBox = new MagicalBox(state.magicalBoxStorage);
        if (state.parameters.Length < formattedCount)
        {
            state.parameters = new InterpolatedStringParameter[formattedCount];
        }
        state.ParameterCount = formattedCount;
        state.refCount = 1;
        state.additionalInfo = default;
        return state;
    }

    public IZLoggerEntry CreateEntry(in LogInfo info)
    {
        return ZLoggerEntry<InterpolatedStringLogState>.Create(in info, this);
    }

    public void Retain()
    {
        Interlocked.Increment(ref refCount);
    }

    public void Release()
    {
        if (Interlocked.Decrement(ref refCount) == 0)
        {
            DisposeCore();
        }
    }

    void DisposeCore()
    {
        parameters.AsSpan(0, ParameterCount).Clear();
        additionalInfo = default;
        unchecked
        {
            version += 1;
        }

        cache.TryPush(this);
    }

    public override string ToString()
    {
        return messageSequence.ToString(magicalBox, parameters);
    }

    public void ToString(IBufferWriter<byte> writer)
    {
        messageSequence.ToString(writer, magicalBox, parameters);
    }

    public string GetOriginalFormat()
    {
        return messageSequence.GetOriginalFormat(parameters);
    }

    public void WriteOriginalFormat(IBufferWriter<byte> writer)
    {
        messageSequence.WriteOriginalFormat(writer, parameters);
    }

    [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode")]
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode")]
    public void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions, IKeyNameMutator? keyNameMutator = null)
    {
        for (var i = 0; i < ParameterCount; i++)
        {
            ref var p = ref parameters[i];
            SystemTextJsonZLoggerFormatter.WriteMutatedJsonKeyName(p.Name.AsSpan(), jsonWriter, keyNameMutator);

            if (magicalBox.TryReadTo(p.Type, p.BoxOffset, jsonWriter, jsonSerializerOptions))
            {
                continue;
            }
            else
            {
                // use BoxedValue
                if (p.Type == typeof(string))
                {
                    jsonWriter.WriteStringValue((string?)p.BoxedValue);
                }
                else
                {
                    JsonSerializer.Serialize(jsonWriter, p.BoxedValue, p.Type, jsonSerializerOptions);
                }
            }
        }
    }

    public ReadOnlySpan<byte> GetParameterKey(int index)
    {
        throw new NotSupportedException();
    }

    public string GetParameterKeyAsString(int index)
    {
        return parameters[index].Name;
    }

    public object? GetParameterValue(int index)
    {
        ref var p = ref parameters[index];
        var value = magicalBox.ReadBoxed(p.Type, p.BoxOffset);
        if (value != null) return value;

        return p.BoxedValue;
    }

    public T? GetParameterValue<T>(int index)
    {
        ref var p = ref parameters[index];
        if (magicalBox.TryRead<T>(p.BoxOffset, out var value))
        {
            return value;
        }
        return (T?)p.BoxedValue;
    }

    public Type GetParameterType(int index)
    {
        return parameters[index].Type;
    }

    public (object? Context, string? MemberName, string? FilePath, int LineNumber) GetAdditionalInfo()
    {
        return additionalInfo;
    }

    struct Enumerator(InterpolatedStringLogState state) : IEnumerator<KeyValuePair<string, object?>>
    {
        InterpolatedStringLogState state = state;
        int currentIndex = -1;

        object IEnumerator.Current => Current;
        public bool MoveNext() => ++currentIndex < state.ParameterCount;
        public void Reset() => currentIndex = -1;

        public KeyValuePair<string, object?> Current => new(
            state.GetParameterKeyAsString(currentIndex),
            state.GetParameterValue(currentIndex));

        public void Dispose() { }
    }
}

[StructLayout(LayoutKind.Auto)]
public readonly struct VersionedLogState : IZLoggerEntryCreatable, IReferenceCountable, IZLoggerAdditionalInfo, IEnumerable<KeyValuePair<string, object?>>
{
    readonly InterpolatedStringLogState state;
    readonly int version;

    public int Version => version;

    internal VersionedLogState(InterpolatedStringLogState state)
    {
        this.state = state;
        this.version = state.Version;
    }

    public IZLoggerEntry CreateEntry(in LogInfo info)
    {
        return state.CreateEntry(info);
    }

    public (object? Context, string? MemberName, string? FilePath, int LineNumber) GetAdditionalInfo()
    {
        return state.GetAdditionalInfo();
    }

    public void Release()
    {
        state.Release();
    }

    public void Retain()
    {
        state.Retain();
    }

    public override string ToString()
    {
        ThrowIfVersionUnmatched();
        return state.ToString();
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        ThrowIfVersionUnmatched();
        return state.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void ThrowIfVersionUnmatched()
    {
        if (state.Version != version)
        {
            throw new InvalidOperationException(
                "ZLogger log state version is unmatched. The reason is that the external log provider is not generating strings immediately, ZLog does not support such providers.");
        }
    }
}
