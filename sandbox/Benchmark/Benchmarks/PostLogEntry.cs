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
    static readonly string NullDevicePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "NUL" : "/dev/null";

    ILogger zLogger = default!;
    ILogger msExtConsoleLogger = default!;
    ILogger serilogMsExtLogger = default!;
    ILogger serilogMsExtLoggerDefault = default!;
    ILogger nLogMsExtLogger = default!;
    ILogger nLogMsExtLoggerDefault = default!;

    Serilog.Core.Logger serilogLogger = default!;
    Serilog.Core.Logger serilogLoggerDefault = default!;
    NLog.Logger nLogLogger = default!;
    NLog.Logger nLogLoggerDefault = default!;

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
                .AddConsole(options =>
                {
                    options.FormatterName = "BenchmarkPlainText";
                })
                .AddConsoleFormatter<BenchmarkPlainTextConsoleFormatter, BenchmarkPlainTextConsoleFormatter.Options>();
        });
        disposables.Add(msExtConsoleLoggerFactory);

        msExtConsoleLogger = msExtConsoleLoggerFactory.CreateLogger<Program>();

        // Serilog

        serilogLogger = new LoggerConfiguration()
            .WriteTo.Async(a => a.TextWriter(TextWriter.Null))
            .CreateLogger();
        disposables.Add(serilogLogger);
        var serilogMsExtLoggerFactory = LoggerFactory.Create(x => x.AddSerilog(serilogLogger));
        serilogMsExtLogger = serilogMsExtLoggerFactory.CreateLogger<PostLogEntry>();
        disposables.Add(serilogMsExtLoggerFactory);

        serilogLoggerDefault = new LoggerConfiguration()
            .WriteTo.TextWriter(TextWriter.Null)
            .CreateLogger();
        var serilogMsExtLoggerFactoryDefault = LoggerFactory.Create(x => x.AddSerilog(serilogLoggerDefault));
        serilogMsExtLoggerDefault = serilogMsExtLoggerFactoryDefault.CreateLogger<PostLogEntry>();
        disposables.Add(serilogMsExtLoggerFactoryDefault);

        // NLog
        var nLogLayout = new NLog.Layouts.SimpleLayout("${longdate} [${level}] ${message}");
        {
            var nLogConfig = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.FileTarget("Null")
            {
                FileName = NullDevicePath,
                Layout = nLogLayout
            };
            var asyncTarget = new NLog.Targets.Wrappers.AsyncTargetWrapper(target, 10000, AsyncTargetWrapperOverflowAction.Grow)
            {
                TimeToSleepBetweenBatches = 0
            };
            nLogConfig.AddTarget(asyncTarget);
            nLogConfig.AddRuleForAllLevels(asyncTarget);
            nLogConfig.LogFactory.Configuration = nLogConfig;

            nLogLogger = nLogConfig.LogFactory.GetLogger("NLog");

            var nLogMsExtLoggerFactory = LoggerFactory.Create(logging => logging.AddNLog(nLogConfig));
            nLogMsExtLogger = nLogMsExtLoggerFactory.CreateLogger<PostLogEntry>();

            disposables.Add(nLogMsExtLoggerFactory);
        }
        {
            var nLogConfigDefault = new NLog.Config.LoggingConfiguration(new LogFactory());
            var target = new NLog.Targets.FileTarget("Null:Default")
            {
                FileName = NullDevicePath,
                Layout = nLogLayout
            };
            nLogConfigDefault.AddTarget(target);
            nLogConfigDefault.AddRuleForAllLevels(target);
            nLogConfigDefault.LogFactory.Configuration = nLogConfigDefault;

            nLogLogger = nLogConfigDefault.LogFactory.GetLogger("NLog");

            var nLogMsExtLoggerFactoryDefault = LoggerFactory.Create(logging => logging.AddNLog(nLogConfigDefault));
            nLogMsExtLoggerDefault = nLogMsExtLoggerFactoryDefault.CreateLogger<PostLogEntry>();

            disposables.Add(nLogMsExtLoggerFactoryDefault);
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
    public void Serilog_MsExt_Log()
    {
        serilogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void Serilog_MsExt_SourceGenerator_Log()
    {
        serilogMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void Serilog_Log()
    {
        serilogLogger.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void Serilog_Default_MsExt_Log()
    {
        serilogMsExtLoggerDefault.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void Serilog_Default_Log()
    {
        serilogLoggerDefault.Information(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }
    
    [Benchmark]
    public void NLog_MsExt_Log()
    {
        nLogMsExtLogger.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }
    
    [Benchmark]
    public void NLog_MsExt_SourceGenerator_Log()
    {
        nLogMsExtLogger.GeneratedLog(MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void NLog_Log()
    {
        nLogLogger.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }
    
    [Benchmark]
    public void NLog_Default_MsExt_Log()
    {
        nLogMsExtLoggerDefault.LogInformation(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }

    [Benchmark]
    public void NLog_Default_Log()
    {
        nLogLoggerDefault.Info(MessageSample.Message, MessageSample.Arg1, MessageSample.Arg2, MessageSample.Arg3);
    }
}