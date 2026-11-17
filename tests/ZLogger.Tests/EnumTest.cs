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

        /// <summary>
        /// Verifies that a format string on an enum value is applied as in C# interpolated strings
        /// (e.g. `D` writes the underlying value instead of the name), both in the message string and in the UTF-8 output.
        /// </summary>
        [Fact]
        public void EnumWithFormatString()
        {
            var processor = CreateProcessor(out var logger);

            var e = MyEnum.Orange;
            var undefined = (MyEnum)10;

            logger.ZLogInformation($"{e:D} {e:X} {e:G} {undefined:D}");

            AssertMessage($"{e:D} {e:X} {e:G} {undefined:D}", processor);
        }

        /// <summary>
        /// Verifies that an alignment on an enum value pads the output as in C# interpolated strings,
        /// both in the message string and in the UTF-8 output.
        /// </summary>
        [Fact]
        public void EnumWithAlignment()
        {
            var processor = CreateProcessor(out var logger);

            var e = MyEnum.Orange;
            var undefined = (MyEnum)10;

            logger.ZLogInformation($"[{e,10}] [{e,-10}] [{undefined,5}] [{undefined,-5}]");

            AssertMessage($"[{e,10}] [{e,-10}] [{undefined,5}] [{undefined,-5}]", processor);
        }

        /// <summary>
        /// Verifies that an enum value with both an alignment and a format string is formatted and then padded
        /// as in C# interpolated strings, both in the message string and in the UTF-8 output.
        /// </summary>
        [Fact]
        public void EnumWithAlignmentAndFormatString()
        {
            var processor = CreateProcessor(out var logger);

            var e = MyEnum.Orange;

            logger.ZLogInformation($"[{e,10:D}] [{e,-10:D}]");

            AssertMessage($"[{e,10:D}] [{e,-10:D}]", processor);
        }

        static TestProcessor CreateProcessor(out ILogger logger)
        {
            var processor = new TestProcessor(new ZLoggerOptions());
            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            logger = loggerFactory.CreateLogger("test");
            return processor;
        }

        static void AssertMessage(string expected, TestProcessor processor)
        {
            // IZLoggerEntry.ToString() returns the message string that ILogger.Log passes to the formatter delegate,
            // and the plain text formatter writes the message with IZLoggerEntry.ToString(IBufferWriter<byte>).
            Assert.Equal(expected, processor.Entries.Peek().ToString());
            Assert.Equal(expected, processor.DequeueAsString());
        }


        public enum MyEnum
        {
            Fruit, Apple, Orange
        }
    }
}
