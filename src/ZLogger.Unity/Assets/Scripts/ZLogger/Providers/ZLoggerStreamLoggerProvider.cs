using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerStream")]
    public class ZLoggerStreamLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        internal const string DefaultOptionName = "ZLoggerStream.Default";

        readonly AsyncStreamLineMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerStreamLoggerProvider(Stream stream, IOptionsMonitor<ZLoggerOptions> options)
            : this(stream, DefaultOptionName, options)
        {
        }

        public ZLoggerStreamLoggerProvider(Stream stream, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            this.streamWriter = new AsyncStreamLineMessageWriter(stream, options.Get(optionName ?? DefaultOptionName));
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
