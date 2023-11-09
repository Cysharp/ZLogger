using System.Reflection;
using Benchmark.Benchmarks;
using Benchmark.IOBenchmarks;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using ZLogger;
using ZLogger.Formatters;

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args);

// var nLogConfig = new NLog.Config.LoggingConfiguration();
// var target = new NLog.Targets.FileTarget("Null")
// {
//     FileName = "/Users/s24061/tmp/log"
// };
// var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target);
// nLogConfig.AddTarget(asyncTarget);
// nLogConfig.AddRuleForAllLevels(asyncTarget);
//
// var nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
// {
//     logging.AddNLog(nLogConfig);
// });
//
// var nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<PostLogEntry>();
//
// Serilog
        
// var serilogLogger = new LoggerConfiguration()
//     .WriteTo.Async(a => a.File("/Users/s24061/tmp/log"))
//     .CreateLogger();
//         
// var serilogMsExtLoggerFactory = LoggerFactory.Create(logging =>
// {
//     logging.AddSerilog(serilogLogger);
// });
//         
// var serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<PostLogEntry>();
//
// for (var i = 0; i < 10000; i++)
// {
//     serilogMsExtLogger.LogInformation("{X}", i);
// }
//
// serilogLogger.Dispose();
// serilogMsExtLoggerFactory.Dispose();