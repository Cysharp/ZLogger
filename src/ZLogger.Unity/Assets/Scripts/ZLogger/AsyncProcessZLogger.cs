using Microsoft.Extensions.Logging;
using System;
using ZLogger.Entries;

namespace ZLogger
{
    public class AsyncProcessZLogger : ILogger
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
            
            var entry = CreateLogEntry<TState>.factory?.Invoke(state, info, scopeState) ?? 
                        StringFormatterEntry<TState>.Create(info, state, scopeState, exception, formatter);

            logProcessor.Post(entry);
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
                    try
                    {
                        var factoryField = typeof(T).GetField("Factory", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        LogForUnity(factoryField);

                        if (factoryField != null)
                        {
                            factory = factoryField.GetValue(null) as Func<T, LogInfo, LogScopeState?, IZLoggerEntry>;
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