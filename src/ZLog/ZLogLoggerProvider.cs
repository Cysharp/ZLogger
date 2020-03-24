using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ZLog
{
    [ProviderAlias("ZLog")]
    public class ZLogLoggerProvider : ILoggerProvider
    {
        // ZLogOptions options;
        LoggerBroker broker;
        
        public ZLogLoggerProvider(/* IOptionsMonitor<> */ IOptions<ZLogOptions> options)
        {
            //options.CurrentValue
            // this.options = options;
            this.broker = new LoggerBroker();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AsyncBufferedUtf8ConsoleLogger(categoryName, broker);
        }

        public void Dispose()
        {
            // TODO:flush wait timeout?
            broker.DisposeAsync().AsTask().Wait();
        }
    }
}
