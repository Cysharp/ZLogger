using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerLogProcessor")]
    public class ZLoggerLogProcessorLoggerProvider : ILoggerProvider
    {
        IAsyncLogProcessor processor;

        public ZLoggerLogProcessorLoggerProvider(IAsyncLogProcessor processor)
        {
            this.processor = processor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AsyncProcessZLogger(categoryName, processor);
        }

        public void Dispose()
        {
            processor.DisposeAsync().AsTask().Wait();
        }
    }
}
