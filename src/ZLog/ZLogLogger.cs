using Microsoft.Extensions.Logging;
using System;
using ZLog.Entries;

namespace ZLog
{
    internal class ZLogLogger : ILogger
    {
        readonly string categoryName;
        readonly AsyncStreamLineMessageWriter streamWriter;

        public ZLogLogger(string categoryName, AsyncStreamLineMessageWriter streamWriter)
        {
            this.categoryName = categoryName;
            this.streamWriter = streamWriter;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (state is IZLogState zstate)
            {
                var info = new LogInfo(categoryName, DateTimeOffset.UtcNow, logLevel, eventId, exception, zstate.IsJson);
                var entry = zstate.CreateLogEntry(info);
                streamWriter.Post(entry);
            }
            else
            {
                var info = new LogInfo(categoryName, DateTimeOffset.UtcNow, logLevel, eventId, exception, false);
                streamWriter.Post(StringFormatterEntry<TState>.Create(info, state, exception, formatter));
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
    }
}