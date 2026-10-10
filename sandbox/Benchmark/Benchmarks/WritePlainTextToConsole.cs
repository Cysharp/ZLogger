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
    const int N = 10_000;

    ILogger zLogger = default!;
    ILogger msExtConsoleLogger = default!;
    ILogger serilogAsyncMsExtLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger nLogAsyncMsExtLogger = default!;
    ILogger nLogMsExtLogger = default!;

    ILoggerFactory zLoggerFactory;
    ILoggerFactory msExtConsoleLoggerFactory;
    ILoggerFactory serilogAsyncMsExtLoggerFactory;
    ILoggerFactory serilogMsExtLoggerFactory;
    ILoggerFactory nLogAsyncMsExtLoggerFactory;
    ILoggerFactory nLogMsExtLoggerFactory;

    Serilog.Core.Logger serilogAsyncLogger = default!;
    Serilog.Core.Logger serilogLogger = default!;

    NLog.Logger nLogAsyncLogger = default!;
    NLog.Logger nLogLogger = default!;
    NLog.Config.LoggingConfiguration nLogAsyncConfig = default!;
    NLog.Config.LoggingConfiguration nLogConfig = default!;

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
                .AddSimpleConsole(options =>
                {
                    options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Disabled;
                    options.IncludeScopes = false;
                    options.SingleLine = true;
                    options.TimestampFormat = "u";
                });
        });
        msExtConsoleLogger = msExtConsoleLoggerFactory.CreateLogger<Program>();

        // Serilog

        var serilogFormatter = new MessageTemplateTextFormatter("{Timestamp} [{Level}] {Message}{NewLine}");

        serilogAsyncLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.Console(serilogFormatter), bufferSize: N)
            .CreateLogger();
        serilogAsyncMsExtLoggerFactory = LoggerFactory.Create(x => x.AddSerilog(serilogAsyncLogger, true));
        serilogAsyncMsExtLogger = serilogAsyncMsExtLoggerFactory.CreateLogger<WritePlainTextToConsole>();

        serilogLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Console(serilogFormatter)
            .CreateLogger();
        serilogMsExtLoggerFactory = LoggerFactory.Create(x => x.AddSerilog(serilogLogger, true));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WritePlainTextToConsole>();

        // NLog

        var nLogLayout = new NLog.Layouts.SimpleLayout("${longdate} [${level}] ${message}");
        {
            nLogAsyncConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.ConsoleTarget("Console_Async")
            {
                Layout = nLogLayout,
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, N, AsyncTargetWrapperOverflowAction.Grow)
            {
                TimeToSleepBetweenBatches = 0
            };
            nLogAsyncConfig.AddTarget(asyncTarget);
            nLogAsyncConfig.AddRuleForAllLevels(asyncTarget);
            nLogAsyncConfig.LogFactory.Configuration = nLogAsyncConfig;

            nLogAsyncLogger = nLogAsyncConfig.LogFactory.GetLogger(nameof(WritePlainTextToConsole));
            nLogAsyncMsExtLoggerFactory = LoggerFactory.Create(x => x.AddNLog(nLogAsyncConfig));
            nLogAsyncMsExtLogger = nLogAsyncMsExtLoggerFactory.CreateLogger<WritePlainTextToConsole>();
        }
        {
            nLogConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.ConsoleTarget("Console")
            {
                Layout = nLogLayout
            };
            nLogConfig.AddTarget(target);
            nLogConfig.AddRuleForAllLevels(target);
            nLogConfig.LogFactory.Configuration = nLogConfig;
            nLogLogger = nLogConfig.LogFactory.GetLogger(nameof(WritePlainTextToConsole));

            nLogMsExtLoggerFactory = LoggerFactory.Create(x => x.AddNLog(nLogConfig));
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WritePlainTextToConsole>();
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
        serilogAsyncMsExtLoggerFactory.Dispose();
        serilogMsExtLoggerFactory.Dispose();
        nLogAsyncMsExtLoggerFactory.Dispose();
        nLogMsExtLoggerFactory.Dispose();

        serilogAsyncLogger.Dispose();
        serilogLogger.Dispose();

        nLogAsyncConfig.LogFactory.Shutdown();
        nLogConfig.LogFactory.Shutdown();

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
    public void Serilog_Async_MsExt_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogAsyncLogger.Dispose();
        serilogAsyncMsExtLoggerFactory.Dispose();
    }
    
    [Benchmark]
    public void Serilog_Async_MsExt_SourceGenerator_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogAsyncLogger.Dispose();
        serilogAsyncMsExtLoggerFactory.Dispose();
    }
    
    [Benchmark]
    public void Serilog_Async_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogAsyncLogger.Dispose();
    }

    [Benchmark]
    public void Serilog_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void Serilog_MsExt_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void NLog_Async_MsExt_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_Async_MsExt_SourceGenerator_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_Async_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_MsExt_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void NLog_PlainTextConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }
}
