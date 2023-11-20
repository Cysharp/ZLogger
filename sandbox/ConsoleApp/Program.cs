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
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

var factory = LoggerFactory.Create(logging =>
{
    logging.AddZLogger(zLogger =>
    {
        zLogger.AddFile("foo.log", option =>
        {
            option.InternalErrorLogger = ex => Console.WriteLine(ex);

            option.UsePlainTextFormatter(formatter =>
            {
                formatter.SetPrefixFormatter($"{0:timeonly} | {1:short} | ", (template, info) => template.Format(info.Timestamp, info.LogLevel));
            });
        });
    });
});

var logger = factory.CreateLogger<Program>();


var x = 10;
var y = 20;
var z = 30;

var c = 0;
while (c++ < 3)
{
    for (int i = 0; i < 100000; i++)
    {
        logger.ZLogInformation($"x={x} y={y} z={z}");
    }

    Thread.Sleep(TimeSpan.FromSeconds(4));
}


factory.Dispose();


unsafe
{
    var obj = new string[] { "hoge", "huga" };

    var p = new IntPtr(Unsafe.AsPointer(ref obj[0]));

    var xx = p.ToInt64();
    MemoryMarshal.CreateSpan(ref xx, sizeof(long));
}

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

readonly struct LiteralList : IEquatable<LiteralList>
{
    readonly List<string?> literals;

    public LiteralList(List<string?> literals)
    {
        this.literals = literals;
    }

    [ThreadStatic]
    static XxHash3? xxhash;

    public override int GetHashCode()
    {
        var h = xxhash;
        if (h == null)
        {
            h = xxhash = new XxHash3();
        }
        else
        {
            h.Reset();
        }

        var span = CollectionsMarshal.AsSpan(literals);
        foreach (var item in span)
        {
            h.Append(MemoryMarshal.AsBytes(item.AsSpan()));
        }

        // https://github.com/Cyan4973/xxHash/issues/453
        // XXH3 64bit -> 32bit, okay to simple cast answered by XXH3 author.
        return unchecked((int)h.GetCurrentHashAsUInt64());
    }

    public bool Equals(LiteralList other)
    {
        var xs = CollectionsMarshal.AsSpan(literals);
        var ys = CollectionsMarshal.AsSpan(other.literals);

        if (xs.Length == ys.Length)
        {
            for (int i = 0; i < xs.Length; i++)
            {
                if (xs[i] != ys[i]) return false;
            }
            return true;
        }

        return false;
    }
}