ZLogger
===
[![GitHub Actions](https://github.com/Cysharp/ZLogger/workflows/Build-Debug/badge.svg)](https://github.com/Cysharp/ZLogger/actions) [![Releases](https://img.shields.io/github/release/Cysharp/ZLogger.svg)](https://github.com/Cysharp/ZLogger/releases)

**Z**ero Allocation Text/Structured **Logger** for .NET Core and Unity, built on top of a Microsoft.Extensions.Logging.

Logging to standard output is very important, especially in the age of containerization(described in [The Twelve Factor App - Logs](https://12factor.net/logs) saids should write to stdout), but traditionally its performance has been very slow, however anyone no concerned about it. It also supports both text logs and structured logs, which are important in cloud log management.

![image](https://user-images.githubusercontent.com/46207/78019524-d4238480-738a-11ea-88ac-00caa8bc5228.png)

ZLogger writes directly as UTF8 by the our zero allocation string builder [ZString](https://github.com/Cysharp/ZString). In addition, thorough generics, struct, and cache are utilized to achieve the maximum performance. Default options is set to the best for performance by the async and buffered, so you don't have to worry about the logger settings.

ZLogger is built directly on top of `Microsoft.Extensions.Logging`. By not having a separate logger framework layer, we are extracting better performance. In addition to ConsoleLogging, we provides **FileLogger**, **RollingFileLogger**, and **StreamLogger**. They too are designed to bring out the best in performance, write to UTF8 directly.

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table of Contents

- [Getting Started](#getting-started)
- [Structured Logging](#structured-logging)
- [Filters](#filters)
- [Output Providers](#output-providers)
  - [Console](#console)
  - [File](#file)
  - [RollingFile](#rollingfile)
  - [Stream](#stream)
  - [LogProcessor](#logprocessor)
- [Multiple Providers](#multiple-providers)
- [Preparing Message Format](#preparing-message-format)
- [Format and DateTime Handling](#format-and-datetime-handling)
- [Options](#options)
  - [Common](#common)
  - [Options for Text Logging](#options-for-text-logging)
  - [Options for Structured Logging](#options-for-structured-logging)
- [Microsoft.CodeAnalysis.BannedApiAnalyzers](#microsoftcodeanalysisbannedapianalyzers)
- [Global LoggerFactory](#global-loggerfactory)
- [Unity](#unity)
- [License](#license)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

Getting Started
---
For .NET Core, use NuGet. For Unity(ZLogger for Unity run on IL2CPP, all platforms), please read [Unity](#unity) section.

> PM> Install-Package [ZLogger](https://www.nuget.org/packages/ZLogger)

You can setup logger by [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host), for ASP.NET Core and if you want to use this in ConsoleApplication, we provides [ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework) to use hosting abstraction.

```csharp
using ZLogger; // namespace

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
public class MyClass
{
    readonly ILogger<MyClass> logger;

    // get logger from DI.
    public class MyClass(ILogger<MyClass> logger)
    {
        this.logger = logger;
    }

    public void Foo()
    {
        // log text.
        logger.ZLogDebug("foo{0} bar{1}", 10, 20);

        // log text with structure in Structured Logging.
        logger.ZLogDebugWithPayload(new { Foo = 10, Bar = 20 }, "foo{0} bar{1}", 10, 20);
    }
}
```

The setup and get the logger follows [Microsoft.Extensions.Logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/). However, writing logs uses `ZLog`, `ZLogDebug`, `ZLogException`, etc. with a prefix of **Z**. 

All logging methods are completely similar as [Microsoft.Extensions.Logging.LoggerExtensions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loggerextensions), but it has **Z** prefix and has many generics overload to avoid allocation of boxing.

```csharp
// ZLog, ZLogTrace, ZLogDebug, ZLogInformation, ZLogWarning, ZLogError, ZLogCritical and *WithPayload.
public static void ZLogDebug(this ILogger logger, string format);
public static void ZLogDebug(this ILogger logger, EventId eventId, string format);
public static void ZLogDebug(this ILogger logger, Exception? exception, string format);
public static void ZLogDebug(this ILogger logger, EventId eventId, Exception? exception, string format);
public static void ZLogDebug<T1>(this ILogger logger, string format, T1 arg1);
public static void ZLogDebug<T1>(this ILogger logger, EventId eventId, string format, T1 arg1);
public static void ZLogDebug<T1>(this ILogger logger, Exception? exception, string format, T1 arg1);
public static void ZLogDebug<T1>(this ILogger logger, EventId eventId, Exception? exception, string format, T1 arg1);
// T1~T16
public static void ZLogDebug<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this ILogger logger, EventId eventId, Exception? exception, string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
```

If you want to replace an existing .NET Core logger, you can setup the builder.AddZLogger and simply replace LogDebug -> ZLogDebug. If you want to check and prohibit standard log methods, see the [Microsoft.CodeAnalysis.BannedApiAnalyzers](#microsoftcodeanalysisbannedapianalyzers) section. If you want to use the logger without DI, see the [Global LoggerFactory](#global-loggerfactory) section.

If you want to use without .NET Generic Host(for simple use, for unit testing, etc.), create the logger factory and store to static/singleton field to it.

```csharp
// in real case, store to static/singleton field.
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.ClearProviders();
    builder.SetMinimumLevel(LogLevel.Debug);
    builder.AddZLoggerConsole();
});

// logger should store to static/singleton field.
var logger = loggerFactory.CreateLogger<Foo>();

// on application quit, Dispose logger factory(flush and wait remains log entries).
loggerFactory.Dispose();
```

Configuring message format(add LogLevel prefix, timestamp prefix, etc...), please see [Options for Text Logging](#options-for-text-logging) section.

Structured Logging
---
Structured logging is important for cloud logging. For example, Stackdriver Logging, Datadog logs, etc..., are provides fileter, query log by simple syntax. Or store to storage by Structured Log(JSON), Amazon Athena, Google BigQuery, Azure Data Lake, etc..., you can query and analyze many log files.

ZLogger natively supports StructuredLogging and uses System.Text.Json.JsonSerializer to achieve complete zero-allocation in the pipeline without ever converting it to a string.

```csharp
// To setup, `EnableStructuredLogging = true`.
logging.AddZLoggerConsole(options =>
{
    options.EnableStructuredLogging = true;
});

// In default, output JSON with log information(categoryName, level, timestamp, exception), message and payload(if exists).

// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":null}
logger.ZLogInformation("Registered User: Id = {0}, UserName = {1}", id, userName);

// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
logger.ZLogInformationWithPayload(new UserRegisteredLog { Id = id, Name = userName }, "Registered User: Id = {0}, UserName = {1}", id, userName);
```

Write log by JSON, supports extra information(category-name, log-level, timestamp and custom metadatas) + message, info + message + custom payload.

To details, see [Options for Structured Logging](#options-for-structured-logging) section.

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

Output to the file that is changed the output file path, depending on date-time or file size.

```csharp
logging.AddZLoggerRollingFile(
    fileNameSelector: (dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", 
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

Output to the Stream. This is useful when writing data to a MemoryStream or a NetworkStream.

```csharp
var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
socket.Connect("127.0.0.1", 12345);
var network = new NetworkStream(socket);
                    
logging.AddZLoggerStream(network);
```

Stream methods are only called `Write(byte[] buffer, int offset, int count)` and `Flush()` from ThreadPool thread. If you implement custom stream, you can hook the write/flush timing and raw buffer data.

### LogProcessor

Output to the custom `IAsyncLogProcessor`. You can create custom output for each message by implementing the `Post(IZLoggerEntry)` method, which is called synchronously when the Log method is called.

```csharp
public interface IAsyncLogProcessor : IAsyncDisposable
{
    void Post(IZLoggerEntry log);

    // DisposeAsync is called when LoggerFactory is disposed(application stopped).
    ValueTask DisposeAsync();
}

public interface IZLoggerEntry
{
    LogInfo LogInfo { get; }
    void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter);
    void SwitchCasePayload<TPayload>(Action<IZLoggerEntry, TPayload, object?> payloadCallback, object? state);
    object? GetPayload();
    void Return();

    // Extension Methods
    string FormatToString(ZLoggerOptions options, Utf8JsonWriter? jsonWriter);
}
```

`IZLoggerEntry` is poolable value, you should call `Return` after used. For example, this is routing log entry to UnityEngine's logger.

```csharp
public void Post(IZLoggerEntry log)
{
    try
    {
        var msg = log.FormatToString(options, null);
        switch (log.LogInfo.LogLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
            case LogLevel.Information:
                UnityEngine.Debug.Log(msg);
                break;
            case LogLevel.Warning:
            case LogLevel.Critical:
                UnityEngine.Debug.LogWarning(msg);
                break;
            case LogLevel.Error:
                if (log.LogInfo.Exception != null)
                {
                    UnityEngine.Debug.LogException(log.LogInfo.Exception);
                }
                else
                {
                    UnityEngine.Debug.LogError(msg);
                }
                break;
            case LogLevel.None:
                break;
            default:
                break;
        }
    }
    finally
    {
        // return to pool.
        log.Return();
    }
}
```

Multiple Providers
---
ZLogger allows to add multiple same type providers. In this case, you need to give it a different name in string optionName.

```csharp
logging.AddZLoggerFile("plain-text.log", "file-plain", x => { x.PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "[{0}]", info.Timestamp.ToLocalTime().DateTime); });
logging.AddZLoggerFile("json.log", "file-structured", x => { x.EnableStructuredLogging = true; });
```

Preparing Message Format
---
As introduced in [High-performance logging with LoggerMessage in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage), ZLogger also allows logging with parsed strings in `ZLoggerMessage.Define` and `DefineWithPayload`.

```csharp
public class UserModel
{
    static readonly Action<ILogger, int, string, Exception?> registerdUser = ZLoggerMessage.Define<int, string>(LogLevel.Information, new EventId(9, "RegisteredUser"), "Registered User: Id = {0}, UserName = {1}");

    readonly ILogger<UserModel> logger;

    public UserModel(ILogger<UserModel> logger)
    {
        this.logger = logger;
    }

    public void RegisterUser(int id, string name)
    {
        // ...do anything

        // use defined delegate instead of ZLog.
        registerdUser(logger, id, name, null);
    }
}
```

If you also want to use Payload in StructuredLogging, you can call DefineWithPayload.

```csharp
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
    
    public struct UserRegisteredLog
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
```

Format and DateTime Handling
---
ZLogger's format string internaly using ZString's format under it uses dotnet [Utf8Formatter.TryFormat](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8formatter.tryformat). There format string is not same as standard format. It uses [StandardFormat](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.standardformat), combinate of symbol char and precision. Supported format string symbol can find in Utf8Formatter.TryFormat document(For example Int32 supports `G`, `D`, `N`, `X` and Boolean supports `G`, `I`). Precision(zero padding) can pass after symbol like `D2`. For example `logger.ZDebug("{0:D2}:{1:D2}:{2:D2}", hour, minute, second)`.

[TryFormat(DateTime)](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8formatter.tryformat?view=netcore-3.1#System_Buffers_Text_Utf8Formatter_TryFormat_System_DateTime_System_Span_System_Byte__System_Int32__System_Buffers_StandardFormat_)(also DateTimeOffset) and [TryFormat(TimeSpan)](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8formatter.tryformat?view=netcore-3.1#System_Buffers_Text_Utf8Formatter_TryFormat_System_TimeSpan_System_Span_System_Byte__System_Int32__System_Buffers_StandardFormat_) symbol is too restricted than standard string format. If you want to use custom format, deconstruct there `Day`, `Hour`, etc.

Options
---
`ZLoggerOptions` can be configured with `Action<ZLoggerOptions> configure` when adding to `ILoggingBuilder`. The options applied to StructuredLogging(`EnableStructuredLogging = true`) are different from those applied to TextLogging. The default is TextLogging(`EnableStructuredLogging = false`).

### Common

* `bool EnableStructuredLogging`
* `Action<LogInfo, Exception>? InternalErrorLogger`
* `TimeSpan? FlushRate`

InternalErrorLogger is an delegate of when exception occured in log writing process(such as serialization error). Default is `Console.WriteLine(exception)`.

`FlushRate` is flush rate of buffer write. Default is null that flush immediately when thread is free, it is recommended option for performance.

### Options for Text Logging

* `Action<IBufferWriter<byte>, LogInfo>? PrefixFormatter`
* `Action<IBufferWriter<byte>, LogInfo>? SuffixFormatter`
* `Action<IBufferWriter<byte>, Exception> ExceptionFormatter`

For performance reason, we do not use string so use the `IBufferWriter<byte>` instead. You can use `ZString.Utf8Format` to help set formatter.

```csharp
logging.AddZLoggerConsole(options =>
{
    options.PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "[{0}][{1}]", info.LogLevel, info.Timestamp.DateTime.ToLocalTime());

    // Tips: use PrepareUtf8 to achive better performance.
    var prefixFormat = ZString.PrepareUtf8<LogLevel, DateTime>("[{0}][{1}]");
    options.PrefixFormatter = (writer, info) => prefixFormat.FormatTo(ref writer, info.LogLevel, info.Timestamp.DateTime.ToLocalTime());
});

// output:
// [Information][04/07/2020 20:21:46]fooooo!
logger.ZLogInformation("fooooo!");
```

Note: formatting DateTime, see [Format and DateTime Handling](#format-and-datetime-handling) section.

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
logging.AddZLoggerConsole(options =>
{
    options.EnableStructuredLogging = true;
});

// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":null}
logger.ZLogInformation("Registered User: Id = {0}, UserName = {1}", id, userName);

// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
logger.ZLogInformationWithPayload(new UserRegisteredLog { Id = id, Name = userName }, "Registered User: Id = {0}, UserName = {1}", id, userName);

// Due to the constraints of System.Text.JSON.JSONSerializer,
// only properties can be serialized.
public struct UserRegisteredLog
{
    public int Id { get; set; }
    public string Name { get; set; }
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
logger.ZLog(....
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

// Own static logger manager
public static class LogManager
{
    static ILogger globalLogger;
    static ILoggerFactory loggerFactory;

    public static void SetLoggerFactory(ILoggerFactory loggerFactory, string categoryName)
    {
        LogManager.loggerFactory = loggerFactory;
        LogManager.globalLogger = loggerFactory.CreateLogger(categoryName);
    }

    public static ILogger Logger => globalLogger;

    public static ILogger<T> GetLogger<T>() where T : class => loggerFactory.CreateLogger<T>();
    public static ILogger GetLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);
}
```

You can use this logger manager like following.

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

        globalLogger = loggerFactory.CreateLogger("Global");

        Application.quitting += () =>
        {
            // when quit, flush unfinished log entries.
            loggerFactory.Dispose();
        };
    }

    public static Microsoft.Extensions.Logging.ILogger Logger => globalLogger;

    public static ILogger<T> GetLogger<T>() where T : class => loggerFactory.CreateLogger<T>();
    public static Microsoft.Extensions.Logging.ILogger GetLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);
}

// ---

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

> Limitation: Currently ZLogger for Unity does not support structured logging so you can not set `EnableStructuredLogging = true`.

License
---
This library is licensed under the MIT License.
