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
using System.Collections.Immutable;
namespace ConsoleApp;



public class InMemoryObservableLogProcessor : IAsyncLogProcessor, IObservable<string>
{
    bool isDisposed;
    ImmutableArray<IObserver<string>> observers = [];

    public void Post(IZLoggerEntry log)
    {
        if (isDisposed) return;
        var msg = log.ToString();
        log.Return();
        foreach (var item in observers)
        {
            item.OnNext(msg);
        }
    }

    public IDisposable Subscribe(IObserver<string> observer)
    {
        if (isDisposed) return NullDisposable.Instance;
        ImmutableInterlocked.Update(ref observers, (xs, arg) => xs.Add(arg), observer);
        return new Subscription(this, observer);
    }

    public ValueTask DisposeAsync()
    {
        isDisposed = true;
        ImmutableInterlocked.Update(ref observers, (xs) => xs.Clear());
        return default;
    }

    sealed class Subscription(InMemoryObservableLogProcessor parent, IObserver<string> observer) : IDisposable
    {
        public void Dispose()
        {
            if (parent != null)
            {
                ImmutableInterlocked.Update(ref parent.observers, (xs, arg) => xs.Remove(arg), observer);
            }
            parent = null!;
            observer = null!;
        }
    }

    sealed class NullDisposable : IDisposable
    {
        public static readonly IDisposable Instance = new NullDisposable();

        public void Dispose()
        {
        }
    }
}

public class InMemoryLogProcessor : IAsyncLogProcessor
{
    public event Action<string>? OnMessageReceived;

    public void Post(IZLoggerEntry log)
    {
        var msg = log.ToString();
        log.Return();
        OnMessageReceived?.Invoke(msg);
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

