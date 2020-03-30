using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

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
        readonly CancellationTokenSource cancellationTokenSource;
        readonly ZLoggerOptions options;

        public AsyncStreamLineMessageWriter(Stream stream, ZLoggerOptions options)
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
            var reader = channel.Reader;
            try
            {
                while (await reader.WaitToReadAsync(cancellationTokenSource.Token).ConfigureAwait(false))
                {
                    try
                    {
                        while (reader.TryRead(out var value))
                        {
                            var info = value.LogInfo;

                            if (options.IsStructuredLogging)
                            {
                                var jsonWriter = options.GetThradStaticUtf8JsonWriter(writer);
                                try
                                {
                                    jsonWriter.WriteStartObject();

                                    options.StructuredLoggingFormatter?.Invoke(jsonWriter, info);

                                    value.FormatUtf8(writer, options, jsonWriter);
                                    value.Return();

                                    jsonWriter.WriteEndObject();
                                    jsonWriter.Flush();
                                }
                                finally
                                {
                                    jsonWriter.Reset();
                                }
                            }
                            else
                            {
                                options.PrefixFormatter?.Invoke(writer, info);

                                value.FormatUtf8(writer, options, null);
                                value.Return();

                                options.SuffixFormatter?.Invoke(writer, info);

                                if (info.Exception != null)
                                {
                                    options.ExceptionFormatter(writer, info.Exception);
                                }
                            }

                            AppendLine(writer);
                        }
                        writer.Flush(); //flush before wait.
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (options.InternalErrorLogger != null)
                            {
                                options.InternalErrorLogger(ex);
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
