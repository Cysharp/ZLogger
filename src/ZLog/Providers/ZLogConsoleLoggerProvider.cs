using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ZLog.Providers
{
    [ProviderAlias("ZLogConsole")]
    public class ZLogConsoleLoggerProvider : ILoggerProvider
    {
        // ZLogOptions options;
        AsyncStreamLineMessageWriter broker;

        public ZLogConsoleLoggerProvider(IOptions<ZLogOptions> options)
        {
            this.broker = new AsyncStreamLineMessageWriter(Console.OpenStandardOutput(), options.Value);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ZLogLogger(categoryName, broker);
        }

        public void Dispose()
        {
            // TODO:flush wait timeout?
            broker.DisposeAsync().AsTask().Wait();
        }
    }
}
