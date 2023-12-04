using System.Text;
using Microsoft.Extensions.Logging;
using ZLogger.Internal;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerInMemory")]
    public class ZLoggerInMemoryLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        readonly ZLoggerOptions options;
        readonly InMemoryObservableLogProcessor processor;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerInMemoryLoggerProvider(InMemoryObservableLogProcessor processor, ZLoggerOptions options)
        {
            this.processor = processor;
            this.options = options;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ZLoggerLogger(categoryName, processor, options, options.IncludeScopes ? scopeProvider : null);
        }

        public void Dispose()
        {
            (processor as IAsyncDisposable).DisposeAsync().AsTask().Wait();
        }

        public async ValueTask DisposeAsync()
        {
            await (processor as IAsyncDisposable).DisposeAsync().ConfigureAwait(false);
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }
    }
}

namespace ZLogger
{
    public class InMemoryObservableLogProcessor : IAsyncLogProcessor, IDisposable
    {
        public event Action<string>? MessageReceived;
        public IZLoggerFormatter Formatter { get; internal set; } = default!;
        
        bool isDisposed;

        void IAsyncLogProcessor.Post(IZLoggerEntry log)
        {
            if (isDisposed) return;

            string msg;
            var buffer = ArrayBufferWriterPool.Rent();
            try
            {
                Formatter.FormatLogEntry(buffer, log);
                msg = Encoding.UTF8.GetString(buffer.WrittenSpan);
            }
            finally
            {
                log.Return();
                ArrayBufferWriterPool.Return(buffer);
            }
            MessageReceived?.Invoke(msg);
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            isDisposed = true;
            
            if (MessageReceived != null)
            {
                var invocationList = MessageReceived.GetInvocationList();
                foreach (var d in invocationList)
                {
                    MessageReceived -= (Action<string>)d;
                }
            }
            return default;
        }

        void IDisposable.Dispose()
        {
            (this as IAsyncDisposable).DisposeAsync().AsTask().Wait();
        }
    }
}