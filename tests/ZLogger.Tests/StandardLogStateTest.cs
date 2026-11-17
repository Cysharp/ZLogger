#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace ZLogger.Tests
{
    public class StandardLogStateTest
    {
        [Fact]
        public void ParametersAreReadBeforeLogReturns()
        {
            var options = new ZLoggerOptions().UseJsonFormatter();
            var processor = new TestProcessor(options);
            using var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var state = new LiveState { Path = "/first" };
            logger.Log(LogLevel.Information, default, state, null, (s, _) => $"Request {s.Path}");

            // ILogger.Log does not promise the state stays valid after it returns
            state.Path = "/second";
            state.Disposed = true;

            var json = processor.DequeueAsString();
            var doc = JsonDocument.Parse(json).RootElement;
            doc.GetProperty("Message").GetString().Should().Be("Request /first");
            doc.GetProperty("Path").GetString().Should().Be("/first");
        }

        [Fact]
        public void OriginalFormatIsKeptInTheSnapshot()
        {
            var options = new ZLoggerOptions();
            var processor = new TestProcessor(options);
            using var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerLogProcessor(processor);
            });
            var logger = loggerFactory.CreateLogger("test");

            var state = new LiveState { Path = "/first" };
            logger.Log(LogLevel.Information, default, state, null, (s, _) => $"Request {s.Path}");
            state.Disposed = true;

            var entry = processor.Entries.Dequeue();
            entry.ParameterCount.Should().Be(1);
            entry.GetParameterKeyAsString(0).Should().Be("Path");
            entry.GetParameterValue(0).Should().Be("/first");
            entry.GetOriginalFormat().Should().Be("Request {Path}");
        }

        class LiveState : IReadOnlyList<KeyValuePair<string, object?>>
        {
            public string Path { get; set; } = "";
            public bool Disposed { get; set; }

            public int Count => 2;

            public KeyValuePair<string, object?> this[int index]
            {
                get
                {
                    if (Disposed) throw new ObjectDisposedException(nameof(LiveState));
                    return index switch
                    {
                        0 => new KeyValuePair<string, object?>("Path", Path),
                        1 => new KeyValuePair<string, object?>("{OriginalFormat}", "Request {Path}"),
                        _ => throw new ArgumentOutOfRangeException(nameof(index)),
                    };
                }
            }

            public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
            {
                for (var i = 0; i < Count; i++) yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
