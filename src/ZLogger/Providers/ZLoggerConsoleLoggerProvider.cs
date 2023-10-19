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
        readonly AsyncStreamMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider; 

        public ZLoggerConsoleLoggerProvider(IOptionsMonitor<ZLoggerOptions> options)
            : this(true, null, options)
        {
        }

        public ZLoggerConsoleLoggerProvider(bool consoleOutputEncodingToUtf8, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
            : this(consoleOutputEncodingToUtf8, false, optionName, options)
        {
        }

        public ZLoggerConsoleLoggerProvider(bool consoleOutputEncodingToUtf8, bool outputToErrorStream, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            if (consoleOutputEncodingToUtf8)
            {
                Console.OutputEncoding = new UTF8Encoding(false);
            }

            this.options = options.Get(optionName ?? DefaultOptionName);
            var stream = outputToErrorStream ? Console.OpenStandardError() : Console.OpenStandardOutput();
            this.streamWriter = new AsyncStreamMessageWriter(stream, this.options);
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = new AsyncProcessZLogger(categoryName, streamWriter);
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
