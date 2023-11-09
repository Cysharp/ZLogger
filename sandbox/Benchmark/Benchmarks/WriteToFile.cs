using BenchmarkDotNet.Attributes;
using ZLogger;
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

        ILoggerProvider ZLogger;
        Microsoft.Extensions.Logging.ILogger ZLoggerLogger;

        public WriteToFile()
        {
            var guid = Guid.NewGuid();
            serilogLogger = new Serilog.LoggerConfiguration()
                .WriteTo.File(@$"C:\logs\{guid}\serilog.log")
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(options =>
            {
                options.AddZLoggerFile(@$"C:\logs\{guid}\ZLogger.log");
                //options.AddZLoggerConsole();
            });
            var serviceProvider = serviceCollection.BuildServiceProvider();
            ZLogger = serviceProvider.GetService<ILoggerProvider>();
            ZLoggerLogger = ZLogger.CreateLogger("temp");
        }

        [Benchmark]
        public void SeriFile()
        {
            serilogLogger.Information("foo{foo} bar{bar} nazo{nazo}", 10, 20, 30);
        }

        [Benchmark]
        public void ZFile()
        {
            var x = 10;
            var y = 20;
            var z = 30;
            ZLoggerLogger.ZLogInformation($"foo{x} bar{y} nazo{z}");
        }
    }
}
