using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerFile")]
    public class ZLoggerFileLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        internal const string DefaultOptionName = "ZLoggerFile.Default";

        readonly AsyncStreamLineMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider;

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
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AsyncProcessZLogger(categoryName, streamWriter)
            {
                ScopeProvider = scopeProvider
            };
        }

        public void Dispose()
        {
            streamWriter.DisposeAsync().AsTask().Wait();
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }
    }
}
