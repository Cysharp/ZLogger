using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ZLogger.MessagePack.Tests;

public class StreamOutputTest
{
    /// <summary>
    /// Ensures that entries are written correctly to a stream-based provider when they cross the boundary of its
    /// 64 KB write buffer: no internal error is reported, and every entry can be read back in order.
    /// The first entry is larger than the whole buffer, so it crosses the boundary however the background writer is scheduled;
    /// the following small entries cross it again whenever enough of them are batched. Copying an entry with
    /// IBufferWriter.Write fails at the boundary, because the writer returns an empty span when its buffer is exactly full.
    /// </summary>
    [Fact]
    public async Task EntriesCrossingTheWriteBufferBoundaryAreAllWritten()
    {
        var errors = new List<Exception>();
        var stream = new MemoryStream();
        const int count = 200;

        using (var loggerFactory = LoggerFactory.Create(x => x.AddZLoggerStream(stream, options =>
        {
            options.UseMessagePackFormatter();
            options.InternalErrorLogger = ex => { lock (errors) errors.Add(ex); };
        })))
        {
            var logger = loggerFactory.CreateLogger("test");
            {
                var i = -1;
                var large = new string('y', 70_000);
                logger.ZLogInformation($"{i} {large}");
            }

            var text = new string('x', 3000);
            for (var i = 0; i < count; i++)
            {
                logger.ZLogInformation($"{i} {text}");
            }
        }

        Assert.Empty(errors);

        // MemoryStream.ToArray works after the provider has disposed the stream.
        var options = new MessagePackSerializerOptions(MessagePackFormatterFactory.Default.WithContractless());
        var indexes = new List<int>();
        await foreach (var entry in MessagePackSerializer.DeserializeMessagesAsync<Dictionary<object, object>>(
                           new MemoryStream(stream.ToArray()), options, TestContext.Current.CancellationToken))
        {
            indexes.Add(Convert.ToInt32(entry["i"]));
        }

        Assert.Equal(System.Linq.Enumerable.Range(-1, count + 1), indexes);
    }
}
