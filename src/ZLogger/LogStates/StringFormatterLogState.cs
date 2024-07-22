using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using ZLogger.Internal;

namespace ZLogger.LogStates
{
    internal struct StringFormatterLogState<TState> : IZLoggerFormattable
    {
        public int ParameterCount { get; }
        public bool IsSupportUtf8ParameterKey => false;

        readonly TState originalState;
        readonly Exception? exception;
        readonly Func<TState, Exception?, string> formatter;
        readonly IReadOnlyList<KeyValuePair<string, object?>>? originalStateParameters;
        readonly string? formattedLog;

        public StringFormatterLogState(TState originalState, Exception? exception, Func<TState, Exception?, string> formatter, bool formatImmediately)
        {
            this.originalState = originalState;
            this.exception = exception;
            this.formatter = formatter;

            if (originalState is IReadOnlyList<KeyValuePair<string, object?>> x)
            {
                originalStateParameters = x;
                if (x.Count != 0 && x[^1].Key == "{OriginalFormat}")
                {
                    ParameterCount = x.Count - 1;
                }
            }
            else
            {
                originalStateParameters = null;
                ParameterCount = 0;
            }

            if (formatImmediately)
            {
                formattedLog = formatter(originalState, exception);
            }
        }

        public IZLoggerEntry CreateEntry(in LogInfo info)
        {
            return ZLoggerEntry<StringFormatterLogState<TState>>.Create(info, this);
        }

        public override string ToString() => formattedLog ?? formatter(originalState, exception);

        public void ToString(IBufferWriter<byte> writer)
        {
            var str = ToString();
            var buffer = writer.GetSpan(Encoding.UTF8.GetMaxByteCount(str.Length));
            var bytesWritten = Encoding.UTF8.GetBytes(str.AsSpan(), buffer);
            writer.Advance(bytesWritten);
        }

        public string GetOriginalFormat()
        {
            var bufferWriter = ArrayBufferWriterPool.GetThreadStaticInstance();
            WriteOriginalFormat(bufferWriter);
            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }

        public void WriteOriginalFormat(IBufferWriter<byte> writer)
        {
            if (originalStateParameters == null) return;

            var len = originalStateParameters.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                var item = originalStateParameters[i]; // maybe last is OriginalFormat
                if (item.Key == "{OriginalFormat}" && item.Value is string str)
                {
                    var buffer = writer.GetSpan(Encoding.UTF8.GetMaxByteCount(str.Length));
                    var bytesWritten = Encoding.UTF8.GetBytes(str.AsSpan(), buffer);
                    writer.Advance(bytesWritten);
                }
            }
        }

        [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode")]
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode")]
        public void WriteJsonParameterKeyValues(Utf8JsonWriter jsonWriter, JsonSerializerOptions jsonSerializerOptions, IKeyNameMutator? keyNameMutator = null)
        {
            if (originalStateParameters == null) return;

            var length = ParameterCount;
            for (var i = 0; i < length; i++)
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
                    JsonSerializer.Serialize(jsonWriter, x.Value, valueType, jsonSerializerOptions);
                }
            }
        }

        public ReadOnlySpan<byte> GetParameterKey(int index)
        {
            throw new NotSupportedException();
        }

        public string GetParameterKeyAsString(int index)
        {
            if (originalStateParameters != null)
            {
                return originalStateParameters[index].Key;
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

        public T? GetParameterValue<T>(int index)
        {
            if (originalStateParameters != null)
            {
                return (T?)originalStateParameters[index].Value;
            }
            throw new IndexOutOfRangeException(nameof(index));
        }

        public Type GetParameterType(int index)
        {
            if (originalStateParameters != null)
            {
                return originalStateParameters[index].Value?.GetType() ?? typeof(string);
            }
            throw new IndexOutOfRangeException(nameof(index));
        }

        public object? GetContext() => null;
    }
}
