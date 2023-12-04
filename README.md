ZLogger
===
[![GitHub Actions](https://github.com/Cysharp/ZLogger/workflows/Build-Debug/badge.svg)](https://github.com/Cysharp/ZLogger/actions) [![Releases](https://img.shields.io/github/release/Cysharp/ZLogger.svg)](https://github.com/Cysharp/ZLogger/releases)

**Z**ero Allocation Text/Structured **Logger** for .NET with StringInterpolation and Source Generator, built on top of a `Microsoft.Extensions.Logging`.

The usual destinations for log output are `Console(Stream)`, `File(Stream)`, `Network(Stream)`, all in UTF8 format. However, since typical logging architectures are based on Strings (UTF16), this requires additional encoding costs. In ZLogger, we utilize the [String Interpolation Improvement of C# 10](https://devblogs.microsoft.com/dotnet/string-interpolation-in-c-10-and-net-6/) and by leveraging .NET 8's [IUtf8SpanFormattable](https://learn.microsoft.com/en-us/dotnet/api/system.iutf8spanformattable?view=net-8.0), we have managed to avoid the boxing of values and maintain high performance by consistently outputting directly in UTF8 from input to output.

ZLogger is built directly on top of `Microsoft.Extensions.Logging`. `Microsoft.Extensions.Logging` is an official log abstraction used in many frameworks, such as ASP.NET Core and Generic Host. However, since regular loggers have their own systems, a bridge is required to connect these systems, and this is where a lot of overhead can be observed. ZLogger eliminates the need for this bridge, thereby completely avoiding overhead.

![Alt text](docs/image.png)

This benchmark is for writing to a file, but the default settings of typical loggers are very slow. This is because they flush after every write. In the benchmark, to ensure fairness, careful attention was paid to set the options in each logger for maximum speed. ZLogger is designed to be the fastest by default, so there is no need to worry about any settings.

ZLogger focuses on the new syntax of C#, and fully adopts Interpolated Strings.

![Alt text](docs/image-1.png)

This allows for providing parameters to logs in the most convenient form. Also, by closely integrating with System.Text.Json's Utf8JsonWriter, it not only enables high-performance output of text logs but also makes it possible to efficiently output structured logs.

ZLogger also emphasizes console output, which is crucial in cloud-native applications. By default, it outputs with performance that can withstand destinations in cloud log management. Of course, it supports both text logs and structured logs.

ZLogger delivers its best performance with .NET 8 and above, but it is designed to maintain consistent performance with .NET Standard 2.0 and .NET 6 through a fallback to its own IUtf8SpanFormattable.

As for standard logger features, it supports loading LogLevel from json, filtering by category, and scopes, as found in Microsoft.Extensions.Logging. In terms of output destinations, it is equipped with sufficient capabilities for `Console`, `File`, `RollingFile`, `InMemory`, `Stream`, and an `AsyncBatchingProcessor` for sending logs over HTTP and similar protocols.

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table of Contents

- [Getting Started](#getting-started)
- [Formatter Configurations](#formatter-configurations)
  - [PlainText](#plaintext)
  - [JSON](#json)
  - [MessagePack](#messagepack)
  - [Custom Formatter](#custom-formatter)
- [TODO: LogInfo ?](#todo-loginfo-)
- [TODO: KeyNameMutator](#todo-keynamemutator)
- [TODO: ZLoggerBuilder](#todo-zloggerbuilder)
  - [Console](#console)
  - [File](#file)
  - [RollingFile](#rollingfile)
  - [Stream](#stream)
  - [In-Memory](#in-memory)
  - [Custom LogProcessor](#custom-logprocessor)
- [ZLoggerOptions](#zloggeroptions)
- [Microsoft.CodeAnalysis.BannedApiAnalyzers](#microsoftcodeanalysisbannedapianalyzers)
- [Global LoggerFactory](#global-loggerfactory)
- [Unity](#unity)
- [License](#license)
- [Structured Logging](#structured-logging)
- [Filters](#filters)
- [Output Providers](#output-providers)
  - [Console](#console-1)
  - [File](#file-1)
  - [RollingFile](#rollingfile-1)
  - [Stream](#stream-1)
  - [LogProcessor](#logprocessor)
- [Multiple Providers](#multiple-providers)
- [Preparing Message Format](#preparing-message-format)
- [Options](#options)
- [Formatters](#formatters)
  - [`PlainTextZLoggerFormatter`](#plaintextzloggerformatter)
    - [Format and DateTime Handling](#format-and-datetime-handling)
    - [Console Coloring](#console-coloring)
  - [`SystemTextJsonZLoggerFormatter`](#systemtextjsonzloggerformatter)
  - [`MessagePackZLoggerFormatter`](#messagepackzloggerformatter)
  - [Custom formatter](#custom-formatter)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

Getting Started
---
This library is distributed via NuGet, supporting `.NET Standard 2.0`, `.NET Standard 2.1`, `.NET 6(.NET 7)` and `.NET 8` or above.

> PM> Install-Package [ZLogger](https://www.nuget.org/packages/ZLogger)

In the simplest case, you generate a logger by adding ZLogger's Provider to Microsoft.Extensions.Logging's [LoggerFactory](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging), and then use ZLogger's own ZLog method.

```csharp
using Microsoft.Extensions.Logging;
using ZLogger;

using var factory = LoggerFactory.Create(logging =>
{
    logging.SetMinimumLevel(LogLevel.Trace);

    // Add ZLogger provider to ILoggingBuilder
    logging.AddZLoggerConsole();
    
    // Output Structured Logging, setup options
    // logging.AddZLoggerConsole(options => options.UseJsonFormatter());
});

var logger = factory.CreateLogger("Program");

var name = "John";
var age = 33;

// Use **Z**Log method and string interpolation to log message
logger.ZLogInformation($"Hello my name is {name}, {age} years old.");
```

Normally, you don't create LoggerFactory yourself. Instead, you set up a Generic Host and receive ILogger through dependency injection (DI).

You can setup logger by [.NET Generic Host](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-8.0)(for ASP.NET Core) and if you want to use this in ConsoleApplication, we provides [ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework) to use hosting abstraction.

Here is the showcase of providers.

```csharp
using ZLogger;

var builder = Host.CreateApplicationBuilder();

builder.Logging
    // optional(MS.E.Logging):clear default providers(recommend to remove all)
    .ClearProviders()

    // optional(MS.E.Logging):setup minimum log level
    .SetMinimumLevel(LogLevel.Trace)
    
    // Add to output to console
    .AddZLoggerConsole();

    // Add to output to the file
    .AddZLoggerFile("/path/to/file.log")
    
    // Add to output the file that rotates at constant intervals.
    .AddZLoggerRollingFile(options =>
    {
        // File name determined by parameters to be rotated
        options.FilePathSelector = (timestamp, sequenceNumber) => $"logs/{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
        
        // The period of time for which you want to rotate files at time intervals.
        options.RollingInterval = RollingInterval.Day;
        
        // Limit of size if you want to rotate by file size. (KB)
        options.RollingSizeKB = 1024;        
    })    
    
    // Add to output of simple rendered strings into memory. You can subscribe to this and use it.
    .AddZLoggerInMemory(processor =>
    {
        processor.MessageReceived += renderedLogString => 
        {
            System.Console.WriteLine(renderedLogString);    
        };
    })
    
    // Add output to any steram (`System.IO.Stream`)
    .AddZLoggerStream(stream);

    // Add custom output
    .AddZLoggerLogProcessor(new YourCustomLogExporter());
    
    // Format as json
    .AddZLoggerConsole(options =>
    {
        options.UseJsonFormatter();
    })
    
    // Format as json and configure output
    .AddZLoggerConsole(options =>
    {
        options.UseJsonFormatter(formatter =>
        {
            formatter.IncludeProperties = IncludeProperties.ParameterKeyValues;
        });
    })

    // Further common settings
    .AddZLoggerConsole(options =>
    {
        // Enable LoggerExtensions.BeginScope
        options.IncludeScopes = true;
        
        // Set TimeProvider
        options.TimeProvider = yourTimeProvider
    });
```

```cs
using Microsoft.Extensions.Logging;
using ZLogger;

public class MyClass
{
    // get ILogger<T> from DI.
    readonly ILogger<MyClass> logger;
    
    public MyClass(ILogger<MyClass> logger)
    {
        this.logger = logger;
    }
    
    // name = "Bill", city = "Kumamoto", age = 21
    public void Foo(string name, string city, int age)
    {
        // plain-text:
        // Hello, Bill lives in Kumamoto 21 years old.
        // json:
        // {"Timestamp":"2023-11-30T17:28:35.869211+09:00","LogLevel":"Information","Category":"MyClass","Message":"Hello, Bill lives in Kumamoto 21 years old.","name":"Bill","city":"Kumamoto","age":21}
        // json(IncludeProperties.ParameterKeyValues):
        // {"name":"Bill","city":"Kumamoto","age":21}
        logger.ZLogInformation($"Hello, {name} lives in {city} {age} years old.");
    
        // Explicit property name, you can use custom format string start with '@'
        logger.ZLogInformation($"Hello, {name:@user-name} id:{100:@id} {age} years old.");
    
        // Dump variables as JSON, you can use custom format string `json`
        var user = new User(1, "Alice");

        // user: {"Id":1,"Name":"Bob"}
        logger.ZLogInformation($"user: {user:json}");
    }
}
```

All standard `.Log` methods are processed as strings by ZLogger's Provider. However, by using our unique `.ZLog*` methods, you can process them at high performance while remaining in UTF8. Additionally, these methods support both text logs and structured logs using String Interpolation syntax.

All logging methods are completely similar as [Microsoft.Extensions.Logging.LoggerExtensions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loggerextensions), but it has **Z** prefix overload.

The ZLog* method uses [InterpolatedStringHandler](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/interpolated-string-handler) in .NET and prepare the template at compile time.

Formatter Configurations
----

// TODO: still being written, please wait a little longer.


### PlainText

```cs
builder.Logging.AddZLoggerConsole(options =>
{
    // Text format
    // e.g) "2023-12-01 16:41:55.775|Information|This is log message. (MyNamespace.MyApp)
    options.UsePlainTextFormatter(formatter => 
    {
        formatter.SetPrefixFormatter($"{0}|{1}|", (template, info) => template.Format(info.Timestamp, info.LogLevel));
        formatter.SetSuffixFormatter($" ({0})", (formatter, info) => formatter.Format(info.Category));
        formatter.SetExceptionFormatter((writer, ex) => Utf8String.Format(writer, $"{ex.Message}"));
    });
        
    // Using various variable formats.
    // e.g) "2023-12-01T16:47:15+09:00|INF|This is log message"
    formatter.SetPrefixFormatter($"{0:yyyy-MM-dd'T'HH:mm:sszzz}|{1:short}|", (writer, info) =>
    {
        var escapeSequence = "";
        // if (info.LogLevel >= LogLevel.Error)
        // {
        //     escapeSequence = "\u001b[31m";
        // }
        // else if (!info.Category.Name.Contains("MyApp"))
        // {
        //     escapeSequence = "\u001b[38;5;08m";
        // }
    
        writer.Format(info.Timestamp, info.LogLevel);
    });
        
    // Console coloring example
    options.UsePlainTextFormatter(formatter =>
    {
        // \u001b[31m => Red(ANSI Escape Code)
        // \u001b[0m => Reset
        // \u001b[38;5;***m => 256 Colors(08 is Gray)
        formatter.SetPrefixFormatter($"{0}{1}|{2:short}|", (writer, info) =>
        {
            var escapeSequence = "";
            if (info.LogLevel >= LogLevel.Error)
            {
                escapeSequence = "\u001b[31m";
            }
            else if (!info.Category.Name.Contains("MyApp"))
            {
                escapeSequence = "\u001b[38;5;08m";
            }
        
            writer.Format(escapeSequence, info.Timestamp, info.LogLevel);
        });

        formatter.SetSuffixFormatter($"{0}", (writer, info) =>
        {
            if (info.LogLevel == LogLevel.Error || !info.Category.Name.Contains("MyApp"))
            {
                writer.Format("\u001b[0m");
            }
        });
    });
});
```

Formatting can be set using the String Interpolation Template, and lambda expression as shown above.

Note: For format strings available for various variables:
-`LogLevel` can be specially specified as `short`. This reduces the length of string to a fixed number of characters, such as `INFO`.
- For other types, ZLogger uses [Cysharp/Utf8StringInterpolation](https://github.com/Cysharp/Utf8StringInterpolation) internally. Please see this.


| Name                                                                                             | Description                                                          |
|:-------------------------------------------------------------------------------------------------|:---------------------------------------------------------------------|
| `SetPrefixFormatter(MessageTemplateHandler format, Action<MessageTemplate, LogInfo> formatter)`  | Set the text to be given before the message body. (Default is empty) |
| `SetSuffixFormatter(MessageTemplateHandler format, Action<MessageTemplate, LogInfo> formatter)`  | Set the text to be given after the message body. (Default is empty)  |
| `SetExceptionFormatter(Action<IBufferWriter<byte>, Exception> formatter)`                        |                                                                      |



### JSON


| Name                                                                | Description                                                                       |
|:--------------------------------------------------------------------|:----------------------------------------------------------------------------------|
| `JsonPropertyNames JsonPropertyNames`                               | Specify the name of each key in the output JSON                                   |
| `IncludeProperties IncludeProperties`                               | Flags that can specify properties to be output. (default: `Timestamp              | LogLevel | CategoryName | Message | Exception | ScopeKeyValues | ParameterKeyValues`) |
| `JsonSerializerOptions JsonSerializerOptions`                       | The options of `System.Text.Json`                                                 |
| `Action<Utf8JsonWriter, LogInfo>? AdditionalFormatter`              | Action when rendering additional properties based on `LogInfo`.                   |
| `JsonEncodedText? PropertyKeyValuesObjectName`                      | If set, the key/value properties is nested under the specified key name.          |
| `IKeyNameMutator? KeyNameMutator`                                   | You can set the naming convention if you want to automatically convert key names. |
| `bool UseUtcTimestamp`                                              | If true, timestamp is output in utc. (default: false)                             |


Sample of Json Formatting customize

```csharp
using System.Text.Json;
using ZLogger;
using ZLogger.Formatters;

namespace ConsoleApp;

using static IncludeProperties;
using static JsonEncodedText; // JsonEncodedText.Encode

public static class CloudLoggingExtensions
{
    // Cloud Logging Json Field
    // https://cloud.google.com/logging/docs/structured-logging?hl=en
    public static ZLoggerOptions UseCloudLoggingJsonFormat(this ZLoggerOptions options)
    {
        return options.UseJsonFormatter(formatter =>
        {
            // Category and ScopeValues is manually write in AdditionalFormatter at labels so remove from include properties.
            formatter.IncludeProperties = Timestamp | LogLevel | Message | ParameterKeyValues;

            formatter.JsonPropertyNames = JsonPropertyNames.Default with
            {
                LogLevel = Encode("severity"),
                LogLevelNone = Encode("DEFAULT"),
                LogLevelTrace = Encode("DEBUG"),
                LogLevelDebug = Encode("DEBUG"),
                LogLevelInformation = Encode("INFO"),
                LogLevelWarning = Encode("WARNING"),
                LogLevelError = Encode("ERROR"),
                LogLevelCritical = Encode("CRITICAL"),

                Message = Encode("message"),
                Timestamp = Encode("timestamp"),
            };

            formatter.PropertyKeyValuesObjectName = Encode("jsonPayload");

            // cache JsonENcodedText outside of AdditionalFormatter
            var labels = Encode("logging.googleapis.com/labels");
            var category = Encode("category");
            var eventId = Encode("eventId");
            var userId = Encode("userId");

            formatter.AdditionalFormatter = (writer, logInfo) =>
            {
                writer.WriteStartObject(labels);
                writer.WriteString(category, logInfo.Category.JsonEncoded);
                writer.WriteString(eventId, logInfo.EventId.Name);

                if (logInfo.ScopeState != null && !logInfo.ScopeState.IsEmpty)
                {
                    foreach (var item in logInfo.ScopeState.Properties)
                    {
                        if (item.Key == "userId")
                        {
                            writer.WriteString(userId, item.Value!.ToString());
                            break;
                        }
                    }
                }
                writer.WriteEndObject();
            };
        });
    }
}

```

### MessagePack


Formats using messagepack are supported in an additional package.

[MessagePack-CSharp](https://github.com/MessagePack-CSharp/MessagePack-CSharp)

> PM> Install-Package [ZLogger.MessagePack](https://www.nuget.org/packages/ZLogger.MessagePack)

| Name                                                               | Description                                                        |
|:-------------------------------------------------------------------|:-------------------------------------------------------------------|
| `MessagePackSerializerOptions MessagePackSerializerOptions`        | The options of `MessagePack-CSharp`.                               |
| `IncludeProperties IncludeProperties`                              | Flags that can specify properties to be output. (default: `Timestamp| LogLevel | CategoryName | Message | Exception | ScopeKeyValues | ParameterKeyValues`) |
| `IKeyNameMutator? KeyNameMutator`                                   | You can set the naming convention if you want to automatically convert key names. |

### Custom Formatter 

todo

TODO: LogInfo ?
---

| Name                        | Description                                                                                              |
|:----------------------------|:---------------------------------------------------------------------------------------------------------|
| `LogCategory Category`      | The category name set for each logger. And holds JsonEncodedText and utf8 byte sequence representations. |
| `Timestamp Timestamp`       | Timestamp                                                                                                |
| `LogLevel LogLevel`         | LogLevel  of `Microsoft.Extensions.Logging`                                                              |
| `EventId EventId`           | EventId of `Microsoft.Extensions.Logging`                                                                |
| `Exception? Exception`      | Exception given as argument when logging.                                                                |
| `LogScopeState? ScopeState` | Additional properties set by `ILogger.BeginScope(...)` (if ZLoggerOptions.IncludeScopes = true)          |


TODO: KeyNameMutator
---


| Name                                  | Description                                                                                               |
|:--------------------------------------|:----------------------------------------------------------------------------------------------------------|
| `LastMemberName`                      | Returns the last member name of the source.                                                               |
| `LowerFirstCharacter`                 | The first character converted to lowercase.                                                               |
| `UpperFirstCharacter`                 | The first character converted to uppercase.                                                               |
| `LastMemberNameLowerFirstCharacter`   | Returns the last member name of the source with the first character converted to lowercase.               |
| `LastMemberNameUpperFirstCharacter`   | Returns the last member name of the source with the first character converted to uppercase.               |                              







TODO: ZLoggerBuilder
----

ZLogger has the following providers.

| Type                                   | Alias               | Builder Extension      |
|----------------------------------------|---------------------|------------------------|
| ZLoggerConsoleLoggerProvider           | ZLoggerConsole      | AddZLoggerConsole      |
| ZLoggerFileLoggerProvider              | ZLoggerFile         | AddZLoggerFile         |
| ZLoggerRollingFileLoggerProvider       | ZLoggerRollingFile  | AddZLoggerRollingFile  |
| ZLoggerStreamLoggerProvider            | ZLoggerStream       | AddZLoggerStream       |
| ZLoggerLogProcessorLoggerProvider      | ZLoggerLogProcessor | AddZLoggerLogProcessor |
| ZLoggerInMemoryProcessorLoggerProvider | ZLoggerInMemory     | AddZLoggerInMemory     |


If you are using `Microsoft.Extensions.Configuration`, you can set the log level through configuration.
In this case, alias of Provider can be used.  for example:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    },
    "ZLoggerConsoleLoggerProvider": {
      "LogLevel": {
        "Default": "Debug"
      }
    }
  }
}
```

All Providers can take an Action that sets ZLoggerOptions as the last argument. As follows.

```cs
builder.Logging
    .ClearProviders()
    // Configure options
    .AddZLoggerConsole(options => 
    {
        options.LogToStandardErrorThreshold = LogLevel.Error;
    });
    
    // Configure options with service provider
    .AddZLoggerConsole((options, services) => 
    {
        options.TimeProvider = services.GetService<YourCustomTimeProvider>();
    });
```


### Console


If you are using `ZLoggerConsoleLoggerProvider`, the following additional options are available:

| Name                                    | Description                                                                                                                               |
|:----------------------------------------|:------------------------------------------------------------------------------------------------------------------------------------------|
| `bool OutputEncodingToUtf8`             | Set `Console.OutputEncoding = new UTF8Encoding(false)` when the provider is created.  (default: true)                                     | |
| `bool ConfigureEnableAnsiEscapeCode`    | If set true, then configure console option on execution and enable virtual terminal processing(enable ANSI escape code). (default: false) |
| `LogLevel LogToStandardErrorThreshold`  | If set, logs at a higher level than the value will be output to standard error. (default: LogLevel.None)                                  |


### File


### RollingFile


If you are using `ZLoggerRollingFileLoggerProvider`, the following additional options are available:

| Name                                                                              | Description                                                                                                        |
|:----------------------------------------------------------------------------------|:-------------------------------------------------------------------------------------------------------------------|
| `Func<DateTimeOffset, int, string> fileNameSelector`                              | The Func to consturct the file path. `DateTimeOffset` is date of file open time(UTC), `int` is number sequence.        | |
| `RollingInterval rollInterval`                                                    | Interval to automatically rotate files.                                                                            | |
| `int rollSizeKB`                                                                  | Limit size of single file.  If the file size is exceeded, a new file is created with the sequence number moved up. | |


### Stream

```cs
builder.Logging
    .ClearProviders()
    .AddZLogger(zlogger => 
    {
        // Default
        zlogger.AddStream(stream);
        
        // Configure stream dynamically
        zlogger.AddStream((options, services) =>
        {
            // ...
            return yourCustomStream;
        });
    });

```

### In-Memory



| Name                                                                                                            | Description |
|:----------------------------------------------------------------------------------------------------------------|:------------|
| `string processorKey`                                                                                           |  If specified, `InMemoryObservableLogProcessor` is registered in the DI container as a keyed service and can be retrieved by name.           |
| `Action<InMemoryObservableLogProcessor> configureProcessor`                                                     |  Custom actions can be added that use processors instead of DI containers.           |



### Custom LogProcessor

todo


```cs

public class TcpLogProcessor : IAsyncLogProcessor
{
    TcpClient tcpClient;
    AsyncStreamLineMessageWriter writer;

    public TcpLogProcessor(ZLoggerOptions options)
    {
        tcpClient = new TcpClient("127.0.0.1", 1111);
        writer = new AsyncStreamLineMessageWriter(tcpClient.GetStream(), options);
    }

    public void Post(IZLoggerEntry log)
    {
        writer.Post(log);
    }

    public async ValueTask DisposeAsync()
    {
        await writer.DisposeAsync();
        tcpClient.Dispose();
    }
}
```

```cs
public class BatchingHttpLogProcessor : BatchingAsyncLogProcessor
{
    HttpClient httpClient;
    ArrayBufferWriter<byte> bufferWriter;
    IZLoggerFormatter formatter;

    public BatchingHttpLogProcessor(int batchSize, ZLoggerOptions options)
        : base(batchSize, options)
    {
        httpClient = new HttpClient();
        bufferWriter = new ArrayBufferWriter<byte>();
        formatter = options.CreateFormatter();
    }

    protected override async ValueTask ProcessAsync(IReadOnlyList<INonReturnableZLoggerEntry> list)
    {
        foreach (var item in list)
        {
            item.FormatUtf8(bufferWriter, formatter);
        }
        
        var byteArrayContent = new ByteArrayContent(bufferWriter.WrittenSpan.ToArray());
        await httpClient.PostAsync("http://foo", byteArrayContent).ConfigureAwait(false);

        bufferWriter.Clear();
    }

    protected override ValueTask DisposeAsyncCore()
    {
        httpClient.Dispose();
        return default;
    }
}
```


ZLoggerOptions
---


| Name                                                                         | Description                                                                                                                                                                                                                    |
|:-----------------------------------------------------------------------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `bool IncludeScopes { get; set; }`                                           | Enable `ILogger.BeginScope`, default is `false`.                                                                                                                                                                               |
| `TimeProvider? TimeProvider { get; set; }`                                   | Gets or sets the time provider for the logger. The Timestamp of LogInfo is generated by TimeProvider's GetUtcNow() and LocalTimeZone when TimeProvider is set. The default value is null, which means use the system standard. |
| `Action<Exception>? InternalErrorLogger { get; set; }`                       | `InternalErrorLogger` is a delegate that is called when an exception occurs in the log writing process (such as a serialization error). The default value is `null`, which means errors are ignored.                           |
| `CreateFormatter()`                                                          | Create an formatter to use in ZLoggerProvider.                                                                                                                                                                                 |
| `UseFormatter(Func<IZLoggerFormatter> formatterFactory)`                     | Set the formatter that defines the output format of the log.                                                                                                                                                                   |
| `UsePlainTextFormatter(Action<PlainTextZLoggerFormatter>? configure = null)` | Use the built-in plain text formatter.                                                                                                                                                                                         |
| `UseJsonFormatter(Action<SystemTextJsonZLoggerFormatter>? configure = null)` | Use the built-in json formatter. (implementation of `System.Text.Json`)                                                                                                                                                        |

TODO:...
default formatter is PlaintTextFormatter.










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

    // standard LoggerFactory caches logger per category so no need to cache in this manager
    public static ILogger<T> GetLogger<T>() where T : class => loggerFactory.CreateLogger<T>();
    public static ILogger GetLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);
}
```

You can use this logger manager like following.

```csharp
public class Foo
{
    static readonly ILogger<Foo> logger = LogManager.GetLogger<Foo>();

    public void Foo(int x)
    {
        logger.ZLogDebug($"do do do: {x}");
    }
}
```

Unity
---
This library requires C# 10.0, however currently Unity C# version is 9.0. Therefore, it is not supported at this time and will be considered when the version of Unity's C# is updated.

License
---
This library is licensed under the MIT License.










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
    options.UseJsonFormatter();
});

// In default, output JSON with log information(categoryName, level, timestamp, exception), message and payload(if exists).

// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":null}
logger.ZLogInformation("Registered User: Id = {0}, UserName = {1}", id, userName);

// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00","Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
logger.ZLogInformationWithPayload(new UserRegisteredLog { Id = id, Name = userName }, "Registered User: Id = {0}, UserName = {1}", id, userName);
```

Write log by JSON, supports extra information(category-name, log-level, timestamp and custom metadatas) + message, info + message + custom payload.

To details, see [Formatters](#formatters) section.

Filters
---
This is the Microsoft.Extensions.Logging rules, if more than one filter is set, only the first match of type -> category and finally no category, SetMinimumLevel will be applied.

If you get confused, only to use `AddFilter<T>(Func<string, LogLevel, bool> categoryLevelFilter)` will help you.

Output Providers
---
ZLogger has the following providers by default.

|Type|Alias|Builder Extension|
|-|-|-|
|ZLoggerConsoleLoggerProvider|ZLoggerConsole|AddZLoggerConsole|
|ZLoggerFileLoggerProvider|ZLoggerFile|AddZLoggerFile|
|ZLoggerRollingFileLoggerProvider|ZLoggerRollingFile|AddZLoggerRollingFile|
|ZLoggerStreamLoggerProvider|ZLoggerStream|AddZLoggerStream|
|ZLoggerLogProcessorLoggerProvider|ZLoggerLogProcessor|AddZLoggerLogProcessor|

Type is used in `AddFilter<T>`, Alias is used when configuring filters from Option, and Builder Extensions is used in `ConfigureLogging`.

All providers write logs asynchronously and buffered; when logger call a Log method, they don't format it, they just store it in a queue, so they don't stop the calling thread at all. The buffer size is 65536, it will flush when the buffer is overflowing or the wait queue is empty.

### Console

Output to the Console.

```csharp
logging.AddZLoggerConsole();
```

It is useful for ConsoleApplication and containerized applications on cloud. For example, AWS CloudWatch, GCP Stackdriver Logging, and Datadog Logs agent collect data on the standard output. This is especially useful for debugging and analysis if these are output in a structured log.

If `consoleOutputEncodingToUtf8 = true`(default is true), set `Console.OutputEncoding = new UTF8Encoding(false)` when the provider is created. If `outputToErrorStream = false`(default is false), use standard error output.

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

Options
---
`ZLoggerOptions` can be configured with `Action<ZLoggerOptions> configure` when adding to `ILoggingBuilder`. The options applied to StructuredLogging(`EnableStructuredLogging = true`) are different from those applied to TextLogging. The default is TextLogging(`EnableStructuredLogging = false`).

* `Action<LogInfo, Exception>? InternalErrorLogger`
* `TimeSpan? FlushRate`

`InternalErrorLogger` is an delegate of when exception occured in log writing process(such as serialization error). Default is `Console.WriteLine(exception)`.

`FlushRate` is flush rate of buffer write. Default is null that flush immediately when thread is free, it is recommended option for performance.

Formatters
---

ZLogger switches the log output format for each provider. 
You can set the formatter by the ZLoggerOptions.UseFormatter* method.
Built-in formatters include the following

### `PlainTextZLoggerFormatter`

Provide to format log entry as plain text. (Default)
The following configuration are available.

* `Action<IBufferWriter<byte>, LogInfo>? PrefixFormatter`
* `Action<IBufferWriter<byte>, LogInfo>? SuffixFormatter`
* `Action<IBufferWriter<byte>, Exception> ExceptionFormatter`

For performance reason, we do not use string so use the `IBufferWriter<byte>` instead. You can use `ZString.Utf8Format` to help set formatter.

```csharp
logging.AddZLoggerConsole(options =>
{
    options.UsePlainTextFormatter(formatter => 
    {
        formatter.PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "[{0}][{1}]", info.LogLevel, info.Timestamp.DateTime.ToLocalTime());
        
        // Tips: use PrepareUtf8 to achive better performance.
        var prefixFormat = ZString.PrepareUtf8<LogLevel, DateTime>("[{0}][{1}]");
        options.PrefixFormatter = (writer, info) => prefixFormat.FormatTo(ref writer, info.LogLevel, info.Timestamp.DateTime.ToLocalTime());
    });
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

#### Format and DateTime Handling

PlainTextZLoggerFormatter's format string internally using ZString's format under it uses dotnet [Utf8Formatter.TryFormat](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8formatter.tryformat). There format string is not same as standard format. It uses [StandardFormat](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.standardformat), combinate of symbol char and precision. Supported format string symbol can find in Utf8Formatter.TryFormat document(For example Int32 supports `G`, `D`, `N`, `X` and Boolean supports `G`, `I`). Precision(zero padding) can pass after symbol like `D2`. For example `logger.ZDebug("{0:D2}:{1:D2}:{2:D2}", hour, minute, second)`.

[TryFormat(DateTime)](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8formatter.tryformat?view=netcore-3.1#System_Buffers_Text_Utf8Formatter_TryFormat_System_DateTime_System_Span_System_Byte__System_Int32__System_Buffers_StandardFormat_)(also DateTimeOffset) and [TryFormat(TimeSpan)](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.text.utf8formatter.tryformat?view=netcore-3.1#System_Buffers_Text_Utf8Formatter_TryFormat_System_TimeSpan_System_Span_System_Byte__System_Int32__System_Buffers_StandardFormat_) symbol is too restricted than standard string format. If you want to use custom format, deconstruct there `Day`, `Hour`, etc.


#### Console Coloring

For performance reason, in default, ZLogger does not colorize in Console(using `System.Console.ForegroundColor = ConsoleColor...` causes poor performance). However, colorization can be useful in debugging. In order to colorize without degrading performance, the [ANSI escape code](https://en.wikipedia.org/wiki/ANSI_escape_code) can be used in `PrefixFormatter` and `SuffixFormatter`.

The following example shows the error in red and the framework log in gray.

![image](https://user-images.githubusercontent.com/46207/126967082-66af362b-dd87-4ae7-833b-aad229054142.png)

```csharp
logging.AddZLoggerConsole(options =>
{
#if DEBUG
    // \u001b[31m => Red(ANSI Escape Code)
    // \u001b[0m => Reset
    // \u001b[38;5;***m => 256 Colors(08 is Gray)
    options.UsePlainTextFormatter(formatter =>
    {
        formatter.PrefixFormatter = (writer, info) =>
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
        formatter.SuffixFormatter = (writer, info) =>
        {
            if (info.LogLevel == LogLevel.Error || !info.CategoryName.StartsWith("MyApp"))
            {
                ZString.Utf8Format(writer, "\u001b[0m", "");
            }
        };
    });
#endif

}, configureEnableAnsiEscapeCode: true); // configureEnableAnsiEscapeCode
```

`configureEnableAnsiEscapeCode: true` is important option for Windows(default is false). Visual Studio Debug Console, other 3rd party terminals, linux terminals are enabled ANSI escape code but default command prompt and powershell are not. If `configureEnableAnsiEscapeCode: true` then configure console option on execution and enable virtual terminal processing(enable ANSI escape code).

### `SystemTextJsonZLoggerFormatter`

Provide JSON format using `System.Text.Json`.
The following configuration are available.

* `Action<Utf8JsonWriter, LogInfo> MetadataFormatter`
* `JsonEncodedText MessagePropertyName`
* `JsonEncodedText PayloadPropertyName`
* `JsonSerializerOptions JsonSerializerOptions`

The StructuredLoggingFormatter is called when `EnableStructuredLogging = true`. `LogInfo.WriteToJsonWriter` is defined by default, which writes all LogInfo properties.

```csharp
logging.AddZLoggerConsole(options =>
{
    options.UseJsonFormatter();
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

Write as JSON in the order MetadataFormatter -> Message -> Payload. The property names "Message" and "Payload" may be changed by "MessagePropertyName" and "PayloadPropertyName".

`ZLog...WithPayload` methods are only meaningful for StructuredLogging. When `UseJsonFormatter()`, payload is serialized to JSON by `System.Text.Json.JsonSerializer`. (`ZLogger` does not support [Message Templates](https://messagetemplates.org/), if you want to output a payload, you must pass an object/struct.)

If you want to add additional information to the JSON, modify the MetadataFormatter as follows, for example

```csharp
logging.AddZLoggerConsole(options =>
{
    options.UseJsonFormatter(formatter => 
    {
        var gitHashName = JsonEncodedText.Encode("GitHash");
        var gitHashValue = JsonEncodedText.Encode(gitHash);
    
        formatter.MetadataFormatter = (writer, info) =>
        {
            writer.WriteString(gitHashName, gitHashValue);
            formatter.DefaultMetadataFormatter(writer, info);
        };
    });
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

### `MessagePackZLoggerFormatter`

`ZLogger.MessagePack` package can be installed to format log entry with [MessagePack-CSharp](https://github.com/MessagePack-CSharp/MessagePack-CSharp) binary format.

The following configuration are available.
* `MessagePackSerializerOptions MessagePackSerializerOptions`

You can change the serialization behavior of the payload by changing the `MessagePackSerializerOptions`. If you want to set up a custom messagepack formatter, set it here. By default, `MessagePackSerializer.DefaultOptions` is used

```csharp
logging.AddZLoggerConsole(options =>
{
    options.UseMessagePackFormatter(formatter => 
    {
        formatter.MessagePackSerializerOptions
        {
            writer.WriteString(gitHashName, gitHashValue);
            formatter.DefaultMetadataFormatter(writer, info);
        };
    });
});

// Outputs a messagepack with strings as keys, such as the following values:
// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":...,"Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":null}
logger.ZLogInformation("Registered User: Id = {0}, UserName = {1}", id, userName);

// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":...,"Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
logger.ZLogInformationWithPayload(new UserRegisteredLog { Id = id, Name = userName }, "Registered User: Id = {0}, UserName = {1}", id, userName);

// Payload must be able to be formatted with MessagePack like this:
[MessagePackObject(keyAsPropertyName: true)]
public struct UserRegisteredLog
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

Note:
Timestamp is formatted by default with used message pack [timestamp-extension-type](https://github.com/msgpack/msgpack/blob/master/spec.md#timestamp-extension-type).
You can change this behavior by setting the MessagePackSerializerOptions and changing the DateTime type formatter.

### Custom formatter

Alternatively, you can implement the `IZLoggerFormatter` yourself to completely customize the formatting results.

```csharp
public class MyCustomJsonZLoggerFormatter : IZLoggerFormatter
{
    Utf8JsonWriter? jsonWriter;

     /// <summary>
     /// Defines the final format of each log entry.
     /// </summary>
     /// <param name="writer">Write a utf8 byte sequence to this buffer.</param>
     /// <param name="entry">Logged entry</param>
     /// <param name="payload">Logged payload</param>
     /// <param name="utf8Message">Logged message already rendered into a single string.</param>
    public void FormatLogEntry<TEntry, TPayload>(
        IBufferWriter<byte> writer,
        TEntry entry,
        TPayload payload,
        ReadOnlySpan<byte> utf8Message)
        where TEntry : IZLoggerEntry
    {
        jsonWriter?.Reset(writer);
        jsonWriter ??= new Utf8JsonWriter(writer);

        jsonWriter.WriteStartObject();

        // ...
        // Write JSON using jsonWriter and arguments.
        jsonWriter.WriteString("Message", utf8Message);
        jsonWriter.WriteString("Timestamp", entry.LogInfo.Timestamp);
        jsonWriter.WritePropertyName(PayloadPropertyName);
        JsonSerializer.Serialize(jsonWriter, payload);
        // ...

        jsonWriter.WriteEndObject();
        jsonWriter.Flush();
    }
}

logging.AddZLoggerConsole(options =>
{
    options.UseFormatter(() =>
    {
        return new MyCustomJsonZLoggerFormatter();
    });
});
```
