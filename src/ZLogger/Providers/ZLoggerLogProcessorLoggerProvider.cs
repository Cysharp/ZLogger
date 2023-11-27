using Microsoft.Extensions.Logging;

namespace ZLogger.Providers;

[ProviderAlias("ZLoggerLogProcessor")]
public class ZLoggerLogProcessorLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
{
    readonly ZLoggerOptions options;
    readonly IAsyncLogProcessor processor;
    IExternalScopeProvider? scopeProvider;

    public ZLoggerLogProcessorLoggerProvider(IAsyncLogProcessor processor, ZLoggerOptions options)
    {
        this.processor = processor;
        this.options = options;
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
