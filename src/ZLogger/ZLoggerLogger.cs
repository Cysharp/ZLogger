﻿using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using ZLogger.LogStates;

namespace ZLogger
{
    public sealed class ZLoggerLogger : ILogger
    {
        readonly LogCategory category;
        readonly IAsyncLogProcessor logProcessor;
        readonly TimeProvider? timeProvider;
        readonly IExternalScopeProvider? scopeProvider;
        readonly bool formatImmediately;

        public ZLoggerLogger(string categoryName, IAsyncLogProcessor logProcessor, ZLoggerOptions options, IExternalScopeProvider? scopeProvider)
        {
            this.category = new LogCategory(categoryName);
            this.logProcessor = logProcessor;
            this.timeProvider = options.TimeProvider;
            this.scopeProvider = scopeProvider;
            this.formatImmediately = options.IsFormatLogImmediatelyInStandardLog;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var scopeState = scopeProvider != null
                ? LogScopeState.Create(scopeProvider)
                : null;

            var callerMemberName = default(string?);
            var callerFilePath = default(string?);
            var callerLineNumber = default(int);
            var context = default(object?);
            if (state is IZLoggerAdditionalInfo additionalInfo)
            {
                (context, callerMemberName, callerFilePath, callerLineNumber) = additionalInfo.GetAdditionalInfo();
            }

            var currentThread = Thread.CurrentThread;
            var threadId = currentThread.ManagedThreadId;
            var isThreadPoolThread = currentThread.IsThreadPoolThread;

            var info = new LogInfo(category, new Timestamp(timeProvider), logLevel, eventId, exception, scopeState, threadId, isThreadPoolThread, context, callerMemberName, callerFilePath, callerLineNumber);

            IZLoggerEntry entry;
            if (state is VersionedLogState)
            {
                var s = Unsafe.As<TState, VersionedLogState>(ref state);
                entry = s.CreateEntry(info);
                s.Retain();
            }
            else if (state is IZLoggerEntryCreatable creatable)
            {
                entry = creatable.CreateEntry(info);
                if (creatable is IReferenceCountable countable)
                {
                    countable.Retain();
                }
            }
            else
            {
                // called from standard `logger.Log`
                entry = new StringFormatterLogState<TState>(state, exception, formatter, formatImmediately).CreateEntry(info);
            }

            logProcessor.Post(entry);
        }

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
        {
            return scopeProvider?.Push(state) ?? NullDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        class NullDisposable : IDisposable
        {
            public static readonly IDisposable Instance = new NullDisposable();

            public void Dispose()
            {
            }
        }
    }
}
