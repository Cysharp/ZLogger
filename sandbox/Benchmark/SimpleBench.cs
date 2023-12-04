using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics;
using ZLogger;

namespace Benchmark
{
    public static class SimpleBench
    {
        public static void Run()
        {
            //new SeriLogBench().Run();
            //new NLogBench().Run();

            new ZLoggerBench().Run();
        }


        abstract class BenchmarkBase<T>
        {
            protected abstract string Name { get; }
            protected abstract T GetLogger();
            protected abstract void WriteLog(T logger, int x, int y, int z);

            protected virtual void Stop() { }
            public void Run()
            {
                var logger = GetLogger();

                var sw = Stopwatch.StartNew();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                sw.Restart();

                for (int i = 0; i < 100000; i++)
                {
                    WriteLog(logger, 10, 20, 30);
                }

                Stop();
                sw.Stop();

                Console.Write($"{Name} totaltime: " + sw.Elapsed.TotalMilliseconds + "ms");
                Console.Write(" line/elapsed: ");
                // line / elapsed
                Console.WriteLine(((double)1000000 / sw.Elapsed.TotalMilliseconds) + "ns");
            }
        }

        class ZLoggerBench : BenchmarkBase<Microsoft.Extensions.Logging.ILogger>
        {
            protected override string Name => "ZLogger";

            ILoggerProvider factory;

            protected override Microsoft.Extensions.Logging.ILogger GetLogger()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddLogging(logging =>
                {
                    logging.AddZLoggerFile("ZLogger.log");
                    //options.AddZLoggerConsole();
                });
                var serviceProvider = serviceCollection.BuildServiceProvider();
                factory = serviceProvider.GetService<ILoggerProvider>();

                var logger = factory.CreateLogger("Test");

                return logger;
            }

            protected override void WriteLog(Microsoft.Extensions.Logging.ILogger logger, int x, int y, int z)
            {
                logger.ZLogInformation($"x:{x} y:{y} z:{z}");
            }

            protected override void Stop()
            {
                factory.Dispose();
            }
        }

        class NLogBench : BenchmarkBase<NLog.Logger>
        {
            private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

            protected override string Name => "NLog";

            protected override NLog.Logger GetLogger()
            {
                return logger;
            }

            protected override void WriteLog(NLog.Logger logger, int x, int y, int z)
            {
                logger.Info("x:{0} y:{1} z:{2}", x, y, z);
            }
        }

        class SeriLogBench : BenchmarkBase<Serilog.Core.Logger>
        {
            protected override string Name => "Serilog";

            protected override Serilog.Core.Logger GetLogger()
            {
                return new Serilog.LoggerConfiguration()
                    .WriteTo.File("seri.txt")
                    .CreateLogger();
            }

            protected override void WriteLog(Serilog.Core.Logger logger, int x, int y, int z)
            {
                logger.Information("x:{x} y:{y} z:{z}", x, y, z);
            }
        }
    }
}
