#pragma warning disable 0219
#pragma warning disable 0169
#pragma warning disable 0414

using Benchmark.Benchmarks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using ZLogger;

namespace Benchmark.InternalBenchmarks;


file class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1).WithToolchain(InProcessNoEmitToolchain.Instance));

    }
}

file class EmptyLogProcessor : IAsyncLogProcessor
{
    public ValueTask DisposeAsync()
    {
        return default;
    }

    public void Post(IZLoggerEntry log)
    {
        log.Return();
    }
}

file class WriteUtf8LogProcessor : IAsyncLogProcessor
{
    ZLoggerOptions options;

    public WriteUtf8LogProcessor(ZLoggerOptions options)
    {
        this.options = options;
    }

    //ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>(65536);
    //IZLoggerFormatter formatter = options.CreateFormatter();

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public void Post(IZLoggerEntry log)
    {
        //writer.ResetWrittenCount();
        // log.FormatUtf8(writer, formatter);
        log.Return();
    }
}

[Config(typeof(BenchmarkConfig))]
[LogWritesPerSecond]
// [PerfCollectProfiler(performExtraBenchmarksRun: false)]
public class EmptyLogging
{
    const int N = 100_000;

    ILogger zLogger = default!;
    ILoggerFactory zLoggerFactory;
    ILogger zLogger2 = default!;
    ILoggerFactory zLoggerFactory2;
    ILogger zLogger3 = default!;
    ILoggerFactory zLoggerFactory3;

    string tempDir = default!;
    string GetLogFilePath(string filename) => Path.Join(tempDir, filename);



    [IterationSetup]
    public void SetUpLogger()
    {
        // ZLogger


        zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerLogProcessor(new EmptyLogProcessor());
        });

        zLogger = zLoggerFactory.CreateLogger<EmptyLogging>();

        zLoggerFactory2 = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerLogProcessor(options => new EmptyLogProcessor(/*options*/));
        });

        zLogger2 = zLoggerFactory2.CreateLogger<EmptyLogging>();

        //zLoggerFactory3 = LoggerFactory.Create(logging =>
        //{
        //    logging.AddZLoggerLogProcessor(options =>
        //    {
        //        options.UsePlainTextFormatter(formatter =>
        //        {
        //            formatter.SetPrefixFormatter($"{0} [{1}]", (template, info) => template.Format(info.Timestamp, info.LogLevel));
        //        });
        //        return new WriteUtf8LogProcessor(options);
        //    });
        //});

        //zLogger3 = zLoggerFactory3.CreateLogger<EmptyLogging>();
    }

    [IterationCleanup]
    public void CleanUpLogger()
    {
        zLoggerFactory.Dispose();
        zLoggerFactory2.Dispose();
        //zLoggerFactory3.Dispose();
    }

    [Benchmark]
    public void ZLogEmpty()
    {
        const int x = 100;
        const int y = 200;
        const int z = 300;
        for (int i = 0; i < N; i++)
        {
            zLogger.ZLogInformation($"");

            //zLogger.ZLogInformation($"foo{x} bar{y} nazo{z}");
            //zLogger2.ZLogInformation($"foo{x} bar{y} nazo{z}");
        }
    }

    //[Benchmark]
    //public void ZLogUtf8()
    //{
    //    const int x = 100;
    //    const int y = 200;
    //    const int z = 300;
    //    for (int i = 0; i < N; i++)
    //    {
    //        zLogger2.ZLogInformation($"foo{x} bar{y} nazo{z}");
    //    }
    //}

    //[Benchmark]
    //public void ZLogUtf8WithPrefix()
    //{
    //    const int x = 100;
    //    const int y = 200;
    //    const int z = 300;

    //    for (int i = 0; i < N; i++)
    //    {
    //        zLogger3.ZLogInformation($"foo{x} bar{y} nazo{z}");
    //    }
    //}
}
