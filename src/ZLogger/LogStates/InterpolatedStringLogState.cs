using System.Buffers;
using System.Runtime.InteropServices;
using System.Text.Json;
using ZLogger.Formatters;
using ZLogger.Internal;

namespace ZLogger.LogStates
{
    internal sealed class InterpolatedStringLogState : IZLoggerFormattable, IReferenceCountable, IObjectPoolNode<InterpolatedStringLogState>
    {
        static readonly ObjectPool<InterpolatedStringLogState> cache = new();

        public ref InterpolatedStringLogState? NextNode => ref next;
        InterpolatedStringLogState? next;

        public int ParameterCount { get; private set; }
        public bool IsSupportUtf8ParameterKey => false;

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

            return state;
        }

        public IZLoggerEntry CreateEntry(LogInfo info)
        {
            return ZLoggerEntry<InterpolatedStringLogState>.Create(info, this);
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
                        throw new Exception("no here!");
                        JsonSerializer.Serialize(jsonWriter, p.BoxedValue, p.Type, jsonSerializerOptions);
                    }
                }
            }
        }

        public ReadOnlySpan<byte> GetParameterKey(int index)
        {
            throw new NotSupportedException();
        }

        public ReadOnlySpan<char> GetParameterKeyAsString(int index)
        {
            return parameters[index].Name.AsSpan();
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
    }

    [StructLayout(LayoutKind.Auto)]
    internal readonly struct VersionedLogState : IZLoggerEntryCreatable, IReferenceCountable
    {
        readonly InterpolatedStringLogState state;
        readonly int version;

        public int Version => version;

        public VersionedLogState(InterpolatedStringLogState state)
        {
            this.state = state;
            this.version = state.Version;
        }

        public IZLoggerEntry CreateEntry(LogInfo info)
        {
            return state.CreateEntry(info);
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
            // with validate
            if (state.Version != version)
            {
                throw new InvalidOperationException("ZLogger log state version is unmatched. The reason is that the external log provider is not generating strings immediately, ZLog does not support such providers.");
            }

            return state.ToString();
        }
    }
}
