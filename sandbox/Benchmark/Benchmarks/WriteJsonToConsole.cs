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
public class WriteJsonToConsole
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

    [IterationSetup]
    public void SetUpLogger()
    {
        System.Console.SetOut(TextWriter.Null);

        // ZLogger

        zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLogger(builder =>
            {
                builder.AddConsole(console =>
                {
                    console.UseJsonFormatter();
                });
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
            .WriteTo.Async(a => a.Console(serilogFormatter), bufferSize: N)
            .CreateLogger();

        serilogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddSerilog(serilogLogger));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WriteJsonToConsole>();

        serilogLoggerDefault = new Serilog.LoggerConfiguration()
            .WriteTo.Console(serilogFormatter)
            .CreateLogger();
        serilogMsExtLoggerFactoryDefault = LoggerFactory.Create(x => x.AddSerilog(serilogLoggerDefault));
        serilogMsExtLoggerDefault = serilogMsExtLoggerFactoryDefault.CreateLogger<WriteJsonToConsole>();

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
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, 10000, AsyncTargetWrapperOverflowAction.Grow)
            {
                TimeToSleepBetweenBatches = 0
            };
            nLogConfig.AddTarget(asyncTarget);
            nLogConfig.AddRuleForAllLevels(asyncTarget);
            nLogConfig.LogFactory.Configuration = nLogConfig;

            nLogLogger = nLogConfig.LogFactory.GetLogger(nameof(WriteJsonToConsole));
            
            nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddNLog(nLogConfig);
            });
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WriteJsonToConsole>();
        }
        {
            nLogConfigDefault = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.ConsoleTarget("Console:Default")
            {
                Layout = nLogLayout,
            };
            nLogConfigDefault.AddTarget(target);
            nLogConfigDefault.AddRuleForAllLevels(target);
            nLogConfig.LogFactory.Configuration = nLogConfig;
            nLogLoggerDefault = nLogConfigDefault.LogFactory.GetLogger(nameof(WriteJsonToConsole));
            
            nLogMsExtLoggerFactoryDefault = LoggerFactory.Create(logging =>
            {
                logging.AddNLog(nLogConfigDefault);
            });
            nLogMsExtLoggerDefault = nLogMsExtLoggerFactoryDefault.CreateLogger<WriteJsonToConsole>();
        }
        
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
    }

    [Benchmark]
    public void ZLogger_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            zLogger.ZLogInformation($"Hello, {MessageSample.Arg1} lives in {MessageSample.Arg2} {MessageSample.Arg3} years old");
        }
        zLoggerFactory.Dispose();
    }

    [Benchmark]
    public void ZLogger_SourceGenerator_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            // TODO: ZLogger.Generator
            zLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        zLoggerFactory.Dispose();
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
    public void Serilog_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLogger.Dispose();
        serilogMsExtLoggerFactory.Dispose();
    }
    
    [Benchmark]
    public void Serilog_MsExt_SourceGenerator_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLogger.Dispose();
        serilogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLogger.Dispose();
    }

    [Benchmark]
    public void Serilog_Default_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLoggerDefault.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLoggerDefault.Dispose();
    }
    
    [Benchmark]
    public void Serilog_Default_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLoggerDefault.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLoggerDefault.Dispose();
        serilogMsExtLoggerFactoryDefault.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogMsExtLoggerFactory.Dispose();
    }
    
    [Benchmark]
    public void NLog_MsExt_SourceGenerator_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void NLog_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogConfig.LogFactory.Shutdown();
    }
    
    [Benchmark]
    public void NLog_Default_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLoggerDefault.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogConfigDefault.LogFactory.Shutdown();
    }

    [Benchmark]
    public void NLog_Default_MsExt_JsonConsole()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLoggerDefault.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogMsExtLoggerFactoryDefault.Dispose();
    }
}