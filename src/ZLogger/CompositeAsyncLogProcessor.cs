namespace ZLogger;

public class CompositeAsyncLogProcessor(params IAsyncLogProcessor[] innerProcessors) : IAsyncLogProcessor
{
    public void Post(IZLoggerEntry log)
    {
        foreach (var processor in innerProcessors)
        {
            processor.Post(log);
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var processor in innerProcessors)
        {
            await processor.DisposeAsync().ConfigureAwait(false);
        }
    }
}