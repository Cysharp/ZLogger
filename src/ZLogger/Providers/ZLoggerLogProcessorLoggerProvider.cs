using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerLogProcessor")]
    public class ZLoggerLogProcessorLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        internal const string DefaultOptionName = "ZLoggerLogProcessor.Default";
        
        readonly ZLoggerOptions options;
        readonly IAsyncLogProcessor processor;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerLogProcessorLoggerProvider(IAsyncLogProcessor processor, IOptionsMonitor<ZLoggerOptions> options)
            : this(processor, DefaultOptionName, options)
        {
        }

        public ZLoggerLogProcessorLoggerProvider(IAsyncLogProcessor processor, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            this.processor = processor;
            this.options = options.Get(optionName ?? DefaultOptionName);
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
}
