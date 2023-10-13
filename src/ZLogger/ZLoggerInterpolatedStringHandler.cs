using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Utf8StringInterpolation;
using ZLogger.Internal;
using ZLogger.LogStates;

namespace ZLogger
{
    [InterpolatedStringHandler]
    public ref struct ZLoggerInterpolatedStringHandler
    {
        int i;
        readonly KeyValuePair<string, object?>[] parameters; // TODO: avoid boxing!
        readonly ArrayBufferWriter<byte> buffer;
        readonly int parametersCount;
        Utf8StringWriter<ArrayBufferWriter<byte>> utf8StringWriter;

        public ZLoggerInterpolatedStringHandler(int literalLength, int formattedCount)
        {
            i = 0;
            parametersCount = formattedCount;
            parameters = ArrayPool<KeyValuePair<string, object?>>.Shared.Rent(formattedCount); 
            buffer = ArrayBufferWriterPool.Rent();
            utf8StringWriter = new Utf8StringWriter<ArrayBufferWriter<byte>>(literalLength, formattedCount, buffer);
        }

        public void AppendLiteral(string s)
        {
            utf8StringWriter.AppendLiteral(s);
        }

        public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        {
            utf8StringWriter.AppendFormatted(value, alignment, format);
            parameters[i++] = new KeyValuePair<string, object?>(argumentName ?? $"Arg{i}", value);
        }

        public InterpolatedStringLogState GetState()
        {
            utf8StringWriter.Flush();
            return new InterpolatedStringLogState(parameters, parametersCount, buffer);
        }
    }    
}
