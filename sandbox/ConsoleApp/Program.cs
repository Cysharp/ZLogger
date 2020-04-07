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

namespace ConsoleApp
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
            logger.ZLogDebug("do do do: {0}", x);
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

    public class UserModel
    {
        static readonly Action<ILogger, UserRegisteredLog, int, string, Exception?> registerdUser = ZLoggerMessage.Define<UserRegisteredLog, int, string>(LogLevel.Information, new EventId(9, "RegisteredUser"), "Registered User: Id = {0}, UserName = {1}");

        readonly ILogger<UserModel> logger;

        public UserModel(ILogger<UserModel> logger)
        {
            this.logger = logger;
        }

        public void RegisterUser(int id, string name)
        {
            // ...do anything

            // use defined delegate instead of ZLog.
            registerdUser(logger, new UserRegisteredLog { Id = id, Name = name }, id, name, null);
        }
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
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();

                    // optional(MS.E.Logging): default is Info, you can use this or AddFilter to filtering log.
                    logging.SetMinimumLevel(LogLevel.Trace);

                    //    .AddZLoggerConsole(configure =>
                    //    {
                    //        configure.IsStructuredLogging = true;
                    //    });


                    // logging.AddFilter(



                    logging.AddFilter<ZLoggerConsoleLoggerProvider>(level => level == LogLevel.Information);
                    logging.AddFilter<ZLoggerFileLoggerProvider>(level => level == LogLevel.Trace);






                    //logging.AddFilter<ZLoggerFileLoggerProvider>(



                    //logging.AddFilter<ZLoggerConsoleLoggerProvider>(x => true).AddZLoggerConsole();

                    //logging.AddFilter<SimpleConsoleLoggerProvider>(x => x == LogLevel.Debug).AddSimpleConsole();

                    //logging.AddFilter((category, level) =>
                    //    {
                    //        if (category == "Microsoft.Extensions.Hosting.Internal.Host") return true;
                    //        return false;
                    //    })
                    //    .AddZLoggerConsole();


                    //logging.AddFilter((category, level) =>
                    //{
                    //    if (category != "Microsoft.Extensions.Hosting.Internal.Host") return true;
                    //    return false;
                    //})
                    //       .AddConsole();


                    /*
                    logging.AddZLoggerRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd_HH-mm-ss}_{x:000}.log",
                        x => { var time = x.ToLocalTime(); return new DateTimeOffset(time.Year, time.Month, time.Day, 0, 0, 0, time.Second, TimeSpan.Zero); },
                        1024);
                        */

                    // logging.ReplaceToSimpleConsole();

                    //logging.AddZLoggerRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", x => x.ToLocalTime().Date, 1024 * 1024);


                    //logging.AddZLoggerLogProcessor(new Processor());


                    //logging.AddZLoggerFile("filelog.log");

                    //logging.AddZLoggerConsole();
                    //logging.AddZLoggerFile("foo.log");


                    //logging.AddZLoggerConsole(x =>
                    //{
                    //    //x.PrefixFormatter = (writer, info) =>
                    //    //{
                    //    //    ZString.Utf8Format(writer, "[{0}]", info.LogLevel);
                    //    //};

                    //    //Utf16PreparedFormat

                    //    // x.IsStructuredLogging = true;

                    //    //x.SuffixFormatter = (writer, info) => prepared.FormatTo(ref writer, info.LogLevel);
                    //});

                    var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect("127.0.0.1", 12345);
                    var network = new NetworkStream(socket);

                    logging.AddZLoggerStream(network);





                    var gitHash = Guid.NewGuid().ToString();


                    logging.AddZLoggerConsole(options =>
                    {
                        options.EnableStructuredLogging = true;

                        var gitHashName = JsonEncodedText.Encode("GitHash");
                        var gitHashValue = JsonEncodedText.Encode(gitHash);

                        options.StructuredLoggingFormatter = (writer, info) =>
                        {
                            writer.WriteString(gitHashName, gitHashValue);
                            info.WriteToJsonWriter(writer);
                        };
                    });

                    logging.AddZLoggerRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", x => x.ToLocalTime().Date, 1024);
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
            // new HoGeMoge().Foo();

            //logger.LogDebug("foooooo  {0} {1}", 10, 20);

            //logger.ZLogger(LogLevel.Debug, "hogehoge", 100,);

            //logger.ZDebug(obj, "foo{0} {1}", 100, 200);
            // Message: foo 100 200, Payload:{hoge:100, fafa:200}

            //var obj = new { Foo = "あいうえお!", Bar = "bar!" };

            //logger.ZDebug("foo {0}, bar {1}", 100, 200);
            //logger.ZDebug(obj, "foo {0}, bar {1}", obj.Foo, obj.Bar);






            logger.ZLogInformation("Registered User: Id = {0}, UserName = {1}", id, userName);

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

            var id = 10;
            var userName = "Mike";


            // {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
            logger.ZLogInformation("Registered User: Id = {0}, UserName = {1}", id, userName);

            // {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
            logger.ZLogInformationWithPayload(new UserLogInfo { Id = id, Name = userName }, "Registered User: Id = {0}, UserName = {1}", id, userName);



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




            await Task.Yield();
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

    public class Processor : IAsyncLogProcessor
    {
        public ValueTask DisposeAsync()
        {
            return default;
        }

        public void Post(IZLoggerEntry log)
        {
            //var takoyaki = log.GetPayloadAs<Takoyaki2>();

            log.SwitchCasePayload<Takoyaki>((entry, obj, state) =>
            {

                // Console.WriteLine(obj.Bar);
            }, null);

        }
    }

}
