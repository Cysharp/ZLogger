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
    static readonly ILogger<NewBehaviourScript> logger = GlobalLogger.GetLogger<NewBehaviourScript>();
    void Start()
    {
        logger.ZLogDebug("Init!");
    }
}

