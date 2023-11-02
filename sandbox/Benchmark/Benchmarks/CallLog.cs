using System.IO;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Serilog;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Benchmark.Benchmarks;

public class CallLog
{
    ILogger zLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger nLogMsExtLogger = default!;

    static string NullDevicePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "NUL" : "/dev/null";
    
    [GlobalSetup]
    public void SetUp()
    {
        // ZLogger
        
        var zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerStream(Stream.Null);
        });

        zLogger = zLoggerFactory.CreateLogger<CallLog>();
        
        // Serilog
        
        var serilogLogger = new LoggerConfiguration()
            .WriteTo.Async(a => a.TextWriter(TextWriter.Null))
            .CreateLogger();
        
        var serilogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddSerilog(serilogLogger);
        });
        
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<CallLog>();
        
        // NLog

        var nLogConfig = new NLog.Config.LoggingConfiguration();
        var target = new NLog.Targets.NullTarget("Null");
        var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target);
        nLogConfig.AddTarget(asyncTarget);
        nLogConfig.AddRuleForAllLevels(asyncTarget);

        var nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddNLog(nLogConfig);
        });

        nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<CallLog>();
    }

    [Benchmark]
    public void ZLogger_ZLog()
    {
        var x = 10;
        var y = 20;
        var z = 30;
        zLogger.ZLogInformation($"foo{x} bar{y} nazo{z}");
    }

    [Benchmark]
    public void Serilog_Log()
    {
        serilogMsExtLogger.LogInformation($"foo{100} bar{200} nazo{300}");
    }

    [Benchmark]
    public void NLog_Log()
    {
        nLogMsExtLogger.LogInformation($"foo{100} bar{200} nazo{300}");
    }
}
