using System.Reflection;
using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using Utf8StringInterpolation;
using ZLogger;

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args);

// var nLogConfig = new NLog.Config.LoggingConfiguration();
// var target = new NLog.Targets.FileTarget("File")
// {
//     FileName = "/Users/s24061/tmp/nlog.log",
//     Layout = new NLog.Layouts.SimpleLayout("${longdate} [${level}] ${message}")
// };
// var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target);
// nLogConfig.AddTarget(asyncTarget);
// nLogConfig.AddRuleForAllLevels(asyncTarget);
//
// NLog.LogManager.Configuration = nLogConfig;
// var nLogLogger = NLog.LogManager.LogFactory.GetLogger("NLog");
//
// for (var i = 0; i < 100; i++)
// {
//     nLogLogger.Info("i={i}", i);
// }
//
// NLog.LogManager.Shutdown();