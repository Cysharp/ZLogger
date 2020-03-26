using BenchmarkDotNet.Attributes;
using ZLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace Benchmark.Benchmarks
{
    [Config(typeof(BenchmarkConfig))]
    public class WriteToFile
    {
        Serilog.Core.Logger serilogLogger;

        ILoggerProvider zlog;
        Microsoft.Extensions.Logging.ILogger zlogLogger;

        public WriteToFile()
        {
            var guid = Guid.NewGuid();
            serilogLogger = new Serilog.LoggerConfiguration()
                .WriteTo.File(@$"C:\logs\{guid}\serilog.log")
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(options =>
            {
                options.AddZLogFile(@$"C:\logs\{guid}\zlog.log");
                //options.AddZLogConsole();
            });
            var serviceProvider = serviceCollection.BuildServiceProvider();
            zlog = serviceProvider.GetService<ILoggerProvider>();
            zlogLogger = zlog.CreateLogger("temp");


        }

        [Benchmark]
        public void SeriFile()
        {
            serilogLogger.Information("foo{foo} bar{bar} nazo{nazo}", 10, 20, 30);
        }

        [Benchmark]
        public void ZFile()
        {
            zlogLogger.ZLogInformation("foo{0} bar{1} nazo{2}", 10, 20, 30);
        }


    }

    internal struct FormatLogState<TPayload, T0, T1> : IZLogState
    {
        public readonly TPayload Payload;
        public readonly string Format;
        public readonly T0 Arg0;
        public readonly T1 Arg1;

        public FormatLogState(TPayload payload, string format, T0 arg0, T1 arg1)
        {
            Payload = payload;
            Format = format;
            Arg0 = arg0;
            Arg1 = arg1;
        }

        public IZLogEntry CreateLogEntry(LogInfo logInfo)
        {
            return null;
        }
    }
}
