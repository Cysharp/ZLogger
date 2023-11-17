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
using Serilog.Formatting.Display;
using Utf8StringInterpolation;
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
public class WritePlainTextToConsole
{
    const int N = 100_000;

    ILogger zLogger = default!;
    ILogger msExtConsoleLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger nLogMsExtLogger = default!;

    ILoggerFactory zLoggerFactory;
    ILoggerFactory msExtConsoleLoggerFactory;
    ILoggerFactory serilogMsExtLoggerFactory;
    ILoggerFactory nLogMsExtLoggerFactory;

    Serilog.Core.Logger serilogLogger = default!;
    Serilog.Core.Logger serilogLoggerForMsExt = default!;

    NLog.Logger nLogLogger = default!;
    NLog.Config.LoggingConfiguration nLogConfig = default!;
    NLog.Config.LoggingConfiguration nLogConfigForMsExt = default!;

    [IterationSetup]
    public void SetUpLogger()
    {
        zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLogger(builder =>
            {
                builder.AddConsole(console =>
                {
                    console.UsePlainTextFormatter(formatter =>
                    {
                        formatter.SetPrefixFormatter($"{0} [{1}]", (template, info) => template.Format(info.Timestamp, info.LogLevel));
                    });
                });
            });
        });

        zLogger = zLoggerFactory.CreateLogger<WritePlainTextToConsole>();

        // Microsoft.Extensions.Logging.Console

        msExtConsoleLoggerFactory = LoggerFactory.Create(logging =>
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

        var serilogFormatter = new MessageTemplateTextFormatter("{Timestamp} [{Level}] {Message}{NewLine}");

        serilogLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.Console(serilogFormatter), bufferSize: N)
            .CreateLogger();

        serilogLoggerForMsExt = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.Console(serilogFormatter), bufferSize: N)
            .CreateLogger();

        serilogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddSerilog(serilogLoggerForMsExt, true));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WritePlainTextToConsole>();

        // NLog

        var nLogLayout = new NLog.Layouts.SimpleLayout("${longdate} [${level}] ${message}");
        {
            nLogConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.ConsoleTarget("Console")
            {
                Layout = nLogLayout,
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, 10000, AsyncTargetWrapperOverflowAction.Grow)
            {
                TimeToSleepBetweenBatches = 0
            };
            nLogConfig.AddTarget(asyncTarget);
            nLogConfig.AddRuleForAllLevels(asyncTarget);
            nLogConfig.LogFactory.Configuration = nLogConfig;

            nLogLogger = nLogConfig.LogFactory.GetLogger(nameof(WritePlainTextToConsole));
        }
        {
            nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
            {
                nLogConfigForMsExt = new NLog.Config.LoggingConfiguration(new LogFactory());
                var target2 = new NLog.Targets.ConsoleTarget("ConsoleMsExt")
                {
                    Layout = nLogLayout
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
        }

        nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WritePlainTextToConsole>();
    }

    [Benchmark]
    public void ZLogger_PlainTextConsole()
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
    public void MsExtConsole_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            msExtConsoleLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        msExtConsoleLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_MsExt_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLoggerForMsExt.Dispose();
        serilogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLogger.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfigForMsExt.LogFactory.Shutdown();
        nLogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void NLog_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfig.LogFactory.Shutdown();
    }
}