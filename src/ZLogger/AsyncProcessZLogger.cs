using Microsoft.Extensions.Logging;
using System;
using ZLogger.LogStates;

namespace ZLogger
{
    public sealed class AsyncProcessZLogger : ILogger
    {
        public IExternalScopeProvider? ScopeProvider { get; set; } 
        
        readonly string categoryName;
        readonly IAsyncLogProcessor logProcessor;

        public AsyncProcessZLogger(string categoryName, IAsyncLogProcessor logProcessor)
        {
            this.categoryName = categoryName;
            this.logProcessor = logProcessor;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var info = new LogInfo(categoryName, DateTimeOffset.UtcNow, logLevel, eventId, exception);
                           
            var scopeState = ScopeProvider != null
                ? LogScopeState.Create(ScopeProvider)
                : null;
            
            var factory = LogEntryFactory<TState>.Create;
            if (factory != null)
            {
                // After the `Log()` call, the state's resources are released, so a copy is needed to delay the write.
                if (LogEntryFactory<TState>.CloneState != null)
                {
                    state = LogEntryFactory<TState>.CloneState(state);
                }
                var entry = factory.Invoke(info, state);
                entry.ScopeState = scopeState;
                logProcessor.Post(entry);
            }
            // Standard `Log` method used
            else
            {
                var stringFormatterState = new StringFormatterLogState<TState>(state, exception, formatter);
                var entry = ZLoggerEntry<StringFormatterLogState<TState>>.Create(info, stringFormatterState);
                entry.ScopeState = scopeState;
                logProcessor.Post(entry);
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return ScopeProvider?.Push(state) ?? NullDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        class NullDisposable : IDisposable
        {
            public static readonly IDisposable Instance = new NullDisposable();

            public void Dispose()
            {
            }
        }
    }
}
