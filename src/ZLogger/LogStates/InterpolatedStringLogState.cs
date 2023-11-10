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

        public int ParameterCount => parameters.Count;
        public bool IsSupportUtf8ParameterKey => false;

        // pooling values.
        ArraySegment<InterpolatedStringParameter> parameters;

        int refCount;

        MessageSequence messageSequence;
        MagicalBox magicalBox;

        public static InterpolatedStringLogState Create(MessageSequence messageSequence, MagicalBox magicalBox, ArraySegment<InterpolatedStringParameter> parameters)
        {
            if (cache.TryPop(out var state))
            {
                state = new InterpolatedStringLogState(messageSequence, magicalBox, parameters)
                {
                    parameters = parameters,
                    messageSequence = messageSequence,
                    magicalBox = magicalBox
                };
                state.Retain();
            }
            else
            {
                state = new InterpolatedStringLogState(messageSequence, magicalBox, parameters);
            }
            return state;
        }

        InterpolatedStringLogState(MessageSequence messageSequence, MagicalBox magicalBox, ArraySegment<InterpolatedStringParameter> parameters)
        {
            this.parameters = parameters;
            this.messageSequence = messageSequence;
            this.magicalBox = magicalBox;
            Retain();
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
            ArrayPool<InterpolatedStringParameter>.Shared.Return(parameters.Array!);
            parameters = null!;
            magicalBox.Dispose();
            messageSequence.Dispose();
            cache.TryPush(this);
        }

        public override string ToString()
        {
            return messageSequence.ToString(magicalBox, parameters.AsSpan());
        }

        public void ToString(IBufferWriter<byte> writer)
        {
            messageSequence.ToString(writer, magicalBox, parameters.AsSpan());
        }

        public void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions, ZLoggerOptions options)
        {
            foreach (var p in parameters.AsSpan())
            {
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
            var p = parameters[index];
            var value = magicalBox.Read(p.Type, p.BoxOffset);
            if (value != null) return value;

            return p.BoxedValue;
        }

        public T? GetParameterValue<T>(int index)
        {
            var p = parameters[index];
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
