using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets.Wrappers;
using Serilog;
using Serilog.Formatting.Json;
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
        //AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1));
        AddJob(Job.ShortRun.WithWarmupCount(5).WithIterationCount(5).WithToolchain(InProcessNoEmitToolchain.Instance));

    }
}

[Config(typeof(BenchmarkConfig))]
[LogWritesPerSecond]
public class WriteJsonToConsole
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

    ZeroLog.Log zeroLogLogger = ZeroLog.LogManager.GetLogger<WriteJsonToConsole>();

    [IterationSetup]
    public void SetUpLogger()
    {
        // System.Console.SetOut(TextWriter.Null);

        // ZLogger

        zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerConsole(console =>
            {
                console.UseJsonFormatter();
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

        serilogAsyncLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.Console(serilogFormatter), bufferSize: N)
            .CreateLogger();

        serilogAsyncMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddSerilog(serilogAsyncLogger));
        serilogAsyncMsExtLogger = serilogAsyncMsExtLoggerFactory.CreateLogger<WriteJsonToConsole>();

        serilogLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Console(serilogFormatter)
            .CreateLogger();
        serilogMsExtLoggerFactory = LoggerFactory.Create(x => x.AddSerilog(serilogLogger));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WriteJsonToConsole>();

        // NLog

        var nLogLayout = new NLog.Layouts.JsonLayout
        {
            IncludeEventProperties = true,
            Attributes =
            {
                new NLog.Layouts.JsonAttribute("timestamp", "${date:format=o}"),
                new NLog.Layouts.JsonAttribute("level", "${level}"),
                new NLog.Layouts.JsonAttribute("message", "${message}"),
                new NLog.Layouts.JsonAttribute("logger", "${logger}"),
            }
        };
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

            nLogAsyncLogger = nLogAsyncConfig.LogFactory.GetLogger("Benchmark.Benchmarks.WriteJsonToConsole");

            nLogAsyncMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddNLog(nLogAsyncConfig));
            nLogAsyncMsExtLogger = nLogAsyncMsExtLoggerFactory.CreateLogger<WriteJsonToConsole>();
        }
        {
            nLogConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.ConsoleTarget("Console")
            {
                Layout = nLogLayout,
            };
            nLogConfig.AddTarget(target);
            nLogConfig.AddRuleForAllLevels(target);
            nLogConfig.LogFactory.Configuration = nLogConfig;
            nLogLogger = nLogConfig.LogFactory.GetLogger("Benchmark.Benchmarks.WriteJsonToConsole");

            nLogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddNLog(nLogConfig));
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WriteJsonToConsole>();
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
    public void ZLogger_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            var Name = MessageSample.Arg1;
            var City = MessageSample.Arg2;
            var Age = MessageSample.Arg3;
            zLogger.ZLogInformation($"Hello, {Name} lives in {City} {Age} years old");
        }
        zLoggerFactory.Dispose();
    }

    [Benchmark]
    public void ZLogger_SourceGenerator_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            zLogger.GeneratedZLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        zLoggerFactory.Dispose();
    }

    [Benchmark]
    public void ZeroLog_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            zeroLogLogger.Info()
                .Append("Hello")
                .AppendKeyValue("Name", MessageSample.Arg1)
                .AppendKeyValue("City", MessageSample.Arg2)
                .AppendKeyValue("Age", MessageSample.Arg3)
                .Log();
        }

        ZeroLog.LogManager.Flush();
    }

    [Benchmark]
    public void MsExtConsole_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            msExtConsoleLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        msExtConsoleLoggerFactory.Dispose();
    }

    [Benchmark]
    public void MsExtConsole_SourceGenerator_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            msExtConsoleLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        msExtConsoleLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_Async_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogAsyncLogger.Dispose();
        serilogAsyncMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_Async_MsExt_SourceGenerator_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogAsyncLogger.Dispose();
        serilogAsyncMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_Async_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogAsyncLogger.Dispose();
    }

    [Benchmark]
    public void Serilog_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void Serilog_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void NLog_Async_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_Async_MsExt_SourceGenerator_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_Async_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void NLog_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }
}
