#pragma warning disable CS8601
#pragma warning disable CS8618

using Cysharp.Text;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ZLogger.Entries
{
    public struct FormatLogState<TPayload, T1> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1> Format;
        public readonly T1 Arg1;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1> format, T1 arg1)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1>> cache = new();

        FormatLogState<TPayload, T1> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1>> cache = new();

        PreparedFormatLogState<TPayload, T1> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2> format, T1 arg1, T2 arg2)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2>> cache = new();

        FormatLogState<TPayload, T1, T2> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3> format, T1 arg1, T2 arg2, T3 arg3)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3>> cache = new();

        FormatLogState<TPayload, T1, T2, T3> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5, T6> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5, T6>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5, T6> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5, T6> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5, T6> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5, T6> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5, T6> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;
        public readonly T11 Arg11;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
            Arg11 = arg11;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;
        public readonly T11 Arg11;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
            Arg11 = arg11;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;
        public readonly T11 Arg11;
        public readonly T12 Arg12;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
            Arg11 = arg11;
            Arg12 = arg12;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;
        public readonly T11 Arg11;
        public readonly T12 Arg12;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
            Arg11 = arg11;
            Arg12 = arg12;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;
        public readonly T11 Arg11;
        public readonly T12 Arg12;
        public readonly T13 Arg13;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
            Arg11 = arg11;
            Arg12 = arg12;
            Arg13 = arg13;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;
        public readonly T11 Arg11;
        public readonly T12 Arg12;
        public readonly T13 Arg13;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
            Arg11 = arg11;
            Arg12 = arg12;
            Arg13 = arg13;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public struct FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IZLoggerState
    {
        public static readonly Func<FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;
        public readonly T11 Arg11;
        public readonly T12 Arg12;
        public readonly T13 Arg13;
        public readonly T14 Arg14;

        public FormatLogState([AllowNull]TPayload payload, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
            Arg11 = arg11;
            Arg12 = arg12;
            Arg13 = arg13;
            Arg14 = arg14;
        }

        static IZLoggerEntry factory(FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Create(logInfo, this, scopeState);
        }
    }

    public struct PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IZLoggerState
    {
        public static readonly Func<PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, LogInfo, LogScopeState?, IZLoggerEntry> Factory = factory;

        public readonly TPayload Payload;
        public readonly Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Format;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;
        public readonly T10 Arg10;
        public readonly T11 Arg11;
        public readonly T12 Arg12;
        public readonly T13 Arg13;
        public readonly T14 Arg14;

        public PreparedFormatLogState([AllowNull]TPayload payload, Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            Payload = payload;
            Format = format;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
            Arg10 = arg10;
            Arg11 = arg11;
            Arg12 = arg12;
            Arg13 = arg13;
            Arg14 = arg14;
        }

        static IZLoggerEntry factory(PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> self, LogInfo logInfo, LogScopeState? scopeState)
        {
            return self.CreateLogEntry(logInfo, scopeState);
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo, LogScopeState? scopeState)
        {
            return PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Create(logInfo, this, scopeState);
        }
    }

    public class FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> cache = new();

        FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Create(in LogInfo logInfo, in FormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            using (var sb = ZString.CreateUtf8StringBuilder(true))
            {
                sb.AppendFormat(state.Format, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

    public class PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> cache = new();

        PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> state;

        public LogInfo LogInfo { get; private set; }
        public LogScopeState? ScopeState { get; private set; }

        public static PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Create(in LogInfo logInfo, in PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> state, LogScopeState? scopeState)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new PreparedFormatLogEntry<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            result.ScopeState = scopeState;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter)
        {
            var sb = ZString.CreateUtf8StringBuilder(true);
            try
            {
                state.Format.FormatTo(ref sb, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14);
                formatter.FormatLogEntry(writer, this, state.Payload, sb.AsSpan());
            }
            finally
            {
                sb.Dispose();
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            ScopeState?.Return();
            ScopeState = default;
            cache.Enqueue(this);
        }

        public void SwitchCasePayload<TPayload1>(System.Action<IZLoggerEntry, TPayload1, object?> payloadCallback, object? state)
        {
            if (typeof(TPayload1) == typeof(TPayload))
            {
                payloadCallback(this, Unsafe.As<TPayload, TPayload1>(ref Unsafe.AsRef(this.state.Payload)), state);
            }
        }

        public object? GetPayload()
        {
            return state.Payload;
        }
    }

}
