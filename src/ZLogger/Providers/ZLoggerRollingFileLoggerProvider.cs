using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerRollingFile")]
    public class ZLoggerRollingFileLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        internal const string DefaultOptionName = "ZLoggerRollingFile.Default";

        readonly ZLoggerOptions options;
        readonly AsyncStreamLineMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerRollingFileLoggerProvider(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, IOptionsMonitor<ZLoggerOptions> options)
            : this(fileNameSelector, timestampPattern, rollSizeKB, DefaultOptionName, options)
        {
        }

        public ZLoggerRollingFileLoggerProvider(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            this.options = options.Get(optionName ?? DefaultOptionName);
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
