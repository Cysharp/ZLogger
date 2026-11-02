using System;
using System.Collections.Generic;

namespace ZLogger.Generator.Tests;

public class InstanceLoggingTest
{
    [Fact]
    public void InstanceField_LogsMessageAndStructuredValues()
    {
        using var factory = TestHelper.CreateMessageLogger<InstanceLoggingTest>(out var logger, out var messages);
        new FieldLoggingExample(logger).Bar(10, 20);
        messages.Should().Equal("Bar: 10 20");

        using var jsonFactory = TestHelper.CreateJsonLogger<InstanceLoggingTest>(out var jsonLogger, out var json);
        new FieldLoggingExample(jsonLogger).Bar(10, 20);
        json.Should().Equal("{\"x\":10,\"y\":20}");
    }

    [Fact]
    public void PrimaryConstructor_LogsMessageAndStructuredValues()
    {
        using var factory = TestHelper.CreateMessageLogger<InstanceLoggingTest>(out var logger, out var messages);
        new PrimaryLoggingExample(logger).Bar(30, 40);
        messages.Should().Equal("Bar: 30 40");

        using var jsonFactory = TestHelper.CreateJsonLogger<InstanceLoggingTest>(out var jsonLogger, out var json);
        new PrimaryLoggingExample(jsonLogger).Bar(30, 40);
        json.Should().Equal("{\"x\":30,\"y\":40}");
    }

    [Fact]
    public void GenericLoggers_AndEscapedNames_AreSupported()
    {
        using var factory = TestHelper.CreateMessageLogger<InstanceLoggingTest>(out var logger, out var messages);
        new GenericFieldLoggingExample(logger).Bar(10, 20);
        new GenericPrimaryLoggingExample(logger).Bar(30, 40);
        messages.Should().Equal("Bar: 10 20", "Bar: 30 40");
    }

    [Fact]
    public void Field_IsNotShadowedByMethodParameter()
    {
        using var factory = TestHelper.CreateMessageLogger<InstanceLoggingTest>(out var logger, out var messages);
        new FieldLoggingExample(logger).ShadowedField("value");
        messages.Should().Equal("Field: value");
    }

    [Fact]
    public void Field_TakesPrecedenceOverPrimaryConstructor()
    {
        using var firstFactory = TestHelper.CreateMessageLogger<InstanceLoggingTest>(out var first, out var firstMessages);
        using var secondFactory = TestHelper.CreateMessageLogger<InstanceLoggingTest>(out var second, out var secondMessages);
        new FieldPrecedenceLoggingExample(first, second).Bar(10, 20);
        firstMessages.Should().BeEmpty();
        secondMessages.Should().Equal("Bar: 10 20");
    }

