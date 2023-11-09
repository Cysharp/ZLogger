using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using ZLogger;
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
[LogWritesPerSecond]
public class WriteToFileJson
{
    const int N = 10_000;
    
    ILogger zLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger nLogMsExtLogger = default!;

    ILoggerFactory zLoggerFactory;
    ILoggerFactory serilogMsExtLoggerFactory;
    ILoggerFactory nLogMsExtLoggerFactory;

    Serilog.Core.Logger serilogLogger = default!;
    Serilog.Core.Logger serilogLoggerForMsExt = default!;
    
    NLog.Logger nLogLogger = default!;
    NLog.Config.LoggingConfiguration nLogConfig = default!;
    NLog.Config.LoggingConfiguration nLogConfigForMsExt = default!;

    string tempDir = default!;

    [GlobalSetup]
    public void SetUpDirectory()
    {
        tempDir = Path.Join(Path.GetTempPath(), "zlogger-benchmark");
        try
        {
            Directory.Delete(tempDir, true);
        }
        catch (DirectoryNotFoundException)
        {
        }
        catch (FileNotFoundException)
        {
        }
        Directory.CreateDirectory(tempDir);
    }

    [IterationSetup]
    public void SetUpLogger()
    {
        // ZLogger
        
        zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerFile(GetLogFilePath("zlogger.log"), options =>
            {
                options.UseJsonFormatter();
            });
        });

        zLogger = zLoggerFactory.CreateLogger<WriteToFilePlainText>();
        
        // Serilog

        var serilogFormatter = new JsonFormatter(renderMessage: true);
        
        serilogLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.File(serilogFormatter, GetLogFilePath("serilog.log"), buffered: true))
            .CreateLogger();

        serilogLoggerForMsExt = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.File(serilogFormatter, GetLogFilePath("serilog_msext.log"), buffered: true))
            .CreateLogger();
        
        serilogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddSerilog(serilogLoggerForMsExt));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WriteToFilePlainText>();
        
        // NLog

        var nLogLayout = new NLog.Layouts.JsonLayout
        {
            IncludeEventProperties = true,
            Attributes =
            {
                new NLog.Layouts.JsonAttribute("date", "${longdate}"),
                new NLog.Layouts.JsonAttribute("level", "${level}"),
                new NLog.Layouts.JsonAttribute("message", "${message}"),
                new NLog.Layouts.JsonAttribute("logger", "${logger}"),
            }
        };
        {
            nLogConfig = new NLog.Config.LoggingConfiguration();
            var target = new NLog.Targets.FileTarget("File")
            {
                FileName = GetLogFilePath("nlog.log"),
                Layout = nLogLayout,
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target);
            nLogConfig.AddTarget(asyncTarget);
            nLogConfig.AddRuleForAllLevels(asyncTarget);

            nLogLogger = nLogConfig.LogFactory.GetLogger(nameof(WriteToFilePlainText));
        }
        {
            nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
            {
                nLogConfigForMsExt = new NLog.Config.LoggingConfiguration();
                var target2 = new NLog.Targets.FileTarget("FileMsExt")
                {
                    FileName = GetLogFilePath("nlog_msext.log"),
                    Layout = nLogLayout
                };
                var asyncTarget2 = new NLog.Targets.Wrappers.AsyncTargetWrapper(target2);
                nLogConfigForMsExt.AddTarget(asyncTarget2);
                nLogConfigForMsExt.AddRuleForAllLevels(asyncTarget2);
                logging.AddNLog(nLogConfigForMsExt);
            });
        }

        nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WriteToFilePlainText>();
    }

    [Benchmark]
    public void ZLogger_JsonFile()
    {
        var x = 100;
        var y = 200;
        var z = 300;
        for (var i = 0; i < N; i++)
        {
            zLogger.ZLogInformation($"x={x} y={y} z={z}");
        }
        zLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLoggerForMsExt.Dispose();
        serilogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLogger.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void NLog_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfig.LogFactory.Shutdown();
    }

    string GetLogFilePath(string filename) => Path.Join(tempDir, filename);
}
