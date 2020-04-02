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


            // most simple Log.
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

        [Test]
        public void ServiceCollectionBuild()
        {
            {
                var services = new ServiceCollection();
                services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
                services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
                var serviceProvider = services.BuildServiceProvider();

                var engineField = typeof(ServiceProvider).GetField("_engine", BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                UnityEngine.Debug.Log("ok? engineName:" + engineField.GetValue(serviceProvider).GetType().FullName);
            }

            {
                var services = new ServiceCollection();
                services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
                services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

                var options = new ServiceProviderOptions();
                var mode = typeof(ServiceProviderOptions).GetProperty("Mode", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                 
                mode.SetValue(options, Convert.ChangeType(Enum.GetValues(mode.PropertyType).GetValue(Array.IndexOf(Enum.GetNames(mode.PropertyType), "Runtime")), mode.PropertyType));

                var serviceProvider = services.BuildServiceProvider(options);

                var engineField = typeof(ServiceProvider).GetField("_engine", BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                UnityEngine.Debug.Log("ok? engineName:" + engineField.GetValue(serviceProvider).GetType().FullName);


                var factory = serviceProvider.GetService<ILoggerFactory>();
                UnityEngine.Debug.Log("go create?");
                var log = factory.CreateLogger("my");
                log.LogDebug("yeah");
            }


        }


        [Test]
        public void OnlyCreate()
        {
            UnityEngine.Debug.Log("run start");
            var factory = LoggerFactory.Create(builder =>
            {
            });
            UnityEngine.Debug.Log("factory create ok");
            var mylogger = factory.CreateLogger<ILogger<NewTestScript>>();
            UnityEngine.Debug.Log("logger create ok");
        }



        [Test]
        public void FromLoggerFactory()
        {
            UnityEngine.Debug.Log("run start");

            var factory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddZLoggerUnityDebug();
                //builder.AddZLoggerUnityDebug(option =>
                //{
                //    option.PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "[{0}][{1}]", info.LogLevel, info.CategoryName);
                //});
            });
            UnityEngine.Debug.Log("factory is not null =>" + (factory == null));

            var mylogger = factory.CreateLogger<ILogger<NewTestScript>>();
            UnityEngine.Debug.Log("mylogger is null" + (mylogger == null));

            mylogger.ZLogDebugMessage("foo");
            UnityEngine.Debug.Log("ok simple message");

            mylogger.ZLogDebug("Age:{0} Name:{1} Money:{2}", 35, "tako", 99);
        }

        // TODO:try check option
    }
}
