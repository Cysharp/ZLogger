using System;
using System.Runtime.CompilerServices;
using ZLogger.Generator.Tests;

namespace ZLogger.Tests.GeneratorTests
{
    using static LogLevel;

    public partial class GenerateTest(ITestOutputHelper output)
    {
        // check for instance
        [ZLoggerMessage(Information, "Hello {name}")]
        partial void Hello(ILogger logger, string name);

        [ZLoggerMessage(Information, "Bye {name}")]
        partial void Bye(ILogger logger, string name, [CallerMemberName] string memberName = "");


        [Fact]
        public void Test()
        {
            using var _ = TestHelper.CreateMessageLogger<GenerateTest>(out var logger, out var list);

            Hello(logger, "foo");
            list[0].Should().Be("Hello foo");

            StaticHolder.Hello(logger, "bar");
            list[1].Should().Be("Hello bar");

            logger.Hello2("baz");
            list[2].Should().Be("Hello baz");
        }

        [Fact]
        public void JsonTest()
        {
            using var _ = TestHelper.CreateJsonLogger<GenerateTest>(out var logger, out var list);

            logger.Hello3(10, 20);

            list[0].Should().Be("{\"x\":10,\"y\":20}");
        }

        [Fact]
        public void AlignmentAndFormat()
        {
            using var _ = TestHelper.CreateMessageLogger<GenerateTest>(out var logger, out var list);

            logger.AlignmentAndFormatCheck(100, new DateTime(1999, 12, 31));

            var expected = $"Hello{100,-10}Bar{new DateTime(1999, 12, 31),15:yyyy-MM-dd}desu";
            output.WriteLine(expected);
            list[0].Should().Be(expected);
        }

        [Fact]
        public void JsonAndEnumerableMessage()
        {
            using var _ = TestHelper.CreateMessageLogger<GenerateTest>(out var logger, out var list);

            logger.JsonAndEnumerable(new MyVec3 { X = 10, Y = 20, Z = 30 }, new[] { 1, 2, 3 });
            list[0].Should().Be("Hello {\"X\":10,\"Y\":20,\"Z\":30} and [1,2,3]");
        }

        [Fact]
        public void ManyTypes()
        {
            var gd = Guid.NewGuid();
            var gd2 = Guid.NewGuid();

            using (TestHelper.CreateMessageLogger<GenerateTest>(out var logger, out var list))
            {
                logger.ManyTypes(MyEnum.Apple, null, new DateTime(1999, 12, 31), new DateTimeOffset(new DateTime(2001, 1, 3), TimeSpan.FromHours(4.0)), null, gd, null, null); // with null
                logger.ManyTypes(MyEnum.Orange, MyEnum.Fruit, new DateTime(1999, 12, 31), new DateTimeOffset(new DateTime(2001, 1, 3), TimeSpan.FromHours(4.0)), new DateTime(2014, 3, 4), gd, gd2, 100); // non null

                list[0].Should().Be($"Apple  1999-12-31 2001-01-03+04:00  {gd}  ");
                list[1].Should().Be($"Orange Fruit 1999-12-31 2001-01-03+04:00 2014-03-04 {gd} {gd2} 100");
            }

            using (TestHelper.CreateJsonLogger<GenerateTest>(out var logger, out var list))
            {
                logger.ManyTypes(MyEnum.Apple, null, new DateTime(1999, 12, 31), new DateTimeOffset(new DateTime(2001, 1, 3), TimeSpan.FromHours(4.0)), null, gd, null, null); // with null
                logger.ManyTypes(MyEnum.Orange, MyEnum.Fruit, new DateTime(1999, 12, 31), new DateTimeOffset(new DateTime(2001, 1, 3), TimeSpan.FromHours(4.0)), new DateTime(2014, 3, 4), gd, gd2, 100); // non null

                list[0].Should().Be("{\"e\":\"Apple\",\"ne\":null,\"dt\":\"1999-12-31T00:00:00\",\"dto\":\"2001-01-03T00:00:00+04:00\",\"dtn\":null,\"gd\":\"" + gd + "\",\"gdd\":null,\"inn\":null}");
                list[1].Should().Be("{\"e\":\"Orange\",\"ne\":\"Fruit\",\"dt\":\"1999-12-31T00:00:00\",\"dto\":\"2001-01-03T00:00:00+04:00\",\"dtn\":\"2014-03-04T00:00:00\",\"gd\":\"" + gd + "\",\"gdd\":\"" + gd2 + "\",\"inn\":100}");
            }
        }

        [Fact]
        public void CustomFormat()
        {
            using (TestHelper.CreateMessageLogger<GenerateTest>(out var logger, out var list))
            {
                logger.CustomFormat("foo", 1234);
                list[0].Should().Be("Hello foo 00001234");
            }
            using (TestHelper.CreateJsonLogger<GenerateTest>(out var logger, out var list))
            {
                logger.CustomFormat("foo", 1234);
                list[0].Should().Be("""
{"no-name":"foo","a ge pa n da":1234}
""");
            }
        }

        // TODO: SkipEnabledCheck test
        // TODO: FirstExceptoin test
        // TODO: FirstLogLevel test
    }


    public static partial class StaticHolder
    {
        // check for static
        [ZLoggerMessage(Information, "Hello {name}")]
        public static partial void Hello(ILogger logger, string name);

        // check for extension method
        [ZLoggerMessage(Information, "Hello {name}")]
        public static partial void Hello2(this ILogger logger, string name);

        // check for typed logger
        [ZLoggerMessage(Information, "Hello {x} {y}")]
        public static partial void Hello3(this ILogger<GenerateTest> logger, int x, int y);

        [ZLoggerMessage(Information, "Hello{x,-10}Bar{dt,15:yyyy-MM-dd}desu")]
        public static partial void AlignmentAndFormatCheck(this ILogger<GenerateTest> logger, int x, DateTime dt);


        [ZLoggerMessage(Information, "Hello {vec3:json} and {array}")]
        public static partial void JsonAndEnumerable(this ILogger<GenerateTest> logger, MyVec3 vec3, int[] array);

        [ZLoggerMessage(Information, "{e} {ne} {dt:yyyy-MM-dd} {dto:yyyy-MM-ddzzz} {dtn:yyyy-MM-dd} {gd} {gdd} {inn}")]
        public static partial void ManyTypes(this ILogger<GenerateTest> logger, MyEnum e, MyEnum? ne, DateTime dt, DateTimeOffset dto, DateTime? dtn, Guid gd, Guid? gdd, int? inn);

        [ZLoggerMessage(Information, "Hello {name:@no-name} {age:@a ge pa n da:00000000}")]
        public static partial void CustomFormat(this ILogger logger, string name, int age);
    }

    public enum MyEnum
    {
        Fruit, Apple, Orange
    }

    public struct MyVec3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    // not supported.
    //public partial class GenericType<T>
    //{
    //    [ZLoggerMessage(Information, "Hello {name}")]
    //    public static partial void Hello(ILogger logger, string name);

    //    [ZLoggerMessage(Information, "Hello {name}")]
    //    public static partial void GenericsParameter(ILogger logger, T name);
    //}
}
