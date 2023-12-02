using Microsoft.Extensions.Logging;
using System;
using ZLogger;

namespace Benchmark;

public static partial class MessageSample
{
    public const string Message = "Hello, {Name} lives in {City} {Age} years old";
    public const string Arg1 = "Bill Evance";
    public const string Arg2 = "Mumbai";
    public const int Arg3 = 31;
    
    //public static readonly Guid Arg1 = Guid.NewGuid();
    //public const int Arg2 = 34252;
    //public const double Arg3 = 32.342;

    [LoggerMessage(LogLevel.Information, Message = Message)]
    public static partial void GeneratedLog(this ILogger logger, string name, string city, int age);


    [ZLoggerMessage(LogLevel.Information, Message)]
    public static partial void GeneratedZLog(this ILogger logger, string name, string city, int age);
}

