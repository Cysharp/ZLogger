using Benchmark.Benchmarks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Serilog;

namespace Benchmark
{
    // internal class BenchmarkConfig : ManualConfig
    // {
    //     public BenchmarkConfig()
    //     {
    //         Add(MemoryDiagnoser.Default);
    //         Add(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1));
    //     }
    // }

    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()).Run(args);
            
            // var serilogLogger = new LoggerConfiguration()
            //     // .WriteTo.Async(a => a.File("/dev/null", buffered: true))
            //     .WriteTo.Async(a => a.Console())
            //     .CreateLogger();
            //
            // var serilogMsExtLoggerFactory = LoggerFactory.Create(logging =>
            // {
            //     logging.AddSerilog(serilogLogger);
            // });
            //
            // var serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<CallLog>();
            // serilogMsExtLogger.LogInformation($"SESESESSESEEE");
            //
            // var nLogConfig = new NLog.Config.LoggingConfiguration();
            // var target = new NLog.Targets.ConsoleTarget("Null");
            // var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target);
            // nLogConfig.AddTarget(asyncTarget);
            // nLogConfig.AddRuleForAllLevels(asyncTarget);
            //
            // var nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
            // {
            //     logging.AddNLog(nLogConfig);
            // });
            //
            // var nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<CallLog>();
            //
            // nLogMsExtLogger.LogInformation($"NNNNNNNNNNNN");
            //
            // nLogMsExtLoggerFactory.Dispose();
        }
    }
}
