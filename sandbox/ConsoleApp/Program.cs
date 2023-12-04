using Microsoft.Extensions.Logging;
using ZLogger;

using var factory = LoggerFactory.Create(logging =>
{
    // Add ZLogger provider to ILoggingBuilder
    logging.AddZLoggerConsole();

    // Output Structured Logging, setup options
    logging.AddZLoggerConsole(options => options.UseJsonFormatter(formatter =>
    {
        formatter.IncludeProperties = IncludeProperties.ParameterKeyValues;
    }));
});

var logger = factory.CreateLogger("Program");

var name = "John";
var age = 33;

// Use **Z**Log method and string interpolation to log message
logger.ZLogInformation($"Hello my name is {name}, {age} years old.");

// Output messages:
// Hello my name is John, 33 years old.
// {"Timestamp":"2023-12-04T19:39:59.9237682+09:00","LogLevel":"Information","Category":"Program","Message":"Hello my name is John, 33 years old.","name":"John","age":33}



public class MyClass
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
        User[] users =
        [
            new User(1, "Alice"),
            new User(1, "Bob"),
        ];
        logger.ZLogInformation($"users: {users:json}");

        // plain-text:
        // users: [{"Id":1,"Name":"Alice"},{"Id":1,"Name":"Bob"}]
        // 
        // json:
        // {"Timestamp":"2023-12-01T16:59:29.908126+09:00","LogLevel":"Information","Category":"my","Message":"users: [{\u0022Id\u0022:1,\u0022Name\u0022:\u0022Alice\u0022},{\u0022Id\u0022:1,\u0022Name\u0022:\u0022Bob\u0022}]","users":[{"Id":1,"Name":"Alice"},{"Id":1,"Name":"Bob"}]}
    }
}