using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerFile")]
    public class ZLoggerFileLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        internal const string DefaultOptionName = "ZLoggerFile.Default";

        readonly ZLoggerOptions options;
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

            this.options = options.Get(optionName ?? DefaultOptionName);

            // useAsync:false, use sync(in thread) processor, don't use FileStream buffer(use buffer size = 1).
            var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1, false);
            this.streamWriter = new AsyncStreamLineMessageWriter(stream, null, this.options);
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = new ZLoggerLogger(categoryName, streamWriter, options);
            if (options.IncludeScopes)
            {
                logger.ScopeProvider = scopeProvider;
            }
            return logger;
        }

        public void Dispose()
        {
            streamWriter.DisposeAsync().AsTask().Wait();
        }

        public async ValueTask DisposeAsync()
        {
            await streamWriter.DisposeAsync().ConfigureAwait(false);
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }
    }
}
