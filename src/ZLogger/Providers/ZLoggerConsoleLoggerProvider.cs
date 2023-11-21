using Microsoft.Extensions.Logging;

namespace ZLogger.Providers
{
    public class ZLoggerConsoleOptions : ZLoggerOptions
    {
        public bool OutputEncodingToUtf8 { get; set; } = true;
        public bool ConfigureEnableAnsiEscapeCode { get; set; } = false;
        public LogLevel LogToStandardErrorThreshold { get; set; } = LogLevel.None;
    }

    public class ZLoggerConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        readonly ZLoggerConsoleOptions options;
        readonly IAsyncLogProcessor processor;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerConsoleLoggerProvider(ZLoggerConsoleOptions options)
        {
            this.options = options;
            if (options.LogToStandardErrorThreshold == LogLevel.None)
            {
                processor = new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), this.options);
            }
            else
            {
                var logToStandardErrorThreshold = options.LogToStandardErrorThreshold;
                processor = new CompositeAsyncLogProcessor(
                     new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), this.options, level => level < logToStandardErrorThreshold),
                     new AsyncStreamLineMessageWriter(Console.OpenStandardError(), this.options, level => level >= logToStandardErrorThreshold));
            }
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
