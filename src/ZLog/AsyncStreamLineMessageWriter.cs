using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ZLog
{
    internal class AsyncStreamLineMessageWriter : IAsyncDisposable
    {
        readonly byte[] newLine;
        readonly bool crlf;
        readonly byte newLine1;
        readonly byte newLine2;

        readonly Stream stream;
        readonly Channel<IZLogEntry> channel;
        readonly Task writeLoop;
        readonly CancellationTokenSource cancellationTokenSource;
        readonly ZLogOptions options;

        public AsyncStreamLineMessageWriter(Stream stream, ZLogOptions options)
        {
            this.newLine = Encoding.UTF8.GetBytes(Environment.NewLine);
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
            this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(options.CancellationToken);
            this.stream = stream;
            this.channel = Channel.CreateUnbounded<IZLogEntry>(new UnboundedChannelOptions
            {
                AllowSynchronousContinuations = false, // always should be in async loop.
                SingleWriter = false,
                SingleReader = true,
            });
            this.writeLoop = WriteLoop();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Post(IZLogEntry log)
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
            var reader = channel.Reader;
            try
            {
                while (await reader.WaitToReadAsync(cancellationTokenSource.Token).ConfigureAwait(false))
                {
                    try
                    {
                        while (reader.TryRead(out var value))
                        {
                            options.PrefixFormatter?.Invoke(writer, value.LogInfo);

                            value.FormatUtf8(writer);
                            value.Return();

                            options.SuffixFormatter?.Invoke(writer, value.LogInfo);

                            AppendLine(writer);
                        }
                        writer.Flush(); //flush before wait.
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (options.ErrorLogger != null)
                            {
                                options.ErrorLogger(ex);
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
                await channel.Reader.Completion;
                cancellationTokenSource.Cancel();
                await writeLoop;
            }
            finally
            {
                this.stream.Dispose();
            }
        }
    }
}
