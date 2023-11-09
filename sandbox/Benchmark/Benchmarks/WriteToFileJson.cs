using ZLogger;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using NLog.Extensions.Logging;
using Serilog.Formatting.Json;
using ZLogger.Formatters;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Benchmark.Benchmarks;

file class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1));
    }
}

[Config(typeof(BenchmarkConfig))]
public class WriteToFileJson
{
    const int N = 10_000;
    static readonly string NullDevicePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "NUL" : "/dev/null";

    readonly ILoggerFactory zloggerFactory;
    readonly ILogger zlogger;
    
    readonly Serilog.Core.Logger serilogLogger;
    readonly ILoggerFactory serilogMsExtLoggerFactory;
    readonly ILogger serilogMsExtLogger;

    readonly NLog.Logger nLogLogger;
    readonly ILoggerFactory nLogMsExtLoggerFactory;
    readonly ILogger nLogMsExtLogger;
    
    public WriteToFileJson()
    {
        var zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerFile(NullDevicePath, options =>
            {
                options.UseJsonFormatter();
            });
        });

        zlogger = zLoggerFactory.CreateLogger<WriteToFileJson>();
        
        // Serilog
        
        serilogLogger = new LoggerConfiguration()
            .WriteTo.Async(a => a.File(new JsonFormatter(), NullDevicePath, buffered: true))
            .WriteTo.File(NullDevicePath, buffered: true)
            .CreateLogger();
        
        serilogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddSerilog(serilogLogger);
        });
        
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WriteToFileJson>();
        
        // NLog

        var nLogConfig = new NLog.Config.LoggingConfiguration();
        var target = new NLog.Targets.FileTarget("File")
        {
            FileName = NullDevicePath,
            BufferSize = 32768 // default 
        };
        // var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target);
        nLogConfig.AddTarget(target);
        nLogConfig.AddRuleForAllLevels(target);
        
        NLog.LogManager.Configuration = nLogConfig;
        nLogLogger = NLog.LogManager.GetCurrentClassLogger();

        nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddNLog(nLogConfig);
        });

        nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WriteToFileJson>();
    }

    [Benchmark]
    public void ZLogger_WriteJsonToFile()
    {
        var x = 100;
        var y = 200;
        var z = 300;
        for (var i = 0; i < N; i++)
        {
            zlogger.LogInformation($"x={x} y={y} z={z}");
        }
        zloggerFactory.Dispose(); // wait
    }

    [Benchmark]
    public void SerilogMsExt_WriteJsonToFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }

        serilogLogger.Dispose(); // wait
        serilogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void NLogMsExt_WriteJsonToFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogMsExtLoggerFactory.Dispose(); // wait
    }
}
