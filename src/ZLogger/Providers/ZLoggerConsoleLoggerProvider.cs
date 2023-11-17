using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerConsole")]
    public class ZLoggerConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        internal const string DefaultOptionName = "ZLoggerConsole.Default";

        readonly ZLoggerOptions options;
        readonly IAsyncLogProcessor processor;
        IExternalScopeProvider? scopeProvider; 

        public ZLoggerConsoleLoggerProvider(IOptionsMonitor<ZLoggerOptions> options)
            : this(DefaultOptionName, options)
        {
        }

        public ZLoggerConsoleLoggerProvider(string optionName, IOptionsMonitor<ZLoggerOptions> options, LogLevel logToStandardErrorThreshold = LogLevel.None)
        {
            this.options = options.Get(optionName);
            if (logToStandardErrorThreshold == LogLevel.None)
            {
                processor = new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), this.options);
            }
            else
            {
                processor = new CompositeAsyncLogProcessor(
                     new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), this.options, level => level < logToStandardErrorThreshold),
                     new AsyncStreamLineMessageWriter(Console.OpenStandardError(), this.options, level => level >= logToStandardErrorThreshold));
            }
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
