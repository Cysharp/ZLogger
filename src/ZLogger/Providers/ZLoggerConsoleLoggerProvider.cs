using Microsoft.Extensions.Logging;

namespace ZLogger.Providers;

public sealed class ZLoggerConsoleOptions : ZLoggerOptions
{
    public bool OutputEncodingToUtf8 { get; set; } = true;
    public bool ConfigureEnableAnsiEscapeCode { get; set; } = false;
    public LogLevel LogToStandardErrorThreshold { get; set; } = LogLevel.None;
}

[ProviderAlias("ZLoggerConsole")]
public sealed class ZLoggerConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
{
    readonly ZLoggerConsoleOptions options;
    readonly IAsyncLogProcessor processor;
    IExternalScopeProvider? scopeProvider;

    public ZLoggerConsoleLoggerProvider(ZLoggerConsoleOptions options)
    {
        this.options = options;
        if (options.LogToStandardErrorThreshold == LogLevel.None)
        {
            processor = new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), this.options);
        }
        else
        {
            var logToStandardErrorThreshold = options.LogToStandardErrorThreshold;
            processor = new DualAsyncLogProcessor(
                 new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), this.options, level => level < logToStandardErrorThreshold),
                 new AsyncStreamLineMessageWriter(Console.OpenStandardError(), this.options, level => level >= logToStandardErrorThreshold));
        }
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

    sealed class DualAsyncLogProcessor(AsyncStreamLineMessageWriter processor1, AsyncStreamLineMessageWriter processor2) : IAsyncLogProcessor
    {
        public void Post(IZLoggerEntry log)
        {
            // Post two entry is dangerous for log-state reference count cache.
            // However filtered reader-loop does not call Return so ok to post multiple.
            processor1.Post(log);
            processor2.Post(log);
        }

        public async ValueTask DisposeAsync()
        {
            var t1 = processor1.DisposeAsync();
            var t2 = processor2.DisposeAsync();
            await t1;
            await t2;
        }
    }
}
