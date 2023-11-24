using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace ZLogger
{
    public sealed class LogScopeState
    {
        const string DefaultScopeKeyName = "Scope";
        static readonly ConcurrentQueue<LogScopeState> cache = new();

        public bool IsEmpty => properties.Count <= 0;

        public ReadOnlySpan<KeyValuePair<string, object?>> Properties => CollectionsMarshal.AsSpan(properties);

        readonly List<KeyValuePair<string, object?>> properties = new();

        internal static LogScopeState Create(IExternalScopeProvider scopeProvider)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new LogScopeState();
            }
            result.Snapshot(scopeProvider);
            return result;
        }

        internal void Return()
        {
            Clear();
            cache.Enqueue(this);
        }

        void Clear()
        {
            properties.Clear();
        }

        void Snapshot(IExternalScopeProvider scopeProvider)
        {
            Clear();
            scopeProvider.ForEachScope(static (state, props) =>
            {
                switch (state)
                {
                    // For example, using the `BeginScope(format, arg1, arg2, ...)` style, state is `FormattedLogValues : IEnumerable<KeyValuePair<string, object>>`.
                    case IEnumerable<KeyValuePair<string, object?>> enumerable:
                        props.AddRange(enumerable);
                        break;
                    case KeyValuePair<string, object?> prop:
                        props.Add(prop);
                        break;
                    default:
                        props.Add(new KeyValuePair<string, object?>(DefaultScopeKeyName, state));
                        break;
                }
            }, properties);
        }
    }
}
