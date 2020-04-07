ZLogger
===
[![CircleCI](https://circleci.com/gh/Cysharp/ZLogger.svg?style=svg)](https://circleci.com/gh/Cysharp/ZLogger)

**Z**ero Allocation Text/Strcutured **Logger** for .NET Core and Unity, built on top of a Microsoft.Extensions.Logging.

Logging to standard output is very important, especially in the age of containerization, but traditionally its performance has been very slow, however anyone no concerned about it. It also supports both text logs and structured logs, which are important in cloud.

![image](https://user-images.githubusercontent.com/46207/78019524-d4238480-738a-11ea-88ac-00caa8bc5228.png)

ZLogger writes directly as UTF8 by the our zero allocation string builder [ZString](https://github.com/Cysharp/ZString). In addition, thorough generics, struct, and cache are utilized to achieve the maximum performance. Default options is set to the best for performance by the async and buffered, so you don't have to worry about the logger settings.

ZLogger is built directly on top of `Microsoft.Extensions.Logging`. By not having a separate logger framework layer, we are extracting better performance. In addition to ConsoleLogging, we provides **FileLogger**, **RollingFileLogger**, and **StreamLogger**. They too are designed to bring out the best in performance.

Getting Started
---
For .NET Core, use NuGet.

> PM> Install-Package [ZLogger](https://www.nuget.org/packages/ZLogger)

You can setup logger by [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host), for ASP.NET Core and if you want to use this in ConsoleApplication, we provides [ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework) to use hosting abstraction.

```csharp
Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        // optional(MS.E.Logging):clear default providers.
        logging.ClearProviders();
        
        // optional(MS.E.Logging): default is Info, you can use this or AddFilter to filtering log.
        logging.SetMinimumLevel(LogLevel.Debug);
        
        // Add Console Logging.
        logging.AddZLoggerConsole();

        // Add File Logging.
        logging.AddZLoggerFile("fileName.log");

        // Add Rolling File Logging.
        logging.AddZLoggerRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", x => x.ToLocalTime().Date, 1024 * 1024);
       
        // Enable Structured Logging
        logging.AddZLoggerConsole(options =>
        {
            options.UseDefaultStructuredLogFormatter();
        });
    })
```

```csharp
// log text.
logger.ZLogDebug("foo{0} bar{1}", 10, 20);

// log text with structure
logger.ZLogDebug(new { Foo = 10, Bar = 20 }, "foo{0} bar{1}", 10, 20);

// Prepared logging
var foobarLogger1 = ZLoggerMessage.Define<int, int>(LogLevel.Debug, new EventId(10, "hoge"), "foo{0} bar{1}");

// Prepared logging with structure
var foobarLogger2 = ZLoggerMessage.DefineWithPayload<MyMessage, int, int>(LogLevel.Warning, new EventId(10, "hoge"), "foo{0} bar{1}");
```

// TODO: more reference.


Providers
---


Options
---


Microsoft.CodeAnalysis.BannedApiAnalyzers
---
[Microsoft.CodeAnalysis.BannedApiAnalyzers](https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md) is an interesting analyzer, you can use this to prohibit the normal Log method.

![image](https://user-images.githubusercontent.com/46207/78545188-56ea8a80-7836-11ea-81f2-6cbf7119f027.png)

All you have to do is prepare the following configuration.

```
T:Microsoft.Extensions.Logging.LoggerExtensions;Don't use this, use ZLog*** instead.
T:System.Console;Don't use this, use logger instead.
```

Global LoggerFactory
---
Like the traditional log manager, how to get and store logger per type without DI(such as `static readonly ILogger logger = LogManager.GetLogger()`). You can get `ILoggerFactory` from `IHost` before Run and set to the global static loggerfactory store.

```csharp
var host = Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddZLoggerConsole();
    })
    .UseConsoleAppFramework<Program>(args) // use framework, example of ConsoleAppFramework
    // .ConfigureWebHostDefaults(x => x.UseStartup<Startup>()) // example of ASP.NET Core
    .Build(); // use Build instead of Run directly

// get configured loggerfactory.
var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

GlobalLogger.SetLoggerFactory(loggerFactory, "Global");

// Run after set global logger.
await host.RunAsync();

// -----

// Own static loggger manager
public static class GlobalLogger
{
    static ILogger globalLogger;
    static ILoggerFactory loggerFactory;

    public static void SetServiceProvider(ILoggerFactory loggerFactory, string categoryName)
    {
        GlobalLogger.loggerFactory = loggerFactory;
        GlobalLogger.globalLogger = loggerFactory.CreateLogger(categoryName);
    }

    public static ILogger Log => globalLogger!;

    public static ILogger<T> GetLogger<T>() where T : class => loggerFactory.CreateLogger<T>();
    public static ILogger GetLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);
}
```

You can use this loggger manager like following.

```csharp
public class Foo
{
    public static readonly ILogger<Foo> logger = GlobalLogger.GetLogger<Foo>();

    public void Foo(int x)
    {
        logger.ZLogDebug("do do do: {0}", x);
    }
}
```

Unity
---
TODO:

License
---
This library is licensed under the the MIT License.