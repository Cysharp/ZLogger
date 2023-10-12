using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerRollingFile")]
    public class ZLoggerRollingFileLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        internal const string DefaultOptionName = "ZLoggerRollingFile.Default";

        readonly AsyncStreamLineMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider;

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
