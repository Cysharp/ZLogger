using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;
using ZLogger;


#if !DEBUG

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args);
//BenchmarkRunner.Run<WriteJsonToFile>(args: args);

#else

var bench = new PostLogEntry();
bench.SetUp();

bench.ZLogger_ZLog();
Thread.Sleep(1000);
bench.ZLogger_ZLog();

bench.NLog_Log();

//var bench = new WriteJsonToFile();

//bench.SetUpLogger();
//bench.ZLogger_JsonFile();

//bench.Cleanup();

//bench.NLog_PlainTextConsole();
//bench.Serilog_PlainTextConsole();
//bench.MsExtConsole_PlainTextConsole();

// ZLOG(DtOffset)     2023/11/14 8:23:52 +00:00 [Information]x=100 y=200 z=300
// NLOG(longdate)     2023-11-14 17:17:21.3210 [Info] x=100 y=200 z=300
// Serilog(Timestamp) 11/14/2023 17:18:46 +09:00 [Information] x=100 y=200 z=300
// MS.Ext(Custom)     2023/11/14 17:20:27 [Information] x=100 y=200 z=300


#endif
