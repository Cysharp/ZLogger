using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ZLogger.Formatters;

namespace ZLogger.Tests
{
    public class NamedParamTest
    {
        [Fact]
        public void StructuredLoggingOptions()
        {
            var options = new ZLoggerOptions();
            options.UseJsonFormatter();

            var processor = new TestProcessor(options);
            using var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogInformation($"{100:@TAKO} {200:@YAKI:D5} {new DateTime(2023, 12, 31),15:@T:yyyy-MM-dd}");

            var json = processor.DequeueAsString();
            var doc = JsonDocument.Parse(json).RootElement;

            Assert.Equal("100 00200      2023-12-31", doc.GetProperty("Message").GetString());
            Assert.Equal(100, doc.GetProperty("TAKO").GetInt32());
            Assert.Equal(200, doc.GetProperty("YAKI").GetInt32());
            Assert.Equal(new DateTime(2023, 12, 31), doc.GetProperty("T").GetDateTime());
        }
    }
}
