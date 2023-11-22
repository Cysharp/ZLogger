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
using System.Net.Sockets;
using System.Net.Http;
using System.Buffers;
using System.Reflection.PortableExecutable;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;




var f = LoggerFactory.Create(logging =>
{
    logging.SetMinimumLevel(LogLevel.Trace);

    //logging.AddFilter<ZLoggerInMemoryLoggerProvider>(x => x is LogLevel.Warning or LogLevel.Trace);


    // logging.AddProvider(


    // logging.Services.TryAddEnumerable

    logging.AddZLogger(zlogger =>
    {
        zlogger.AddConsole(options =>
        {


            options.OutputEncodingToUtf8 = true;
            options.ConfigureEnableAnsiEscapeCode = true;

            options.LogToStandardErrorThreshold = LogLevel.Error;
        });



    });




    // sc.AddTransient<IOptionsMonitor<ZLoggerConsoleOptions>>();
    //logging.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerConsoleLoggerProvider>(x => new ZLoggerConsoleLoggerProvider("foo", x.GetRequiredService<IOptionsMonitor<ZLoggerConsoleOptions>>())));
    //logging.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerConsoleLoggerProvider>(x => new ZLoggerConsoleLoggerProvider("foo", x.GetRequiredService<IOptionsMonitor<ZLoggerConsoleOptions>>())));

});

var l = f.CreateLogger("my");
var x = 10;
var y = 20;
var z = 30;


Console.Error.WriteLine("ERROR");
// l.ZLogTrace($"foo: {("tako.", x)} bar: {y} baz: {z}");
// l.ZLogError($"foo: {("tako.", x)} bar: {y} baz: {z}");

f.Dispose();

//// p.GetService<TakoYakiX>();

////return;




//Foo(100); // 100
//Foo((((((((100)))))))); // 100

//var bar = new { X = 100 };
//Foo(bar.X); // bar.X

//Foo(((((bar.X))))); // bar.X

//Foo(new { X = 100 }.X); // new { X = 100 }.X

//Foo((100, 200).Item1); // (100, 200).Item1


//static void Foo(int x, [CallerArgumentExpression("x")] string? caller = null)
//{
//    Console.WriteLine(caller);
//}










public static class Test
{
    public static void Call(Action<A> action) { }
    public static void Call(Action<B> action) { }
}

public class A
{
    public void AMethod() { }
}


public class B
{
    public void BMethod() { }
}





//var sp = new ServiceCollection().AddLogging(logging =>
//{
//    logging.AddZLogger(zLogger =>
//    {
//        zLogger.AddInMemory();


//        //zLogger.AddFile("foo.log", option =>
//        //{
//        //    option.InternalErrorLogger = ex => Console.WriteLine(ex);

//        //    option.UsePlainTextFormatter(formatter =>
//        //    {
//        //        formatter.SetPrefixFormatter($"{0:timeonly} | {1:short} | ", (template, info) => template.Format(info.Timestamp, info.LogLevel));
//        //    });
//        //});
//    });
//}).BuildServiceProvider();

//var inmemory = sp.GetRequiredService<InMemoryObservableLogProcessor>();
//inmemory.Subscribe(new LoggingObserver());

//var factory = sp.GetRequiredService<ILoggerFactory>();


//var logger = factory.CreateLogger<Program>();


//var x = 10;
//var y = 20;
//var z = 30;
//var v3 = new MyVector3 { X = 1.0f, Y = 2.2f, Z = 9.9f };

//logger.ZLogInformation($"v3 = {v3:json} is dead.");


//factory.Dispose();

public struct MyVector3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}


class LoggingObserver : IObserver<string>
{
    public void OnCompleted()
    {
        // Console.WriteLine(  "END");
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(string value)
    {
        Console.WriteLine("Observer: " + value);
    }
}


class TakoYakiX
{
    public TakoYakiX(int one)
    {
        Console.WriteLine("ctor called:" + one);
    }
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

public class BatchingHttpLogProcessor : IAsyncLogProcessor
{
    Channel<IZLoggerEntry> channel;
    IZLoggerFormatter formatter;
    HttpClient httpClient;
    Task loop;

    public BatchingHttpLogProcessor(ZLoggerOptions options)
    {
        formatter = options.CreateFormatter();
        httpClient = new HttpClient();
        this.channel = Channel.CreateUnbounded<IZLoggerEntry>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false,
            SingleWriter = false,
            SingleReader = true,
        });
        loop = Task.Run(WriteLoop);
    }

    public void Post(IZLoggerEntry log)
    {
        channel.Writer.TryWrite(log);
    }

    async Task WriteLoop()
    {
        var list = new List<IZLoggerEntry>();
        var reader = channel.Reader;
        var buffer = new ArrayBufferWriter<byte>();
        var batchCount = 0;

        try
        {
            while (await reader.WaitToReadAsync().ConfigureAwait(false))
            {
                while (reader.TryRead(out var item) && batchCount++ < 100)
                {
                    try
                    {
                        item.FormatUtf8(buffer, formatter);
                        AppendNewLine(buffer);
                    }
                    finally
                    {
                        item.Return(); // IMPORTANT: reuse LogEntry.
                    }
                }

                var byteArrayContent = new ByteArrayContent(buffer.WrittenSpan.ToArray());

                await httpClient.PostAsync("http://foo", byteArrayContent).ConfigureAwait(false);

                buffer.Clear();
                batchCount = 0;
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    static void AppendNewLine(IBufferWriter<byte> writer)
    {
        var span = writer.GetSpan(1);
        "\n"u8.CopyTo(span);
        writer.Advance(1);
    }

    public async ValueTask DisposeAsync()
    {
        channel.Writer.Complete();
        await loop.ConfigureAwait(false);
        httpClient.Dispose();
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