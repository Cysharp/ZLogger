using System;
using System.Text.Json;
using FluentAssertions;
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

            doc.GetProperty("Message").GetString().Should().Be("100 00200      2023-12-31");
            doc.GetProperty("TAKO").GetInt32().Should().Be(100);
            doc.GetProperty("YAKI").GetInt32().Should().Be(200);
            doc.GetProperty("T").GetDateTime().Should().Be(new DateTime(2023, 12, 31));
        }
    }
}
