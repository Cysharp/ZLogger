using ConsoleApp;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using ZLogger;

using var factory = LoggerFactory.Create(logging =>
{
    // Add ZLogger provider to ILoggingBuilder
    logging.AddZLoggerConsole(options =>
    {
        options.UsePlainTextFormatter(formatter =>
        {
            formatter.SetPrefixFormatter($"foo", (in MessageTemplate template, in LogInfo info) => template.Format());
            //MessageTemplate
            // LogInfo
            //formatter.SetPrefixFormatter($"foo", (template, info) => template.Format());
        });
    });

    // output to  yyyy-MM-dd_*.log, roll by 1MB or changed date
    //logging.AddZLoggerRollingFile((dt, index) => $"{dt:yyyy-MM-dd}_{index}.log", 1024 * 1024);


    var ms = new MemoryStream();




    logging.AddZLoggerInMemory(processor =>
    {
        processor.MessageReceived += msg =>
        {
            Console.WriteLine($"Received:{msg}");
        };
    });

    //// Output Structured Logging, setup options
    //logging.AddZLoggerConsole(options => options.UseJsonFormatter(formatter =>
    //{
    //    formatter.IncludeProperties = IncludeProperties.ParameterKeyValues | IncludeProperties.MemberName | IncludeProperties.FilePath | IncludeProperties.LineNumber;
    //}));

    //logging.AddZLoggerConsole(options =>
    //{
    //    options.InternalErrorLogger = ex => Console.WriteLine(ex);
    //    options.UseFormatter(() => new CLEFMessageTemplateFormatter());
    //});


});





var logger = factory.CreateLogger("Program");

var name = "John";


logger.Log(LogLevel.Information, "aiueo {0}", 100);

//logger.LogInformation("aiueo {id:0,0000} ", 100);

LogLog.Foo3(logger, "a", "b", 100, new { aaaaa = 1000 });
var age = 33;
// Use **Z**Log method and string interpolation to log message
logger.ZLogInformation($"Hello my name is {name}, {age:-1,0000} years old.", new { tako = 9999 });

LogLog.Foo(logger, "tako", "huga", 1000);


public partial class MyClass
{
    // get ILogger<T> from DI.
    readonly ILogger<MyClass> logger;

    public MyClass(ILogger<MyClass> logger)
    {
        this.logger = logger;
    }

    // name = "Bill", city = "Kumamoto", age = 21
    public void Foo(string name, string city, int age)
    {
        // plain-text:
        // Hello, Bill lives in Kumamoto 21 years old.
        // json:
        // {"Timestamp":"2023-11-30T17:28:35.869211+09:00","LogLevel":"Information","Category":"MyClass","Message":"Hello, Bill lives in Kumamoto 21 years old.","name":"Bill","city":"Kumamoto","age":21}
        // json(IncludeProperties.ParameterKeyValues):
        // {"name":"Bill","city":"Kumamoto","age":21}
        logger.ZLogInformation($"Hello, {name} lives in {city} {age} years old.");

        // Explicit property name        
        logger.ZLogInformation($"Hello, {name:@user-name} id:{100:@id} {age} years old.");

        // plain-text:
        // Hello, Bill id:100 21 years old.
        //  
        // json:
        // {"Timestamp":"2023-11-30T17:28:35.869211+09:00","LogLevel":"Information","Category":"MyClass","Message":"Hello, Bill id:100 21 years old.","username":"Bill","id":100,"age":21}

        // Dump variables as JSON
        //var user =
        //[
        //    new User(1, "Alice"),
        //    new User(1, "Bob"),
        //];
        //logger.ZLogInformation($"users: {users:json}");

        // plain-text:
        // users: [{"Id":1,"Name":"Alice"},{"Id":1,"Name":"Bob"}]
        // 
        // json:
        // {"Timestamp":"2023-12-01T16:59:29.908126+09:00","LogLevel":"Information","Category":"my","Message":"users: [{\u0022Id\u0022:1,\u0022Name\u0022:\u0022Alice\u0022},{\u0022Id\u0022:1,\u0022Name\u0022:\u0022Bob\u0022}]","users":[{"Id":1,"Name":"Alice"},{"Id":1,"Name":"Bob"}]}
    }
}

public static partial class LogLog
{
    [ZLoggerMessage(LogLevel.Information, "foo is {name} {city} {age:-1,0000}")]
    public static partial void Foo(ILogger logger, string name, string city, int age, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = 0);


    [ZLoggerMessage(LogLevel.Information, "foo is {name} {city} {age}")]
    public static partial void Foo2(ILogger logger, string name, string city, int age);


    [ZLoggerMessage(LogLevel.Information, "foo is {name} {city} {age:-1,0000}")]
    public static partial void Foo3(ILogger logger, string name, string city, int age, [ZLoggerContext] object? context = null, [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = 0);
}