using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
            // logProcessor.Post(entry);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // TODO: v2.
            var factory2 = LogEntryFactory<TState>.Create;
            if (factory2 != null)
            {
                var info = new LogInfo(categoryName, DateTimeOffset.UtcNow, logLevel, eventId, exception);
                var entry = factory2.Invoke(info, state);
                // logProcessor.Post(entry);
            }

            // Legacy...

            {
                var info = new LogInfo(categoryName, DateTimeOffset.UtcNow, logLevel, eventId, exception);
                
                var scopeState = ScopeProvider != null
                    ? LogScopeState.Create(ScopeProvider)
                    : null;
                
                var entry = CreateLogEntry<TState>.factory?.Invoke(state, info, scopeState) ?? 
                            StringFormatterEntry<TState>.Create(info, state, scopeState, exception, formatter);

                logProcessor.Post(entry);
            }
        }


        static class StateTypeDetector<TState>
        {
            public static readonly bool IsInternalFormattedLogValues;

            static StateTypeDetector()
            {
                IsInternalFormattedLogValues = typeof(TState).FullName == "Microsoft.Extensions.Logging.FormattedLogValues";
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


        // call CreateLogEntry without cast(boxing)
        static class CreateLogEntry<T>
        // where T:IZLoggerState
        {
            public static readonly Func<T, LogInfo, LogScopeState?, IZLoggerEntry>? factory;

            static CreateLogEntry()
            {
                if (typeof(IZLoggerState).IsAssignableFrom(typeof(T)))
                {
                    var factoryField = typeof(T).GetField("Factory", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    if (factoryField != null)
                    {
                        factory = factoryField.GetValue(null) as Func<T, LogInfo, LogScopeState?, IZLoggerEntry>;
                    }
                }
                else
                {
                    factory = null;
                }
            }
        }
    }
}