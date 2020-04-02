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
        AsyncStreamLineMessageWriter streamWriter;

        public ZLoggerFileLoggerProvider(string filePath, IOptions<ZLoggerOptions> options)
        {
            var di = new FileInfo(filePath).Directory;
            if (!di.Exists)
            {
                di.Create();
            }

            // useAsync:false, use sync(in thread) processor, don't use FileStream buffer(use buffer size = 1).
            var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1, false);
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
