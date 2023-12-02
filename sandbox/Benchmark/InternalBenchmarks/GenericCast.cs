using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.InternalBenchmarks
{
    file class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1).WithToolchain(InProcessNoEmitToolchain.Instance));

        }
    }


    [Config(typeof(BenchmarkConfig))]
    public class GenericCast
    {
        int value;

        public GenericCast()
        {
            value = int.Parse("100");
        }

        [Benchmark]
        public int DirectCall()
        {
            return new StructFoo(value).Foo();
        }

        [Benchmark]
        public int GenericsWhere()
        {
            return CallConstrained(new StructFoo(value));
        }

        [Benchmark]
        public int GenericsCast()
        {
            return CallCast(new StructFoo(value));
        }

        [Benchmark]
        public int GenericsBoxing()
        {
            return CallCastBoxing(new StructFoo(value));
        }



        static int CallConstrained<T>(T t) where T : IFoo
        {
            return t.Foo();
        }

        static int CallCast<T>(T t)
        {
            if (t is IFoo)
            {
                return ((IFoo)t).Foo();
            }
            else
            {
                return -1;
            }
        }

        static int CallCastBoxing<T>(T t)
        {
            if (t is IFoo f)
            {
                return f.Foo();
            }
            else
            {
                return -1;
            }
        }
    }

    public interface IFoo
    {
        int Foo();
    }

    public struct StructFoo : IFoo
    {
        int i;
        public StructFoo(int i)
        {
            this.i = i;  
        }

        public int Foo()
        {
            return i;
        }
    }
}
