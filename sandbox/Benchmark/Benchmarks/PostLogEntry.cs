using System.IO;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets.Wrappers;
using Serilog;
using Utf8StringInterpolation;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Benchmark.Benchmarks;

// file class MsExtConsoleLoggerFormatter : ConsoleFormatter
// {
//     public class Options : ConsoleFormatterOptions
//     {
//     }
//
//     public MsExtConsoleLoggerFormatter() : base("Benchmark")
//     {
//     }
//
//     public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
//     {
//         var message = logEntry.Formatter.Invoke(logEntry.State, logEntry.Exception);
//         var timestamp = DateTime.UtcNow;
//         textWriter.Write(timestamp);
//         textWriter.Write(" [");
//         textWriter.Write(logEntry.LogLevel);
//         textWriter.Write("] ");
//         textWriter.WriteLine(message);
//     }
// }

file class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.ShortRun);
    }
}

[Config(typeof(BenchmarkConfig))]
public class PostLogEntry
{
    static readonly string NullDevicePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "NUL" : "/dev/null";
    
    ILogger zLogger = default!;
    ILogger msExtConsoleLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger nLogMsExtLogger = default!;

    Serilog.ILogger serilogLogger = default!;
    NLog.Logger nLogLogger = default!;

    [GlobalSetup]
    public void SetUp()
    {
        System.Console.SetOut(TextWriter.Null);
        
        // ZLogger
        
        var zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerStream(Stream.Null, options =>
            {
                options.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter($"{0} [{1}]", (template, info) => template.Format(info.Timestamp.DateTime, info.LogLevel));
                });
            });
        });

        zLogger = zLoggerFactory.CreateLogger<PostLogEntry>();
        
        // Microsoft.Extensions.Logging.Console
        
        var msExtConsoleLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging
                .AddConsole(options =>
                {
                    options.FormatterName = "BenchmarkPlainText";
                })
                .AddConsoleFormatter<BenchmarkPlainTextConsoleFormatter, BenchmarkPlainTextConsoleFormatter.Options>();
        });

        msExtConsoleLogger = msExtConsoleLoggerFactory.CreateLogger<Program>();
        
        // Serilog
        
        serilogLogger = new LoggerConfiguration()
            .WriteTo.Async(a => a.TextWriter(TextWriter.Null))
            .CreateLogger();
        
        var serilogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddSerilog(new LoggerConfiguration()
                .WriteTo.Async(a => a.TextWriter(TextWriter.Null))
                .CreateLogger());
        });
        
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<PostLogEntry>();
        
        // NLog
        var nLogLayout = new NLog.Layouts.SimpleLayout("${longdate} [${level}] ${message}");
        {
            var nLogConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.FileTarget("Null")
            {
                FileName = NullDevicePath,
                Layout = nLogLayout
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, 10000, AsyncTargetWrapperOverflowAction.Grow)
            {
                TimeToSleepBetweenBatches = 0
            };
            nLogConfig.AddTarget(asyncTarget);
            nLogConfig.AddRuleForAllLevels(asyncTarget);
            nLogConfig.LogFactory.Configuration = nLogConfig;

            nLogLogger = nLogConfig.LogFactory.GetLogger("NLog");
        }
        {
            var nLogConfigForMsExt = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.FileTarget("Null")
            {
                FileName = NullDevicePath,
                Layout = nLogLayout
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, 10000, AsyncTargetWrapperOverflowAction.Grow)
            {
                TimeToSleepBetweenBatches = 0
            };
            nLogConfigForMsExt.AddTarget(asyncTarget);
            nLogConfigForMsExt.AddRuleForAllLevels(asyncTarget);
            nLogConfigForMsExt.LogFactory.Configuration = nLogConfigForMsExt;

            var nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddNLog(nLogConfigForMsExt);
            });
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<PostLogEntry>();
        }
    }

    [Benchmark]
    public void ZLogger_ZLog()
    {
        const int x = 100;
        const int y = 200;
        const int z = 300;
        zLogger.ZLogInformation($"foo{x} bar{y} nazo{z}");
    }

    [Benchmark]
    public void MsExtConsole_Log()
    {
        msExtConsoleLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
    }

    [Benchmark]
    public void SerilogMsExt_Log()
    {
        serilogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
    }

    [Benchmark]
    public void NLogMsExt_Log()
    {
        nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
    }
    
    [Benchmark]
    public void Serilog_Log()
    {
        serilogLogger.Information("x={X} y={Y} z={Z}", 100, 200, 300);
    }
    
    [Benchmark]
    public void NLog_Log()
    {
        nLogLogger.Info("x={X} y={Y} z={Z}", 100, 200, 300);
    }
}