using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
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

        public void AppendLiteral(string s)
        {
            literals.Add(s);
        }

        public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        {
            // Add for MessageSequence
            literals.Add(null);

            // Use MagicalBox(set value without boxing)
            if (!state.magicalBox.TryWrite(value, out var offset))
            {
                offset = -1;
            }

            var parameter = new InterpolatedStringParameter(typeof(T), argumentName ?? "", alignment, format, offset, (offset == -1) ? (object?)value : null);
            state.parameters[parameterWritten++] = parameter;
        }

        public void AppendFormatted<T>(Nullable<T> value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
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

        public void AppendFormatted<T>((string, T) namedValue, int alignment = 0, string? format = null, string? _ = null)
        {
            AppendFormatted(namedValue.Item2, alignment, format, namedValue.Item1);
        }

        internal InterpolatedStringLogState GetState()
        {
            // MessageSequence is immutable
            state.messageSequence = MessageSequence.GetOrCreate(literalLength, parametersLength, literals);
            literals.Clear();

            return state;
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
                    if (!box.TryReadTo(p.Type, p.BoxOffset, p.Alignment, p.Format, ref stringWriter))
                    {
                        if (p.BoxedValue is string s)
                        {
                            stringWriter.AppendFormatted(s, p.Alignment, p.Format);
                        }
                        else if (p.BoxedValue is IEnumerable enumerable)
                        {
                            // require Flush before use Utf8JsonWriter
                            stringWriter.Dispose();
                            using var jsonWriter = new Utf8JsonWriter(writer);
                            JsonSerializer.Serialize(jsonWriter, enumerable);
                            jsonWriter.Flush();

                            // require re-create after use Utf8JsonWriter for control internal buffer
                            stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(literalLength, parametersLength, writer);
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
                    if (!box.TryReadTo(p.Type, p.BoxOffset, p.Alignment, p.Format, ref stringHandler))
                    {
                        if (p.BoxedValue is string s)
                        {
                            stringHandler.AppendFormatted(s, p.Alignment, p.Format);
                        }
                        else if (p.BoxedValue is IEnumerable enumerable)
                        {
                            var jsonString = JsonSerializer.Serialize(enumerable);
                            stringHandler.AppendLiteral(jsonString);
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

            public override int GetHashCode()
            {
                var span = CollectionsMarshal.AsSpan(literals);
                // https://github.com/Cyan4973/xxHash/issues/453
                // XXH3 64bit -> 32bit, okay to simple cast answered by XXH3 author.
                var source = AsBytes(span);
                return unchecked((int)System.IO.Hashing.XxHash3.HashToUInt64(source));
            }

            public bool Equals(LiteralList other)
            {
                var xs = CollectionsMarshal.AsSpan(literals);
                var ys = CollectionsMarshal.AsSpan(other.literals);

                return AsBytes(xs).SequenceEqual(AsBytes(ys));
            }

            // convert const strings as address sequence
            static ReadOnlySpan<byte> AsBytes(ReadOnlySpan<string?> literals)
            {
                return MemoryMarshal.CreateSpan(
                    ref Unsafe.As<string?, byte>(ref MemoryMarshal.GetReference(literals)),
                    literals.Length * Unsafe.SizeOf<string>());
            }
        }
    }

    internal readonly struct MessageSequenceSegment
    {
        public bool IsLiteral => Literal != null;

        public readonly string Literal;
        public readonly byte[] Utf8Bytes;

        public MessageSequenceSegment(string? literal, byte[] utf8Bytes)
        {
            this.Literal = literal!;
            this.Utf8Bytes = utf8Bytes;
        }

        public override string ToString()
        {
            return Literal;
        }
    }

    internal readonly struct InterpolatedStringParameter
    {
        public readonly Type Type;
        public readonly string Name;
        public readonly int Alignment;
        public readonly string? Format;
        public readonly int BoxOffset;  // if -1, use boxed value
        public readonly object? BoxedValue;

        public InterpolatedStringParameter(Type type, string name, int alignment, string? format, int boxOffset, object? boxedValue)
        {
            Type = type;
            Name = name;
            Alignment = alignment;
            Format = format;
            BoxOffset = boxOffset;
            BoxedValue = boxedValue;
        }
    }
}
