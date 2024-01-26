using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using ZLogger.Internal;

namespace ZLogger
{
    public sealed class AsyncStreamLineMessageWriter : IAsyncLogProcessor, IAsyncDisposable
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
        
        public AsyncStreamLineMessageWriter(Stream stream, ZLoggerOptions options)
            : this(stream, options, null)
        {
        }

        // handling `Func<LogLevel, bool>? levelFilter` correctly is very context dependent.
        // so only allows internal provider.
        internal AsyncStreamLineMessageWriter(Stream stream, ZLoggerOptions options, Func<LogLevel, bool>? levelFilter = null)
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
            this.stream = stream;
            this.levelFilter = levelFilter;

            channel = this.options.FullMode switch
            {
                BackgroundBufferFullMode.Grow => Channel.CreateUnbounded<IZLoggerEntry>(new UnboundedChannelOptions
                {
                    AllowSynchronousContinuations = false, // always should be in async loop.
                    SingleWriter = false,
                    SingleReader = true,
                }),
                BackgroundBufferFullMode.Block => Channel.CreateBounded<IZLoggerEntry>(new BoundedChannelOptions(options.BackgroundBufferCapacity)
                {
                    AllowSynchronousContinuations = false,
                    SingleWriter = false,
                    SingleReader = true,
                    FullMode = BoundedChannelFullMode.Wait,
                }),
                BackgroundBufferFullMode.Drop => Channel.CreateBounded<IZLoggerEntry>(new BoundedChannelOptions(options.BackgroundBufferCapacity)
                {
                    AllowSynchronousContinuations = false,
                    SingleWriter = false,
                    SingleReader = true,
                    FullMode = BoundedChannelFullMode.DropWrite,
                }),
                _ => throw new ArgumentOutOfRangeException()
            };
                
            this.writeLoop = Task.Run(WriteLoop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Post(IZLoggerEntry log)
        {
            var written = channel.Writer.TryWrite(log);
            if (!written && options.FullMode == BackgroundBufferFullMode.Block)
            {
                PostSlow(log);
            }
        }

        void PostSlow(IZLoggerEntry log)
        {
            channel.Writer.WriteAsync(log).AsTask().Wait();
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
                writer.Advance(2);
            }
        }

        async Task WriteLoop()
        {
            var writer = new StreamBufferWriter(stream);
            var formatter = options.CreateFormatter();
            var withLineBreak = formatter.WithLineBreak;
            var requireFilterCheck = levelFilter != null;
            var reader = channel.Reader;

            while (await reader.WaitToReadAsync().ConfigureAwait(false))
            {
                try
                {
                    while (reader.TryRead(out var value))
                    {
                        if (requireFilterCheck && levelFilter!.Invoke(value.LogInfo.LogLevel) == false)
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

                        if (withLineBreak)
                        {
                            AppendLine(writer);
                        }
                    }

                    writer.Flush(); // flush before wait.
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
            try
            {
                channel.Writer.Complete();
                await writeLoop.ConfigureAwait(false);
            }
            finally
            {
                this.stream.Dispose();
            }
        }
    }
}
