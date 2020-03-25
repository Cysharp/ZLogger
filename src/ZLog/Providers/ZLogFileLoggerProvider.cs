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
        AsyncStreamLineMessageWriter streamWriter;

        public ZLogFileLoggerProvider(string filePath, IOptions<ZLogOptions> options)
        {
            var di = new FileInfo(filePath).Directory;
            if (!di.Exists)
            {
                di.Create();
            }

            // useAsync:false, use sync(in thread) processor
            var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, false);
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
