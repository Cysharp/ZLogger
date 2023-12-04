#nullable enable

using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmark.InternalBenchmarks
{
    public class CompareLiteralsMatch
    {
        readonly ConcurrentDictionary<List<string?>, object> dictListEqualityComparer = new(new MessageSequenceEqualityComparer());
        readonly ConcurrentDictionary<LiteralList, object> dictLiteralList = new();

        List<string?> key;

        public CompareLiteralsMatch()
        {
            key = GetLiterals($"x={10}y={20}z={30}");
            dictListEqualityComparer[key] = new object();
            dictLiteralList[new LiteralList(key.ToList())] = new object();
        }

        [Benchmark]
        public bool GetListEqualityComparer()
        {
            return dictListEqualityComparer.TryGetValue(key, out _);
        }

        [Benchmark]
        public bool GetLiteralList()
        {
            return dictLiteralList.TryGetValue(new LiteralList(key), out _);
        }

        List<string?> GetLiterals(ForLiteralMatchStringHandler handler)
        {
            return handler.Literals;
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

        [StructLayout(LayoutKind.Auto)]
        readonly struct LiteralList : IEquatable<LiteralList>
        {
            readonly List<string?> literals;
            readonly int hashCode;

            public LiteralList(List<string?> literals)
            {
                this.literals = literals;
                this.hashCode = BuildHashCode(AsBytes(CollectionsMarshal.AsSpan(literals)));
            }

            public override int GetHashCode()
            {
                return hashCode;
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

            static int BuildHashCode(ReadOnlySpan<byte> source)
            {
                // https://github.com/Cyan4973/xxHash/issues/453
                // XXH3 64bit -> 32bit, okay to simple cast answered by XXH3 author.
                return unchecked((int)System.IO.Hashing.XxHash3.HashToUInt64(source));
            }
        }
    }

    [InterpolatedStringHandler]
    internal class ForLiteralMatchStringHandler
    {
        public List<string?> Literals;

        public ForLiteralMatchStringHandler(int literalLength, int formattedCount)
        {
            Literals = new();
        }

        public void AppendLiteral(string literal)
        {
            Literals.Add(literal);
        }

        public void AppendFormatted<T>(T value)
        {
            Literals.Add(null);
        }
    }
}
