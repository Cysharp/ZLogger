using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CommandTools
{
    public class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureLogging(x =>
                {
                    x.ReplaceToSimpleConsole();
                })
                .RunConsoleAppFrameworkAsync<Program>(args);
        }

        [Command("remove-nullable-reference")]
        public void RemoveNullableReferenceDefine(string directory)
        {
            var replaceSet = new Dictionary<string, string>
            {
                {"Exception?", "Exception" },
                {"Utf8JsonWriter?", "Utf8JsonWriter" },
                {"string?", "string" },
                {"object?", "object" },
                {"default!", "default" },
                {"fn!", "fn" },
                {"byte[]?", "byte[]" },
                {"null!", "null" },
                {"className!", "className" },
                {"Action<Utf8JsonWriter, LogInfo>?", "Action<Utf8JsonWriter, LogInfo>" },
                {"Action<IBufferWriter<byte>, LogInfo>?", "Action<IBufferWriter<byte>, LogInfo>" },
                {"Action<Exception>?", "Action<Exception>" },
                {"Func<T, LogInfo, IZLoggerEntry>?", "Func<T, LogInfo, IZLoggerEntry>" },
                {"ToString()!", "ToString()" },
                {"FieldInfo?", "FieldInfo" }
            };

            foreach (var path in Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories))
            {
                var text = File.ReadAllText(path, Encoding.UTF8);

                foreach (var item in replaceSet)
                {
                    text = text.Replace(item.Key, item.Value);
                }

                File.WriteAllText(path, text);
            }
        }
    }
}
