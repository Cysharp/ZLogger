using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ZLogger.Tests
{
    public class InterpolatedManyTypesTest
    {
        [Fact]
        public void EnumerableJson()
        {
            var stringProcessor = new TestProcessor(new ZLoggerOptions());
            var jsonProcessor = new TestProcessor(new ZLoggerOptions().UseJsonFormatter(x => x.IncludeProperties = IncludeProperties.ParameterKeyValues));
            using var loggerFactory = LoggerFactory.Create(logging =>
            {
                logging.AddZLoggerLogProcessor(stringProcessor);
                logging.AddZLoggerLogProcessor(jsonProcessor);
            });
            var logger = loggerFactory.CreateLogger<InterpolatedManyTypesTest>();


            int? foo = 10;
            int? bar = null;
            DateTime dt = new DateTime(1999, 12, 10);
            DateTime? dtn = null;
            var seq = Enumerable.Range(1, 5);
            var vec = new Vec { X = 1, Y = 2, Z = 3 };
            var vec2 = new Vec { X = 4, Y = 5, Z = 6 };

            logger.ZLogInformation($"f:{foo} b:{bar} d:{dt:yyyy-MM-dd} n:{dtn} s:{seq} v:{vec:json} v2:{vec2:@v2:json}");

            var msg = stringProcessor.DequeueAsString();
            msg.Should().Be("f:10 b: d:1999-12-10 n: s:[1,2,3,4,5] v:{\"X\":1,\"Y\":2,\"Z\":3} v2:{\"X\":4,\"Y\":5,\"Z\":6}");

            var json = jsonProcessor.DequeueAsString();
            json.Should().Be("""
{"foo":10,"bar":null,"dt":"1999-12-10T00:00:00","dtn":null,"seq":[1,2,3,4,5],"vec":{"X":1,"Y":2,"Z":3},"v2":{"X":4,"Y":5,"Z":6}}
""");
        }
    }

    public class Vec
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
