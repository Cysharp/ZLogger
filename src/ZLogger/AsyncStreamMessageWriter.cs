using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace ZLogger
{
    public class AsyncStreamMessageWriter : IAsyncLogProcessor, IAsyncDisposable
    {
        readonly Stream stream;
        readonly Channel<IZLoggerEntry> channel;
        readonly Task writeLoop;
        readonly ZLoggerOptions options;
        readonly CancellationTokenSource cancellationTokenSource;

        public AsyncStreamMessageWriter(Stream stream, ZLoggerOptions options)
        {
            this.cancellationTokenSource = new CancellationTokenSource();

            this.options = options;
            this.stream = stream;
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

        async Task WriteLoop()
        {
            var writer = new StreamBufferWriter(stream);
            var formatter = options.CreateFormatter();
            var reader = channel.Reader;
            var sw = Stopwatch.StartNew();
            try
            {
                while (await reader.WaitToReadAsync().ConfigureAwait(false))
                {
                    LogInfo info = default;
                    try
                    {
                        while (reader.TryRead(out var value))
                        {
                            info = value.LogInfo;
                            value.FormatUtf8(writer, formatter);
                            (value as IReturnableZLoggerEntry)?.Return();
                        }
                        info = default;

                        if (options.FlushRate != null && !cancellationTokenSource.IsCancellationRequested)
                        {
                            sw.Stop();
                            var sleepTime = options.FlushRate.Value - sw.Elapsed;
                            if (sleepTime > TimeSpan.Zero)
                            {
                                try
                                {
                                    await Task.Delay(sleepTime, cancellationTokenSource.Token).ConfigureAwait(false);
                                }
                                catch (OperationCanceledException)
                                {
                                }
                            }
                        }
                        writer.Flush(); // flush before wait.

                        sw.Reset();
                        sw.Start();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (options.InternalErrorLogger != null)
                            {
                                options.InternalErrorLogger(info, ex);
                            }
                            else
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                try
                {
                    writer.Flush();
                }
                catch { }
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                channel.Writer.Complete();
                cancellationTokenSource.Cancel();
                await writeLoop.ConfigureAwait(false);
            }
            finally
            {
                this.stream.Dispose();
            }
        }
    }
}
