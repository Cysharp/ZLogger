using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZLogger.Tests
{
    public class StateTest
    {
        [Fact]
        public void VersionUnmatch()
        {
            Queue<IFormatter> logs = new();
            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Trace);
                x.AddProvider(new MyProvider(logs));
            });

            var logger = loggerFactory.CreateLogger("MyCategory");

            logger.ZLogDebug($"foo");

            var formatter = logs.Dequeue();

            Assert.Throws<InvalidOperationException>(() => formatter.Format());
        }

        [Fact]
        public void EnumerateState()
        {
            Queue<IFormatter> logs = new();
            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Trace);
                x.AddProvider(new MyProvider(logs));
            });

            var logger = loggerFactory.CreateLogger("MyCategory");
            
            var handler = Hoge(logger, LogLevel.Information, $"any state {100:@x} {200:@y}");
            var state = handler.GetState();
            using (var enumerator = state.GetEnumerator())
            {
                enumerator.MoveNext().Should().BeTrue();
                enumerator.Current.Key.Should().Be("x");
                enumerator.Current.Value.Should().Be(100);
            
                enumerator.MoveNext().Should().BeTrue();
                enumerator.Current.Key.Should().Be("y");
                enumerator.Current.Value.Should().Be(200);
            
                enumerator.MoveNext().Should().BeFalse();
            }
            state.Release();
            
            handler = Hoge(logger, LogLevel.Information, $"empty state");
            state = handler.GetState();
            using (var enumerator = state.GetEnumerator())
            {
                enumerator.MoveNext().Should().BeFalse();
            }
            state.Release();
            
            ZLoggerInterpolatedStringHandler Hoge(ILogger logger, LogLevel logLevel, [InterpolatedStringHandlerArgument("logger", "logLevel")] ZLoggerInterpolatedStringHandler handler)
            {
                return handler;
            }
        }
    }

    file class MyProvider : ILoggerProvider
    {
        Queue<IFormatter> logs;

        public MyProvider(Queue<IFormatter> logs)
        {
            this.logs = logs;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger(logs);
        }

        public void Dispose()
        {
        }
    }

    file class MyLogger : ILogger
    {
        Queue<IFormatter> logs;

        public MyLogger(Queue<IFormatter> logs)
        {
            this.logs = logs;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return new NullDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            logs.Enqueue(new LogState<TState> { State = state, Formatter = formatter });
        }

        class NullDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    file interface IFormatter
    {
        string Format();
    }

    file class LogState<TState> : IFormatter
    {
        public TState State { get; set; }
        public Func<TState, Exception, string> Formatter { get; set; }

        public string Format()
        {
            return Formatter(State, null);
        }
    }
}
