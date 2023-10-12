using System;
using System.Buffers;
using System.Text.Json;

namespace ZLogger.Entries
{
    public struct StringFormatterLogState<TState> : IZLoggerFormattable
    {
        public int ParameterCount => 0;
        public bool IsSupportStructuredLogging => false;
        
        readonly TState state;
        readonly Func<TState, Exception?, string> formatter;
        readonly Exception? exception;

        public StringFormatterLogState(TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            this.state = state;
            this.exception = exception;
            this.formatter = formatter;
        }

        public override string ToString() => formatter(state, exception);

        public void ToString(IBufferWriter<byte> writer)
        {
            
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

        public object? GetParameterValue(int index) => throw new NotImplementedException();

        public T? GetParameterValue<T>(int index) => throw new NotImplementedException();

        public Type GetParameterType(int index) => throw new NotImplementedException();
    }
}