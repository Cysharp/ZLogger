using System.Buffers;
using System.Text.Json;
using ZLogger.Formatters;
using ZLogger.Internal;

namespace ZLogger.LogStates
{
    internal class InterpolatedStringLogState : IReferenceCountZLoggerFormattable, IObjectPoolNode<InterpolatedStringLogState>
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

        public static InterpolatedStringLogState Create(int formattedCount)
        {
            if (!cache.TryPop(out var state))
            {
                state = new InterpolatedStringLogState();
            }

            state.magicalBoxStorage = ArrayPool<byte>.Shared.Rent(2048);
            state.magicalBox = new MagicalBox(state.magicalBoxStorage);
            state.parameters = ArrayPool<InterpolatedStringParameter>.Shared.Rent(formattedCount);
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
                Dispose();
            }
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(magicalBoxStorage);
            ArrayPool<InterpolatedStringParameter>.Shared.Return(parameters);

            magicalBoxStorage = null!;
            parameters = null!;

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

        public void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions, ZLoggerOptions options)
        {
            for (var i = 0; i < ParameterCount; i++)
            {
                ref var p = ref parameters[i];
                SystemTextJsonZLoggerFormatter.WriteMutatedJsonKeyName(p.ParseKeyName(), jsonWriter, options.KeyNameMutator);

                if (magicalBox.TryReadTo(p.Type, p.BoxOffset, jsonWriter))
                {
                    continue;
                }

                var value = magicalBox.Read(p.Type, p.BoxOffset);
                if (value != null)
                {
                    JsonSerializer.Serialize(jsonWriter, value, p.Type, jsonSerializerOptions);
                }
                else
                {
                    JsonSerializer.Serialize(jsonWriter, p.BoxedValue, p.Type, jsonSerializerOptions);
                }
            }
        }

        public ReadOnlySpan<byte> GetParameterKey(int index)
        {
            throw new NotSupportedException();
        }

        public ReadOnlySpan<char> GetParameterKeyAsString(int index)
        {
            return parameters[index].ParseKeyName();
        }

        public object? GetParameterValue(int index)
        {
            ref var p = ref parameters[index];
            var value = magicalBox.Read(p.Type, p.BoxOffset);
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
}
