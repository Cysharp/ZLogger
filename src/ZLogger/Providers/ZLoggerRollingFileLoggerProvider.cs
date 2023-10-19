using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerRollingFile")]
    public class ZLoggerRollingFileLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        internal const string DefaultOptionName = "ZLoggerRollingFile.Default";

        readonly ZLoggerOptions options;
        readonly AsyncStreamMessageWriter streamWriter;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerRollingFileLoggerProvider(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, IOptionsMonitor<ZLoggerOptions> options)
            : this(fileNameSelector, timestampPattern, rollSizeKB, DefaultOptionName, options)
        {
        }

        public ZLoggerRollingFileLoggerProvider(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            this.options = options.Get(optionName ?? DefaultOptionName);
            var stream = new RollingFileStream(fileNameSelector, timestampPattern, rollSizeKB, this.options);
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
