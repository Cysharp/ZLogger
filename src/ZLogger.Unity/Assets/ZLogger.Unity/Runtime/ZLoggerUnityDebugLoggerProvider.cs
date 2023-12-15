using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZLogger.Providers;

namespace ZLogger.Unity;

public static class ZLoggerUnityExtensions
{
    public static ILoggingBuilder AddZLoggerUnityDebug(this ILoggingBuilder builder) => builder.AddZLoggerUnityDebug(_ => { });
    public static ILoggingBuilder AddZLoggerUnityDebug(this ILoggingBuilder builder, Action<ZLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, ZLoggerUnityDebugLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerConsoleOptions();
            configure(options);
            return new ZLoggerUnityDebugLoggerProvider(options);
        });
        return builder;
    }
    
}

public class UnityDebugLogProcessor : IAsyncLogProcessor
{
    [ThreadStatic]
    static ArrayBufferWriter<byte>? bufferWriter;
    
    readonly ZLoggerOptions options;
    readonly IZLoggerFormatter formatter;

    public UnityDebugLogProcessor(ZLoggerOptions options)
    {
        this.options = options;
        formatter = options.CreateFormatter();
    }

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public void Post(IZLoggerEntry log)
    {
        bufferWriter ??= new ArrayBufferWriter<byte>();
        
        try
        {
            var msg = FormatToString(log, formatter);
            switch (log.LogInfo.LogLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Information:
                    UnityEngine.Debug.Log(msg);
                    break;
                case LogLevel.Warning:
                case LogLevel.Critical:
                    UnityEngine.Debug.LogWarning(msg);
                    break;
                case LogLevel.Error:
                    if (log.LogInfo.Exception != null)
                    {
                        UnityEngine.Debug.LogException(log.LogInfo.Exception);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(msg);
                    }
                    break;
                case LogLevel.None:
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

    public ZLoggerUnityDebugLoggerProvider(ZLoggerOptions options)
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
