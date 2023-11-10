using System;
using System.Reflection;
using System.Threading;
using Benchmark.Benchmarks;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using Utf8StringInterpolation;
using ZLogger;

// BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args);

var zLoggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddZLoggerConsole(options =>
    {
    });
});

var zLogger = zLoggerFactory.CreateLogger<PostLogEntry>();

var x = 100;
var y = 200;
var z = 300;
 for (var i = 0; i < 1000; i++)
{
    zLogger.ZLogInformation($"i={i} x={x} y={y} z={z}");
}

JetBrains.Profiler.Api.MeasureProfiler.StopCollectingData();
JetBrains.Profiler.Api.MeasureProfiler.StartCollectingData();

for (var i = 0; i < 1000; i++)
{
    zLogger.ZLogInformation($"i={i}");
}

JetBrains.Profiler.Api.MeasureProfiler.SaveData();

zLoggerFactory.Dispose();
