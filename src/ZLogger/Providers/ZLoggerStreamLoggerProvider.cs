using Microsoft.Extensions.Logging;

namespace ZLogger.Providers;

[ProviderAlias("ZLoggerStream")]
public class ZLoggerStreamLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
{
    readonly ZLoggerOptions options;
    readonly AsyncStreamLineMessageWriter streamWriter;
    IExternalScopeProvider? scopeProvider;

    public ZLoggerStreamLoggerProvider(Stream stream, ZLoggerOptions options)
    {
        this.options = options;
        this.streamWriter = new AsyncStreamLineMessageWriter(stream, this.options);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new ZLoggerLogger(categoryName, streamWriter, options, options.IncludeScopes ? scopeProvider : null);
    }

    public void Dispose()
    {
        streamWriter.DisposeAsync().AsTask().Wait();
    }

    public async ValueTask DisposeAsync()
    {
        await streamWriter.DisposeAsync().ConfigureAwait(false);
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopeProvider = scopeProvider;
    }
}
