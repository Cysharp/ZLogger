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

namespace ConsoleApp
{
    public class MyClass22
    {

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

    public class SimpleServiceProvider : IServiceProvider, IDisposable
    {
        readonly Dictionary<Type, List<ServiceItem>> items;

        public SimpleServiceProvider(IServiceCollection services)
        {
            items = new Dictionary<Type, List<ServiceItem>>();
            foreach (var item in services)
            {
                if (!items.TryGetValue(item.ServiceType, out var list))
                {
                    list = new List<ServiceItem>();
                    items.Add(item.ServiceType, list);
                }
                var serviceItem = new ServiceItem(item)
                {
                    Item = item.ImplementationInstance
                };

                list.Add(serviceItem);
            }
        }

        public void Dispose()
        {
            foreach (var item in items)
            {
                foreach (var item2 in item.Value)
                {
                    if (items is IDisposable d)
                    {
                        d.Dispose();
                    }
                }
            }
        }

        static bool IsCollection(Type type)
        {
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
                if (type == typeof(IEnumerable<>) || type == typeof(IList<>) || type == typeof(ICollection<>))
                {
                    return true;
                }
            }
            return false;
        }

        public object? GetService(Type serviceType)
        {
            return GetServiceCore1(serviceType, null);
        }

        public object? GetServiceCore1(Type serviceType, Type? genericsType)
        {
            if (!items.TryGetValue(serviceType, out var list))
            {
                if (IsCollection(serviceType))
                {
                    var elemType = serviceType.GetGenericArguments()[0];
                    return GetServiceCore2(elemType, genericsType);
                }

                Type? useGenericsType = genericsType;
                if (serviceType.IsConstructedGenericType)
                {
                    var gen0 = serviceType.GetGenericArguments()[0];
                    if (gen0.IsGenericTypeParameter && gen0.IsConstructedGenericType)
                    {
                        useGenericsType = gen0;
                    }
                    else if (!gen0.IsGenericType && !gen0.IsGenericTypeParameter)
                    {
                        useGenericsType = gen0;
                    }
                }

                if (serviceType == serviceType.GetGenericTypeDefinition())
                {
                    throw new InvalidOperationException("Can not get service from: " + serviceType);
                }

                var innerService = GetServiceCore1(serviceType.GetGenericTypeDefinition(), useGenericsType);
                return innerService;
            }

            return list.Last().Instantiate(this, genericsType);
        }

        public object?[] GetServiceCore2(Type serviceType, Type? genericsType)
        {
            if (!items.TryGetValue(serviceType, out var list))
            {
                if (IsCollection(serviceType))
                {
                    var elemType = serviceType.GetGenericArguments()[0];
                    return GetServiceCore2(elemType, genericsType);
                }

                Type? useGenericsType = genericsType;
                if (serviceType.IsConstructedGenericType)
                {
                    var gen0 = serviceType.GetGenericArguments()[0];
                    if (gen0.IsGenericTypeParameter && gen0.IsConstructedGenericType)
                    {
                        useGenericsType = gen0;
                    }
                    else if (!gen0.IsGenericType && !gen0.IsGenericTypeParameter)
                    {
                        useGenericsType = gen0;
                    }
                }

                if (serviceType == serviceType.GetGenericTypeDefinition())
                {
                    throw new InvalidOperationException("Can not get service from: " + serviceType);
                }

                var innerService = GetServiceCore1(serviceType.GetGenericTypeDefinition(), useGenericsType);
                return new object?[] { innerService };
            }

            return list.Select(x => x.Instantiate(this, genericsType)).ToArray();
        }

        class ServiceItem
        {
            public readonly ServiceDescriptor descriptor;
            public object? Item;
            public Dictionary<Type, object>? ItemPerGenerics;

            bool isGenericType;

            public ServiceItem(ServiceDescriptor descriptor)
            {
                this.descriptor = descriptor;
                this.isGenericType = descriptor.ServiceType.IsGenericType && !descriptor.ServiceType.IsConstructedGenericType;
                if (isGenericType)
                {
                    ItemPerGenerics = new Dictionary<Type, object>();
                }
            }

            public object Instantiate(SimpleServiceProvider provider, Type? genericsElementType)
            {
                if (descriptor.Lifetime == ServiceLifetime.Singleton)
                {
                    if (isGenericType && genericsElementType != null)
                    {
                        if (ItemPerGenerics.TryGetValue(genericsElementType, out var value))
                        {
                            return value;
                        }
                    }
                    else
                    {
                        if (Item != null)
                        {
                            return Item;
                        }
                    }
                }

