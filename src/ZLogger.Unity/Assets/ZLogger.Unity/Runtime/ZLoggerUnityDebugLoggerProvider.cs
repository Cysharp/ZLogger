#nullable enable

using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger.Unity.Runtime;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ZLogger.Unity
{
    public sealed class ZLoggerUnityDebugOptions : ZLoggerOptions
    {
        public bool PrettyStacktrace { get; set; } = true;
    }

    public static class ZLoggerUnityExtensions
    {
        public static ILoggingBuilder AddZLoggerUnityDebug(this ILoggingBuilder builder) => builder.AddZLoggerUnityDebug(_ => { });
        public static ILoggingBuilder AddZLoggerUnityDebug(this ILoggingBuilder builder, Action<ZLoggerUnityDebugOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, ZLoggerUnityDebugLoggerProvider>(serviceProvider =>
            {
                var options = new ZLoggerUnityDebugOptions();
                configure(options);
                return new ZLoggerUnityDebugLoggerProvider(options);
            });
            return builder;
        }

        public static UnityEngine.LogType AsUnityLogType(this LogInfo logInfo)
        {
            if (logInfo.Exception != null)
            {
                return UnityEngine.LogType.Exception;
            }
            return logInfo.LogLevel switch
            {
                LogLevel.Warning => UnityEngine.LogType.Warning,
                LogLevel.Error or LogLevel.Critical => UnityEngine.LogType.Error,
                _ => UnityEngine.LogType.Log
            };
        }
    }

    public class UnityDebugLogProcessor : IAsyncLogProcessor
    {
        [ThreadStatic]
        static ArrayBufferWriter<byte>? bufferWriter;

        readonly ZLoggerUnityDebugOptions options;
        readonly IZLoggerFormatter formatter;

        public UnityDebugLogProcessor(ZLoggerUnityDebugOptions options)
        {
            this.options = options;
            formatter = options.CreateFormatter();
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        [UnityEngine.HideInCallstack]
        public void Post(IZLoggerEntry log)
        {
            try
            {
                var context = log.LogInfo.Context as UnityEngine.Object;
                var msg = FormatToString(log, formatter);
                var unityLogType = log.LogInfo.AsUnityLogType();

                if (LogTypeMapper.GetStackTraceLogType(unityLogType) != StackTraceLogType.None && options.PrettyStacktrace)
                {
                    var stacktrace = new StackTrace(5, true);
                    msg = $"{msg}{Environment.NewLine}{DiagnosticsHelper.CleanupStackTrace(stacktrace)}{Environment.NewLine}---";
                }

                switch (unityLogType)
                {
                    case UnityEngine.LogType.Log:
                        if (context != null)
                        {
                            UnityEngine.Debug.Log(msg, context);
                        }
                        else
                        {
                            UnityEngine.Debug.Log(msg);
                        }
                        break;
                    case UnityEngine.LogType.Warning:
                        if (context != null)
                        {
                            UnityEngine.Debug.LogWarning(msg, context);
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning(msg);
                        }
                        break;
                    case UnityEngine.LogType.Error:
                        if (context != null)
                        {
                            UnityEngine.Debug.LogError(msg, context);
                        }
                        else
                        {
                            UnityEngine.Debug.LogError(msg);
                        }
                        break;
                    case UnityEngine.LogType.Exception:
                        if (context != null)
                        {
                            UnityEngine.Debug.LogException(log.LogInfo.Exception, context);
                        }
                        else
                        {
                            UnityEngine.Debug.LogException(log.LogInfo.Exception);
                        }
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                log.Return();
            }
        }

        static string FormatToString(IZLoggerEntry entry, IZLoggerFormatter formatter)
        {
            bufferWriter ??= new ArrayBufferWriter<byte>();
            bufferWriter.Clear();

            formatter.FormatLogEntry(bufferWriter, entry);
            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }
    }

    [ProviderAlias("ZLoggerUnityDebug")]
    public class ZLoggerUnityDebugLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        readonly ZLoggerOptions options;
        readonly UnityDebugLogProcessor processor;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerUnityDebugLoggerProvider(ZLoggerUnityDebugOptions options)
        {
            this.options = options;
            this.processor = new UnityDebugLogProcessor(options);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ZLoggerLogger(categoryName, processor, options, options.IncludeScopes ? scopeProvider : null);
        }

        public void Dispose()
        {
            processor.DisposeAsync().AsTask().Wait();
        }

        public async ValueTask DisposeAsync()
        {
            await processor.DisposeAsync().ConfigureAwait(false);
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }
    }

    static class LogTypeMapper
    {
        static StackTraceLogType[]? stackTraceLogTypes = default!;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            var logTypes = new[] { LogType.Error, LogType.Assert, LogType.Warning, LogType.Log, LogType.Exception };
            stackTraceLogTypes = new StackTraceLogType[logTypes.Length];
            for (int i = 0; i < logTypes.Length; i++)
            {
                stackTraceLogTypes[i] = UnityEngine.Application.GetStackTraceLogType(logTypes[i]);
            }
        }

        public static StackTraceLogType GetStackTraceLogType(LogType logType)
        {
            var i = (int)logType;
            if (stackTraceLogTypes != null && i >= 0 && i < stackTraceLogTypes.Length)
            {
                return stackTraceLogTypes[i];
            }
            return StackTraceLogType.None;
        }
    }
}