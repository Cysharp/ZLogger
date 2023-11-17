using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using ZLogger.Internal;

namespace ZLogger
{
    public class AsyncStreamLineMessageWriter : IAsyncLogProcessor, IAsyncDisposable
    {
        readonly byte[] newLine;
        readonly bool crlf;
        readonly byte newLine1;
        readonly byte newLine2;

        readonly Stream stream;
        readonly Channel<IZLoggerEntry> channel;
        readonly Task writeLoop;
        readonly ZLoggerOptions options;
        readonly Func<LogLevel, bool>? levelFilter;
        readonly CancellationTokenSource cancellationTokenSource;

        public AsyncStreamLineMessageWriter(Stream stream, ZLoggerOptions options, Func<LogLevel, bool>? levelFilter = null)
        {
            this.newLine = Encoding.UTF8.GetBytes(Environment.NewLine);
            this.cancellationTokenSource = new CancellationTokenSource();
            if (newLine.Length == 1)
            {
                // cr or lf
                this.newLine1 = newLine[0];
                this.newLine2 = default;
                this.crlf = false;
            }
            else
            {
                // crlf(windows)
                this.newLine1 = newLine[0];
                this.newLine2 = newLine[1];
                this.crlf = true;
            }

            this.options = options;
            this.stream = stream;
            this.levelFilter = levelFilter;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AppendLine(StreamBufferWriter writer)
        {
            if (writer.TryGetForNewLine(out var buffer, out var index))
            {
                if (crlf)
                {
                    buffer[index] = newLine1;
                    buffer[index + 1] = newLine2;
                    writer.Advance(2);
                }
                else
                {
                    buffer[index] = newLine1;
                    writer.Advance(1);
                }
            }
            else
            {
                var span = writer.GetSpan(newLine.Length);
                newLine.CopyTo(span);
            }
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
                    try
                    {
                        while (reader.TryRead(out var value))
                        {
                            if (levelFilter != null && levelFilter.Invoke(value.LogInfo.LogLevel) == false)
                            {
                                continue;
                            }
                            try
                            {
                                value.FormatUtf8(writer, formatter);
                            }
                            finally
                            {
                                value.Return();
                            }
                            if (formatter.WithLineBreak)
                            {
                                AppendLine(writer);
                            }
                        }

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
                                options.InternalErrorLogger(ex);
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
