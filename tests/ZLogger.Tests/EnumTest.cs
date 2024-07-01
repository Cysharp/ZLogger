using System.IO;
using System.Text.Json;
using System;

namespace ZLogger.Tests
{
    public class EnumTest
    {
        [Fact]
        public void EnumFormatAsString()
        {
            var options = new ZLoggerOptions();
            options.UsePlainTextFormatter(plainText =>
            {
                plainText.SetExceptionFormatter((writer, ex) => { });
            });

            var processor = new TestProcessor(options);

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            MyEnum e = MyEnum.Orange;
            MyEnum? ne = MyEnum.Fruit;
            DateTime dt = new DateTime(1999, 12, 31);
            DateTimeOffset dto = new DateTimeOffset(new DateTime(2001, 1, 3), TimeSpan.FromHours(4.0));
            DateTime? dtn = new DateTime(2014, 3, 4);
            Guid gd = Guid.NewGuid();
            Guid gd2 = Guid.NewGuid();
            int? inn = 100;

            logger.ZLogDebug($"{e} {ne} {dt:yyyy-MM-dd} {dto:yyyy-MM-ddzzz} {dtn:yyyy-MM-dd} {gd} {gd2} {inn}");

            processor.DequeueAsString().Should().Be($"Orange Fruit 1999-12-31 2001-01-03+04:00 2014-03-04 {gd} {gd2} 100");
        }


        public enum MyEnum
        {
            Fruit, Apple, Orange
        }
    }
}
