using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerConsole")]
    public class ZLoggerConsoleLoggerProvider : ILoggerProvider
    {
        internal const string DefaultOptionName = "ZLoggerConsole.Default";

        AsyncStreamLineMessageWriter streamWriter;

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

            var opt = options.Get(optionName ?? DefaultOptionName);
            var stream = outputToErrorStream ? Console.OpenStandardError() : Console.OpenStandardOutput();
            this.streamWriter = new AsyncStreamLineMessageWriter(stream, opt);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AsyncProcessZLogger(categoryName, streamWriter);
        }

        public void Dispose()
        {
            streamWriter.DisposeAsync().AsTask().Wait();
        }
    }
}
