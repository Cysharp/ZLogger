using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerLogProcessor")]
    public class ZLoggerLogProcessorLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        internal const string DefaultOptionName = "ZLoggerFile.Default";
        
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
            var logger = new ZLoggerLogger(categoryName, processor, options);
            if (options.IncludeScopes)
            {
                logger.ScopeProvider = scopeProvider;
            }
            return logger;
        }

        public void Dispose()
        {
            processor.DisposeAsync().AsTask().Wait();
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }
    }
}
