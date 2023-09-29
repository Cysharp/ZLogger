using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Cysharp.Text;
using ZLogger;
using ZLogger.Providers;

namespace Tests
{
    public class NewTestScript
    {
        [Test]
        public void SimpleLogging()
        {
            var provider = new ZLoggerUnityLoggerProvider(Options.Create(new ZLoggerOptions()));
            var logger = provider.CreateLogger("mylogger");
            logger.Log(LogLevel.Debug, "foo");
        }

        [Test]
        public void SimpleZLog()
        {
            var provider = new ZLoggerUnityLoggerProvider(Options.Create(new ZLoggerOptions()));
            var logger = provider.CreateLogger("mylogger1");
            logger.ZLogDebug("foo");
        }

        [Test]
        public void SimpleZLogFormat()
        {
            var provider = new ZLoggerUnityLoggerProvider(Options.Create(new ZLoggerOptions()));
            var logger = provider.CreateLogger("mylogger2");
            logger.ZLogDebug("foo{0} bar{1}", 100, 200);
        }

        static ILogger<NewTestScript> CreaterLogger()
        {
            var factory = UnityLoggerFactory.Create(builder =>
            {
                builder.AddZLoggerUnityDebug();
            });

            var mylogger = factory.CreateLogger<NewTestScript>();
            return mylogger;
        }

        [Test]
        public void FromUnityLoggerFactory()
        {
            var logger = CreaterLogger();
            logger.ZLogDebug("foo");
            logger.ZLogDebug("foo{0} bar{1}", 10, 20);
        }

        [Test]
        public void AddFilterTest()
        {
            var factory = UnityLoggerFactory.Create(builder =>
            {
                builder.AddFilter<ZLoggerUnityLoggerProvider>("NewTestScript", LogLevel.Information);
                builder.AddFilter<ZLoggerUnityLoggerProvider>("OldTestScript", LogLevel.Debug);
                builder.AddZLoggerUnityDebug(x =>
                {
                    x.UsePlainTextFormatter(formatter =>
                    {
                        formatter.PrefixFormatter = (buf, info) => ZString.Utf8Format(buf, "[{0}][{1}]", info.LogLevel, info.Timestamp.LocalDateTime);
                    });
                });
            });

            var newLogger = factory.CreateLogger<NewTestScript>();
            var oldLogger = factory.CreateLogger("OldTestScript");

            newLogger.ZLogInformation("NEW OK INFO");
            newLogger.ZLogDebug("NEW OK DEBUG");

            oldLogger.ZLogInformation("OLD OK INFO");
            oldLogger.ZLogDebug("OLD OK DEBUG");
        }

        [Test]
        public void AddManyProviderTest()
        {
            var factory = UnityLoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);

                builder.AddZLoggerUnityDebug(x =>
                {
                    x.UsePlainTextFormatter(f =>
                    {
                        f.PrefixFormatter = (buf, info) => ZString.Utf8Format(buf, "UNI [{0}][{1}]", info.LogLevel, info.Timestamp.LocalDateTime);
                    });
                });
                builder.AddZLoggerFile("test_il2cpp.log", x =>
                {
                    x.UsePlainTextFormatter(f =>
                    {
                        f.PrefixFormatter = (buf, info) => ZString.Utf8Format(buf, "FIL [{0}][{1}]", info.LogLevel, info.Timestamp.LocalDateTime);
                    });
                });
            });

            var newLogger = factory.CreateLogger<NewTestScript>();
            newLogger.ZLogInformation("NEW OK INFO");
            newLogger.ZLogDebug("NEW OK DEBUG");
        }
    }
}
