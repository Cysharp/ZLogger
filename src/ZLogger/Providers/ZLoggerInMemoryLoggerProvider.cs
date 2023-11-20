using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;

namespace ZLogger.Providers
{
    [ProviderAlias("ZLoggerInMemory")]
    public class ZLoggerInMemoryLoggerProvider : ILoggerProvider, ISupportExternalScope, IAsyncDisposable
    {
        internal const string DefaultOptionName = "ZLoggerInMemory.Default";

        readonly ZLoggerOptions options;
        readonly InMemoryObservableLogProcessor processor;
        IExternalScopeProvider? scopeProvider;

        public ZLoggerInMemoryLoggerProvider(InMemoryObservableLogProcessor processor, IOptionsMonitor<ZLoggerOptions> options)
            : this(processor, DefaultOptionName, options)
        {
        }

        public ZLoggerInMemoryLoggerProvider(InMemoryObservableLogProcessor processor, string? optionName, IOptionsMonitor<ZLoggerOptions> options)
        {
            this.processor = processor;
            this.options = options.Get(optionName ?? DefaultOptionName);
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
    public class InMemoryObservableLogProcessor : IAsyncLogProcessor, IObservable<string>
    {
        bool isDisposed;
        ImmutableArray<IObserver<string>> observers = ImmutableArray<IObserver<string>>.Empty;

        void IAsyncLogProcessor.Post(IZLoggerEntry log)
        {
            if (isDisposed) return;
            var msg = log.ToString();
            log.Return();
            foreach (var item in observers)
            {
                item.OnNext(msg);
            }
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            if (isDisposed) return NullDisposable.Instance;
            ImmutableInterlocked.Update(ref observers, (xs, arg) => xs.Add(arg), observer);
            return new Subscription(this, observer);
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            isDisposed = true;
            foreach (var item in observers)
            {
                item.OnCompleted();
            }
            ImmutableInterlocked.Update(ref observers, (xs) => xs.Clear());
            return default;
        }

        sealed class Subscription(InMemoryObservableLogProcessor parent, IObserver<string> observer) : IDisposable
        {
            public void Dispose()
            {
                if (parent != null)
                {
                    ImmutableInterlocked.Update(ref parent.observers, (xs, arg) => xs.Remove(arg), observer);
                }
                parent = null!;
                observer = null!;
            }
        }

        sealed class NullDisposable : IDisposable
        {
            public static readonly IDisposable Instance = new NullDisposable();

            public void Dispose()
            {
            }
        }
    }
}