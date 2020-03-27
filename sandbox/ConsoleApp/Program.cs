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

                    /*
                    logging.AddZLogRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd_HH-mm-ss}_{x:000}.log",
                        x => { var time = x.ToLocalTime(); return new DateTimeOffset(time.Year, time.Month, time.Day, 0, 0, 0, time.Second, TimeSpan.Zero); },
                        1024);
                        */

                    logging.AddZLogConsole(options =>
                    {
                        options.UseDefaultStructuredLogFormatter();

                    });
                })
                .RunConsoleAppFrameworkAsync<Program>(args);
        }

        readonly ILogger<Program> logger;

        public Program(ILogger<Program> logger)
        {
            this.logger = logger;
        }


        public  struct MyMessage
        {
            public int Foo { get; set; }
            public int Bar { get; set; }
        }

        public async Task Run()
        {
            //logger.LogDebug("foooooo  {0} {1}", 10, 20);

            //logger.ZLog(LogLevel.Debug, "hogehoge", 100,);

            //logger.ZDebug(obj, "foo{0} {1}", 100, 200);
            // Message: foo 100 200, Payload:{hoge:100, fafa:200}

            var obj = new { Foo = "foo!", Bar = "bar!" };

            //logger.ZDebug("foo {0}, bar {1}", 100, 200);
            //logger.ZDebug(obj, "foo {0}, bar {1}", obj.Foo, obj.Bar);




            //var message = LoggerMessage.Define<int, int, int, int>(LogLevel.Debug, default, "foo{0}bar{1}");
            // message(

            // logger.LogDebug(
            //logger.ZLogDebug(

            // logger.

            //logger.ZLogMessage(LogLevel.Debug, "foobarbaz");
            //logger.ZLogMessage(LogLevel.Debug, new { tako = 100 }, "nano");


            var tako2 = LoggerMessage.Define<int, int, int>(LogLevel.Debug, new EventId(10, "hogehoge"), "foo{0} bar {1} baz{2}");

            var tako = ZLoggerMessage.Define<int, int, int>(LogLevel.Debug, new EventId(10, "hogehoge"), "foo{0} bar {1} baz{2}");

            var tako3 = ZLoggerMessage.DefineWithPayload<MyMessage, int, int, int>(LogLevel.Warning, new EventId(10, "hogehoge"), "foo{0} bar{1} baz{2}");


            //logger.ZLogDebug("foo{0} bar {1} baz{2}", 10, 20, 30);

            //logger.ZLogDebug(new { foo = 100 }, "foo{0} bar {1} baz{2}", 10, 20, 30);

            //tako(logger, 100, 200, 300, null);


            tako3(logger, new MyMessage(100, 200), 100, 200, 300, null);


            //tako(logger, 100, 200, 300,


            await Task.Yield();

            // logger.ZLogDebug(

            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);

            //await Task.Delay(TimeSpan.FromSeconds(10));
            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLog(LogLevel.Debug, "foo{0}", 100);


            //logger.ZDebug(new Takoyaki { Foo = "e-!", Bar = "b-!" });
        }
    }

    public struct Takoyaki
    {
        public string Foo { get; set; }
        public string Bar { get; set; }
    }


}
