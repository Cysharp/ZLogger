using Benchmark.Benchmarks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Reflection;

namespace Benchmark
{
    internal class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(MemoryDiagnoser.Default);
            Add(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1));
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()).Run(args);

            //new WriteToFile().ZFile();

            //SimpleBench.Run();
        }
    }
}
