using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ZLogger.Unity;


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
    }
}
