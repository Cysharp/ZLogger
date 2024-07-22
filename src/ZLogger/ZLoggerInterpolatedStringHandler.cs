using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Utf8StringInterpolation;
using ZLogger.Internal;
using ZLogger.LogStates;

namespace ZLogger
{
    [InterpolatedStringHandler]
    public ref struct ZLoggerInterpolatedStringHandler
    {
        [ThreadStatic]
        static List<string?>? literalPool;

        public bool IsLoggerEnabled { get; }

        // fields
        readonly int literalLength;
        readonly int parametersLength;
        readonly List<string?> literals;
        readonly InterpolatedStringLogState state;
        int parameterWritten;

        /// <summary>
        /// DO NOT ALLOW DIRECT USE.
        /// </summary>
        public ZLoggerInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, LogLevel logLevel, out bool enabled)
        {
            enabled = logger.IsEnabled(logLevel);
            if (!enabled)
            {
                IsLoggerEnabled = false;
                this.literals = default!;
                this.state = default!;
                return;
            }

            var literals = literalPool;
            if (literals == null)
            {
                literals = literalPool = new List<string?>();
            }

            this.literalLength = literalLength;
            this.parametersLength = formattedCount;
            this.state = InterpolatedStringLogState.Create(formattedCount); // start to use from Pool.
            this.literals = literals;
            this.IsLoggerEnabled = true;
        }

        public static void PreAllocateEntry(int count)
        {
            var array = new InterpolatedStringLogState[count];
            var entries = new IZLoggerEntry[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = InterpolatedStringLogState.Create(0);
                entries[i] = array[i].CreateEntry(default);
            }

            foreach (var item in array)
            {
                item.Release();
            }

            foreach (var item in entries)
            {
                item.Return();
            }
        }

        public void AppendLiteral([System.Diagnostics.CodeAnalysis.ConstantExpected] string s)
        {
            literals.Add(s);
        }

        public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        {
            // Add for MessageSequence
            literals.Add(null);

            int offset;
            if (format == null)
            {
                // branch first
            }
            else if (format.StartsWith('@'))
            {
                var (altName, altFormat) = CustomFormatParser.GetOrAdd(format);
                argumentName = altName;
                format = altFormat;
                if (format == "json")
                {
                    offset = -2; // "json" is offset = -2
                    goto SKIP_MAGICALBOX_WRITE;
                }
            }
            else if (format == "json")
            {
                offset = -2;
                goto SKIP_MAGICALBOX_WRITE;
            }

            // Use MagicalBox(set value without boxing)
            if (!state.magicalBox.TryWrite(value, out offset))
            {
                offset = -1;
            }

        SKIP_MAGICALBOX_WRITE:
            var parameter = new InterpolatedStringParameter(typeof(T), argumentName ?? "", alignment, format, offset, (offset < 0) ? (object?)value : null);
            state.parameters[parameterWritten++] = parameter;
        }

        public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Nullable<T> value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
            where T : struct
        {
            // Nullable, check and unwrap here.
            if (value != null)
            {
                AppendFormatted<T>(value.Value, alignment, format, argumentName);
            }
            else
            {
                literals.Add(null);
                var parameter = new InterpolatedStringParameter(typeof(Nullable<T>), argumentName ?? "", alignment, format, -1, null);
                state.parameters[parameterWritten++] = parameter;
            }
        }

