#nullable enable

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmark.InternalBenchmarks
{
    file class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1));
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class StringHashingSpeed
    {
        ReadOnlySpan<byte> source => "An error occurred running an IConnectionCompleteFeature.OnCompleted callback."u8;

        [Benchmark]
        public int System_HashCode()
        {
            var hashCode = new HashCode();
            hashCode.AddBytes(source);
            return hashCode.ToHashCode();
        }

        [Benchmark]
        public int XxHash32()
        {
            return unchecked((int)System.IO.Hashing.XxHash32.HashToUInt32(source));
        }

        [Benchmark]
        public int XxHash3()
        {
            // https://github.com/Cyan4973/xxHash/issues/453
            // XXH3 64bit -> 32bit, okay to simple cast answered by XXH3 author.
            return unchecked((int)System.IO.Hashing.XxHash3.HashToUInt64(source));
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class PtrArrayHashingSpeed
    {
        byte[] source;
        List<string?> original;
        XxHash3 xxh3;

        public PtrArrayHashingSpeed()
        {
            // var literals = GetLiterals($"An error occurred {100} running an {10} ICon{20} nectio {39} nCompleteFeature.OnCompleted callba{3}ck{3}.");
            var literals = GetLiterals($"x={10}y={20}z={30}");
            original = literals.ToList();
            this.source = AsBytes(CollectionsMarshal.AsSpan(literals)).ToArray();
            this.xxh3 = new XxHash3();
        }

        static List<string?> GetLiterals(ForLiteralMatchStringHandler handler)
        {
            return handler.Literals;
        }

        static ReadOnlySpan<byte> AsBytes(ReadOnlySpan<string?> literals)
        {
            return MemoryMarshal.CreateSpan(
                ref Unsafe.As<string?, byte>(ref MemoryMarshal.GetReference(literals)),
                literals.Length * Unsafe.SizeOf<string>());
        }

        [Benchmark]
        public int System_HashCode()
        {
            var hashCode = new HashCode();
            hashCode.AddBytes(source);
            return hashCode.ToHashCode();
        }

        [Benchmark]
        public int XxHash32()
        {
            return unchecked((int)System.IO.Hashing.XxHash32.HashToUInt32(source));
        }

        [Benchmark]
        public int XxHash3()
        {
            return unchecked((int)System.IO.Hashing.XxHash3.HashToUInt64(source));
        }

        [Benchmark]
        public int System_HashCode_OriginalBuild()
        {
            var hashCode = new HashCode();

            var span = CollectionsMarshal.AsSpan(original);
            foreach (var item in span)
            {
                if (item != null)
                {
                    hashCode.AddBytes(MemoryMarshal.AsBytes(item.AsSpan()));
                }
            }

            return hashCode.ToHashCode();
        }

        [Benchmark]
        public int XxHash3_OriginalBuild()
        {
            var hashCode = xxh3;
            hashCode.Reset();

            var span = CollectionsMarshal.AsSpan(original);
            foreach (var item in span)
            {
                if (item != null)
                {
                    hashCode.Append(MemoryMarshal.AsBytes(item.AsSpan()));
                }
            }

            return unchecked((int)hashCode.GetCurrentHashAsUInt64());
        }
    }
}
