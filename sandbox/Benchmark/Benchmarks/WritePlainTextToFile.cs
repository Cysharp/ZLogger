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

        //AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1).WithToolchain(InProcessNoEmitToolchain.Instance));
    }
}

[Config(typeof(BenchmarkConfig))]
[LogWritesPerSecond]
public class WritePlainTextToFile
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

    ZeroLog.Log zeroLogLogger = ZeroLog.LogManager.GetLogger<WritePlainTextToFile>();

    string tempDir = default!;

    string GetLogFilePath(string filename) => Path.Join(tempDir, filename);

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
            logging.AddZLoggerFile(GetLogFilePath("zlogger.log"), options =>
            {
                options.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter($"{0} [{1}]",
                        (in MessageTemplate template, in LogInfo info) => template.Format(info.Timestamp, info.LogLevel));
                });
            });
        });

        zLogger = zLoggerFactory.CreateLogger<WritePlainTextToFile>();

        // Serilog

        var serilogFormatter = new MessageTemplateTextFormatter("{Timestamp} [{Level}] {Message}{NewLine}");

        serilogAsyncLogger = new Serilog.LoggerConfiguration()
            .WriteTo.Async(
                a => a.File(serilogFormatter, GetLogFilePath("serilog_async.log"), buffered: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1)), bufferSize: N)
            .CreateLogger();
        serilogAsyncMsExtLoggerFactory = LoggerFactory.Create(x => x.AddSerilog(serilogAsyncLogger));
        serilogAsyncMsExtLogger = serilogAsyncMsExtLoggerFactory.CreateLogger<WritePlainTextToFile>();

        serilogLogger = new Serilog.LoggerConfiguration().WriteTo.File(serilogFormatter, GetLogFilePath("serilog.log")).CreateLogger();
        serilogMsExtLoggerFactory = LoggerFactory.Create(x => x.AddSerilog(serilogLogger));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<WritePlainTextToFile>();

        // NLog

        var nLogLayout = new NLog.Layouts.SimpleLayout("${longdate} [${level}] ${message}");
        {
            nLogAsyncConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.FileTarget("File_Async")
            {
                FileName = GetLogFilePath("nlog_async.log"),
                Layout = nLogLayout,
                KeepFileOpen = true,
                AutoFlush = false,
                OpenFileFlushTimeout = 1,
            };
            var asyncTarget =
                new NLog.Targets.Wrappers.AsyncTargetWrapper(target, N, AsyncTargetWrapperOverflowAction.Grow)
                {
                    TimeToSleepBetweenBatches = 0
                };
            nLogAsyncConfig.AddTarget(asyncTarget);
            nLogAsyncConfig.AddRuleForAllLevels(asyncTarget);
            nLogAsyncConfig.LogFactory.Configuration = nLogAsyncConfig;
            nLogAsyncLogger = nLogAsyncConfig.LogFactory.GetLogger(nameof(WritePlainTextToFile));

            nLogAsyncMsExtLoggerFactory = LoggerFactory.Create(x => x.AddNLog(nLogAsyncConfig));
            nLogAsyncMsExtLogger = nLogAsyncMsExtLoggerFactory.CreateLogger<WritePlainTextToFile>();
        }
        {
            nLogConfig = new NLog.Config.LoggingConfiguration();
            var target = new NLog.Targets.FileTarget("File")
            {
                FileName = GetLogFilePath("nlog.log"),
                Layout = nLogLayout,
            };
            nLogConfig.AddTarget(target);
            nLogConfig.AddRuleForAllLevels(target);
            nLogConfig.LogFactory.Configuration = nLogConfig;
            nLogLogger = nLogConfig.LogFactory.GetLogger(nameof(WritePlainTextToFile));

            nLogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddNLog(nLogConfig));
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<WritePlainTextToFile>();
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
        zLoggerFactory?.Dispose();
        serilogAsyncMsExtLoggerFactory?.Dispose();
        serilogMsExtLoggerFactory?.Dispose();
        nLogAsyncMsExtLoggerFactory?.Dispose();
        nLogMsExtLoggerFactory?.Dispose();

        serilogAsyncLogger?.Dispose();
        serilogLogger?.Dispose();

        nLogAsyncConfig?.LogFactory?.Shutdown();
        nLogConfig?.LogFactory?.Shutdown();

        ZeroLog.LogManager.Shutdown();
    }

    //[Benchmark]
    //public void Empty()
    //{
    //}

    [Benchmark]
    public void ZLogger_PlainTextFile()
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
    public void ZLogger_SourceGenerator_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            zLogger.GeneratedZLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }

        zLoggerFactory.Dispose();
    }

    [Benchmark]
    public void ZeroLog_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            zeroLogLogger.Info($"Hello, {MessageSample.Arg1} lives in {MessageSample.Arg2} {MessageSample.Arg3} years old");
        }

        ZeroLog.LogManager.Flush();
    }

    [Benchmark]
    public void Serilog_Async_MsExt_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }

        serilogAsyncLogger.Dispose();
        serilogAsyncMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_Async_MsExt_SourceGenerator_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }

        serilogAsyncLogger.Dispose();
        serilogAsyncMsExtLoggerFactory.Dispose();
    }

    [Benchmark]
    public void Serilog_Async_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogAsyncLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2,
                MessageSample.Arg3);
        }

        serilogAsyncLogger.Dispose();
    }

    [Benchmark]
    public void Serilog_MsExt_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2,
                MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void Serilog_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            serilogLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2,
                MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void NLog_Async_MsExt_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2,
                MessageSample.Arg3);
        }

        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_Async_MsExt_SourceGenerator_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }

        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_Async_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogAsyncLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }

        nLogAsyncConfig.LogFactory.Flush();
    }

    [Benchmark]
    public void NLog_MsExt_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2,
                MessageSample.Arg3);
        }
    }

    [Benchmark]
    public void NLog_PlainTextFile()
    {
        for (var i = 0; i < N; i++)
        {
            nLogLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
        }
    }
}
