using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZLogger;
using ZLogger.Providers;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //var factory = new LoggerFactory(;
     


        //var factory = LoggerFactory.Create(builder =>
        //{
        //    builder.SetMinimumLevel(LogLevel.Trace);
        //    builder.AddZLoggerUnityDebug();
        //});

        //var mylogger = factory.CreateLogger<ILogger<NewBehaviourScript>>();

        //mylogger.ZLogDebugMessage("foo bar baz");

    }

    // Update is called once per frame
    void Update()
    {

    }
}

internal static class MyProviderAliasUtilities
{
    private const string AliasAttibuteTypeFullName = "Microsoft.Extensions.Logging.ProviderAliasAttribute";
    private const string AliasAttibuteAliasProperty = "Alias";

    internal static string GetAlias(Type providerType)
    {
        foreach (var attribute in providerType.GetTypeInfo().GetCustomAttributes(inherit: false))
        {
            if (attribute.GetType().FullName == AliasAttibuteTypeFullName)
            {
                var valueProperty = attribute
                    .GetType()
                    .GetProperty(AliasAttibuteAliasProperty, BindingFlags.Public | BindingFlags.Instance);

                if (valueProperty != null)
                {
                    return valueProperty.GetValue(attribute) as string;
                }
            }
        }

        return null;
    }
}