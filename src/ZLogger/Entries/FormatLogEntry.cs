using Cysharp.Text;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ZLogger.Entries
{
    internal struct FormatLogState<TPayload, T0> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0>>();

        FormatLogState<TPayload, T0> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
            Arg1 = arg1;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1>>();

        FormatLogState<TPayload, T0, T1> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;
        public readonly T2 Arg2;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2>>();

        FormatLogState<TPayload, T0, T1, T2> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3>>();

        FormatLogState<TPayload, T0, T1, T2, T3> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;
        public readonly T2 Arg2;
        public readonly T3 Arg3;
        public readonly T4 Arg4;
        public readonly T5 Arg5;
        public readonly T6 Arg6;
        public readonly T7 Arg7;
        public readonly T8 Arg8;
        public readonly T9 Arg9;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
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

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
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

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
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

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
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

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
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

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
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

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
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

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
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

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
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

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
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

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
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

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

    internal struct FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IZLoggerState
    {
        public readonly TPayload Payload;
        public readonly string? Format;
        public readonly T0 Arg0;
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
        public readonly T15 Arg15;

        public FormatLogState([AllowNull]TPayload payload, string? format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
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
            Arg15 = arg15;
        }

        public IZLoggerEntry CreateLogEntry(LogInfo logInfo)
        {
            return FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Create(logInfo, this);
        }
    }

    internal class FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IZLoggerEntry
    {
        static readonly ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> cache = new ConcurrentQueue<FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>();

        FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> state;

        public LogInfo LogInfo { get; private set; }

        FormatLogEntry()
        {
        }

        public static FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Create(in LogInfo logInfo, in FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> state)
        {
            if (!cache.TryDequeue(out var result))
            {
                result = new FormatLogEntry<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>();
            }

            result.LogInfo = logInfo;
            result.state = state;
            return result;
        }

        public void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            if (options.IsStructuredLogging && jsonWriter != null)
            {
                using (var sb = ZString.CreateUtf8StringBuilder(true))
                {
                    sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14, state.Arg15);
                    jsonWriter.WriteString(options.MessagePropertyName, sb.AsSpan());
                }

                jsonWriter.WritePropertyName(options.PayloadPropertyName);
                JsonSerializer.Serialize(jsonWriter, state.Payload, options.JsonSerializerOptions);
            }
            else
            {
                if (state.Format != null)
                {
                    // TODO: ZString.FormatUtf8(writer);
                    using (var sb = ZString.CreateUtf8StringBuilder(true))
                    {
                        sb.AppendFormat(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14, state.Arg15);
                        var dest = writer.GetSpan(sb.Length);
                        sb.TryCopyTo(dest, out var written);
                        writer.Advance(written);
                    }
                }
                else
                {
                    using (var writer2 = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(writer2, state.Payload, options.JsonSerializerOptions);
                    }
                }
            }
        }

        public void Return()
        {
            state = default;
            LogInfo = default!;
            cache.Enqueue(this);
        }
    }

}
