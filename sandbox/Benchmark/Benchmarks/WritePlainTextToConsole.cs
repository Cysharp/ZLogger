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
using ZeroLog.Appenders;
using ZeroLog.Configuration;
using ZeroLog.Formatting;
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
public class WritePlainTextToConsole
{
    const int N = 100_000;

    ILogger zLogger = default!;
    ILogger msExtConsoleLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger serilogMsExtLoggerDefault = default!;
    ILogger nLogMsExtLogger = default!;
    ILogger nLogMsExtLoggerDefault = default!;

    ILoggerFactory zLoggerFactory;
    ILoggerFactory msExtConsoleLoggerFactory;
    ILoggerFactory serilogMsExtLoggerFactory;
    ILoggerFactory serilogMsExtLoggerFactoryDefault;
    ILoggerFactory nLogMsExtLoggerFactory;
    ILoggerFactory nLogMsExtLoggerFactoryDefault;

    Serilog.Core.Logger serilogLogger = default!;
    Serilog.Core.Logger serilogLoggerDefault = default!;

    NLog.Logger nLogLogger = default!;
    NLog.Logger nLogLoggerDefault = default!;
    NLog.Config.LoggingConfiguration nLogConfig = default!;
    NLog.Config.LoggingConfiguration nLogConfigDefault = default!;

    ZeroLog.Log zeroLogLogger = ZeroLog.LogManager.GetLogger<WritePlainTextToConsole>();

    [IterationSetup]
    public void SetUpLogger()
    {
        zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerConsole(console =>
            {
                console.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter($"{0} [{1}]",
                        (in MessageTemplate template, in LogInfo info) => template.Format(info.Timestamp, info.LogLevel));
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
        serilogMsExtLoggerFactory = LoggerFactory.Create(x => x.AddSerilog(serilogLogger, true));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WritePlainTextToConsole>();

        serilogLoggerDefault = new Serilog.LoggerConfiguration()
            .WriteTo.Console(serilogFormatter)
            .CreateLogger();
        serilogMsExtLoggerFactoryDefault = LoggerFactory.Create(x => x.AddSerilog(serilogLoggerDefault, true));
        serilogMsExtLoggerDefault = serilogMsExtLoggerFactoryDefault.CreateLogger<WritePlainTextToConsole>();

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
            nLogMsExtLoggerFactory = LoggerFactory.Create(x => x.AddNLog(nLogConfig));
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WritePlainTextToConsole>();
        }
        {
            nLogConfigDefault = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.ConsoleTarget("Console:Default")
            {
                Layout = nLogLayout
            };
            nLogConfigDefault.AddTarget(target);
            nLogConfigDefault.AddRuleForAllLevels(target);
            nLogConfigDefault.LogFactory.Configuration = nLogConfigDefault;

            nLogLoggerDefault = nLogConfigDefault.LogFactory.GetLogger(nameof(WritePlainTextToConsole));
            nLogMsExtLoggerFactoryDefault = LoggerFactory.Create(x => x.AddNLog(nLogConfigDefault));
            nLogMsExtLoggerDefault = nLogMsExtLoggerFactoryDefault.CreateLogger<WritePlainTextToConsole>();
        }

        // ZeroLog

        ZeroLog.LogManager.Initialize(new()
        {
            LogMessagePoolSize = 16 * 1024,
            RootLogger =
            {
                LogMessagePoolExhaustionStrategy = LogMessagePoolExhaustionStrategy.WaitUntilAvailable,
                Appenders =
                {
                    new ConsoleAppender
                    {
                        ColorOutput = false,
                        Formatter = new DefaultFormatter { PrefixPattern = "%{date:yyyy-MM-dd HH:mm:ss.fff} [%level] " }
                    }
                }
            }
        });
    }
    
    [IterationCleanup]
    public void Cleanup()
    {
        zLoggerFactory.Dispose();
        msExtConsoleLoggerFactory.Dispose();
        serilogMsExtLoggerFactory.Dispose();
        serilogMsExtLoggerFactoryDefault.Dispose();
        nLogMsExtLoggerFactory.Dispose();
        nLogMsExtLoggerFactoryDefault.Dispose();

        serilogLogger.Dispose();
        serilogLoggerDefault.Dispose();

        nLogConfig.LogFactory.Shutdown();
        nLogConfigDefault.LogFactory.Shutdown();

        ZeroLog.LogManager.Shutdown();
    }

    [Benchmark]
    public void ZLogger_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            zLogger.ZLogInformation($"Hello, {MessageSample.Arg1} lives in {MessageSample.Arg2} {MessageSample.Arg3} years old");
        }
        zLoggerFactory.Dispose();
    }
    
    [Benchmark]
    public void ZLogger_SourceGenerator_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            zLogger.GeneratedZLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        zLoggerFactory.Dispose();
    }

    [Benchmark]
    public void ZeroLog_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            zeroLogLogger.Info($"Hello, {MessageSample.Arg1} lives in {MessageSample.Arg2} {MessageSample.Arg3} years old");
        }

        ZeroLog.LogManager.Flush();
    }

    [Benchmark]
    public void MsExtConsole_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            msExtConsoleLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        msExtConsoleLoggerFactory.Dispose();
    }

    [Benchmark]
    public void MsExtConsole_SourceGenerator_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            msExtConsoleLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
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
        serilogLogger.Dispose();
        serilogMsExtLoggerFactory.Dispose();
    }
    
    [Benchmark]
    public void Serilog_MsExt_SourceGenerator_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLogger.Dispose();
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
    public void Serilog_Default_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLoggerDefault.Information("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLoggerDefault.Dispose();
    }

    [Benchmark]
    public void Serilog_Default_MsExt_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLoggerDefault.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        serilogLoggerDefault.Dispose();
        serilogMsExtLoggerFactoryDefault.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfig.LogFactory.Shutdown();
        nLogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_SourceGenerator_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogConfig.LogFactory.Shutdown();
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

    [Benchmark]
    public void NLog_Default_MsExt_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLoggerDefault.LogInformation("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfigDefault.LogFactory.Shutdown();
        nLogMsExtLoggerFactoryDefault.Dispose();
    }

    [Benchmark]
    public void NLog_Default_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLoggerDefault.Info("x={X} y={Y} z={Z}", 100, 200, 300);
        }
        nLogConfigDefault.LogFactory.Shutdown();
    }
}