                lock (this)
                {
                    object? instance;
                    if (descriptor.ImplementationFactory != null)
                    {
                        instance = descriptor.ImplementationFactory(provider);
                    }
                    else
                    {
                        var implType = descriptor.ImplementationType;
                        if (isGenericType && genericsElementType != null)
                        {
                            implType = implType.MakeGenericType(genericsElementType);
                        }

                        var ctor = descriptor.ImplementationType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
                        var parameters = ctor.GetParameters();
                        var args = parameters.Select(x => provider.GetServiceCore1(x.ParameterType, genericsElementType)).ToArray();
                        instance = ctor.Invoke(args);
                    }

                    if (descriptor.Lifetime == ServiceLifetime.Singleton)
                    {
                        if (isGenericType && genericsElementType != null)
                        {
                            ItemPerGenerics.TryAdd(genericsElementType, instance);
                        }
                        else
                        {
                            Item = instance;
                        }
                        return instance;
                    }
                    else
                    {
                        // Transient, Scoped(not supported, same as Transient).
                        return instance;
                    }
                }
            }
        }
    }

    public static class UnityLoggerFactory
    {
        public static ILoggerFactory Create(Action<ILoggingBuilder> configure)
        {
            var services = new ServiceCollection();
            services.AddLogging(configure);

            // use simple ServiceProvider for IL2CPP.
            var provider = new SimpleServiceProvider(services);

            var loggerFactory = provider.GetService<ILoggerFactory>();
            // new LoggerFactory(
            return new DisposingLoggerFactory(loggerFactory, provider);
        }

        class DisposingLoggerFactory : ILoggerFactory, IDisposable
        {
            readonly ILoggerFactory loggerFactory;
            readonly SimpleServiceProvider serviceProvider;

            public DisposingLoggerFactory(ILoggerFactory loggerFactory, SimpleServiceProvider serviceProvider)
            {
                this.loggerFactory = loggerFactory;
                this.serviceProvider = serviceProvider;
            }

            public void Dispose()
            {
                serviceProvider.Dispose();
            }

            public ILogger CreateLogger(string categoryName)
            {
                return loggerFactory.CreateLogger(categoryName);
            }

            public void AddProvider(ILoggerProvider provider)
            {
                loggerFactory.AddProvider(provider);
            }
        }
    }


    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            var factory = UnityLoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddZLoggerConsole();
                builder.AddZLoggerFile("foo.log");
            });

            //var services = new ServiceCollection();
            //services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptions<>), typeof(OptionsManager<>)));
            //services.TryAdd(ServiceDescriptor.Scoped(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>)));
            //services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>)));
            //services.TryAdd(ServiceDescriptor.Transient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>)));
            //services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsMonitorCache<>), typeof(OptionsCache<>)));

            //services.AddLogging();
            
            
            //var provider = new SimpleServiceProvider(services);


            //var myOption = provider.GetService<IOptions<ZLoggerOptions>>();


            //services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
            //services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            //var serviceProvider = services.BuildServiceProvider();

            //var myLogger = factory.CreateLogger<ILogger<Program>>();





            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();

                    // optional(MS.E.Logging): default is Info, you can use this or AddFilter to filtering log.
                    logging.SetMinimumLevel(LogLevel.Debug);

                    //    .AddZLoggerConsole(configure =>
                    //    {
                    //        configure.IsStructuredLogging = true;
                    //    });


                    // logging.AddFilter(





                    logging.AddFilter<ZLoggerConsoleLoggerProvider>(x => true).AddZLoggerConsole();

                    logging.AddFilter<SimpleConsoleLoggerProvider>(x => x == LogLevel.Debug).AddSimpleConsole();

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

                    logging.AddZLoggerConsole();
                    logging.AddZLoggerFile("foo.log");


                    //logging.AddZLoggerConsole(x =>
                    //{
                    //    //x.PrefixFormatter = (writer, info) =>
                    //    //{
                    //    //    Console.Write(info.LogLevel);




                    //    //x.UseDefaultStructuredLogFormatter();
                    //    //};
                    //});
                    //logging.AddZLoggerConsole(options =>
                    //{
                    //    // options.UseDefaultStructuredLogFormatter();
                    //});
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









            //logmsg(logger, new MyMessage { Foo = 100, Bar = 200 }, 100, 200, 300, null);


            //tako(logger, 100, 200, 300,


            //await Task.Yield();

            // logger.ZLoggerDebug(

            logger.ZLogDebug(LogLevel.Debug, "foo{0}", 100);
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

                Console.WriteLine(obj.Bar);
            }, null);

        }
    }

}
