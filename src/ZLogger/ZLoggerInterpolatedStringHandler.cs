using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Collections;
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
        public bool IsLoggerEnabled { get; }

        // fields
        readonly int literalLength;
        readonly int parametersLength;
        readonly string?[] literals;
        readonly InterpolatedStringParameter[] parameters;
        MagicalBox box;

        int currentLiteralIndex;
        int currentParameterIndex;

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

            this.literalLength = literalLength;
            this.parametersLength = formattedCount;
            this.box = new MagicalBox();
            this.parameters = ArrayPool<InterpolatedStringParameter>.Shared.Rent(formattedCount);
            this.literals = ArrayPool<string?>.Shared.Rent(32);
            this.IsLoggerEnabled = true;
        }

        public void AppendLiteral(string s)
        {
            literals[currentLiteralIndex++] = s;
        }

        public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        {
            // Add for MessageSequence
            literals[currentLiteralIndex++] = null;

            // Use MagicalBox(set value without boxing)
            if (!box.TryWrite(value, out var offset))
            {
                offset = -1;
            }

            parameters[currentParameterIndex++] = new InterpolatedStringParameter(typeof(T), argumentName ?? "", alignment, format, offset, (offset == -1) ? (object?)value : null);
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
                literals[currentLiteralIndex++] = null;
                parameters[currentParameterIndex++] = new InterpolatedStringParameter(typeof(Nullable<T>), argumentName ?? "", alignment, format, -1, null);
            }
        }

        public void AppendFormatted<T>((string, T) namedValue, int alignment = 0, string? format = null, string? _ = null)
        {
            AppendFormatted(namedValue.Item2, alignment, format, namedValue.Item1);
        }

        internal InterpolatedStringLogState GetStateAndClear()
        {
            // MessageSequence is immutable
            var sequence = new MessageSequence(literalLength, parametersLength, new ArraySegment<string?>(literals, 0, currentLiteralIndex));

            // MagicalBox and Parameters are cloned in ctor.
            var result = InterpolatedStringLogState.Create(sequence, box, new ArraySegment<InterpolatedStringParameter>(parameters, 0, parametersLength));

            return result;
        }
    }

    // MessageSequence is immutable, can cache per same string format.
    internal struct MessageSequence : IDisposable
    {
        // // TODO: use specialized impl dictionary?(for example, only check message length)
        // static readonly ConcurrentDictionary<ArraySegment<string?>, MessageSequence> cache = new(new MessageSequenceEqualityComparer());
        //
        // // literals null represents parameter hole
        // public static MessageSequence GetOrCreate(int literalLength, int parameterLength, ArraySegment<string?> literals)
        // {
        //     if (cache.TryGetValue(literals, out var sequence))
        //     {
        //         return sequence;
        //     }
        //
        //     sequence = new MessageSequence(literalLength, parameterLength, literals);
        //
        //     // if add failed, ok to use duplicate sequence.
        //     cache.TryAdd(literals, sequence);
        //     return sequence;
        // }

        static readonly ConcurrentDictionary<string, byte[]> utf8LiteralCache = new();

        readonly int literalLength;
        readonly int parametersLength;
        readonly ArraySegment<string?> literals;

        public MessageSequence(int literalLength, int parametersLength, ArraySegment<string?> literals)
        {
            this.literalLength = literalLength;
            this.parametersLength = parametersLength;
            this.literals = literals;
        }

        public void Dispose()
        {
            ArrayPool<string?>.Shared.Return(literals.Array!);
        }

        public void ToString(IBufferWriter<byte> writer, MagicalBox box, Span<InterpolatedStringParameter> parameters)
        {
            var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(literalLength, parametersLength, writer);

            var parameterIndex = 0; foreach (var literal in literals)
            {
                if (literal != null)
                {
                    var utf8Bytes = utf8LiteralCache.GetOrAdd(literal, s => Encoding.UTF8.GetBytes(s));
                    stringWriter.AppendUtf8(utf8Bytes);
                }
                else
                {
                    ref var p = ref parameters[parameterIndex++];
                    if (!box.TryReadTo(p.Type, p.BoxOffset, p.Alignment, p.Format, ref stringWriter))
                    {
                        if (p.BoxedValue is IEnumerable enumerable)
                        {
                            var jsonWriter = new Utf8JsonWriter(writer);
                            JsonSerializer.Serialize(jsonWriter, enumerable);                            
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
            foreach (var literal in literals)
            {
                if (literal != null)
                {
                    stringHandler.AppendLiteral(literal);
                }
                else
                {
                    ref var p = ref parameters[parameterIndex++];
                    if (!box.TryReadTo(p.Type, p.BoxOffset, p.Alignment, p.Format, ref stringHandler))
                    {
                        if (p.BoxedValue is IEnumerable enumerable)
                        {
                            var jsonString = JsonSerializer.Serialize(enumerable);
                            stringHandler.AppendFormatted(jsonString);
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

            foreach (var literal in literals)
            {
                if (literal != null)
                {
                    stringHandler.AppendLiteral(literal);
                }
                else
                {
                    stringHandler.AppendLiteral("{}");
                }
            }

            return stringHandler.ToStringAndClear();
        }

        // internal sealed class MessageSequenceEqualityComparer : IEqualityComparer<List<string?>>
        // {
        //     public bool Equals(List<string?>? x, List<string?>? y)
        //     {
        //         if (x == null && y == null) return true;
        //         if (x == null) return false;
        //         if (y == null) return false;
        //         if (x.Count != y.Count) return false;
        //
        //         var xs = CollectionsMarshal.AsSpan(x);
        //         var ys = CollectionsMarshal.AsSpan(y);
        //
        //         for (int i = 0; i < xs.Length; i++)
        //         {
        //             if (xs[i] != ys[i]) return false;
        //         }
        //
        //         return true;
        //     }
        //
        //     public int GetHashCode([DisallowNull] List<string?>? obj)
        //     {
        //         if (obj == null) return 0;
        //
        //         var hashCode = new HashCode();
        //
        //         var span = CollectionsMarshal.AsSpan(obj);
        //         foreach (var item in span)
        //         {
        //             if (item != null)
        //             {
        //                 hashCode.AddBytes(MemoryMarshal.AsBytes(item.AsSpan()));
        //             }
        //         }
        //
        //         return hashCode.ToHashCode();
        //     }
        // }
    }

    // internal readonly struct MessageSequenceSegment
    // {
    //     public bool IsLiteral => Literal != null;
    //
    //     public readonly string Literal;
    //     public readonly byte[] Utf8Bytes;
    //
    //     public MessageSequenceSegment(string? literal, byte[] utf8Bytes)
    //     {
    //         this.Literal = literal!;
    //         this.Utf8Bytes = utf8Bytes;
    //     }
    //
    //     public override string ToString()
    //     {
    //         return Literal;
    //     }
    // }

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
    }
}
