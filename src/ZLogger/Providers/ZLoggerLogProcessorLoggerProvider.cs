using Microsoft.Extensions.Logging;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerLogProcessor")]
    public class ZLoggerLogProcessorLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        readonly IAsyncLogProcessor processor;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerLogProcessorLoggerProvider(IAsyncLogProcessor processor)
        {
            this.processor = processor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AsyncProcessZLogger(categoryName, processor)
            {
                ScopeProvider = scopeProvider
            };
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
