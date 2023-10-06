// See https://aka.ms/new-console-template for more information
using GeneratorSandbox;
using MessagePack;
using System.Buffers;
using System.Reflection.Emit;
using System.Text;

Console.WriteLine("Hello, World!");


var writer = new MessagePackWriter();




// var writer2 = new StructuredKeyValueWriter();




//public static partial class Log
//{
//    [ZLoggerMessage(
//        EventId = 0,
//        Level = LogLevel.Critical,
//        Message = "Could not open socket to {hostName} {ipAddress}.")]
//    public static partial void CouldNotOpenSocket(
//        this ILogger logger, string hostName, int ipAddress);
//}


public struct MessagePackStructuredKeyValueWriter
{
    public MessagePackStructuredKeyValueWriter()
    {
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