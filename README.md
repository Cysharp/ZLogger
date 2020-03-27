ZLogger
===
[![CircleCI](https://circleci.com/gh/Cysharp/ZLogger.svg?style=svg)](https://circleci.com/gh/Cysharp/ZLogger)

Coming soon.

Preview:

> PM> Install-Package [ZLogger](https://www.nuget.org/packages/ZLogger)

```csharp
Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        // optional:clear default providers.
        logging.ClearProviders();
        
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

License
---
This library is licensed under the the MIT License.