    [Fact]
    public void MethodParameter_TakesPrecedenceOverInstanceSources()
    {
        using var firstFactory = TestHelper.CreateMessageLogger<InstanceLoggingTest>(out var first, out var firstMessages);
        using var secondFactory = TestHelper.CreateMessageLogger<InstanceLoggingTest>(out var second, out var secondMessages);
        new FieldLoggingExample(first).Explicit(second, 10);
        new PrimaryLoggingExample(first).Explicit(second, 20);
        firstMessages.Should().BeEmpty();
        secondMessages.Should().Equal("Explicit: 10", "Explicit: 20");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InstanceLogger_RespectsEnabledCheck_AndSkipEnabledCheck(bool primaryConstructor)
    {
        var logger = new RecordingLogger { Enabled = false };
        if (primaryConstructor)
        {
            var instance = new PrimaryLoggingExample(logger);
            instance.Bar(10, 20);
            logger.Messages.Should().BeEmpty();
            instance.Unchecked(30);
        }
        else
        {
            var instance = new FieldLoggingExample(logger);
            instance.Bar(10, 20);
            logger.Messages.Should().BeEmpty();
            instance.Unchecked(30);
        }

        logger.EnabledChecks.Should().Be(1);
        logger.Messages.Should().Equal("Unchecked: 30");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InstanceLogger_PreservesDynamicLevelEventAndException(bool primaryConstructor)
    {
        var logger = new RecordingLogger();
        var exception = new InvalidOperationException("test");
        if (primaryConstructor)
        {
            new PrimaryLoggingExample(logger).Dynamic(LogLevel.Warning, exception, 42);
        }
        else
        {
            new FieldLoggingExample(logger).Dynamic(LogLevel.Warning, exception, 42);
        }

        logger.EnabledChecks.Should().Be(1);
        logger.CheckedLevel.Should().Be(LogLevel.Warning);
        logger.Level.Should().Be(LogLevel.Warning);
        logger.Event.Id.Should().Be(primaryConstructor ? 43 : 42);
        logger.Event.Name.Should().Be("Dynamic");
        logger.Exception.Should().BeSameAs(exception);
        logger.Messages.Should().Equal("Value: 42");
    }

    sealed class RecordingLogger : ILogger
    {
        public bool Enabled { get; set; } = true;
        public int EnabledChecks { get; private set; }
        public LogLevel CheckedLevel { get; private set; }
        public LogLevel Level { get; private set; }
        public EventId Event { get; private set; }
        public Exception Exception { get; private set; }
        public List<string> Messages { get; } = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            EnabledChecks++;
            CheckedLevel = logLevel;
            return Enabled;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Level = logLevel;
            Event = eventId;
            Exception = exception;
            Messages.Add(formatter(state, exception));
        }
    }
}

public partial class FieldLoggingExample
{
    readonly ILogger _logger;

    public FieldLoggingExample(ILogger logger) => _logger = logger;

    [ZLoggerMessage(LogLevel.Information, "Bar: {x} {y}")]
    public partial void Bar(int x, int y);

    [ZLoggerMessage(LogLevel.Information, "Field: {_logger}")]
    public partial void ShadowedField(string _logger);

    [ZLoggerMessage(LogLevel.Information, "Explicit: {value}")]
    public partial void Explicit(ILogger logger, int value);

    [ZLoggerMessage(LogLevel.Information, "Unchecked: {value}", SkipEnabledCheck = true)]
    public partial void Unchecked(int value);

    [ZLoggerMessage("Value: {value}", EventId = 42, EventName = "Dynamic")]
    public partial void Dynamic(LogLevel level, Exception exception, int value);
}

public partial class PrimaryLoggingExample(ILogger logger)
{
    [ZLoggerMessage(LogLevel.Information, "Bar: {x} {y}")]
    public partial void Bar(int x, int y);

    [ZLoggerMessage(LogLevel.Information, "Explicit: {value}")]
    public partial void Explicit(ILogger logger, int value);
}

public partial class PrimaryLoggingExample
{
    [ZLoggerMessage(LogLevel.Information, "Unchecked: {value}", SkipEnabledCheck = true)]
    public partial void Unchecked(int value);

    [ZLoggerMessage("Value: {value}", EventId = 43, EventName = "Dynamic")]
    public partial void Dynamic(LogLevel level, Exception exception, int value);
}

public partial class GenericFieldLoggingExample
{
    readonly ILogger<InstanceLoggingTest> @event;

    public GenericFieldLoggingExample(ILogger<InstanceLoggingTest> logger) => @event = logger;

    [ZLoggerMessage(LogLevel.Information, "Bar: {x} {y}")]
    public partial void Bar(int x, int y);
}

public partial class GenericPrimaryLoggingExample(ILogger<InstanceLoggingTest> @event);

public partial class GenericPrimaryLoggingExample
{
    [ZLoggerMessage(LogLevel.Information, "Bar: {x} {y}")]
    public partial void Bar(int x, int y);
}

public partial class FieldPrecedenceLoggingExample(ILogger logger, ILogger fieldLogger)
{
    readonly ILogger _logger = fieldLogger;
    public ILogger ConstructorLogger => logger;

    [ZLoggerMessage(LogLevel.Information, "Bar: {x} {y}")]
    public partial void Bar(int x, int y);
}
