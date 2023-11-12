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
        byte[] magicalBoxStorage;
        InterpolatedStringParameter[] parameters;

        int refCount;

        MessageSequence messageSequence;
        MagicalBox magicalBox;

        public static InterpolatedStringLogState Create(MessageSequence messageSequence, MagicalBox magicalBox, ReadOnlySpan<InterpolatedStringParameter> parameters)
        {
            if (cache.TryPop(out var state))
            {
                state.magicalBoxStorage = ArrayPool<byte>.Shared.Rent(magicalBox.Written);
                magicalBox.AsSpan().CopyTo(state.magicalBoxStorage);
                state.magicalBox = new MagicalBox(state.magicalBoxStorage, magicalBox.Written);

                state.parameters = ArrayPool<InterpolatedStringParameter>.Shared.Rent(parameters.Length);
                parameters.CopyTo(state.parameters);
                state.ParameterCount = parameters.Length;

                state.messageSequence = messageSequence;
            }
            else
            {
                state = new InterpolatedStringLogState(messageSequence, magicalBox, parameters);
            }

            state.Retain();
            return state;
        }

        InterpolatedStringLogState(MessageSequence messageSequence, MagicalBox magicalBox, ReadOnlySpan<InterpolatedStringParameter> parameters)
        {
            // need clone.
            this.magicalBoxStorage = ArrayPool<byte>.Shared.Rent(magicalBox.Written);
            magicalBox.AsSpan().CopyTo(magicalBoxStorage);
            this.magicalBox = new MagicalBox(magicalBoxStorage, magicalBox.Written);

            this.parameters = ArrayPool<InterpolatedStringParameter>.Shared.Rent(parameters.Length);
            parameters.CopyTo(this.parameters);
            ParameterCount = parameters.Length;

            this.messageSequence = messageSequence;
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
