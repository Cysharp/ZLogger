using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using ZLogger.Entries;

namespace ZLogger
{
    public class AsyncProcessZLogger : ILogger
    {
        readonly Func<string, Exception?, string> ReturnStringStateFormatter = (state, _) => state;

        readonly string categoryName;
        readonly IAsyncLogProcessor logProcessor;

        public AsyncProcessZLogger(string categoryName, IAsyncLogProcessor logProcessor)
        {
            this.categoryName = categoryName;
            this.logProcessor = logProcessor;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
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
                if (StateTypeDetector<TState>.IsInternalFormattedLogValues || state == null)
                {
                    // null state automatically converted to FormattedLogValues struct.
                    logProcessor.Post(StringFormatterEntry<TState>.Create(info, state, exception, formatter));
                }
                else
                {
                    // sometimes state has context(like HttpContext), it require to format in scope.
                    var message = formatter(state, exception);
                    logProcessor.Post(StringFormatterEntry<string>.Create(info, message, exception, ReturnStringStateFormatter));
                }
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
            // currently scope is not supported...
            return NullDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        class NullDisposable : IDisposable
        {
            public static IDisposable Instance = new NullDisposable();

            NullDisposable()
            {

            }

            public void Dispose()
            {
            }
        }


        // call CreateLogEntry without cast(boxing)
        static class CreateLogEntry<T>
        // where T:IZLoggerState
        {
            public static readonly Func<T, LogInfo, IZLoggerEntry>? factory;

            static CreateLogEntry()
            {
                if (typeof(IZLoggerState).IsAssignableFrom(typeof(T)))
                {
                    try
                    {
                        var factoryField = typeof(T).GetField("Factory", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        LogForUnity(factoryField);

                        if (factoryField != null)
                        {
                            factory = factoryField.GetValue(null) as Func<T, LogInfo, IZLoggerEntry>;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogForUnity(ex);
                    }
                }
                else
                {
                    factory = null;
                }
            }

            static void LogForUnity(System.Reflection.FieldInfo? fieldInfo)
            {
#if UNITY_2018_3_OR_NEWER
                if(fieldInfo == null)
                {
                    UnityEngine.Debug.Log("State.Factory FieldInfo is null.");
                }
#endif
            }

            static void LogForUnity(Exception ex)
            {
#if UNITY_2018_3_OR_NEWER
                UnityEngine.Debug.Log(ex);
#endif
            }
        }
    }
}