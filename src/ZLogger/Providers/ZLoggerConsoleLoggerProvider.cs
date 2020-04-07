using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerConsole")]
    public class ZLoggerConsoleLoggerProvider : ILoggerProvider
    {
        AsyncStreamLineMessageWriter streamWriter;

        public ZLoggerConsoleLoggerProvider(IOptions<ZLoggerOptions> options)
            : this(true, options)
        {
        }

        public ZLoggerConsoleLoggerProvider(bool consoleOutputEncodingToUtf8, IOptions<ZLoggerOptions> options)
        {
            if (consoleOutputEncodingToUtf8)
            {
                Console.OutputEncoding = new UTF8Encoding(false);
            }

            this.streamWriter = new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), options.Value);
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
