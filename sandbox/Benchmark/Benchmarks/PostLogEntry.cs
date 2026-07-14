using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets.Wrappers;
using Serilog;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Benchmark.Benchmarks;

// file class MsExtConsoleLoggerFormatter : ConsoleFormatter
// {
//     public class Options : ConsoleFormatterOptions
//     {
//     }
//
//     public MsExtConsoleLoggerFormatter() : base("Benchmark")
//     {
//     }
//
//     public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
//     {
//         var message = logEntry.Formatter.Invoke(logEntry.State, logEntry.Exception);
//         var timestamp = DateTime.UtcNow;
//         textWriter.Write(timestamp);
//         textWriter.Write(" [");
//         textWriter.Write(logEntry.LogLevel);
//         textWriter.Write("] ");
//         textWriter.WriteLine(message);
//     }
// }

file class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.ShortRun);
    }
}

file class NullProcessor : IAsyncLogProcessor
{
    Channel<IZLoggerEntry> channel;

    public NullProcessor()
    {
        this.channel = Channel.CreateUnbounded<IZLoggerEntry>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false, // always should be in async loop.
            SingleWriter = false,
            SingleReader = true,
        });
    }

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public void Post(IZLoggerEntry log)
    {
        channel.Writer.TryWrite(log);
        log.Return();
    }
}

[Config(typeof(BenchmarkConfig))]
public class PostLogEntry
{
    const int N = 10_000;

    static readonly string NullDevicePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "NUL" : "/dev/null";

    ILogger zLogger = default!;
    ILogger msExtConsoleLogger = default!;
    ILogger serilogAsyncMsExtLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger nLogAsyncMsExtLogger = default!;
    ILogger nLogMsExtLogger = default!;

    Serilog.Core.Logger serilogAsyncLogger = default!;
    Serilog.Core.Logger serilogLogger = default!;
    NLog.Logger nLogAsyncLogger = default!;
    NLog.Logger nLogLogger = default!;

    List<IDisposable> disposables = new();

    [GlobalSetup]
    public void SetUp()
    {
        System.Console.SetOut(TextWriter.Null);

        // ZLogger

        var zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerStream(Stream.Null, options =>
            {
                options.UsePlainTextFormatter(formatter => formatter.SetPrefixFormatter($"{0} [{1}]", (in MessageTemplate template, in LogInfo info) => template.Format(info.Timestamp, info.LogLevel)));
            });
        });
        disposables.Add(zLoggerFactory);

        zLogger = zLoggerFactory.CreateLogger<PostLogEntry>();

        // Microsoft.Extensions.Logging.Console

        var msExtConsoleLoggerFactory = LoggerFactory.Create(logging =>
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
        disposables.Add(msExtConsoleLoggerFactory);

        msExtConsoleLogger = msExtConsoleLoggerFactory.CreateLogger<Program>();

        // Serilog

        serilogAsyncLogger = new LoggerConfiguration()
            .WriteTo.Async(a => a.TextWriter(TextWriter.Null), bufferSize: N)
            .CreateLogger();
        disposables.Add(serilogAsyncLogger);
        var serilogAsyncMsExtLoggerFactory = LoggerFactory.Create(x => x.AddSerilog(serilogAsyncLogger));
        serilogAsyncMsExtLogger = serilogAsyncMsExtLoggerFactory.CreateLogger<PostLogEntry>();
        disposables.Add(serilogAsyncMsExtLoggerFactory);

        serilogLogger = new LoggerConfiguration()
            .WriteTo.TextWriter(TextWriter.Null)
            .CreateLogger();
        disposables.Add(serilogLogger);
        var serilogMsExtLoggerFactory = LoggerFactory.Create(x => x.AddSerilog(serilogLogger));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<PostLogEntry>();
        disposables.Add(serilogMsExtLoggerFactory);

        // NLog
        var nLogLayout = new NLog.Layouts.SimpleLayout("${longdate} [${level}] ${message}");
        {
            var nLogAsyncConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.NullTarget("Null Async")
            {
                Layout = nLogLayout,
                FormatMessage = true,
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, N, AsyncTargetWrapperOverflowAction.Grow)
            {
                TimeToSleepBetweenBatches = 0
            };
            nLogAsyncConfig.AddTarget(asyncTarget);
            nLogAsyncConfig.AddRuleForAllLevels(asyncTarget);
            nLogAsyncConfig.LogFactory.Configuration = nLogAsyncConfig;

            nLogAsyncLogger = nLogAsyncConfig.LogFactory.GetLogger("NLog");

            var nLogAsyncMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddNLog(nLogAsyncConfig));
            nLogAsyncMsExtLogger = nLogAsyncMsExtLoggerFactory.CreateLogger<PostLogEntry>();

            disposables.Add(nLogAsyncMsExtLoggerFactory);
            disposables.Add(nLogAsyncConfig.LogFactory);
        }
        {
            var nLogConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.NullTarget("Null")
            {
                Layout = nLogLayout,
                FormatMessage = true,
            };
            nLogConfig.AddTarget(target);
            nLogConfig.AddRuleForAllLevels(target);
            nLogConfig.LogFactory.Configuration = nLogConfig;

            nLogLogger = nLogConfig.LogFactory.GetLogger("NLog");

            var nLogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddNLog(nLogConfig));
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<PostLogEntry>();

            disposables.Add(nLogMsExtLoggerFactory);
            disposables.Add(nLogConfig.LogFactory);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        foreach (var item in disposables)
        {
            item.Dispose();
        }
    }

    [Benchmark]
    public void ZLogger_ZLog()
    {
        zLogger.ZLogInformation($"Hello, {MessageSample.Arg1} lives in {MessageSample.Arg2} {MessageSample.Arg3} years old");
    }
    
    [Benchmark]
    public void ZLogger_GeneratedLog()
    {
        zLogger.GeneratedZLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void MsExtConsole_Log()
    {
        msExtConsoleLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void MsExtConsole_SourceGenerator_Log()
    {
        msExtConsoleLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void Serilog_Async_MsExt_Log()
    {
        serilogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void Serilog_Async_MsExt_SourceGenerator_Log()
    {
        serilogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void Serilog_Async_Log()
    {
        serilogAsyncLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void Serilog_MsExt_Log()
    {
        serilogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void Serilog_Log()
    {
        serilogLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }
    
    [Benchmark]
    public void NLog_Async_MsExt_Log()
    {
        nLogAsyncMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }
    
    [Benchmark]
    public void NLog_Async_MsExt_SourceGenerator_Log()
    {
        nLogAsyncMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void NLog_Async_Log()
    {
        nLogAsyncLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }
    
    [Benchmark]
    public void NLog_MsExt_Log()
    {
        nLogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void NLog_Log()
    {
        nLogLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }
}
