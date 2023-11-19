using ConsoleAppFramework;
using ZLogger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using Utf8StringInterpolation;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Profiler.Api;
using System.Threading;
using ZLogger.Formatters;
using ZLogger.Internal;
using ZLogger.Providers;
using System.Numerics;
using System.Text.Json;

var factory = LoggerFactory.Create(logging =>
{
    logging.AddZLogger(zLogger =>
    {
        zLogger.AddConsole(console =>
        {
            console.InternalErrorLogger = ex => Console.WriteLine(ex);

            console.ConfigureEnableAnsiEscapeCode = true;
            console.OutputEncodingToUtf8 = true;
            console.UseJsonFormatter(formatter =>
            {
                formatter.IncludeProperties = IncludeProperties.None;

            });

            //console.UsePlainTextFormatter(formatter =>
            //{
            //    formatter.SetPrefixFormatter($"{0:timeonly} | {1:short} | ", (template, info) => template.Format(info.Timestamp, info.LogLevel));
            //});
        });
    });
});

var logger = factory.CreateLogger<Program>();


var v3 = new MyVector3 { X = 10.2f, Y = 999.99f, Z = 32.1f };

logger.ZLogInformation($"foo = {v3}");



factory.Dispose();







public struct MyVector3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}




public class InMemoryLogProcessor : IAsyncLogProcessor
{
    ConcurrentQueue<IZLoggerEntry> queue = new ConcurrentQueue<IZLoggerEntry>();

    public void Post(IZLoggerEntry log)
    {
        queue.Enqueue(log);
    }

    public string[] ConsumeMessages()
    {
        var result = new string[queue.Count];
        for (int i = 0; i < result.Length; i++)
        {
            if (queue.TryDequeue(out var entry))
            {
                try
                {
                    result[i] = entry.ToString();
                }
                finally
                {
                    entry.Return();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        return result;
    }

    public ValueTask DisposeAsync()
    {
        return default;
    }
}