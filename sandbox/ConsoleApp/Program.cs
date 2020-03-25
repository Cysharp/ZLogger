using ConsoleAppFramework;
using ZLog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using Cysharp.Text;

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

                    //logging.AddZLogFile("log.txt");

                    logging.AddZLogConsole(options =>
                    {
                        options.PrefixFormatter = (writer, state) =>
                        {
                            using (var sb = ZString.CreateUtf8StringBuilder())
                            {
                                sb.AppendFormat("{0} {1} Message:", state.Timestamp, state.CategoryName);

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
            logger.LogDebug("fo{0}o {1}", 10, 20);

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
