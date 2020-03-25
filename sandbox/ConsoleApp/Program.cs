using ConsoleAppFramework;
using ZLog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using Cysharp.Text;
using System.Threading;

namespace ConsoleApp
{
    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);

                    logging.AddZLogRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd_HH-mm-ss}_{x:000}.log",
                        x => { var time = x.ToLocalTime(); return new DateTimeOffset(time.Year, time.Month, time.Day, 0, 0, 0, time.Second, TimeSpan.Zero); },
                        1024);

                    logging.AddZLogConsole(options =>
                    {
                        options.PrefixFormatter = (writer, state) =>
                        {
                            using (var sb = ZString.CreateUtf8StringBuilder())
                            {
                                sb.AppendFormat("{0} {1} Message:", state.Timestamp.ToLocalTime(), state.CategoryName);

                                var dest = writer.GetSpan(sb.Length);
                                sb.TryCopyTo(dest, out var written);
                                writer.Advance(written);
                            }
                        };

                    });
                })
                .RunConsoleAppFrameworkAsync<Program>(args);
        }

        readonly ILogger<Program> logger;

        public Program(ILogger<Program> logger)
        {
            this.logger = logger;
        }


        public void Run()
        {
            logger.LogDebug("foooooo  {0} {1}", 10, 20);
            Thread.Sleep(TimeSpan.FromSeconds(3));
            logger.ZDebug("foo{0} {1}", 100, 200);
            logger.ZDebug(new { Foo = "foo!", Bar = "bar!" });
            logger.ZDebug(new Takoyaki { Foo = "e-!", Bar = "b-!" });
        }
    }

    public struct Takoyaki
    {
        public string Foo { get; set; }
        public string Bar { get; set; }
    }


}
