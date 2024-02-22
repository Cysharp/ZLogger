using Microsoft.Extensions.Logging;
using Sample;
using UnityEngine;
using ZLogger;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

//public static partial class Log
//{
//    [ZLoggerMessage(LogLevel.Information, "Could not open socket to {hostName} {ipAddress}.")]
//    public static partial void Hello(this ILogger logger, string hostName, string ipAddress);
//}

public class SampleBehaviour : MonoBehaviour
{
    ILogger logger = default!;
    
    void Start()
    {
        using var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddZLoggerUnityDebug(options =>
            {
                options.UsePlainTextFormatter();
                options.IncludeScopes = true;
            });
            
            logging.AddZLoggerUnityDebug(options =>
            {
                options.UseJsonFormatter();
                options.IncludeScopes = true;
            });

        });

        logger = loggerFactory.CreateLogger(nameof(SampleBehaviour));

        var name = "Hoge";
        var id = 12345;

        logger.ZLogInformation($"with context {name}", this);

        logger.Log(LogLevel.Information, $"Log");
        logger.LogInformation($"LogInformation");
        
        logger.ZLog(LogLevel.Information, $"ZLog Hello {name} your id is {id:@userId}");
        logger.ZLogInformation($"ZLogInformation Hello {name} your id is {id:@userId}");

        using (logger.BeginScope("{Id}", id))
        {
            logger.ZLogInformation($"Scoped log {name}");
        }
        
        logger.ZLogInformation($"with context {name}", this);
        
        new SampleClass1(logger).Method1();
    }
}
