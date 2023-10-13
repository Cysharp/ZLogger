using Microsoft.Extensions.Logging;
using System;
using ZLogger.Entries;

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

        public void ZLog<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            where TState : IZLoggerFormattable
        {
            var info = new LogInfo(categoryName, DateTimeOffset.UtcNow, logLevel, eventId, exception);
            var entry = ZLoggerEntry<TState>.Create(info, state);
            logProcessor.Post(entry);
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
