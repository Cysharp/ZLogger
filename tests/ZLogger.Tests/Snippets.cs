using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZLogger.Tests
{
    class Snippets
    {
        readonly ILogger<Snippets> logger;
        void Startup()
        {
            #region startup
            Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    // optional:clear default providers.
                    logging.ClearProviders();

                    // Add Console Logging.
                    logging.AddZLoggerConsole();

                    // Add File Logging.
                    logging.AddZLoggerFile("fileName.log");

                    // Add Rolling File Logging.
                    logging.AddZLoggerRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", x => x.ToLocalTime().Date, 1024 * 1024);

                    // Enable Structured Logging
                    logging.AddZLoggerConsole(options =>
                    {
                        options.UseDefaultStructuredLogFormatter();
                    });
                });
            #endregion
        }

        void Writing()
        {
            #region log-text
            logger.ZLogDebug("foo{0} bar{1}", 10, 20);
            #endregion

            #region log-with-structure
            logger.ZLogDebug(new { Foo = 10, Bar = 20 }, "foo{0} bar{1}", 10, 20);
            #endregion

            #region log-prepared
            var foobarLogger1 = ZLoggerMessage.Define<int, int>(LogLevel.Debug, new EventId(10, "hoge"), "foo{0} bar{1}");
            #endregion

            #region log-prepared-with-structure
            var foobarLogger2 = ZLoggerMessage.DefineWithPayload<MyMessage, int, int>(LogLevel.Warning, new EventId(10, "hoge"), "foo{0} bar{1}");
            #endregion
        }
        #region message-structure
        public struct MyMessage
        {
            public int Foo { get; set; }
            public int Bar { get; set; }
        }
        #endregion

    }
}
