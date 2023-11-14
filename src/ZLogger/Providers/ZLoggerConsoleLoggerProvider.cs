using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerConsole")]
    public class ZLoggerConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        internal const string DefaultOptionName = "ZLoggerConsole.Default";

        readonly ZLoggerOptions options;
        readonly AsyncStreamLineMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider; 

        public ZLoggerConsoleLoggerProvider(IOptionsMonitor<ZLoggerOptions> options)
            : this(true, null, options)
        {
        }

        public ZLoggerConsoleLoggerProvider(bool consoleOutputEncodingToUtf8, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
            : this(consoleOutputEncodingToUtf8, LogLevel.None, optionName, options)
        {
        }

        public ZLoggerConsoleLoggerProvider(bool consoleOutputEncodingToUtf8, LogLevel logToStandardErrorThreshold, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            if (consoleOutputEncodingToUtf8)
            {
                Console.OutputEncoding = new UTF8Encoding(false);
            }

            this.options = options.Get(optionName ?? DefaultOptionName);
            this.streamWriter = new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), Console.OpenStandardError(), this.options);
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = new ZLoggerLogger(categoryName, streamWriter);
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

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }
    }
}
