using System.IO;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Serilog;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Benchmark.Benchmarks;

[MemoryDiagnoser]
public class PostLogEntry
{
    static readonly string NullDevicePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "NUL" : "/dev/null";
    
    ILogger zLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger nLogMsExtLogger = default!;

    [GlobalSetup]
    public void SetUp()
    {
        // ZLogger
        
        var zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerStream(Stream.Null);
        });

        zLogger = zLoggerFactory.CreateLogger<PostLogEntry>();
        
        // Serilog
        
        var serilogLogger = new LoggerConfiguration()
            .WriteTo.Async(a => a.TextWriter(TextWriter.Null))
            .CreateLogger();
        
        var serilogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddSerilog(serilogLogger);
        });
        
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<PostLogEntry>();
        
        // NLog

        var nLogConfig = new NLog.Config.LoggingConfiguration();
        var target = new NLog.Targets.FileTarget("Null")
        {
            FileName = NullDevicePath
        };
        var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target);
        nLogConfig.AddTarget(asyncTarget);
        nLogConfig.AddRuleForAllLevels(asyncTarget);

        var nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddNLog(nLogConfig);
        });

        nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<PostLogEntry>();
    }

    [Benchmark]
    public void ZLogger_ZLog()
    {
        var x = 100;
        var y = 200;
        var z = 300;
        zLogger.ZLogInformation($"foo{x} bar{y} nazo{z}");
    }

    [Benchmark]
    public void Serilog_Log()
    {
        serilogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
    }

    [Benchmark]
    public void NLog_Log()
    {
        nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
    }
}
