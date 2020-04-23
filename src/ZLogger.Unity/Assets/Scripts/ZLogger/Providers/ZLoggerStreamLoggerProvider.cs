using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace ZLogger.Providers
{

    [ProviderAlias("ZLoggerStream")]
    public class ZLoggerStreamLoggerProvider : ILoggerProvider
    {
        internal const string DefaultOptionName = "ZLoggerStream.Default";

        AsyncStreamLineMessageWriter streamWriter;

        public ZLoggerStreamLoggerProvider(Stream stream, IOptionsMonitor<ZLoggerOptions> options)
            : this(stream, DefaultOptionName, options)
        {
        }

        public ZLoggerStreamLoggerProvider(Stream stream, string optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            this.streamWriter = new AsyncStreamLineMessageWriter(stream, options.Get(optionName ?? DefaultOptionName));
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
