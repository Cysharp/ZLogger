using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerStream")]
    public class ZLoggerStreamLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        internal const string DefaultOptionName = "ZLoggerStream.Default";

        readonly ZLoggerOptions options;
        readonly AsyncStreamLineMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerStreamLoggerProvider(Stream stream, IOptionsMonitor<ZLoggerOptions> options)
            : this(stream, DefaultOptionName, options)
        {
        }

        public ZLoggerStreamLoggerProvider(Stream stream, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            this.options = options.Get(optionName ?? DefaultOptionName);
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
