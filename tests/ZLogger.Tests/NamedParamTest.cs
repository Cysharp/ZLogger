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

            logger.ZLogInformation($"{ZLogger.Param("TAKO", 100)} {ZLogger.Param("YAKI", 200):D5} {ZLogger.Param("T", new DateTime(2023, 12, 31)),15:yyyy-MM-dd}");

            var json = processor.EntryMessages.Dequeue();
            var doc = JsonDocument.Parse(json).RootElement;

            doc.GetProperty("Message").GetString().Should().Be("100 00200      2023-12-31");
            doc.GetProperty("TAKO").GetInt32().Should().Be(100);
            doc.GetProperty("YAKI").GetInt32().Should().Be(200);
            doc.GetProperty("T").GetDateTime().Should().Be(new DateTime(2023, 12, 31));
        }
    }    
}
