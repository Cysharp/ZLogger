using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets.Wrappers;
using Serilog;
using Serilog.Formatting.Json;
using ZLogger;
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
public class WriteJsonToFile
{
    const int N = 100_000;

    ILogger zLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger serilogMsExtLoggerDefault = default!;
    ILogger nLogMsExtLogger = default!;
    ILogger nLogMsExtLoggerDefault = default!;

    ILoggerFactory zLoggerFactory;
    ILoggerFactory serilogMsExtLoggerFactory;
    ILoggerFactory serilogMsExtLoggerFactoryDefault;
    ILoggerFactory nLogMsExtLoggerFactory;
    ILoggerFactory nLogMsExtLoggerFactoryDefault;

    Serilog.Core.Logger serilogLogger = default!;
    Serilog.Core.Logger serilogLoggerDefault = default!;
    Serilog.Core.Logger serilogLoggerForMsExt = default!;
    Serilog.Core.Logger serilogLoggerForMsExtDefault = default!;

    NLog.Logger nLogLogger = default!;
    NLog.Logger nLogLoggerDefault = default!;
    NLog.Config.LoggingConfiguration nLogConfig = default!;
    NLog.Config.LoggingConfiguration nLogConfigDefault = default!;
    NLog.Config.LoggingConfiguration nLogConfigForMsExt = default!;
    NLog.Config.LoggingConfiguration nLogConfigForMsExtDefault = default!;

    static string tempDir = default!;

    [IterationSetup]
    public void SetUpLogger()
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

        // ZLogger

        zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLogger(builder =>
            {
                builder.AddFile(GetLogFilePath("zlogger.log"), options =>
                {
                    options.UseJsonFormatter();
                });
            });
        });

        zLogger = zLoggerFactory.CreateLogger<WritePlainTextToFile>();

        // Serilog

        var serilogFormatter = new JsonFormatter(renderMessage: true);

        serilogLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.File(serilogFormatter, GetLogFilePath("serilog.log"), buffered: true, flushToDiskInterval: TimeSpan.Zero), bufferSize: N)
            .CreateLogger();

        serilogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            serilogLoggerForMsExt = new Serilog.LoggerConfiguration()
                .WriteTo.Async(a => a.File(serilogFormatter, GetLogFilePath("serilog_msext.log"), buffered: true, flushToDiskInterval: TimeSpan.Zero), bufferSize: N)
                .CreateLogger();
            
            logging.AddSerilog(serilogLoggerForMsExt, true);
        });
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WritePlainTextToFile>();

        serilogLoggerDefault = new Serilog.LoggerConfiguration()
            .WriteTo.File("serilog_default.log")
            .CreateLogger();

        serilogMsExtLoggerFactoryDefault = LoggerFactory.Create(logging =>
        {
            serilogLoggerForMsExtDefault = new Serilog.LoggerConfiguration()
                .WriteTo.File("serilog_default.log")
                .CreateLogger();
            
            logging.AddSerilog(serilogLoggerForMsExtDefault, true);
        });
        serilogMsExtLoggerDefault = serilogMsExtLoggerFactory.CreateLogger<WritePlainTextToFile>();

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
            nLogConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.FileTarget("File")
            {
                FileName = GetLogFilePath("nlog.log"),
                Layout = nLogLayout,
                KeepFileOpen = true,
                ConcurrentWrites = false,
                AutoFlush = true
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, 10000, AsyncTargetWrapperOverflowAction.Grow)
            {
                TimeToSleepBetweenBatches = 0,
            };
            nLogConfig.AddTarget(asyncTarget);
            nLogConfig.AddRuleForAllLevels(asyncTarget);
            nLogConfig.LogFactory.Configuration = nLogConfig;

            nLogLogger = nLogConfig.LogFactory.GetLogger(nameof(WritePlainTextToFile));
        }
        {
            nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
            {
                nLogConfigForMsExt = new NLog.Config.LoggingConfiguration(new LogFactory());
                var target2 = new NLog.Targets.FileTarget("FileMsExt")
                {
                    FileName = GetLogFilePath("nlog_msext.log"),
                    Layout = nLogLayout,
                    KeepFileOpen = true,
                    ConcurrentWrites = false,
                    AutoFlush = true
                };
                var asyncTarget2 = new NLog.Targets.Wrappers.AsyncTargetWrapper(target2, 10000, AsyncTargetWrapperOverflowAction.Grow)
                {
                    TimeToSleepBetweenBatches = 0
                };
                nLogConfigForMsExt.AddTarget(asyncTarget2);
                nLogConfigForMsExt.AddRuleForAllLevels(asyncTarget2);
                nLogConfigForMsExt.LogFactory.Configuration = nLogConfigForMsExt;
                logging.AddNLog(nLogConfigForMsExt);
            });
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WritePlainTextToFile>();
        }
        {
            nLogConfigDefault = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.FileTarget("FileDefault")
            {
                FileName = GetLogFilePath("nlog_default.log"),
                Layout = nLogLayout
            };
            nLogConfigDefault.AddTarget(target);
            nLogConfigDefault.AddRuleForAllLevels(target);
            nLogConfigDefault.LogFactory.Configuration = nLogConfigDefault;
            nLogLoggerDefault = nLogConfigDefault.LogFactory.GetLogger(nameof(WritePlainTextToFile));
        }
        {
            nLogMsExtLoggerFactoryDefault = LoggerFactory.Create(logging =>
            {
                nLogConfigForMsExtDefault = new NLog.Config.LoggingConfiguration(new LogFactory());
                var target = new NLog.Targets.FileTarget("FileMsExtDefault")
                {
                    FileName = GetLogFilePath("nlog_default_msext.log"),
                    Layout = nLogLayout,
                };
                nLogConfigForMsExtDefault.AddTarget(target);
                nLogConfigForMsExtDefault.AddRuleForAllLevels(target);
                nLogConfigForMsExtDefault.LogFactory.Configuration = nLogConfigForMsExtDefault;
                logging.AddNLog(nLogConfigForMsExtDefault);
            });
            nLogMsExtLoggerDefault = nLogMsExtLoggerFactoryDefault.CreateLogger<WritePlainTextToFile>();
        }
    }

    [IterationCleanup]
    public void Cleanup()
    {
        zLoggerFactory.Dispose();
        serilogLoggerForMsExt.Dispose();
        serilogLoggerForMsExtDefault.Dispose();
        serilogMsExtLoggerFactory.Dispose();
        serilogMsExtLoggerFactoryDefault.Dispose();
        serilogLogger.Dispose();
        serilogLoggerDefault.Dispose();
        nLogConfigForMsExt.LogFactory.Shutdown();
        nLogConfigForMsExtDefault.LogFactory.Shutdown();
        nLogMsExtLoggerFactory.Dispose();
        nLogMsExtLoggerFactoryDefault.Dispose();
        nLogConfig.LogFactory.Shutdown();
        nLogConfigDefault.LogFactory.Shutdown();
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
    public void Serilog_Default_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLoggerDefault.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLoggerForMsExtDefault.Dispose();
        serilogMsExtLoggerFactoryDefault.Dispose();
    }

    [Benchmark]
    public void Serilog_Default_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLoggerDefault.Information("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLoggerDefault.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfigForMsExt.LogFactory.Shutdown();
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

    [Benchmark]
    public void NLog_Default_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLoggerDefault.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfigForMsExtDefault.LogFactory.Shutdown();
        nLogMsExtLoggerFactoryDefault.Dispose();
    }

    [Benchmark]
    public void NLog_Default_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLoggerDefault.Info("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfigDefault.LogFactory.Shutdown();
    }

    string GetLogFilePath(string filename) => Path.Join(tempDir, filename);
}