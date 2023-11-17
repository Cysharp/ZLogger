using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerConsole")]
    public class ZLoggerConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        internal const string DefaultOptionName = "ZLoggerConsole.Default";

        readonly ZLoggerOptions options;
        readonly AsyncStreamLineMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider; 

        public ZLoggerConsoleLoggerProvider(IOptionsMonitor<ZLoggerOptions> options)
            : this(DefaultOptionName, options)
        {
        }

        public ZLoggerConsoleLoggerProvider(string optionName, IOptionsMonitor<ZLoggerOptions> options, LogLevel logToStandardErrorThreshold = LogLevel.None)
        {
            this.options = options.Get(optionName);
            this.streamWriter = new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), this.options, Console.OpenStandardError(), logToStandardErrorThreshold);
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = new ZLoggerLogger(categoryName, streamWriter, options);
            if (options.IncludeScopes)
            {
                logger.ScopeProvider = scopeProvider;
            }
            return logger;
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
