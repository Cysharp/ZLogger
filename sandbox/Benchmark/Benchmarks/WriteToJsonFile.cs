using System;
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

namespace Benchmark.IOBenchmarks;

class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1));
    }
}

[Config(typeof(BenchmarkConfig))]
public class WriteJsonToFile
{
    const int N = 10_000;
    static readonly string NullDevicePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "NUL" : "/dev/null";

    readonly ILoggerFactory zloggerFactory;
    readonly ILoggerFactory serilogMsExtLoggerFactory;
    readonly ILoggerFactory nLogMsExtLoggerFactory;
    readonly ILogger zlogger;
    readonly ILogger serilogMsExtLogger;
    readonly ILogger nLogMsExtLogger;
    
    public WriteJsonToFile()
    {
        var zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerFile(NullDevicePath, options =>
            {
                options.UseJsonFormatter();
            });
        });

        zlogger = zLoggerFactory.CreateLogger<WriteJsonToFile>();
        
        // Serilog
        
        var serilogLogger = new LoggerConfiguration()
            // .WriteTo.Async(a => a.File(new JsonFormatter(), NullDevicePath, buffered: true))
            .WriteTo.File(NullDevicePath, buffered: true)
            .CreateLogger();
        
        serilogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddSerilog(serilogLogger);
        });
        
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WriteJsonToFile>();
        
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

        nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddNLog(nLogConfig);
        });

        nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WriteJsonToFile>();
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
    public void Serilog_WriteJsonToFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogMsExtLoggerFactory.Dispose(); // wait
    }

    [Benchmark]
    public void NLog_WriteJsonToFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogMsExtLoggerFactory.Dispose(); // wait
    }
}
