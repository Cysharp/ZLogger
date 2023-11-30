using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace ZLogger;

public interface IAsyncLogProcessor : IAsyncDisposable
{
    void Post(IZLoggerEntry log);
}

public abstract class BatchingAsyncLogProcessor : IAsyncLogProcessor
{
    readonly Channel<IZLoggerEntry> channel;
    readonly Task writeLoop;
    readonly ZLoggerOptions options;

    readonly int batchSize;

    public BatchingAsyncLogProcessor(int batchSize, ZLoggerOptions options)
    {
        this.batchSize = batchSize;
        this.options = options;
        this.channel = Channel.CreateUnbounded<IZLoggerEntry>(new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false, // always should be in async loop.
            SingleWriter = false,
            SingleReader = true,
        });
        this.writeLoop = Task.Run(WriteLoop);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Post(IZLoggerEntry log)
    {
        channel.Writer.TryWrite(log);
    }

    protected abstract ValueTask ProcessAsync(IReadOnlyList<INonReturnableZLoggerEntry> list);
    protected abstract ValueTask DisposeAsyncCore();

    async Task WriteLoop()
    {
        var reader = channel.Reader;
        var list = new List<IZLoggerEntry>(batchSize);

        while (await reader.WaitToReadAsync().ConfigureAwait(false))
        {
            try
            {
                while (reader.TryRead(out var value))
                {
                    list.Add(value);
                    if (batchSize < list.Count)
                    {
                        break;
                    }
                }

                try
                {
                    await ProcessAsync(list).ConfigureAwait(false);
                }
                finally
                {
                    foreach (var item in list)
                    {
                        item.Return();
                    }
                    list.Clear();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    options.InternalErrorLogger?.Invoke(ex);
                }
                catch { }
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        channel.Writer.Complete();
        await writeLoop.ConfigureAwait(false);
        await DisposeAsyncCore().ConfigureAwait(false);
    }
}
