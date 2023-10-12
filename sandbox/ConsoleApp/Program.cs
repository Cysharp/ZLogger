using ConsoleAppFramework;
using ZLogger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using Cysharp.Text;
using System.Threading;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Buffers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ZLogger.Providers;
using ConsoleAppFramework.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Configuration;
using System.Reflection;
using System.Threading.Channels;
using System.Net.Sockets;

namespace MyApp
{
    public class MyClass22
    {

        public void Main2()
        {
            //logger.LogDebug("foo");
























            //logger.Log(LogLevel.Debug, new EventId(0), new Exception(), "foo", "bar");

            //System.Console.WriteLine("hoge");
            // new BannedType();
        }
    }


    public static class GlobalLogger
    {
        static ILogger? globalLogger;
        static ILoggerFactory? loggerFactory;

        public static void SetServiceProvider(ILoggerFactory loggerFactory, string categoryName)
        {
            GlobalLogger.loggerFactory = loggerFactory;
            GlobalLogger.globalLogger = loggerFactory.CreateLogger(categoryName);
        }

        public static ILogger Log => globalLogger!;

        public static ILogger<T> GetLogger<T>() where T : class => loggerFactory!.CreateLogger<T>();
        public static ILogger GetLogger(string categoryName) => loggerFactory!.CreateLogger(categoryName);
    }



    public class HoGeMoge
    {
        public static readonly ILogger<HoGeMoge> logger = GlobalLogger.GetLogger<HoGeMoge>();

        public void Foo(int x)
        {
            logger.ZLogDebug($"do do do: {x}");
        }
    }





    public class MyClassA
    {

    }

    public class MyClassB
    {

    }

    public class MyClassC
    {

        public MyClassC()
        {

            //Console.WriteLine("called ()");
        }

        public MyClassC(MyClassA a)
        {
            //    Console.WriteLine("called a");
        }

        public MyClassC(MyClassA a, MyClassB b)
        {
            //  Console.WriteLine("called a+b");
        }
    }

    public struct UserLogInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BattleLogic
    {

    }


    public class View
    {

    }
    // Due to the constraints of System.Text.JSON.JSONSerializer,
    // only properties can be serialized.
    public struct UserRegisteredLog
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Program : ConsoleAppBase
    {
        static async Task Loop(ChannelReader<int> reader)
        {
            Console.WriteLine("Wait Start");
            while (await reader.WaitToReadAsync())
            {
                //throw new Exception();
                await Task.Delay(TimeSpan.FromSeconds(5));
                reader.TryRead(out var i);
                reader.TryRead(out i);
                reader.TryRead(out i);
            }
            Console.WriteLine("Wait Complete");
            await Task.Delay(TimeSpan.FromSeconds(10));
            Console.WriteLine("END");
        }

        static async Task Main(string[] args)
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");


            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(x =>
                {


                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();

                    logging.SetMinimumLevel(LogLevel.Trace);

                    logging.AddZLoggerConsole(options =>
                    {
                        //options.FlushRate = TimeSpan.FromSeconds(5);

#if DEBUG
                        options.UsePlainTextFormatter(plainText =>
                        {
                            // \u001b[31m => Red(ANSI Escape Code)
                            // \u001b[0m => Reset
                            // \u001b[38;5;***m => 256 Colors(08 is Gray)
                            plainText.PrefixFormatter = (writer, info) =>
                            {
                                if (info.LogLevel == LogLevel.Error)
                                {
                                    ZString.Utf8Format(writer, "\u001b[31m[{0}]", info.LogLevel);
                                }
                                else
                                {
                                    if (!info.CategoryName.StartsWith("MyApp")) // your application namespace.
                                    {
                                        ZString.Utf8Format(writer, "\u001b[38;5;08m[{0}]", info.LogLevel);
                                    }
                                    else
                                    {
                                        ZString.Utf8Format(writer, "[{0}]", info.LogLevel);
                                    }
                                }
                            };
                            plainText.SuffixFormatter = (writer, info) =>
                            {
                                if (info.LogLevel == LogLevel.Error || !info.CategoryName.StartsWith("MyApp"))
                                {
                                    ZString.Utf8Format(writer, "\u001b[0m", "");
                                }
                            };
                        });
#endif

                    }, configureEnableAnsiEscapeCode: true);



                })
                .UseConsoleAppFramework<Program>(args)
                .Build();

            GlobalLogger.SetServiceProvider(host.Services.GetRequiredService<ILoggerFactory>(), "MyApp");

            await host.RunAsync();
        }

        readonly ILogger<Program> logger;

        public Program(ILogger<Program> logger)
        {
            this.logger = logger;
        }


        public struct MyMessage
        {
            public int Foo { get; set; }
            public int Bar { get; set; }
        }

