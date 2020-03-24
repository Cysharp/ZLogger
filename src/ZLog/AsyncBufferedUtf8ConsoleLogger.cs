using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLog.Entries;

namespace ZLog
{
    internal class AsyncBufferedUtf8ConsoleLogger : ILogger
    {
        readonly string categoryName;
        readonly LoggerBroker broker;

        public AsyncBufferedUtf8ConsoleLogger(string categoryName, LoggerBroker broker)
        {
            this.categoryName = categoryName;
            this.broker = broker;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (state is IZLogState zstate)
            {
                var entry = zstate.CreateLogEntry();
                broker.Post(entry);
            }
            else
            {
                broker.Post(StringFormatterEntry<TState>.Create(logLevel, eventId, state, exception, formatter));
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            //TODO:scope
            return NUllDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // TODO:isEnabled
            return true;
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