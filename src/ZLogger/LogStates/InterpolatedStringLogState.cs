using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ZLogger.Internal;

namespace ZLogger.LogStates
{
    public readonly struct InterpolatedStringLogState : IZLoggerFormattable
    {
        static InterpolatedStringLogState()
        {
            LogEntryFactory<InterpolatedStringLogState>.Create = CreateEntry;
            LogEntryFactory<InterpolatedStringLogState>.CloneState = CloneState;
        }

        static IZLoggerEntry CreateEntry(in LogInfo logInfo, in InterpolatedStringLogState logState)
        {
            return ZLoggerEntry<InterpolatedStringLogState>.Create(logInfo, logState);
        }

        static InterpolatedStringLogState CloneState(in InterpolatedStringLogState state)
        {
            var newParameters = ArrayPool<KeyValuePair<string, object?>>.Shared.Rent(state.ParameterCount);
            state.parameters.AsSpan(0, state.ParameterCount).CopyTo(newParameters.AsSpan());

            var newBuffer = ArrayBufferWriterPool.Rent();
            state.buffer.WrittenSpan.CopyTo(newBuffer.GetSpan(state.buffer.WrittenCount));
            newBuffer.Advance(state.buffer.WrittenCount);

            return new InterpolatedStringLogState(newParameters, state.ParameterCount, newBuffer);
        }

        public int ParameterCount { get; }

        public bool IsSupportUtf8ParameterKey => false;

        readonly KeyValuePair<string, object?>[] parameters; // pooled
        readonly ArrayBufferWriter<byte> buffer; // pooled

        public InterpolatedStringLogState(KeyValuePair<string, object?>[] parameters, int parameterCount, ArrayBufferWriter<byte> buffer)
        {
            this.parameters = parameters;
            this.buffer = buffer;
            ParameterCount = parameterCount;
        }

        public void Dispose()
        {
            ArrayPool<KeyValuePair<string, object?>>.Shared.Return(parameters);
            ArrayBufferWriterPool.Return(buffer);
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(buffer.WrittenSpan);
        }

        public void ToString(IBufferWriter<byte> writer)
        {
            var written = buffer.WrittenSpan;
            var dest = writer.GetSpan(written.Length);
            written.CopyTo(dest);
            writer.Advance(written.Length);
        }

        public void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions)
        {
            for (var i = 0; i < ParameterCount; i++)
            {
                var (key, value) = parameters[i];
                jsonWriter.WritePropertyName(key);
                if (value == null)
                {
                    jsonWriter.WriteNullValue();
                }
                else
                {
                    var valueType = GetParameterType(i);
                    JsonSerializer.Serialize(jsonWriter, value, valueType, jsonSerializerOptions); // TODO: more optimize ?
                }
            }
        }

        public ReadOnlySpan<byte> GetParameterKey(int index)
        {
            throw new NotSupportedException();
        }

        public string GetParameterKeyAsString(int index)
        {
            return parameters[index].Key;
        }

        public object? GetParameterValue(int index)
        {
            return parameters[index].Value;
        }

        public T? GetParameterValue<T>(int index)
        {
            return (T?)parameters[index].Value;
        }

        public Type GetParameterType(int index)
        {
            return parameters[index].Value?.GetType() ?? typeof(string);
        }
    }
}
