using Microsoft.Extensions.Logging;
using ZLogger;

namespace Sample;

public class SampleClass1
{
    readonly ILogger logger;
    
    public SampleClass1(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Method1()
    {
        Method2();
    }

    public void Method2()
    {
        new SampleClass2<int>(logger).Method1();
    }
}

public class SampleClass2<T>
{
    readonly ILogger logger;
    
    public SampleClass2(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Method1()
    {
        logger.ZLog(LogLevel.Information, $"ZLog Hello");
        logger.ZLogInformation($"ZLogInformation Hello");
        // logger.Hello("example.com", "111.111.111.111");
    }
}