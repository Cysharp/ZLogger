using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
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
        static byte[]? boxStoragePool;

        [ThreadStatic]
        static List<InterpolatedStringParameter>? parametersPool;

        [ThreadStatic]
        static List<string?>? literalPool;

        public bool IsLoggerEnabled { get; }

        // fields
        int literalLength;
        int parametersLength;
        List<string?> literals;
        MagicalBox box;
        List<InterpolatedStringParameter> parameters;

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
                this.parameters = default!;
                return;
            }

            var boxStorage = boxStoragePool;
            if (boxStorage == null)
            {
                boxStorage = boxStoragePool = new byte[2048];
            }

            var parameters = parametersPool;
            if (parameters == null)
            {
                parameters = parametersPool = new List<InterpolatedStringParameter>();
            }

            var literals = literalPool;
            if (literals == null)
            {
                literals = literalPool = new List<string?>();
            }

            this.literalLength = literalLength;
            this.parametersLength = formattedCount;
            this.box = new MagicalBox(boxStorage);
            this.parameters = parameters;
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
            if (!box.TryWrite(value, out var offset))
            {
                offset = -1;
            }

            var parameter = new InterpolatedStringParameter(typeof(T), argumentName ?? "", alignment, format, offset, (offset == -1) ? (object?)value : null);
            parameters.Add(parameter);
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
                parameters.Add(parameter);
            }
        }

        internal InterpolatedStringLogState GetStateAndClear()
        {
            // MessageSequence is immutable
            var sequence = MessageSequence.GetOrCreate(literalLength, parametersLength, literals);

            // MagicalBox and Parameters are cloned in ctor.
            var result = new InterpolatedStringLogState(sequence, box, CollectionsMarshal.AsSpan(parameters));

            // clear state
            literals.Clear();
            parameters.Clear();

            return result;
        }
    }

    // MessageSequence is immutable, can cache per same string format.
    internal sealed class MessageSequence
    {
        // TODO: use specialized impl dictionary?(for example, only check message length)
        static readonly ConcurrentDictionary<List<string?>, MessageSequence> cache = new(new MessageSequenceEqualityComparer());

        // literals null represents parameter hole
        public static MessageSequence GetOrCreate(int literalLength, int parameterLength, List<string?> literals)
        {
            if (cache.TryGetValue(literals, out var sequence))
            {
                return sequence;
            }

            // create copy
            var key = literals.ToList();
            sequence = new MessageSequence(literalLength, parameterLength, CollectionsMarshal.AsSpan(key));

            // if add failed, ok to use duplicate sequence.
            cache.TryAdd(key, sequence);
            return sequence;
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

            var parameterIndex = 0; foreach (var item in segments)
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
                        stringWriter.AppendFormatted(p.BoxedValue, p.Alignment, p.Format);
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
                        stringHandler.AppendFormatted(p.BoxedValue, p.Alignment, p.Format);
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

        internal sealed class MessageSequenceEqualityComparer : IEqualityComparer<List<string?>>
        {
            public bool Equals(List<string?>? x, List<string?>? y)
            {
                if (x == null && y == null) return true;
                if (x == null) return false;
                if (y == null) return false;
                if (x.Count != y.Count) return false;

                var xs = CollectionsMarshal.AsSpan(x);
                var ys = CollectionsMarshal.AsSpan(y);

                for (int i = 0; i < xs.Length; i++)
                {
                    if (xs[i] != ys[i]) return false;
                }

                return true;
            }

            public int GetHashCode([DisallowNull] List<string?>? obj)
            {
                if (obj == null) return 0;

                var hashCode = new HashCode();

                var span = CollectionsMarshal.AsSpan(obj);
                foreach (var item in span)
                {
                    if (item != null)
                    {
                        hashCode.AddBytes(MemoryMarshal.AsBytes(item.AsSpan()));
                    }
                }

                return hashCode.ToHashCode();
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

        public ReadOnlySpan<char> ParseKeyName()
        {
            return CallerArgumentExpressionParser.GetParameterizedName(Name);
        }

        public void WriteJsonKeyName(Utf8JsonWriter jsonWriter, IKeyNameMutator? mutator = null)
        {
            var keyName = ParseKeyName();
            if (mutator != null)
            {
                Span<char> buffer = stackalloc char[keyName.Length * 2]; 
                if (mutator.TryMutate(keyName, buffer, out var written))
                {
                    jsonWriter.WritePropertyName(buffer[..written]);
                    return;
                }
            }
            jsonWriter.WritePropertyName(keyName);
        }
    }
}
