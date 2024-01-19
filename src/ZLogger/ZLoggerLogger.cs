using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using ZLogger.LogStates;

namespace ZLogger
{
    public sealed class ZLoggerLogger : ILogger
    {
        readonly LogCategory category;
        readonly IAsyncLogProcessor logProcessor;
        readonly TimeProvider? timeProvider;
        readonly IExternalScopeProvider? scopeProvider;

        public ZLoggerLogger(string categoryName, IAsyncLogProcessor logProcessor, ZLoggerOptions options, IExternalScopeProvider? scopeProvider)
        {
            this.category = new LogCategory(categoryName);
            this.logProcessor = logProcessor;
            this.timeProvider = options.TimeProvider;
            this.scopeProvider = scopeProvider;
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var scopeState = scopeProvider != null
                ? LogScopeState.Create(scopeProvider)
                : null;

            IZLoggerEntry entry;
            if (state is VersionedLogState)
            {
                var s = Unsafe.As<TState, VersionedLogState>(ref state);
                var info = new LogInfo(category, new Timestamp(timeProvider), logLevel, eventId, exception, scopeState, s.CallerInfo);
                entry = s.CreateEntry(info);
                s.Retain();
            }
            else if (state is IZLoggerEntryCreatable)
            {
                var info = new LogInfo(category, new Timestamp(timeProvider), logLevel, eventId, exception, scopeState, null);
                entry = ((IZLoggerEntryCreatable)state).CreateEntry(info);
                if (state is IReferenceCountable)
                {
                    ((IReferenceCountable)state).Retain();
                }
            }
            else
            {
                var info = new LogInfo(category, new Timestamp(timeProvider), logLevel, eventId, exception, scopeState, null);
                // called from standard `logger.Log`
                entry = new StringFormatterLogState<TState>(state, exception, formatter).CreateEntry(info);
            }

            logProcessor.Post(entry);
        }

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
        {
            return scopeProvider?.Push(state) ?? NullDisposable.Instance;
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
