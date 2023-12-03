using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ZLogger.Generator.Tests;

public static class TestHelper
{
    public static IDisposable CreateMessageLogger<T>(out ILogger<T> logger, out List<string> messages)
    {
        var list = new List<string>();
        var factory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerInMemory(processor =>
            {
                processor.MessageReceived += msg => list.Add(msg);
            });
        });

        logger = factory.CreateLogger<T>();
        messages = list;
        return factory;
    }

    public static IDisposable CreateJsonLogger<T>(out ILogger<T> logger, out List<string> messages)
    {
        var list = new List<string>();
        var factory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerInMemory((options, provider) =>
            {
                options.UseJsonFormatter(formatter => formatter.IncludeProperties = IncludeProperties.ParameterKeyValues);
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