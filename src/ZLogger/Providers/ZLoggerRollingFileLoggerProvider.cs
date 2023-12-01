using Microsoft.Extensions.Logging;

namespace ZLogger.Providers;

public enum RollingInterval
{
    Infinite,
    Year,
    Month,
    Day,
    Hour,
    Minute
}

[ProviderAlias("ZLoggerRollingFile")]
public class ZLoggerRollingFileLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
{
    readonly ZLoggerOptions options;
    readonly AsyncStreamLineMessageWriter streamWriter;
    IExternalScopeProvider? scopeProvider;

    public ZLoggerRollingFileLoggerProvider(Func<DateTimeOffset, int, string> filePathSelector, RollingInterval rollInterval, int rollSizeKB, ZLoggerOptions options)
    {
        this.options = options;
        var stream = new RollingFileStream(filePathSelector, rollInterval, rollSizeKB, options.TimeProvider);
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
