using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerRollingFile")]
    public class ZLoggerRollingFileLoggerProvider : ILoggerProvider
    {
        internal const string DefaultOptionName = "ZLoggerRollingFile.Default";

        AsyncStreamLineMessageWriter streamWriter;

        public ZLoggerRollingFileLoggerProvider(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, IOptionsMonitor<ZLoggerOptions> options)
            : this(fileNameSelector, timestampPattern, rollSizeKB, DefaultOptionName, options)
        {
        }

        public ZLoggerRollingFileLoggerProvider(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            var opt = options.Get(optionName ?? DefaultOptionName);
            var stream = new RollingFileStream(fileNameSelector, timestampPattern, rollSizeKB, opt);
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
