using Microsoft.Extensions.Logging;

namespace Benchmark;

public static partial class MessageSample
{
    public const string Message = "Hello, {Name} lives in {City} {Age} years old";
    public const string Arg1 = "Bill Evance";
    public const string Arg2 = "Mumbai";
    public const int Arg3 = 32;

    [LoggerMessage(LogLevel.Information, Message = Message)]
    public static partial void GeneratedLog(this ILogger logger, string name, string city, int age);
}

