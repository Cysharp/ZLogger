using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace ZLogger.Tests;

public class BatchingAsyncLogProcessorTest
{
    [Fact]
    public void Batch()
    {
        var options = new ZLoggerOptions();
        var batchingProcessor = new TestBatchingProcessor(3, options);

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.AddZLogger(zLogger => zLogger.AddLogProcessor(batchingProcessor));
        });

        var logger = loggerFactory.CreateLogger("test");
        for (var i = 0; i < 6; i++)
        {
            logger.LogInformation($"i={i}");
        }

        batchingProcessor.Records.Count.Should().Be(6);
        
        loggerFactory.Dispose();
        batchingProcessor.DisposeCalled.Should().BeTrue();
    }
}

file class ErrorBatchingProcessor(int batchSize, ZLoggerOptions options) : BatchingAsyncLogProcessor(batchSize, options)
{
    public List<INonReturnableZLoggerEntry> Records { get; private set; }
    public bool DisposeCalled { get; private set; }

    protected override ValueTask ProcessAsync(IReadOnlyList<INonReturnableZLoggerEntry> list)
    {
        Records.AddRange(list);
        if (Records.Count is > 3 and <= 4)
        {
            throw new InvalidOperationException();
        }
        return default;
    }

    protected override ValueTask DisposeAsyncCore()
    {
        DisposeCalled = true;
        return default;
    }
}

file class TestBatchingProcessor(int batchSize, ZLoggerOptions options) : BatchingAsyncLogProcessor(batchSize, options)
{
    public List<INonReturnableZLoggerEntry> Records { get; private set; } = [];
    public bool DisposeCalled { get; private set; }

    protected override ValueTask ProcessAsync(IReadOnlyList<INonReturnableZLoggerEntry> list)
    {
        Records.AddRange(list);
        return default;
    }

    protected override ValueTask DisposeAsyncCore()
    {
        DisposeCalled = true;
        return default;
    }
}