using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ZLogger.Entries
{
    public struct StringFormatterLogState<TState> : IZLoggerFormattable
    {
        public int ParameterCount => originalStateParameters?.Count ?? 0;

        readonly TState originalState;
        readonly Exception? exception;
        readonly Func<TState, Exception?, string> formatter;
        readonly IReadOnlyList<KeyValuePair<string, object?>>? originalStateParameters;

        public StringFormatterLogState(TState originalState, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            this.originalState = originalState;
            this.exception = exception;
            this.formatter = formatter;
            
            // In most case, TState is `Microsoft.Extensions.Logging.FormattedLogValues`.
            // TODO: can avoid boxing ?
            if (originalState is IReadOnlyList<KeyValuePair<string, object?>> x)
            {
                originalStateParameters = x;
            }
            else
            {
                originalStateParameters = null;
            }
        }

        public override string ToString() => formatter(originalState, exception);

        public void ToString(IBufferWriter<byte> writer)
        {
            var str = ToString();
            var buffer = writer.GetSpan(Encoding.UTF8.GetMaxByteCount(str.Length));
            var bytesWritten = Encoding.UTF8.GetBytes(str.AsSpan(), buffer);
            writer.Advance(bytesWritten);
        }

        public void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions)
        {
            if (originalStateParameters == null) return;

            for (var i = 0; i < originalStateParameters.Count; i++)
            {
                var x = originalStateParameters[i];
                jsonWriter.WritePropertyName(x.Key);
                if (x.Value == null)
                {
                    jsonWriter.WriteNullValue();
                }
                else
                {
                    var valueType = GetParameterType(i);
                    JsonSerializer.Serialize(jsonWriter, x.Value, valueType, jsonSerializerOptions); // TODO: more optimize ?
                }
            }
        }

        public ReadOnlySpan<byte> GetParameterKey(int index)
        {
            if (originalStateParameters != null)
            {
                throw new NotImplementedException();
                // return originalStateParameters[index].Key;
            }
            throw new IndexOutOfRangeException(nameof(index));
        }

        public object? GetParameterValue(int index)
        {
            if (originalStateParameters != null)
            {
                return originalStateParameters[index].Value;
            }
            throw new IndexOutOfRangeException(nameof(index));
        }

        public T? GetParameterValue<T>(int index) => throw new NotImplementedException();

        public Type GetParameterType(int index) => throw new NotImplementedException();
    }
}
