using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ZLogger.Tests;

public class BatchingAsyncLogProcessorTest
{
    [Fact]
    public void LessThanBatchSize()
    {
        var options = new ZLoggerOptions();
        var batchingProcessor = new TestBatchingProcessor(10, options);

        using (var loggerFactory = LoggerFactory.Create(x => x.AddZLoggerLogProcessor(batchingProcessor)))
        {
            var logger = loggerFactory.CreateLogger("test");

            for (var i = 0; i < 5; i++)
            {
                logger.ZLogInformation($"{i}");
            }
        }

        Assert.Equal(5, batchingProcessor.Records.Count);
        Assert.True(batchingProcessor.DisposeCalled);
    }
    
    [Fact]
    public void OverflowBatchSize()
    {
        var options = new ZLoggerOptions();
        var batchingProcessor = new TestBatchingProcessor(3, options);

        using (var loggerFactory = LoggerFactory.Create(x => x.AddZLoggerLogProcessor(batchingProcessor)))
        {
            var logger = loggerFactory.CreateLogger("test");

            for (var i = 0; i < 10; i++)
            {
                logger.ZLogInformation($"{i}");
            }
        }

        Assert.Equal(10, batchingProcessor.Records.Count);
        Assert.True(batchingProcessor.DisposeCalled);
    }

    [Fact]
    public void ErrorProcess()
    {
        var options = new ZLoggerOptions();
        var batchingProcessor = new ErrorBatchingProcessor(1, options);

        using (var loggerFactory = LoggerFactory.Create(x => x.AddZLoggerLogProcessor(batchingProcessor)))
        {
            var logger = loggerFactory.CreateLogger("test");
            logger.ZLogInformation($"hehehe");
            logger.ZLogInformation($"ahahah");
        }

        Assert.Equal("hehehe", batchingProcessor.Records[0]);
        Assert.Equal("ahahah", batchingProcessor.Records[1]);
        Assert.True(batchingProcessor.DisposeCalled);
    }
}

file class TestBatchingProcessor(int batchSize, ZLoggerOptions options) : BatchingAsyncLogProcessor(batchSize, options)
{
    public List<string> Records { get; private set; } = [];
    public int ProcessCalls { get; private set; }
    public bool DisposeCalled { get; private set; }

    protected override async ValueTask ProcessAsync(IReadOnlyList<INonReturnableZLoggerEntry> list)
    {
        ProcessCalls++;
        foreach (var x in list)
        {
            Records.Add(x.ToString());
        }
        await Task.Delay(100);
    }

    protected override ValueTask DisposeAsyncCore()
    {
        DisposeCalled = true;
        return default;
    }
}

file class ErrorBatchingProcessor(int batchSize, ZLoggerOptions options) : BatchingAsyncLogProcessor(batchSize, options)
{
    public List<string> Records { get; private set; } = [];
    public int ProcessCalls { get; private set; }
    public bool DisposeCalled { get; private set; }

    protected override async ValueTask ProcessAsync(IReadOnlyList<INonReturnableZLoggerEntry> list)
    {
        ProcessCalls++;
        
        foreach (var x in list)
        {
            Records.Add(x.ToString());
        }
        await Task.Delay(100);
        
        throw new InvalidOperationException();
    }

    protected override ValueTask DisposeAsyncCore()
    {
        DisposeCalled = true;
        return default;
    }
}
