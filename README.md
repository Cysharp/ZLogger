ZLogger
===
[![CircleCI](https://circleci.com/gh/Cysharp/ZLogger.svg?style=svg)](https://circleci.com/gh/Cysharp/ZLogger)

**Z**ero Allocation Text/Strcutured **Logger** for .NET Core and Unity, built on top of a Microsoft.Extensions.Logging.

Logging to standard output is very important, especially in the age of containerization, but traditionally its performance has been very slow, however anyone no concerned about it. It also supports both text logs and structured logs, which are important in cloud.

![image](https://user-images.githubusercontent.com/46207/78019524-d4238480-738a-11ea-88ac-00caa8bc5228.png)

ZLogger writes directly as UTF8 by the our zero allocation string builder [ZString](https://github.com/Cysharp/ZString). In addition, thorough generics, struct, and cache are utilized to achieve the maximum performance. Default options is set to the best for performance by the async and buffered, so you don't have to worry about the logger settings.

ZLogger is built directly on top of `Microsoft.Extensions.Logging`. By not having a separate logger framework layer, we are extracting better performance. In addition to ConsoleLogging, we provides **FileLogger**, **RollingFileLogger**, and **StreamLogger**. They too are designed to bring out the best in performance, write to UTF8 directly.

Getting Started
---
For .NET Core, use NuGet. For Unity, please read [Unity](#unity) section.

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
        logging.AddZLoggerRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", x => x.ToLocalTime().Date, 1024);
       
        // Enable Structured Logging
        logging.AddZLoggerConsole(options =>
        {
            options.EnableStructuredLogging = true;
        });
    })
```

```csharp
// log text.
logger.ZLogDebug("foo{0} bar{1}", 10, 20);

// log text with structure
logger.ZLogDebugWithPayload(new { Foo = 10, Bar = 20 }, "foo{0} bar{1}", 10, 20);

// Prepared logging
var foobarLogger1 = ZLoggerMessage.Define<int, int>(LogLevel.Debug, new EventId(10, "hoge"), "foo{0} bar{1}");

// Prepared logging with structure
var foobarLogger2 = ZLoggerMessage.DefineWithPayload<MyMessage, int, int>(LogLevel.Warning, new EventId(10, "hoge"), "foo{0} bar{1}");
```

// TODO: more reference.




Structured Logging
---
TODO:

Filters
---
This is the Microsoft.Extensions.Logging rules, if more than one filter is set, only the first match of type -> category and finally no category, SetMinimumLevel will be applied.

If you get confused, only to use `AddFilter<T>(Func<string, LogLevel, bool> categoryLevelFilter)` will help you.

Output Providers
---
ZLogger has the following providers by default.

|Type|Alias|Builder Extension
|-|-|-|
|ZLoggerConsoleLoggerProvider|ZLoggerConsole|AddZLoggerConsole
|ZLoggerFileLoggerProvider|ZLoggerFile|AddZLoggerFile
|ZLoggerRollingFileLoggerProvider|ZLoggerRollingFile|AddZLoggerRollingFile
|ZLoggerStreamLoggerProvider|ZLoggerStream|AddZLoggerStream
|ZLoggerLogProcessorLoggerProvider|ZLoggerLogProcessor|AddZLoggerLogProcessor

Type is used in `AddFilter<T>`, Alias is used when configuring filters from Option, and Builder Extensions is used in `ConfigureLogging`.

All providers write logs asynchronously and buffered; when logger call a Log method, they don't format it, they just store it in a queue, so they don't stop the calling thread at all. The buffer size is 65536, it will flush when the buffer is overflowing or the wait queue is empty.

### Console

Output to the Console.

```csharp
logging.AddZLoggerConsole();
```

It is useful for ConsoleApplication and containerized applications on cloud. For example, AWS CloudWatch, GCP Stackdriver Logging, and Datadog Logs agent collect data on the standard output. This is especially useful for debugging and analysis if these are output in a structured log.

If `consoleOutputEncodingToUtf8 = true`(default is true), set `Console.OutputEncoding = new UTF8Encoding(false)` when the provider is created.

### File

Output to the file to specified `string filePath`. The file will be appended to the last line.

```csharp
logging.AddZLoggerFile("applog.log");
```

### RollingFile

Changes the output file path, depending on date-time or file size.

```csharp
logging.AddZLoggerRollingFile((dt, x) => 
    fileNameSelector: $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", 
    timestampPattern: x => x.ToLocalTime().Date, 
    rollSizeKB: 1024);
```

The specification is a little more complicated for performance reasons of avoiding string allocations/comparisons.

* `Func<DateTimeOffset, int, string> fileNameSelector`
* `Func<DateTimeOffset, DateTimeOffset> timestampPattern`
* `int rollSizeKB`

`fileNameSelector` is selector of generated file path. `DateTimeOffset` is UTC of creating time, `int` is sequence no, it is 0 origin. fileNameSelector's format must be int(sequence no) is last.

`timestampPattern` is predicate of should generate new file. The argument is the current time (UTC) when the log is written, and if the return value is different from the last time it was written, it calls fileNameSelector and writes to a new file.

`rollSizeKB` is limit file size, if there is an overflowed, it calls fileNameSelector and writes to a new file.

The generated files will not be deleted. If you want to do a log rotation, please do it from outside.

### Stream
TODO:

### LogProcessor
TODO:

Preaparing Message
---
TODO:

Options
---
`ZLoggerOptions` can be configured with `Action<ZLoggerOptions> configure` when adding to `ILoggingBuilder`. The options applied to StructuredLogging(`EnableStructuredLogging = true`) are different from those applied to TextLogging. The default is TextLogging(`EnableStructuredLogging = false`).

### Common

* `bool EnableStructuredLogging`
* `Action<Exception>? InternalErrorLogger`

InternalErrorLogger is an delegate of when exception occured in log writing process(such as serialization error). Default is `Console.WriteLine(exception)`.

### Options for Text Logging

* `Action<IBufferWriter<byte>, LogInfo>? PrefixFormatter`
* `Action<IBufferWriter<byte>, LogInfo>? SuffixFormatter`
* `Action<IBufferWriter<byte>, Exception> ExceptionFormatter`

For performance reason, we do not use string so use the `IBufferWriter<byte>` instead. You can use `ZString.Utf8Format` to help set formatter.

```csharp
logging.AddZLoggerConsole(option =>
{
    option.PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "[{0}][{1}]", info.LogLevel, info.Timestamp.DateTime.ToLocalTime());

    // Tips: use PrepareUtf8 to achive better performance.
    var prefixFormat = ZString.PrepareUtf8<LogLevel, DateTime>("[{0}][{1}]");
    option.PrefixFormatter = (writer, info) => prefixFormat.FormatTo(ref writer, info.LogLevel, info.Timestamp.DateTime.ToLocalTime());
});

// output:
// [Information][04/07/2020 20:21:46]fooooo!
logger.ZLogInformation("fooooo!");
```

LogInfo has these informations.

```csharp
public readonly struct LogInfo
{
    public readonly string CategoryName;
    public readonly DateTimeOffset Timestamp;
    public readonly LogLevel LogLevel;
    public readonly EventId EventId;
    public readonly Exception? Exception;

    public void WriteToJsonWriter(Utf8JsonWriter writer);
}
```

`LogInfo.Timestamp` is UTC, if you want to output human-readable local time, use `.ToLocalTime()`.

`ExceptionFormatter` is called when `LogInfo.Exception` is not null. Default is `\n + exception`.

### Options for Structured Logging

* `Action<Utf8JsonWriter, LogInfo> StructuredLoggingFormatter`
* `JsonEncodedText MessagePropertyName`
* `JsonEncodedText PayloadPropertyName`
* `JsonSerializerOptions JsonSerializerOptions`

The StructuredLoggingFormatter is called when `EnableStructuredLogging = true`. `LogInfo.WriteToJsonWriter` is defined by default, which writes all LogInfo properties.

```csharp
logging.AddZLoggerConsole(option =>
{
    options.EnableStructuredLogging = true;
});

// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
logger.ZLogInformation("Registered User: Id = {0}, UserName = {1}", id, userName);

// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
logger.ZLogInformationWithPayload(new UserLogInfo { Id = id, Name = userName }, "Registered User: Id = {0}, UserName = {1}", id, userName);

// Due to the constraints of System.Text.JSON.JSONSerializer,
// only properties can be serialized.
public struct MyMessage
{
    public int Foo { get; set; }
    public int Bar { get; set; }
}
```

Write as JSON in the order StructuredLoggingFormatter -> Message -> Payload. The property names "Message" and "Payload" may be changed by "MessagePropertyName" and "PayloadPropertyName".

`ZLog...WithPayload` methods are only meaningful for StructuredLogging. When `EnableStructuredLogging = true`, payload is serialized to JSON by `System.Text.Json.JsonSerializer`. (`ZLogger` does not support [Message Templates](https://messagetemplates.org/), if you want to output a payload, you must pass an object/struct.)

If you want to add additional information to the JSON, modify the StructuredLoggingFormatter as follows, for example

```csharp
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

// {"GitHash":"XXXX","CategoryName":...,"Message":"...","Payload":...}
```

You can change the serialization behavior of the payload by changing the `JsonSerializerOptions`. If you want to set up a custom Converter, set it here. By default, the following configuration is used

```csharp
new JsonSerializerOptions
{
    WriteIndented = false,
    IgnoreNullValues = false,
    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
}
```

Microsoft.CodeAnalysis.BannedApiAnalyzers
---
[Microsoft.CodeAnalysis.BannedApiAnalyzers](https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md) is an interesting analyzer, you can prohibit the normal Log method and induce the user to call ZLogger's ZLog method.

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

LogManager.SetLoggerFactory(loggerFactory, "Global");

// Run after set global logger.
await host.RunAsync();

// -----

// Own static loggger manager
public static class LogManager
{
    static ILogger globalLogger;
    static ILoggerFactory loggerFactory;

    public static void SetServiceProvider(ILoggerFactory loggerFactory, string categoryName)
    {
        LogManager.loggerFactory = loggerFactory;
        LogManager.globalLogger = loggerFactory.CreateLogger(categoryName);
    }

    public static ILogger Logger => globalLogger;

    public static ILogger<T> GetLogger<T>() where T : class => loggerFactory.CreateLogger<T>();
    public static ILogger GetLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);
}
```

You can use this loggger manager like following.

```csharp
public class Foo
{
    public static readonly ILogger<Foo> logger = LogManager.GetLogger<Foo>();

    public void Foo(int x)
    {
        logger.ZLogDebug("do do do: {0}", x);
    }
}
```

Unity
---
ZLogger can also be used in Unity. unitypackage is in [ZLogger/Releases](https://github.com/Cysharp/ZLogger/releases), so if you download it and extract it, it will extract a file that contains a dll from Microsoft.Extensions.Logging. However, because [ZString](https://github.com/Cysharp/ZString/) is not included, the download of ZString is also necessary.

Here is the sample of usage, use `UnityLoggerFactory` and store logger factory to field. You can use `AddZLoggerUnityDebug` to show ZLogger entry to Unity.Debug.Log.

```csharp
public static class LogManager
{
    static Microsoft.Extensions.Logging.ILogger globalLogger;
    static ILoggerFactory loggerFactory;

    // Setup on first called GetLogger<T>.
    static LogManager()
    {
        // Standard LoggerFactory does not work on IL2CPP,
        // But you can use ZLogger's UnityLoggerFactory instead,
        // it works on IL2CPP, all platforms(includes mobile).
        loggerFactory = UnityLoggerFactory.Create(builder =>
        {
            // or more configuration, you can use builder.AddFilter
            builder.SetMinimumLevel(LogLevel.Trace);

            // AddZLoggerUnityDebug is only available for Unity, it send log to UnityEngine.Debug.Log.
            // LogLevels are translate to
            // * Trace/Debug/Information -> LogType.Log
            // * Warning/Critical -> LogType.Warning
            // * Error without Exception -> LogType.Error
            // * Error with Exception -> LogException
            builder.AddZLoggerUnityDebug();

            // and other configuration(AddFileLog, etc...)
        });

        globalLogger = loggerFactory.GetLogger("Global");

        Application.quitting += () =>
        {
            // when quit, flush unfinished log entries.
            loggerFactory.Dispose();
        };
    }

    public static Microsoft.Extensions.Logging.ILogger Loggger => globalLogger;

    public static ILogger<T> GetLogger<T>() where T : class => loggerFactory.CreateLogger<T>();
    public static Microsoft.Extensions.Logging.ILogger GetLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);
}

// ----

public class MyScript : MonoBehaviour
{
    // store logger per class to static readonly field.
    static readonly ILogger<MyScript> logger = LogManager.GetLogger<MyScript>();

    void Start()
    {
        logger.ZLogDebug("Init!");
    }
}
```

The advantages of using ZLogger include more log levels, filtering, automatic category(typename from `GetLogger<T>`) granting, and common log headers/footers than the standard Unity logger. Adding categories can be very useful for filtering logs using something like [EditorConsolePro](https://assetstore.unity.com/packages/tools/utilities/editor-console-pro-11889) (e.g. [UI], [Battle], [Network], etc.).

You can also use `ZLoggerFileLoggerProvider`, `ZLoggerRollingFileLoggerProvider` to write out logs to files, which can be useful if you want to output as a PC application(Steam, VR, etc...). You can also use `ZLoggerStreamLoggerProvider` or `ZLoggerLogProcessorProvider` as an extension point for your own log output.

License
---
This library is licensed under the the MIT License.