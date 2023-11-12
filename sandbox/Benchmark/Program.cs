using System.Reflection;
using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;


#if !DEBUG

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args);
//BenchmarkRunner.Run<WriteJsonToFile>(args: args);

#else

var bench= new WriteJsonToFile();
bench.SetUpLogger();

bench.ZLogger_JsonFile();

bench.Cleanup();


#endif


