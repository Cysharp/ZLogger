using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace ZLog.Providers
{
    [ProviderAlias("ZLogConsole")]
    public class ZLogConsoleLoggerProvider : ILoggerProvider
    {
        AsyncStreamLineMessageWriter streamWriter;

        public ZLogConsoleLoggerProvider(IOptions<ZLogOptions> options)
        {
            this.streamWriter = new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), options.Value);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ZLogLogger(categoryName, streamWriter);
        }

        public void Dispose()
        {
            streamWriter.DisposeAsync().AsTask().Wait();
        }
    }
}
