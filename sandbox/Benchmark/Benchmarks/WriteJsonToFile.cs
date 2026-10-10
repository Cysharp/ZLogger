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
    ILogger serilogAsyncMsExtLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger nLogAsyncMsExtLogger = default!;
    ILogger nLogMsExtLogger = default!;

    ILoggerFactory zLoggerFactory;
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
        serilogAsyncLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(a => a.File(serilogFormatter, GetLogFilePath("serilog_async.log"), buffered: true, flushToDiskInterval: TimeSpan.FromSeconds(1)), bufferSize: N)
            .CreateLogger();

        serilogAsyncMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddSerilog(serilogAsyncLogger, true));
        serilogAsyncMsExtLogger = serilogAsyncMsExtLoggerFactory.CreateLogger<WriteJsonToFile>();

        serilogLogger = new Serilog.LoggerConfiguration()
            .WriteTo.File(serilogFormatter, GetLogFilePath("serilog.log"))
            .CreateLogger();

        serilogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddSerilog(serilogLogger, true));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WriteJsonToFile>();

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
            var target = new NLog.Targets.FileTarget("FileAsync")
            {
                FileName = GetLogFilePath("nlog_async.log"),
                Layout = nLogLayout,
                KeepFileOpen = true,
                AutoFlush = false,
                OpenFileFlushTimeout = 1,
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, N, AsyncTargetWrapperOverflowAction.Grow)
            {
                TimeToSleepBetweenBatches = 0,
            };
            nLogAsyncConfig.AddTarget(asyncTarget);
            nLogAsyncConfig.AddRuleForAllLevels(asyncTarget);
            nLogAsyncConfig.LogFactory.Configuration = nLogAsyncConfig;

            nLogAsyncLogger = nLogAsyncConfig.LogFactory.GetLogger("Benchmark.Benchmarks.WriteJsonToFile");

            nLogAsyncMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddNLog(nLogAsyncConfig));
            nLogAsyncMsExtLogger = nLogAsyncMsExtLoggerFactory.CreateLogger<WriteJsonToFile>();
        }
        {
            nLogConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.FileTarget("File")
            {
                FileName = GetLogFilePath("nlog.log"),
                Layout = nLogLayout,
            };
            nLogConfig.AddTarget(target);
            nLogConfig.AddRuleForAllLevels(target);
            nLogConfig.LogFactory.Configuration = nLogConfig;
            nLogLogger = nLogConfig.LogFactory.GetLogger("Benchmark.Benchmarks.WriteJsonToFile");

            nLogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddNLog(nLogConfig));
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WriteJsonToFile>();
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
    public void Serilog_Async_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogAsyncLogger.Dispose();
        serilogAsyncMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_Async_MsExt_SourceGenerator_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogAsyncLogger.Dispose();
        serilogAsyncMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_Async_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        serilogAsyncLogger.Dispose();
    }

    [Benchmark]
    public void Serilog_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void Serilog_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void NLog_Async_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_Async_MsExt_SourceGenerator_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_Async_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_MsExt_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void NLog_JsonFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }
}
