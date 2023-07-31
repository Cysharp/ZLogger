using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerFile")]
    public class ZLoggerFileLoggerProvider : ILoggerProvider
    {
        internal const string DefaultOptionName = "ZLoggerFile.Default";

        AsyncStreamLineMessageWriter streamWriter;
        private readonly bool isStructured;

        public ZLoggerFileLoggerProvider(string filePath, IOptionsMonitor<ZLoggerOptions> options)
            : this(filePath, DefaultOptionName, options)
        {
        }

        public ZLoggerFileLoggerProvider(string filePath, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            var di = new FileInfo(filePath).Directory;
            if (!di!.Exists)
            {
                di.Create();
            }

            var opt = options.Get(optionName ?? DefaultOptionName);

            // useAsync:false, use sync(in thread) processor, don't use FileStream buffer(use buffer size = 1).
            var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1, false);
            this.streamWriter = new AsyncStreamLineMessageWriter(stream, opt);
            isStructured = opt.EnableStructuredLogging;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AsyncProcessZLogger(categoryName, streamWriter, shouldIncrementLogId: isStructured);
        }

        public void Dispose()
        {
            // busy-wait for a bit
            var task = streamWriter.DisposeAsync().AsTask();
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (!task.IsCompleted)
            {
                if (sw.ElapsedMilliseconds > 1000)
                {
                    break;
                }
                Thread.Sleep(10);
            }
        }
    }
}
