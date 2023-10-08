// See https://aka.ms/new-console-template for more information
using GeneratorSandbox;
using MessagePack;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using ZLogger;

using static Microsoft.Extensions.Logging.LogLevel;

Console.WriteLine("Hello, World!");


var writer = new MessagePackWriter();




// var writer2 = new StructuredKeyValueWriter();

ILogger log;




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
    [ZLoggerMessage(Information, "Could not open socket to {hostName} {ipAddress}.")]
    public static partial void CouldNotOpenSocket(this ILogger<FooBarBaz> logger, string hostName, int ipAddress);
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