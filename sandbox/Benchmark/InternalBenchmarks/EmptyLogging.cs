using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Logging;
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
        AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1));
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

file class WriteUtf8LogProcessor(ZLoggerOptions options) : IAsyncLogProcessor
{
    ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>(65536);
    IZLoggerFormatter formatter = options.CreateFormatter();

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public void Post(IZLoggerEntry log)
    {
        writer.ResetWrittenCount();
        log.FormatUtf8(writer, formatter);
        log.Return();
    }
}

[Config(typeof(BenchmarkConfig))]
[LogWritesPerSecond]
public class EmptyLogging
{
    ILogger zLogger = default!;
    ILoggerFactory zLoggerFactory;
    ILogger zLogger2 = default!;
    ILoggerFactory zLoggerFactory2;
    ILogger zLogger3 = default!;
    ILoggerFactory zLoggerFactory3;

    string tempDir = default!;
    string GetLogFilePath(string filename) => Path.Join(tempDir, filename);

    [GlobalSetup]
    public void SetUpDirectory()
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
    }

    [IterationSetup]
    public void SetUpLogger()
    {
        // ZLogger

        zLoggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLogger(zLogger =>
            {
                zLogger.AddLogProcessor(new EmptyLogProcessor());
            });
        });

        zLogger = zLoggerFactory.CreateLogger<EmptyLogging>();

        zLoggerFactory2 = LoggerFactory.Create(logging =>
        {
            logging.AddZLogger(z =>
            {
                z.AddLogProcessor(options => new WriteUtf8LogProcessor(options));
            });
        });

        zLogger2 = zLoggerFactory2.CreateLogger<EmptyLogging>();

        zLoggerFactory3 = LoggerFactory.Create(logging =>
        {
            logging.AddZLogger(z =>
            {
                z.AddLogProcessor(options =>
                {
                    options.UsePlainTextFormatter(formatter =>
                    {
                        formatter.SetPrefixFormatter($"{0} [{1}]", (template, info) => template.Format(info.Timestamp, info.LogLevel));
                    });

                    return new WriteUtf8LogProcessor(options);
                });

            });
        });

        zLogger3 = zLoggerFactory3.CreateLogger<EmptyLogging>();
    }

    [IterationCleanup]
    public void CleanUpLogger()
    {
        zLoggerFactory.Dispose();
        zLoggerFactory2.Dispose();
        zLoggerFactory3.Dispose();
    }

    [Benchmark]
    public void ZLogEmpty()
    {
        const int x = 100;
        const int y = 200;
        const int z = 300;
        zLogger.ZLogInformation($"foo{x} bar{y} nazo{z}");
    }

    [Benchmark]
    public void ZLogUtf8()
    {
        const int x = 100;
        const int y = 200;
        const int z = 300;
        zLogger2.ZLogInformation($"foo{x} bar{y} nazo{z}");
    }

    [Benchmark]
    public void ZLogUtf8WithPrefix()
    {
        const int x = 100;
        const int y = 200;
        const int z = 300;
        zLogger3.ZLogInformation($"foo{x} bar{y} nazo{z}");
    }
}
