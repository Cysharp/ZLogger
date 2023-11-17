using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets.Wrappers;
using Serilog;
using Serilog.Formatting.Display;
using Utf8StringInterpolation;
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
public class WritePlainTextToFile
{
    const int N = 100_000;

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
            logging.AddZLogger(zLogger =>
            {
                zLogger.AddFile(GetLogFilePath("zlogger.log"), options =>
                {
                    options.UsePlainTextFormatter(formatter =>
                    {
                        formatter.SetPrefixFormatter($"{0} [{1}]", (template, info) => template.Format(info.Timestamp, info.LogLevel));
                    });
                });
            });
        });

        zLogger = zLoggerFactory.CreateLogger<WritePlainTextToFile>();

        // Serilog

        var serilogFormatter = new MessageTemplateTextFormatter("{Timestamp} [{Level}] {Message}{NewLine}");

        serilogLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.File(serilogFormatter, GetLogFilePath("serilog.log"), buffered: true, flushToDiskInterval: TimeSpan.Zero), bufferSize: N)
            .CreateLogger();

        serilogLoggerForMsExt = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.File(serilogFormatter, GetLogFilePath("serilog_msext.log"), buffered: true, flushToDiskInterval: TimeSpan.Zero), bufferSize: N)
            .CreateLogger();

        serilogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddSerilog(serilogLoggerForMsExt));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WritePlainTextToFile>();

        // NLog

        var nLogLayout = new NLog.Layouts.SimpleLayout("${longdate} [${level}] ${message}");
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
                TimeToSleepBetweenBatches = 0
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
    }

    [Benchmark]
    public void ZLogger_PlainTextFile()
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
    public void Serilog_MsExt_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLoggerForMsExt.Dispose();
        serilogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLogger.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void NLog_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfig.LogFactory.Shutdown();
    }

    string GetLogFilePath(string filename) => Path.Join(tempDir, filename);
}