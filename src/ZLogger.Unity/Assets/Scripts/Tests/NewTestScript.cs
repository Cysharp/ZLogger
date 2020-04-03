using System.Collections;
using ZLogger;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.Extensions.Logging;
using ZLogger.Providers;
using Microsoft.Extensions.Options;
using ZLogger.Entries;
using Cysharp.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using System;

namespace Tests
{
    public class NewTestScript
    {
        [Test]
        public void SimpleLogging()
        {
            var provider = new ZLoggerUnityLoggerProvider(Options.Create(new ZLoggerOptions()));
            var logger = provider.CreateLogger("mylogger");
            logger.Log(LogLevel.Debug, "foo");
        }

        [Test]
        public void SimpleZLog()
        {
            var provider = new ZLoggerUnityLoggerProvider(Options.Create(new ZLoggerOptions()));
            var logger = provider.CreateLogger("mylogger1");
            logger.ZLogDebugMessage("foo");
        }

        [Test]
        public void SimpleZLogFormat()
        {
            var provider = new ZLoggerUnityLoggerProvider(Options.Create(new ZLoggerOptions()));
            var logger = provider.CreateLogger("mylogger2");
            logger.ZLogDebug("foo{0} bar{1}", 100, 200);
        }

        static ILogger<NewTestScript> CreaterLogger()
        {
            var factory = UnityLoggerFactory.Create(builder =>
            {
                builder.AddZLoggerUnityDebug();
            });

            var mylogger = factory.CreateLogger<NewTestScript>();
            return mylogger;
        }

        [Test]
        public void FromUnityLoggerFactory()
        {
            var logger = CreaterLogger();
            logger.ZLogDebugMessage("foo");
            logger.ZLogDebug("foo{0} bar{1}", 10, 20);
        }

        [Test]
        public void AddFilterTest()
        {
            var factory = UnityLoggerFactory.Create(builder =>
            {
                builder.AddFilter<ZLoggerUnityLoggerProvider>("NewTestScript", LogLevel.Information);
                builder.AddFilter<ZLoggerUnityLoggerProvider>("OldTestScript", LogLevel.Debug);
                builder.AddZLoggerUnityDebug(x =>
                {
                    x.PrefixFormatter = (buf, info) => ZString.Utf8Format(buf, "[{0}][{1}]", info.LogLevel, info.Timestamp.LocalDateTime);
                });
            });

            var newLogger = factory.CreateLogger<NewTestScript>();
            var oldLogger = factory.CreateLogger("OldTestScript");

            newLogger.ZLogInformationMessage("NEW OK INFO");
            newLogger.ZLogDebugMessage("NEW OK DEBUG");

            oldLogger.ZLogInformationMessage("OLD OK INFO");
            oldLogger.ZLogDebugMessage("OLD OK DEBUG");
        }

        [Test]
        public void AddManyProviderTest()
        {
            var factory = UnityLoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);

                builder.AddZLoggerUnityDebug(x =>
                {
                    x.PrefixFormatter = (buf, info) => ZString.Utf8Format(buf, "UNI [{0}][{1}]", info.LogLevel, info.Timestamp.LocalDateTime);
                });
                builder.AddZLoggerFile("test_il2cpp.log", x =>
                {
                    x.PrefixFormatter = (buf, info) => ZString.Utf8Format(buf, "FIL [{0}][{1}]", info.LogLevel, info.Timestamp.LocalDateTime);
                });
            });

            var newLogger = factory.CreateLogger<NewTestScript>();
            newLogger.ZLogInformationMessage("NEW OK INFO");
            newLogger.ZLogDebugMessage("NEW OK DEBUG");
        }
    }
}
