using System;
using System.Collections.Generic;

namespace ZLogger.Generator.Tests;

public static class TestHelper
{
    public static IDisposable CreateMessageLogger<T>(out ILogger<T> logger, out List<string> messages)
    {
        return CreateLogger(x => { }, out logger, out messages);
    }

    public static IDisposable CreateJsonLogger<T>(out ILogger<T> logger, out List<string> messages)
    {
        return CreateLogger(x => x.UseJsonFormatter(formatter =>
        {
            formatter.IncludeProperties = IncludeProperties.ParameterKeyValues;
        }), out logger, out messages);
    }

    public static IDisposable CreateLogger<T>(Action<ZLoggerOptions> configure, out ILogger<T> logger, out List<string> messages)
    {
        return CreateLogger(LogLevel.Trace, configure, out logger, out messages);
    }

    public static IDisposable CreateLogger<T>(LogLevel minimumLevel, Action<ZLoggerOptions> configure, out ILogger<T> logger, out List<string> messages)
    {
        var list = new List<string>();
        var factory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(minimumLevel);
            logging.AddZLoggerInMemory((options, services) =>
            {
                configure(options);
            }, processor =>
            {
                processor.MessageReceived += msg => list.Add(msg);
            });
        });

        logger = factory.CreateLogger<T>();
        messages = list;
        return factory;
    }
}