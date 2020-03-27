using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ZLog.Entries;

namespace ZLog
{
    internal class ZLogLogger : ILogger
    {
        readonly string categoryName;
        readonly IAsyncLogProcessor logProcessor;

        public ZLogLogger(string categoryName, IAsyncLogProcessor logProcessor)
        {
            this.categoryName = categoryName;
            this.logProcessor = logProcessor;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var factory = CreateLogEntry<TState>.factory;
            if (factory != null)
            {
                var info = new LogInfo(categoryName, DateTimeOffset.UtcNow, logLevel, eventId, exception);
                var entry = factory.Invoke(state, info);
                logProcessor.Post(entry);
            }
            else
            {
                var info = new LogInfo(categoryName, DateTimeOffset.UtcNow, logLevel, eventId, exception);
                logProcessor.Post(StringFormatterEntry<TState>.Create(info, state, exception, formatter));
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // currently scope is not supported...
            return NUllDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        class NUllDisposable : IDisposable
        {
            public static IDisposable Instance = new NUllDisposable();

            NUllDisposable()
            {

            }

            public void Dispose()
            {
            }
        }


        // call CreateLogEntry without cast(boxing)
        static class CreateLogEntry<T>
        // where T:IZLogState
        {
            public static readonly Func<T, LogInfo, IZLogEntry>? factory;

            static CreateLogEntry()
            {
                if (typeof(IZLogState).IsAssignableFrom(typeof(T)))
                {
                    var create = typeof(T).GetMethod(nameof(IZLogState.CreateLogEntry));

                    var state = Expression.Parameter(typeof(T), "state");
                    var info = Expression.Parameter(typeof(LogInfo), "info");

                    var callCreate = Expression.Call(state, create, info);

                    var lambda = Expression.Lambda<Func<T, LogInfo, IZLogEntry>>(callCreate, state, info);

                    factory = lambda.Compile();
                }
                else
                {
                    factory = null;
                }
            }
        }
    }
}