using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ZLog.Providers
{
    [ProviderAlias("ZLogRollingFile")]
    public class ZLogRollingFileLoggerProvider : ILoggerProvider
    {
        AsyncStreamLineMessageWriter streamWriter;

        public ZLogRollingFileLoggerProvider(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, IOptions<ZLogOptions> options)
        {
            var stream = new RollingFilestream(fileNameSelector, timestampPattern, rollSizeKB, options.Value);
            this.streamWriter = new AsyncStreamLineMessageWriter(stream, options.Value);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ZLogLogger(categoryName, streamWriter);
        }

        public void Dispose()
        {
            // TODO:flush wait timeout?
            streamWriter.DisposeAsync().AsTask().Wait();
        }
    }
}
