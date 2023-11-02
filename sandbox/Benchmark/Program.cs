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
//     FileName = "/dev/null"
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
// nLogMsExtLogger.LogInformation("{X}", 1);