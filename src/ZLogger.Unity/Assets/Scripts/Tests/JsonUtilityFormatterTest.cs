using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UnityEngine;
using ZLogger;
using ZLogger.Formatters;

namespace Tests
{
    [Serializable]
    class SerializablePayload
    {
        public int X;
    }

    class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
    }

    class TestProcessor : IAsyncLogProcessor
    {
        public Queue<string> EntryMessages = new();
        readonly ZLoggerOptions options;
        readonly IZLoggerFormatter formatter;

        public string Dequeue() => EntryMessages.Dequeue();

        public TestProcessor(ZLoggerOptions options)
        {
            this.options = options;
            formatter = options.CreateFormatter();
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public void Post(IZLoggerEntry log)
        {
            EntryMessages.Enqueue(log.FormatToString(formatter));
        }
    }

    [TestFixture]
    public class JsonUtilityFormatterTest
    {
        TestProcessor processor;
        ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            var options = new ZLoggerOptions();
            options.UseUnityJsonUtilityFormatter();
            processor = new TestProcessor(options);

            loggerFactory = UnityLoggerFactory.Create(x =>
            {
                x.AddZLoggerLogProcessor(processor);
            });
        }

        [Test]
        public void PlainMessage()
        {
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogInformation(new EventId(1, "DO"), "AAA {0} BBB {1}", 111, "Hello");
            var json = processor.Dequeue();
            var value = JsonUtility.FromJson<SerializableLogEntry>(json);
            Assert.That(value.CategoryName, Is.EqualTo("test"));
            Assert.That(value.LogLevel, Is.EqualTo("Information"));
            Assert.That(value.EventId, Is.EqualTo(1));
            Assert.That(value.EventIdName, Is.EqualTo("DO"));
            Assert.That(value.Message, Is.EqualTo("AAA 111 BBB Hello"));
            Assert.That(json, Is.Not.Contain("Exception"));
            Assert.That(json, Is.Not.Contain("Payload"));
        }

        [Test]
        public void WithException()
        {
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogError(new EventId(1, "OH"), new TestException("Hoge"), "DAMEDA {0}", 111);

            var json = processor.Dequeue();
            var value = JsonUtility.FromJson<SerializableLogEntryWithException>(json);
            Assert.That(value.CategoryName, Is.EqualTo("test"));
            Assert.That(value.LogLevel, Is.EqualTo("Error"));
            Assert.That(value.EventId, Is.EqualTo(1));
            Assert.That(value.EventIdName, Is.EqualTo("OH"));
            Assert.That(value.Message, Is.EqualTo("DAMEDA 111"));
            Assert.That(value.Exception.Name, Is.EqualTo("Tests.TestException"));
            Assert.That(json, Is.Not.Contain("Payload"));
        }

        [Test]
        public void WithPayload()
        {
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogInformationWithPayload(new EventId(1, "OH"), new SerializablePayload { X = 999}, "DAMEDA {0}", 111);

            var json = processor.Dequeue();
            var value = JsonUtility.FromJson<SerializableLogEntryWithPayload<SerializablePayload>>(json);
            Assert.That(value.CategoryName, Is.EqualTo("test"));
            Assert.That(value.LogLevel, Is.EqualTo("Information"));
            Assert.That(value.EventId, Is.EqualTo(1));
            Assert.That(value.EventIdName, Is.EqualTo("OH"));
            Assert.That(value.Message, Is.EqualTo("DAMEDA 111"));
            Assert.That(value.Payload.X, Is.EqualTo(999));
            Assert.That(json, Is.Not.Contain("Exception"));
        }


        [Test]
        public void WithPayloadAndException()
        {
            var logger = loggerFactory.CreateLogger("test");

            logger.ZLogErrorWithPayload(new EventId(1, "OH"), new TestException("hogehoge"), new SerializablePayload { X = 999 }, "DAMEDA {0}", 111);

            var json = processor.Dequeue();
            var value = JsonUtility.FromJson<SerializableLogEntryWithPayloadAndException<SerializablePayload>>(json);
            Assert.That(value.CategoryName, Is.EqualTo("test"));
            Assert.That(value.LogLevel, Is.EqualTo("Error"));
            Assert.That(value.EventId, Is.EqualTo(1));
            Assert.That(value.EventIdName, Is.EqualTo("OH"));
            Assert.That(value.Message, Is.EqualTo("DAMEDA 111"));
            Assert.That(value.Exception.Name, Is.EqualTo("Tests.TestException"));
            Assert.That(value.Payload.X, Is.EqualTo(999));
        }
    }
}