        public async Task Run()
        {
            for (int i = 0; i < 10; i++)
            {
                logger.LogDebug("Debug Message:" + i);
                logger.LogError("Error Message:" + i);
                await Task.Delay(TimeSpan.FromSeconds(2), Context.CancellationToken);
            }



            logger.LogDebug("foo");
            logger.Log(LogLevel.Debug, default(EventId), new { a = "tako" }, null, (x, y) => x.a + "yaki");

            logger.LogInformation("started");

            var x = 1;
            logger.ZLogInformation($"abc{1}");

            var a = "a";
            logger.ZLog(LogLevel.Information, $"{a}");

            // new HoGeMoge().Foo();

            //logger.LogDebug("foooooo  {0} {1}", 10, 20);

            //logger.ZLogger(LogLevel.Debug, "hogehoge", 100,);

            //logger.ZDebug(obj, "foo{0} {1}", 100, 200);
            // Message: foo 100 200, Payload:{hoge:100, fafa:200}

            //var obj = new { Foo = "あいうえお!", Bar = "bar!" };

            //logger.ZDebug("foo {0}, bar {1}", 100, 200);
            //logger.ZDebug(obj, "foo {0}, bar {1}", obj.Foo, obj.Bar);


            var id = 10;
            var userName = "Mike";

            logger.ZLogInformation($"Registered User: Id = {id}, UserName = {userName}");


            return;

            // logger.ZLogInformation("Registered User: Id = {0}, UserName = {1}", id, userName);

            //            // {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
            //            logger.ZLogInformationWithPayload(new UserRegisteredLog
            //            {
            //                Id = id,
            //                Name = userName
            //            }, "Registered User: Id = {0}, UserName = {1}", id, userName);
            ////

            //RunExce();

            ////var message = LoggerMessage.Define<int, int, int, int>(LogLevel.Debug, default, "foo{0}bar{1}");
            //// message(

            //// logger.LogDebug(
            ////logger.ZLoggerDebug(

            //// logger.

            ////logger.ZLoggerMessage(LogLevel.Debug, "foobarbaz");
            ////logger.ZLoggerMessage(LogLevel.Debug, new { tako = 100 }, "nano");


            //var tako2 = LoggerMessage.Define<int, int, int>(LogLevel.Debug, new EventId(10, "hogehoge"), "foo{0} bar {1} baz{2}");

            //var tako = ZLoggerMessage.Define<int, int, int>(LogLevel.Debug, new EventId(10, "hogehoge"), "foo{0} bar {1} baz{2}");

            //var logmsg = ZLoggerMessage.DefineWithPayload<MyMessage, int, int, int>(LogLevel.Warning, new EventId(10, "hogehoge"), "foo{0} bar{1} baz{2}");


            //logger.ZLoggerDebug("foo{0} bar {1} baz{2}", 10, 20, 30);



            //new

            //logger.ZLogDebug(new Exception("かきくけこ"), new { 名前 = "あいうえお" }, "さしすせそ{0}", "なにぬねの");
            //logger.ZLogInformation("さしすせそ{0}", "なにぬねの");


            //tako(logger, 100, 200, 300, null);


            //var opt = new JsonSerializerOptions
            //{
            //    WriteIndented = false,
            //    IgnoreNullValues = false,
            //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            //};


            //var buff = new ArrayBufferWriter<byte>();

            //var writer = new Utf8JsonWriter(buff, new JsonWriterOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) });
            //writer.WriteStartObject();
            //writer.WritePropertyName("foo");
            //JsonSerializer.Serialize(writer, new { Tako = "あいうえお" }, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) } );
            //writer.WriteEndObject();


            //var str = Encoding.UTF8.GetString(buff.WrittenSpan);
            //Console.WriteLine(str);



            // {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}

            // {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
            //logger.ZLogInformationWithPayload(new UserLogInfo { Id = id, Name = userName }, "Registered User: Id = {0}, UserName = {1}", id, userName);



            //logger.ZLogInformationWithPayload(

            //ZLogger.ZLoggerMessage.DefineWithPayload<string,int>(LogLevel.Information, default, "").Invoke(

            //logmsg(logger, new MyMessage { Foo = 100, Bar = 200 }, 100, 200, 300, null);


            //tako(logger, 100, 200, 300,


            //await Task.Yield();

            // logger.ZLoggerDebug(

            //logger.ZLogDebug(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLogger(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLogger(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLogger(LogLevel.Debug, "foo{0}", 100);

            //await Task.Delay(TimeSpan.FromSeconds(10));
            //logger.ZLogger(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLogger(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLogger(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLogger(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLogger(LogLevel.Debug, "foo{0}", 100);
            //logger.ZLogger(LogLevel.Debug, "foo{0}", 100);




        }

        void RunExce()
        {
            try
            {
                RunExce2();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("invalid???", ex);
            }
        }
        void RunExce2()
        {
            throw new ArgumentException("invalid???");
        }
    }

    public struct Takoyaki
    {
        public string Foo { get; set; }
        public string Bar { get; set; }
    }

    public class Takoyaki2
    {
        public string? Foo { get; set; }
        public string? Bar { get; set; }
    }
}

