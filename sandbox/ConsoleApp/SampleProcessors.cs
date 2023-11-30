using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using ZLogger;
namespace ConsoleApp;




public class SimpleInMemoryLogProcessor : IAsyncLogProcessor
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


