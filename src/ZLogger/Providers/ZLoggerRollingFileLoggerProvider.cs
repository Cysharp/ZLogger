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

public sealed class ZLoggerRollingFileOptions : ZLoggerFileOptions
{
    public Func<DateTimeOffset, int, string>? FilePathSelector { get; set; }
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
    public int RollingSizeKB { get; set; } = 512 * 1024;
}

[ProviderAlias("ZLoggerRollingFile")]
public class ZLoggerRollingFileLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
{
    readonly ZLoggerRollingFileOptions options;
    readonly AsyncStreamLineMessageWriter streamWriter;
    IExternalScopeProvider? scopeProvider;

    public ZLoggerRollingFileLoggerProvider(ZLoggerRollingFileOptions options)
    {
        if (options.FilePathSelector is null)
        {
            throw new ArgumentException(nameof(options.FilePathSelector));
        }
        this.options = options;
        var stream = new RollingFileStream(options.FilePathSelector!, options.RollingInterval, options.RollingSizeKB, options.TimeProvider, options.FileShared);
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
