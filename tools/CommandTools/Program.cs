using ConsoleAppFramework;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        public void RemoveNullableReferenceDefine([Option(0)] string directory)
        {
            var mutex = new Mutex(false, "ZLogger." + nameof(RemoveNullableReferenceDefine));
            if (!mutex.WaitOne(0, false))
            {
                System.Console.WriteLine("running in another process, quit.");
                return; // mutex will release after quit.
            }

            var replaceSet = new Dictionary<string, string>
            {
                // keeping nullable.
                //{"Exception?", "Exception" },
                //{"Action<LogInfo, Exception>?", "Action<LogInfo, Exception>" },
                //{"Utf8JsonWriter?", "Utf8JsonWriter" },
                //{"string?", "string" },
                //{"object?", "object" },
                //{"default!", "default" },
                //{"fn!", "fn" },
                //{"di!", "di" },
                //{"byte[]?", "byte[]" },
                //{"null!", "null" },
                //{"className!", "className" },
                //{"Action<Utf8JsonWriter, LogInfo>?", "Action<Utf8JsonWriter, LogInfo>" },
                //{"Action<IBufferWriter<byte>, LogInfo>?", "Action<IBufferWriter<byte>, LogInfo>" },
                //{"Action<Exception>?", "Action<Exception>" },
                //{"Func<T, LogInfo, IZLoggerEntry>?", "Func<T, LogInfo, IZLoggerEntry>" },
                //{"ToString()!", "ToString()" },
                //{"FieldInfo?", "FieldInfo" }
            };

            System.Console.WriteLine("Start to remove nullable reference.");

            foreach (var path in Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories))
            {
                var text = File.ReadAllText(path, Encoding.UTF8);

                foreach (var item in replaceSet)
                {
                    text = text.Replace(item.Key, item.Value);
                }

                File.WriteAllText(path, text);
            }

            System.Console.WriteLine("Remove complete.");
        }

        [Command("collect-dll")]
        public void CollectDll([Option(0)] string dest)
        {
            const string ExtensionsVersion = "6.0.0";

            var dict = new Dictionary<string, string>
            {
                {"Microsoft.Extensions.Logging", ExtensionsVersion },
                {"Microsoft.Extensions.Logging.Configuration", ExtensionsVersion },
                {"Microsoft.Extensions.Logging.Abstractions", ExtensionsVersion },
                {"Microsoft.Extensions.Configuration", ExtensionsVersion },
                {"Microsoft.Extensions.Configuration.Binder", ExtensionsVersion },
                {"Microsoft.Extensions.Configuration.Abstractions", ExtensionsVersion },
                {"Microsoft.Extensions.DependencyInjection", ExtensionsVersion },
                {"Microsoft.Extensions.DependencyInjection.Abstractions", ExtensionsVersion },
                {"Microsoft.Extensions.Options", ExtensionsVersion },
                {"Microsoft.Extensions.Options.ConfigurationExtensions", ExtensionsVersion },
                {"Microsoft.Extensions.Primitives", ExtensionsVersion },
                {"System.Runtime.CompilerServices.Unsafe", ExtensionsVersion },
                {"System.Threading.Channels", ExtensionsVersion },

                //{"Microsoft.Bcl.AsyncInterfaces", "1.1.0" },
            };

            foreach (var item in dict)
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages", item.Key.ToLower(), item.Value, "lib", "netstandard2.1");
                if (!Directory.Exists(path))
                {
                    // fallback to 2.0
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages", item.Key.ToLower(), item.Value, "lib", "netstandard2.0");
                    if (!Directory.Exists(path))
                    {
                        throw new InvalidOperationException("Directory does not exists. Path: " + path);
                    }
                }

                foreach (var file in Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    var destPath = Path.Combine(dest, Path.GetFileName(file));
                    File.Copy(file, destPath, true);
                    Console.WriteLine("MoveFile: " + destPath);
                }
            }
        }

        // build-table-of-contents ../../../../../ReadMe.md
        [Command("build-table-of-contents")]
        public void BuildTableOfContents([Option(0)] string readMePath)
        {
            var md = Markdig.Markdown.Parse(File.ReadAllText(readMePath));

            var sb = new StringBuilder();
            foreach (var block in md.OfType<Markdig.Syntax.HeadingBlock>())
            {
                if (block.Level == 1) continue; // skip title

                var headerText = ToStringInline(block.Inline);

                sb.Append(' ', (block.Level - 2) * 4);

                sb.Append("- [");
                sb.Append(headerText);
                sb.Append("](#");
                sb.Append(headerText.ToLower().Replace(' ', '-').Replace(".", "").Replace("/", "").Replace("(", "").Replace(")", "").Replace("`", "").Replace("<", "").Replace(">", "").Replace(",", "").Replace("#", ""));
                sb.Append(")");
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }


        static string ToStringInline(ContainerInline inline)
        {
            var sb = new StringBuilder();
            foreach (var item in inline)
            {
                if (item is LiteralInline li)
                {
                    sb.Append(li.Content.ToString());
                }
                else if (item is CodeInline ci)
                {
                    sb.Append('`');
                    sb.Append(ci.Content);
                    sb.Append('`');
                }
            }
            return sb.ToString();
        }
    }
}