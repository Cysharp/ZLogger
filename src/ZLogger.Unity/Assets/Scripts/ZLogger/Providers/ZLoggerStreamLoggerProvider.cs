using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace ZLogger.Providers
{

    [ProviderAlias("ZLoggerStream")]
    public class ZLoggerStreamLoggerProvider : ILoggerProvider
    {
        AsyncStreamLineMessageWriter streamWriter;

        public ZLoggerStreamLoggerProvider(Stream stream, IOptions<ZLoggerOptions> options)
        {
            this.streamWriter = new AsyncStreamLineMessageWriter(stream, options.Value);
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
