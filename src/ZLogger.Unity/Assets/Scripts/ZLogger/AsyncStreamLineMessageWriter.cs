using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZLogger.Entries;

namespace ZLogger
{
    public class AsyncStreamLineMessageWriter : IAsyncLogProcessor, IAsyncDisposable
    {
        readonly byte[] newLine;
        readonly bool   crlf;
        readonly byte   newLine1;
        readonly byte   newLine2;

        readonly Stream                  stream;
        readonly Channel<IZLoggerEntry>  channel;
        readonly Task                    writeLoop;
        readonly Task                    summaryWriteLoop;
        readonly ZLoggerOptions          options;
        readonly CancellationTokenSource cancellationTokenSource;

        public AsyncStreamLineMessageWriter(Stream stream, ZLoggerOptions options)
        {
            this.newLine                 = Encoding.UTF8.GetBytes(Environment.NewLine);
            this.cancellationTokenSource = new CancellationTokenSource();
            if (newLine.Length == 1)
            {
                // cr or lf
                this.newLine1 = newLine[0];
                this.newLine2 = default;
                this.crlf     = false;
            }
            else
            {
                // crlf(windows)
                this.newLine1 = newLine[0];
                this.newLine2 = newLine[1];
                this.crlf     = true;
            }

            this.options = options;
            this.stream  = stream;
            this.channel = Channel.CreateUnbounded<IZLoggerEntry>(new UnboundedChannelOptions
            {
                AllowSynchronousContinuations = false, // always should be in async loop.
                SingleWriter                  = false,
                SingleReader                  = true,
            });

            this.writeLoop        = Task.Run(WriteLoop);
            this.summaryWriteLoop = Task.Run(SummaryWriteLoop);
        }

        private const bool ENABLE_SPAM_DROPPER  = true;
        private const bool DEBUG_SPAM           = true;
        private const uint LIMIT_IN             = 3;
        private const uint LIMIT_OUT            = 2;
        private const uint POSTS_SECONDS_WINDOW = 1;
        private       bool isSpamming;
        private       bool didDropped;

        private readonly ConcurrentDictionary<LogLevel, int> dropSummary = new(new KeyValuePair<LogLevel, int>[]
        {
            new(LogLevel.Trace, 0),
            new(LogLevel.Debug, 0),
            new(LogLevel.Information, 0),
            new(LogLevel.Warning, 0),
            new(LogLevel.Error, 0),
            new(LogLevel.Critical, 0),
            new(LogLevel.None, 0),
        });

        private readonly ConcurrentQueue<DateTimeOffset> postTimesQ                  = new();
        private readonly ConcurrentQueue<DateTimeOffset> postTimesTempQ              = new();
        private readonly ArrayBufferWriter<byte>         bufferWriterForDebugSpamMsg = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Post(IZLoggerEntry log)
        {
#pragma warning disable 162
            if (!ENABLE_SPAM_DROPPER)
            {
                channel.Writer.TryWrite(log);
                return;
            }
#pragma warning restore 162

            CheckPostsTimesAndSetIsSpamming();

            if (isSpamming)
            {
                if (DEBUG_SPAM)
                {
                    var logInfo = new LogInfo(
                        log.LogInfo.LogId,
                        "ZLogger",
                        log.LogInfo.Timestamp,
                        log.LogInfo.LogLevel,
                        log.LogInfo.EventId,
                        log.LogInfo.Exception
                    );

                    bufferWriterForDebugSpamMsg.Clear();
                    bufferWriterForDebugSpamMsg.Write(Encoding.UTF8.GetBytes("\""));
                    log.FormatUtf8(bufferWriterForDebugSpamMsg, new(), null);
                    bufferWriterForDebugSpamMsg.Write(Encoding.UTF8.GetBytes("\""));
                    string logStr = Encoding.UTF8.GetString(bufferWriterForDebugSpamMsg.WrittenSpan);

                    IZLoggerEntry? entry =
                        FormatLogState<object, int, int, int, int, int, int, int, bool, int, string>.Factory(
                            new(
                                null,
                                "LOG DROPPED {9}, postTimes.Count {8} did Drop: {7}. Dropping: ({0}) Trace. ({1}) Debug. ({2})  Information. ({3})  Warning. ({4})  Error. ({5})  Critical. ({6})  None",
                                dropSummary[LogLevel.Trace],
                                dropSummary[LogLevel.Debug],
                                dropSummary[LogLevel.Information],
                                dropSummary[LogLevel.Warning],
                                dropSummary[LogLevel.Error],
                                dropSummary[LogLevel.Critical],
                                dropSummary[LogLevel.None],
                                didDropped,
                                postTimesQ.Count,
                                logStr
                            ),
                            logInfo
                        );

                    channel.Writer.TryWrite(entry);
                }

                dropSummary[log.LogInfo.LogLevel]++;
                didDropped = true;
            }
            else
            {
                if (channel.Writer.TryWrite(log))
                    postTimesQ.Enqueue(log.LogInfo.Timestamp);
            }
        }

        private bool CheckPostsTimesAndSetIsSpamming()
        {
            bool? spamming = PostsTimesCheck();
            if (spamming.HasValue)
                isSpamming = spamming.Value;

            return isSpamming;
        }

