using Microsoft.Extensions.Logging;

namespace ZLogger.Providers
{
    public class ZLoggerRollingFileLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        readonly ZLoggerOptions options;
        readonly AsyncStreamLineMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerRollingFileLoggerProvider(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, ZLoggerOptions options)
        {
            this.options = options;
            var stream = new RollingFileStream(fileNameSelector, timestampPattern, rollSizeKB, this.options);
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
}
