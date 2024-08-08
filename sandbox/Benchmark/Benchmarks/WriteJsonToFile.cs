using System;
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
        AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1).WithToolchain(InProcessNoEmitToolchain.Instance));
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

    NLog.Logger nLogLogger = default!;
    NLog.Logger nLogLoggerDefault = default!;
    NLog.Config.LoggingConfiguration nLogConfig = default!;
    NLog.Config.LoggingConfiguration nLogConfigDefault = default!;

    ZeroLog.Log zeroLogLogger = ZeroLog.LogManager.GetLogger<WriteJsonToFile>();

    static string tempDir = default!;

    static string GetLogFilePath(string filename) => Path.Join(tempDir, filename);

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

        zLoggerFactory = LoggerFactory.Create(logging => logging
            .AddZLoggerFile(GetLogFilePath("zlogger.log"), options =>
            {
                options.UseJsonFormatter();
            }));

        zLogger = zLoggerFactory.CreateLogger<WriteJsonToFile>();

        // Serilog

        var serilogFormatter = new JsonFormatter(renderMessage: true);

        serilogLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.File(serilogFormatter, GetLogFilePath("serilog.log"), buffered: true, flushToDiskInterval: TimeSpan.Zero), bufferSize: N)
            .CreateLogger();

        serilogMsExtLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddSerilog(serilogLogger, true);
        });
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WriteJsonToFile>();

        serilogLoggerDefault = new Serilog.LoggerConfiguration()
            .WriteTo.File("serilog_default.log")
            .CreateLogger();

        serilogMsExtLoggerFactoryDefault = LoggerFactory.Create(logging =>
        {
            logging.AddSerilog(serilogLoggerDefault, true);
        });
        serilogMsExtLoggerDefault = serilogMsExtLoggerFactory.CreateLogger<WriteJsonToFile>();

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

            nLogLogger = nLogConfig.LogFactory.GetLogger("Benchmark.Benchmarks.WriteJsonToFile");

            nLogMsExtLoggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddNLog(nLogConfig);
            });
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WriteJsonToFile>();
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
            nLogLoggerDefault = nLogConfigDefault.LogFactory.GetLogger("Benchmark.Benchmarks.WriteJsonToFile");

            nLogMsExtLoggerFactoryDefault = LoggerFactory.Create(logging =>
            {
                logging.AddNLog(nLogConfigDefault);
            });
            nLogMsExtLoggerDefault = nLogMsExtLoggerFactoryDefault.CreateLogger<WriteJsonToFile>();
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
                    new DateAndSizeRollingFileAppender(tempDir)
                    {
                        MaxFileSizeInBytes = 0,
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
    public void ZLogger_JsonFile()
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
    public void ZLogger_SourceGenerator_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            zLogger.GeneratedZLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        zLoggerFactory.Dispose();
    }

    [Benchmark]
    public void ZeroLog_JsonFile()
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
    public void Serilog_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLogger.Dispose();
        serilogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_MsExt_SourceGenerator_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLogger.Dispose();
        serilogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLogger.Dispose();
    }

    [Benchmark]
    public void Serilog_Default_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLoggerDefault.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLoggerDefault.Dispose();
        serilogMsExtLoggerFactoryDefault.Dispose();
    }

    [Benchmark]
    public void Serilog_Default_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLoggerDefault.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogLoggerDefault.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogConfig.LogFactory.Shutdown();
        nLogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void NLog_MsExt_SourceGenerator_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogConfig.LogFactory.Shutdown();
        nLogMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void NLog_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogConfig.LogFactory.Shutdown();
    }

    [Benchmark]
    public void NLog_Default_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLoggerDefault.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogConfigDefault.LogFactory.Shutdown();
        nLogMsExtLoggerFactoryDefault.Dispose();
    }

    [Benchmark]
    public void NLog_Default_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLoggerDefault.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogConfigDefault.LogFactory.Shutdown();
    }
}