        private bool? PostsTimesCheck()
        {
            DateTimeOffset currentDateTime = DateTimeOffset.Now;

            if (DEBUG_SPAM && postTimesTempQ.Count > 0)
            {
                throw new Exception("postTimesQH.Count > 0");
            }

            postTimesTempQ.Clear(); // technically redundant

            while (postTimesQ.TryDequeue(out var postTime))
            {
                TimeSpan timeDiff = currentDateTime - postTime;

                if (timeDiff <= TimeSpan.FromSeconds(POSTS_SECONDS_WINDOW))
                {
                    postTimesTempQ.Enqueue(postTime);
                }
            }

            if (DEBUG_SPAM && postTimesQ.Count > 0)
            {
                throw new Exception("postTimesQ.Count > 0");
            }

            postTimesQ.Clear(); // technically redundant
            while (postTimesTempQ.TryDequeue(out var postTime))
            {
                postTimesQ.Enqueue(postTime);
            }

            if (postTimesQ.Count >= LIMIT_IN) return true;

            if (postTimesQ.Count <= LIMIT_OUT) return false;

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AppendLine(StreamBufferWriter writer)
        {
            if (writer.TryGetForNewLine(out var buffer, out var index))
            {
                if (crlf)
                {
                    buffer[index]     = newLine1;
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

        private static object allThreadsLock = new();

        async Task WriteLoop()
        {
            var            writer = new StreamBufferWriter(stream);
            var            reader = channel.Reader;
            var            sw     = Stopwatch.StartNew();
            IZLoggerEntry? value  = default;
            try
            {
                while (await reader.WaitToReadAsync().ConfigureAwait(false))
                {
                    LogInfo info = default;
                    try
                    {
                        while (reader.TryRead(out value))
                        {
                            // XXX this solves the problem of corruption when alternating threads but I'm not sure why (OP)
                            lock (allThreadsLock)
                            {
                                info = value.LogInfo;

                                // OP: try to rewrite event id from payload if exists (see ILogEvent.cs)
                                var payload = value.GetPayload();
                                if (payload is ILogEvent logEvent)
                                    value.LogInfo
                                        = new LogInfo(info.LogId, info.CategoryName, info.Timestamp, info.LogLevel,
                                                      logEvent.GetEventId(), info.Exception);

                                if (options.EnableStructuredLogging)
                                {
                                    var jsonWriter = options.GetThreadStaticUtf8JsonWriter(writer);
                                    try
                                    {
                                        jsonWriter.WriteStartObject();

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
                                    value.FormatUtf8(writer, options, null);
                                    value.Return();
                                }

                                AppendLine(writer);
                            }

                            info = default;

                            if (options.FlushRate != null && !cancellationTokenSource.IsCancellationRequested)
                            {
                                sw.Stop();
                                TimeSpan sleepTime = options.FlushRate.Value - sw.Elapsed;
                                if (sleepTime > TimeSpan.Zero)
                                {
                                    try
                                    {
                                        await Task.Delay(sleepTime, cancellationTokenSource.Token)
                                                  .ConfigureAwait(false);
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
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (options.InternalErrorLogger != null)
                            {
                                options.InternalErrorLogger(info, ex, value);
                            }
                            else
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        catch
                        {
                            // ignored
                        }
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
                catch
                {
                    // ignored
                }
            }
        }

        private void SummaryWriteLoop()
        {
            while (didDropped)
            {
                try
                {
                    CheckPostsTimesAndSetIsSpamming();
                }
                catch
                {
                    // ignored
                }

                if (isSpamming) continue;

                // create summary log entry
                try
                {
                    Exception? exception = null; // can add an exception new Exception("Log spamming detected")
                    var logInfo = new LogInfo(
                        0,
                        "ZLogger",
                        DateTimeOffset.Now,
                        LogLevel.Warning,
                        new EventId(0),
                        exception
                    );

                    IZLoggerEntry? entry = FormatLogState<object, int, int, int, int, int, int, int>.Factory(
                        new
                        (
                            null,
                            "Log spamming summary. Dropped: ({0}) Trace. ({1}) Debug. ({2})  Information. ({3})  Warning. ({4})  Error. ({5})  Critical. ({6})  None",
                            dropSummary[LogLevel.Trace],
                            dropSummary[LogLevel.Debug],
                            dropSummary[LogLevel.Information],
                            dropSummary[LogLevel.Warning],
                            dropSummary[LogLevel.Error],
                            dropSummary[LogLevel.Critical],
                            dropSummary[LogLevel.None]
                        ),
                        logInfo
                    );

                    channel.Writer.TryWrite(entry);

                    // reset drop summary
                    foreach (var key in dropSummary.Keys)
                    {
                        dropSummary[key] = 0;
                    }
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    didDropped = false;
                }
            }

            ;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                channel.Writer.Complete();
                cancellationTokenSource.Cancel();
                await writeLoop.ConfigureAwait(false);
                await summaryWriteLoop.ConfigureAwait(false);
            }
            finally
            {
                this.stream.Dispose();
            }
        }
    }
}
