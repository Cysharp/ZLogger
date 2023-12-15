using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

public static partial class Log
{
    [ZLoggerMessage(LogLevel.Information, "Could not open socket to {hostName} {ipAddress}.")]
    public static partial void Hello(this ILogger logger, string hostName, string ipAddress);
}

public class SampleBehaviour : MonoBehaviour
{
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

        var logger = loggerFactory.CreateLogger(nameof(SampleBehaviour));

        var name = "Hoge";
        var id = 12345;
        
        logger.ZLogInformation($"!!!!!! Hello {name} your id is {id:@userId}");

        using (logger.BeginScope("{Id}", id))
        {
            logger.ZLogInformation($"@@@@@@@@ Hello {name}");
        }
        
        logger.Hello("example.com", "111.111.111.111");
    }
}
