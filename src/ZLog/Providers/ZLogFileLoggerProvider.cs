using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ZLog.Providers
{
    [ProviderAlias("ZLogFile")]
    public class ZLogFileLoggerProvider : ILoggerProvider
    {
        // ZLogOptions options;
        AsyncStreamLineMessageWriter broker;

        public ZLogFileLoggerProvider(string filePath, IOptions<ZLogOptions> options)
        {
            // useAsync:false, use dedicated processor
            var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, false);
            this.broker = new AsyncStreamLineMessageWriter(stream, options.Value);
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
