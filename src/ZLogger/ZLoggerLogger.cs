using Microsoft.Extensions.Logging;
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
            var info = new LogInfo(category, new Timestamp(timeProvider), logLevel, eventId, exception);

            var scopeState = scopeProvider != null
                ? LogScopeState.Create(scopeProvider)
                : null;

            var entry = state is IZLoggerEntryCreatable
                ? ((IZLoggerEntryCreatable)state).CreateEntry(info) // constrained call avoiding boxing for value types
                : new StringFormatterLogState<TState>(state, exception, formatter).CreateEntry(info); // standard `log`

            entry.ScopeState = scopeState;

            if (state is IReferenceCountable)
            {
                ((IReferenceCountable)state).Retain();
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