        public InterpolatedStringLogState GetState()
        {
            // MessageSequence is immutable
            state.messageSequence = MessageSequence.GetOrCreate(literalLength, parametersLength, literals);
            literals.Clear();

            return state;
        }
    }

    internal static class CustomFormatParser
    {
        static readonly ConcurrentDictionary<string, (string name, string? format)> alternateNameCache = new();

        // @name:format
        public static (string name, string? format) GetOrAdd(string format)
        {
            return alternateNameCache.GetOrAdd(format, static format =>
            {
                // parse alt format
                var moreFormat = format.IndexOf(':');
                if (moreFormat == -1)
                {
                    var altName = format.AsSpan(1, format.Length - 1).ToString();
                    alternateNameCache.TryAdd(format, (altName, null));
                    return (altName, null);
                }
                else
                {
                    var altName = format.AsSpan(1, moreFormat - 1).ToString();
                    var altFormat = format.AsSpan(moreFormat + 1, format.Length - moreFormat - 1).ToString();
                    return (altName, altFormat);
                }
            });
        }
    }

    // MessageSequence is immutable, can cache per same string format.
    internal sealed class MessageSequence
    {
        static readonly ConcurrentDictionary<LiteralList, MessageSequence> cache = new();

        // literals null represents parameter hole
        public static MessageSequence GetOrCreate(int literalLength, int parameterLength, List<string?> literals)
        {
            var key = new LiteralList(literals);
            if (cache.TryGetValue(key, out var sequence))
            {
                return sequence;
            }

            lock (cache)
            {
                if (cache.TryGetValue(key, out sequence))
                {
                    return sequence;
                }

                // create copy
                var clonedList = literals.ToList();
                clonedList.TrimExcess();
                var span = CollectionsMarshal.AsSpan(clonedList);
                sequence = new MessageSequence(literalLength, parameterLength, span);
                cache.TryAdd(new LiteralList(clonedList), sequence);
                return sequence;
            }
        }

        readonly int literalLength;
        readonly int parametersLength;
        readonly MessageSequenceSegment[] segments;

        public MessageSequence(int literalLength, int parametersLength, ReadOnlySpan<string?> literals)
        {
            this.literalLength = literalLength;
            this.parametersLength = parametersLength;
            this.segments = new MessageSequenceSegment[literals.Length];
            for (int i = 0; i < literals.Length; i++)
            {
                var str = literals[i];
                if (str == null)
                {
                    this.segments[i] = new MessageSequenceSegment(null, null!);
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(str);
                    this.segments[i] = new MessageSequenceSegment(str, bytes);
                }
            }
        }

        public void ToString(IBufferWriter<byte> writer, MagicalBox box, Span<InterpolatedStringParameter> parameters)
        {
            var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(literalLength, parametersLength, writer);

            var parameterIndex = 0;
            foreach (var item in segments)
            {
                if (item.IsLiteral)
                {
                    stringWriter.AppendUtf8(item.Utf8Bytes);
                }
                else
                {
                    ref var p = ref parameters[parameterIndex++];
                    if (p.BoxOffset == -2) // as "json"
                    {
                        CodeGeneratorUtil.AppendAsJson(ref stringWriter, p.BoxedValue, p.Type);
                    }
                    else if (!box.TryReadTo(p.Type, p.BoxOffset, p.Alignment, p.Format, ref stringWriter))
                    {
                        if (p.BoxedValue is string s)
                        {
                            stringWriter.AppendFormatted(s, p.Alignment, p.Format);
                        }
                        else if (p.BoxedValue is IEnumerable enumerable)
                        {
#if NET8_0
                            if (p.BoxedValue is ISpanFormattable spanFormattable)
                            {
                                stringWriter.AppendFormatted(p.BoxedValue, p.Alignment, p.Format);
                            }
                            else
                            {
                                CodeGeneratorUtil.AppendAsJson(ref stringWriter, p.BoxedValue, p.Type);
                            }
#else
                            CodeGeneratorUtil.AppendAsJson(ref stringWriter, p.BoxedValue, p.Type);
#endif
                        }
                        else
                        {
                            stringWriter.AppendFormatted(p.BoxedValue, p.Alignment, p.Format);
                        }
                    }
                }
            }
            stringWriter.Flush();
        }

        [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode")]
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode")]
        public string ToString(MagicalBox box, Span<InterpolatedStringParameter> parameters)
        {
            var stringHandler = new DefaultInterpolatedStringHandler(literalLength, parametersLength);

            var parameterIndex = 0;
            foreach (var item in segments)
            {
                if (item.IsLiteral)
                {
                    stringHandler.AppendLiteral(item.Literal);
                }
                else
                {
                    ref var p = ref parameters[parameterIndex++];
                    if (p.BoxOffset == -2) // as "json"
                    {
                        var jsonString = JsonSerializer.Serialize(p.BoxedValue, p.Type);
                        stringHandler.AppendLiteral(jsonString);
                    }
                    else if (!box.TryReadTo(p.Type, p.BoxOffset, p.Alignment, p.Format, ref stringHandler))
                    {
                        if (p.BoxedValue is string s)
                        {
                            stringHandler.AppendFormatted(s, p.Alignment, p.Format);
                        }
                        else if (p.BoxedValue is IEnumerable enumerable)
                        {
#if NET8_0
                            if (p.BoxedValue is IFormattable)
                            {
                                stringHandler.AppendFormatted(p.BoxedValue, p.Alignment, p.Format);
                            }
                            else
                            {
                                var jsonString = JsonSerializer.Serialize(p.BoxedValue, p.Type);
                                stringHandler.AppendLiteral(jsonString);
                            }
#else
                            var jsonString = JsonSerializer.Serialize(p.BoxedValue, p.Type);
                            stringHandler.AppendLiteral(jsonString);
#endif
                        }
                        else
                        {
                            stringHandler.AppendFormatted(p.BoxedValue, p.Alignment, p.Format);
                        }
                    }
                }
            }

            return stringHandler.ToStringAndClear();
        }

        public string GetOriginalFormat(Span<InterpolatedStringParameter> parameters)
        {
            var stringHandler = new DefaultInterpolatedStringHandler(literalLength, parametersLength);
            var parameterIndex = 0;
            foreach (var item in segments)
            {
                if (item.IsLiteral)
                {
                    stringHandler.AppendLiteral(item.Literal);
                }
                else
                {
                    ref var p = ref parameters[parameterIndex++];
                    stringHandler.AppendLiteral("{");
                    stringHandler.AppendLiteral(p.Name);

                    if (p.Alignment != 0)
                    {
                        stringHandler.AppendLiteral(":");
                        stringHandler.AppendFormatted(p.Alignment);
                    }
                    if (p.Format != null)
                    {
                        stringHandler.AppendLiteral(",");
                        stringHandler.AppendLiteral(p.Format);
                    }

                    stringHandler.AppendLiteral("}");
                }
            }

            return stringHandler.ToStringAndClear();
        }

        public void WriteOriginalFormat(IBufferWriter<byte> writer, Span<InterpolatedStringParameter> parameters)
        {
            var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(literalLength, parametersLength, writer);

            var parameterIndex = 0;
            foreach (var item in segments)
            {
                if (item.IsLiteral)
                {
                    stringWriter.AppendUtf8(item.Utf8Bytes);
                }
                else
                {
                    ref var p = ref parameters[parameterIndex++];
                    stringWriter.AppendLiteral("{");
                    stringWriter.AppendLiteral(p.Name);

                    if (p.Alignment != 0)
                    {
                        stringWriter.AppendLiteral(":");
                        stringWriter.AppendFormatted(p.Alignment);
                    }
                    if (p.Format != null)
                    {
                        stringWriter.AppendLiteral(",");
                        stringWriter.AppendLiteral(p.Format);
                    }

                    stringWriter.AppendLiteral("}");
                }
            }
            stringWriter.Flush();
        }

        public override string ToString()
        {
            // for debugging.
            var stringHandler = new DefaultInterpolatedStringHandler(literalLength, parametersLength);
            foreach (var item in segments)
            {
                if (item.IsLiteral)
                {
                    stringHandler.AppendLiteral(item.Literal);
                }
                else
                {
                    stringHandler.AppendLiteral("{}");
                }
            }

            return stringHandler.ToStringAndClear();
        }

        readonly struct LiteralList : IEquatable<LiteralList>
        {
            readonly List<string?> literals;

            public LiteralList(List<string?> literals)
            {
                this.literals = literals;
            }

#if NET8_0_OR_GREATER

            // literals are all const string, in .NET 8 it is allocated in Non-GC Heap so can compare by address.
            // https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-8/#non-gc-heap
            static ReadOnlySpan<byte> AsBytes(ReadOnlySpan<string?> literals)
            {
                return MemoryMarshal.CreateSpan(
                    ref Unsafe.As<string?, byte>(ref MemoryMarshal.GetReference(literals)),
                    literals.Length * Unsafe.SizeOf<string>());
            }

            public override int GetHashCode()
            {
                return unchecked((int)XxHash3.HashToUInt64(AsBytes(CollectionsMarshal.AsSpan(literals))));
            }

            public bool Equals(LiteralList other)
            {
                var xs = CollectionsMarshal.AsSpan(literals);
                var ys = CollectionsMarshal.AsSpan(other.literals);

                return AsBytes(xs).SequenceEqual(AsBytes(ys));
            }

#else

            [ThreadStatic]
            static XxHash3? xxhash;

            public override int GetHashCode()
            {
                var h = xxhash;
                if (h == null)
                {
                    h = xxhash = new XxHash3();
                }
                else
                {
                    h.Reset();
                }

                var span = CollectionsMarshal.AsSpan(literals);
                foreach (var item in span)
                {
                    h.Append(MemoryMarshal.AsBytes(item.AsSpan()));
                }

                // https://github.com/Cyan4973/xxHash/issues/453
                // XXH3 64bit -> 32bit, okay to simple cast answered by XXH3 author.
                return unchecked((int)h.GetCurrentHashAsUInt64());
            }

            public bool Equals(LiteralList other)
            {
                var xs = CollectionsMarshal.AsSpan(literals);
                var ys = CollectionsMarshal.AsSpan(other.literals);

                if (xs.Length == ys.Length)
                {
                    for (int i = 0; i < xs.Length; i++)
                    {
                        if (xs[i] != ys[i]) return false;
                    }
                    return true;
                }

                return false;
            }

#endif
        }
    }

    internal readonly struct MessageSequenceSegment(string? literal, byte[] utf8Bytes)
    {
        public bool IsLiteral => Literal != null;

        public readonly string Literal = literal!;
        public readonly byte[] Utf8Bytes = utf8Bytes;

        public override string ToString()
        {
            return Literal;
        }
    }

    internal readonly struct InterpolatedStringParameter(Type type, string name, int alignment, string? format, int boxOffset, object? boxedValue)
    {
        public readonly Type Type = type;
        public readonly string Name = name;
        public readonly int Alignment = alignment;
        public readonly string? Format = format;
        public readonly int BoxOffset = boxOffset;  // if -1, use boxed value
        public readonly object? BoxedValue = boxedValue;
    }
}
