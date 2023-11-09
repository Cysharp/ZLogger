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
public class WriteJsonToConsole
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
        System.Console.SetOut(TextWriter.Null);

        // ZLogger

        zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerConsole(options =>
            {
                options.UseJsonFormatter();
            });
        });

        zLogger = zLoggerFactory.CreateLogger<WritePlainTextToFile>();

        // Microsoft.Extensions.Logging.Console

        msExtConsoleLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddJsonConsole();
        });

        msExtConsoleLogger = msExtConsoleLoggerFactory.CreateLogger<WriteJsonToConsole>();

        // Serilog

        var serilogFormatter = new JsonFormatter(renderMessage: true);

        serilogLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.Console(serilogFormatter), bufferSize: int.MaxValue)
            .CreateLogger();

        serilogLoggerForMsExt = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.Console(serilogFormatter), bufferSize: int.MaxValue)
            .CreateLogger();

        serilogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddSerilog(serilogLoggerForMsExt));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WriteJsonToConsole>();

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
            var target = new NLog.Targets.ConsoleTarget("Console")
            {
                Layout = nLogLayout,
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, int.MaxValue, AsyncTargetWrapperOverflowAction.Grow);
            nLogConfig.AddTarget(asyncTarget);
            nLogConfig.AddRuleForAllLevels(asyncTarget);
            nLogConfig.LogFactory.Configuration = nLogConfig;

            nLogLogger = nLogConfig.LogFactory.GetLogger(nameof(WriteJsonToConsole));
        }
        {
            nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
            {
                nLogConfigForMsExt = new NLog.Config.LoggingConfiguration(new LogFactory());
                var target2 = new NLog.Targets.ConsoleTarget("ConsoleMsExt")
                {
                    Layout = nLogLayout
                };
                var asyncTarget2 = new NLog.Targets.Wrappers.AsyncTargetWrapper(target2, int.MaxValue, AsyncTargetWrapperOverflowAction.Grow);
                nLogConfigForMsExt.AddTarget(asyncTarget2);
                nLogConfigForMsExt.AddRuleForAllLevels(asyncTarget2);
                nLogConfigForMsExt.LogFactory.Configuration = nLogConfigForMsExt;
                logging.AddNLog(nLogConfigForMsExt);
            });
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WriteJsonToConsole>();
        }
    }

    [Benchmark]
    public void ZLogger_JsonConsole()
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
    public void MsExtConsole_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            msExtConsoleLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        msExtConsoleLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLoggerForMsExt.Dispose();
        serilogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLogger.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void NLog_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfig.LogFactory.Shutdown();
    }
}
