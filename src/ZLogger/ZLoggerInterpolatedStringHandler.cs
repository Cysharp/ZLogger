using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ZLogger
{
    [InterpolatedStringHandler]
    public struct ZLoggerInterpolatedStringHandler : IZLoggerFormattable
    {
        static ZLoggerInterpolatedStringHandler()
        {
            LogEntryFactory<ZLoggerInterpolatedStringHandler>.Create = CreateEntry;
        }

        static IZLoggerEntry2 CreateEntry(in LogInfo logInfo, in ZLoggerInterpolatedStringHandler state)
        {
            return ZLoggerEntry<ZLoggerInterpolatedStringHandler>.Create(logInfo, state);
        }
        
        public int ParameterCount => arguments.Length;
        public bool IsSupportStructuredLogging => true;
        
        int i;
        readonly (string?, object?)[] arguments; // TODO: avoid boxing!

        public ReadOnlySpan<(string?, object?)> Parameters => arguments;

        public ZLoggerInterpolatedStringHandler(int literalLength, int formattedCount)
        {
            i = 0;
            arguments = new (string?, object?)[formattedCount];
        }

        public void AppendLiteral(string s)
        {
            // TODO:...
        }

        public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        {
            arguments[i++] = (argumentName, (object?)value);
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public void ToString(IBufferWriter<byte> writer)
        {
            throw new NotImplementedException();
        }

        public void WriteJsonMessage(Utf8JsonWriter writer)
        {
            throw new NotImplementedException();
        }

        public void WriteJsonParameterKeyValues(Utf8JsonWriter writer)
        {
            throw new NotImplementedException();
        }

        public ReadOnlySpan<byte> GetParameterKey(int index) => throw new NotImplementedException();

        public object GetParameterValue(int index) => throw new NotImplementedException();

        public T GetParameterValue<T>(int index) => throw new NotImplementedException();

        public Type GetParameterType(int index) => throw new NotImplementedException();
    }    
}
