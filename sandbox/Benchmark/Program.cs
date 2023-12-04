using System;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Benchmark.Benchmarks;
using Benchmark.InternalBenchmarks;
using BenchmarkDotNet.Running;
using ZLogger;


#if !DEBUG

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args);

//BenchmarkRunner.Run<WritePlainTextToFile>(args: args);
//BenchmarkRunner.Run<WriteJsonToFile>(args: args);
//BenchmarkRunner.Run<WriteJsonToFile>(args: args);


//return;
//var bench = new WriteJsonToFile();
//bench.SetUpLogger();
//ZLoggerInterpolatedStringHandler.PreAllocateEntry(100_000);



//Thread.Sleep(5000);

//bench.ZLogger_JsonFile();

//Thread.Sleep(5000);

//bench.Cleanup();

//var bench = new EmptyLogging();
//bench.SetUpDirectory();
//bench.SetUpLogger();

//Thread.Sleep(5000);


//bench.ZLogEmpty();


//Thread.Sleep(5000);

//bench.CleanUpLogger();

#else
{
    var bench = new WriteJsonToFile();
    bench.SetUpLogger();
    //ZLoggerInterpolatedStringHandler.PreAllocateEntry(100_000);


    bench.NLog_JsonFile();
    //bench.ZLogger_JsonFile();


    //Thread.Sleep(5000);

    //bench.ZLogger_JsonFile();

    //Thread.Sleep(5000);

    //bench.Cleanup();

    //return;
}
//var bench = new WriteJsonToFile();
//bench.SetUpLogger();
//ZLoggerInterpolatedStringHandler.PreAllocateEntry(100_000);
//{

//    var bench = new WriteJsonToConsole();

//    bench.SetUpLogger();
//    bench.ZLogger_JsonConsole();
//    bench.Cleanup();

//    bench.SetUpLogger();
//    bench.ZLogger_SourceGenerator_JsonConsole();
//    bench.Cleanup();



//    bench.SetUpLogger();
//    bench.NLog_JsonConsole();
//    bench.Cleanup();

//    bench.SetUpLogger();
//    bench.NLog_MsExt_JsonConsole();
//    bench.Cleanup();

//    bench.SetUpLogger();
//    bench.NLog_MsExt_SourceGenerator_JsonConsole();
//    bench.Cleanup();


//    bench.SetUpLogger();
//    bench.Serilog_JsonConsole();
//    bench.Cleanup();

//    bench.SetUpLogger();
//    bench.Serilog_MsExt_JsonConsole();
//    bench.Cleanup();

//    bench.SetUpLogger();
//    bench.Serilog_MsExt_SourceGenerator_JsonConsole();
//    bench.Cleanup();

//    bench.Cleanup();

//    Console.WriteLine("END");
//}






#endif
