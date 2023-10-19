using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerStream")]
    public class ZLoggerStreamLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        internal const string DefaultOptionName = "ZLoggerStream.Default";

        readonly ZLoggerOptions options;
        readonly AsyncStreamMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerStreamLoggerProvider(Stream stream, IOptionsMonitor<ZLoggerOptions> options)
            : this(stream, DefaultOptionName, options)
        {
        }

        public ZLoggerStreamLoggerProvider(Stream stream, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            this.options = options.Get(optionName ?? DefaultOptionName);
            this.streamWriter = new AsyncStreamMessageWriter(stream, this.options);
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = new AsyncProcessZLogger(categoryName, streamWriter);
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

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }
    }
}
