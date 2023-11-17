using ConsoleAppFramework;
using ZLogger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using Utf8StringInterpolation;

var factory = LoggerFactory.Create(logging =>
{
    logging.AddZLogger(zLogger =>
    {
        zLogger.AddConsole(console =>
        {
            console.ConfigureEnableAnsiEscapeCode = true;
            console.OutputEncodingToUtf8 = true;
            console.UsePlainTextFormatter(formatter =>
            {
                formatter.SetPrefixFormatter($"{0:timeonly} | {1:short} | ", (template, info) => template.Format(info.Timestamp, info.LogLevel));
            });
        });
    });
});

var logger = factory.CreateLogger<Program>();

logger.ZLogInformation($"aaaaaa");

factory.Dispose();