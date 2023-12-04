#nullable disable
#pragma warning disable SYSLIB1015
#pragma warning disable SYSLIB1025

// See https://aka.ms/new-console-template for more information
using GeneratorSandbox;
using MessagePack;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using ZLogger;



var i = 100;
Log($"hogemoge{i}");



static void Log(LogInterpolatedStringHandler format)
{


}

[InterpolatedStringHandler]
public ref struct LogInterpolatedStringHandler
{
    // Storage for the built-up string
    StringBuilder builder;

    public LogInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        builder = new StringBuilder(literalLength);
        Console.WriteLine($"\tliteral length: {literalLength}, formattedCount: {formattedCount}");
    }

    public void AppendLiteral(string s)
    {
        Console.WriteLine($"\tAppendLiteral called: {{{s}}}");

        builder.Append(s);
        Console.WriteLine($"\tAppended the literal string");
    }

    public void AppendFormatted<T>(T t)
    {
        Console.WriteLine($"\tAppendFormatted called: {{{t}}} is of type {typeof(T)}");

        builder.Append(t?.ToString());
        Console.WriteLine($"\tAppended the formatted object");
    }

    internal string GetFormattedText() => builder.ToString();
}

public static partial class Log
{
    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Critical,
        Message = "Could not open socket to {list}.")]
    public static partial void TestTest(this ILogger logger, LogLevel levels, int[] list, string hostName, Exception ex);

    [LoggerMessage(
        EventId = 0,
        EventName = "Z",
        Level = LogLevel.Critical,
        Message = "Could not open socket to {ipAddress,100}.")]
    public static partial void CouldNotOpenSocket(this ILogger<FooBarBaz> logger, string hostName, int ipAddress, Exception ex);




    [LoggerMessage(
        EventId = 1,
        EventName = "Z",
        Level = LogLevel.Critical,
        Message = "Could not open socket to {hostName} {ipAddress} {hogemoge}.")]
    public static partial void CouldNotOpenSocket(this ILogger<FooBarBaz> logger, string hostName, int ipAddress, int hogemoge);





}

public static partial class Log2
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Critical,
        Message = "Could not open socket to {hostName} {ipAddress}.")]
    public static partial void CouldNotOpenSocket(this ILogger<FooBarBaz> logger, string hostName, int ipAddress);



    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Critical,
        Message = "Could not open socket to {hostName} {ipAddress}.")]
    public static partial void CouldNotOpenSocket2(this ILogger<FooBarBaz> logger, string hostName, int ipAddress);
}


public static partial class LogZ
{

    [ZLoggerMessage(LogLevel.Information, "Could not open socket to {banana} {dt}.")]
    public static partial void Sampler(this ILogger<FooBarBaz> logger, Nullable<Guid> banana, DateTime? dt);

    [ZLoggerMessage(LogLevel.Information, "Could not open socket to {hostName} {ipAddress}.")]
    public static partial void CouldNotOpenSocket(this ILogger<FooBarBaz> logger, string hostName, int ipAddress);
}


public enum Fruit
{
    Orange, Ringo, Banana
}

public class FooBarBaz
{

}


public struct MessagePackStructuredKeyValueWriter
{
    public MessagePackStructuredKeyValueWriter()
    {
        //ILogger logger;
        //logger.Log(

        // writer = new MessagePackWriter();
    }

    public void WriteKey(ReadOnlySpan<byte> key, MessagePackWriter writer)
    {



        throw new NotImplementedException();
    }

    public void WriteValue<T>(T value)
    {
        throw new NotImplementedException();
    }
}