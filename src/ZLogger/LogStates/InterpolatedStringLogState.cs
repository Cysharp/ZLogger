using System.Buffers;
using System.Text.Json;
using ZLogger.Internal;

namespace ZLogger.LogStates
{
    internal struct InterpolatedStringLogState : IZLoggerFormattable, IDisposable
    {
        public int ParameterCount { get; }

        public bool IsSupportUtf8ParameterKey => false;

        // pooling values.
        byte[] magicalBoxStorage;
        InterpolatedStringParameter[] parameters;

        readonly MessageSequence messageSequence;
        readonly MagicalBox magicalBox;

        public InterpolatedStringLogState(MessageSequence messageSequence, MagicalBox magicalBox, ReadOnlySpan<InterpolatedStringParameter> parameters)
        {
            // need clone.
            this.magicalBoxStorage = ArrayPool<byte>.Shared.Rent(magicalBox.Written);
            magicalBox.AsSpan().CopyTo(magicalBoxStorage);

            this.parameters = ArrayPool<InterpolatedStringParameter>.Shared.Rent(parameters.Length);
            parameters.CopyTo(this.parameters);
            ParameterCount = parameters.Length;

            this.messageSequence = messageSequence;
            this.magicalBox = new MagicalBox(magicalBoxStorage, magicalBox.Written);
        }

        public IZLoggerEntry CreateEntry(LogInfo info)
        {
            // state needs clone.
            var newState = new InterpolatedStringLogState(messageSequence, this.magicalBox, this.parameters.AsSpan(0, ParameterCount));

            // Create Entry with cloned state(state will dispose when entry was disposed)
            return ZLoggerEntry<InterpolatedStringLogState>.Create(info, newState);
        }

        public void Dispose()
        {
            if (magicalBoxStorage != null)
            {
                ArrayPool<byte>.Shared.Return(magicalBoxStorage);
                ArrayPool<InterpolatedStringParameter>.Shared.Return(parameters);

                magicalBoxStorage = null!;
                parameters = null!;
            }
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
                if (magicalBox.TryReadTo(p.Type, p.BoxOffset, p.Name, jsonWriter))
                {
                    continue;
                }

                p.WriteJsonKeyName(jsonWriter, options.KeyNameMutator);

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
