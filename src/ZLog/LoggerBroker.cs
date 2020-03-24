using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ZLog
{
    internal class LoggerBroker : IAsyncDisposable
    {
        readonly byte[] newLine;
        readonly bool crlf;
        readonly byte newLine1;
        readonly byte newLine2;

        readonly Stream stream;
        readonly Channel<IUtf8LogEntry> channel;
        readonly Task writeLoop;
        readonly CancellationTokenSource cancellationTokenSource;

        public LoggerBroker(CancellationToken cancellationToken = default)
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

            this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            this.stream = Console.OpenStandardOutput();
            this.channel = Channel.CreateUnbounded<IUtf8LogEntry>(new UnboundedChannelOptions
            {
                AllowSynchronousContinuations = true,
                SingleWriter = false,
                SingleReader = true,
            });
            this.writeLoop = WriteLoop();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Post(IUtf8LogEntry log)
        {
            channel.Writer.TryWrite(log);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AppendLine(ConsoleStreamBufferWriter writer)
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
            var writer = new ConsoleStreamBufferWriter(stream);
            var reader = channel.Reader;
            try
            {
                while (await reader.WaitToReadAsync(cancellationTokenSource.Token).ConfigureAwait(false))
                {
                    while (reader.TryRead(out var value))
                    {
                        value.FormatUtf8(writer);
                        value.Return();
                        AppendLine(writer);
                    }

                    writer.Flush(); //flush before wait.
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
            channel.Writer.Complete();
            await channel.Reader.Completion;
            cancellationTokenSource.Cancel();
            await writeLoop;
        }
    }
}
