using ConsoleAppFramework;
using Markdig.Syntax.Inlines;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CommandTools
{
    public class Program
    {
        static void Main(string[] args)
        {
            var app = ConsoleApp.Create();
            app.Add<Program>();
            app.Run(args);
        }

        // build-table-of-contents ../../../../../ReadMe.md
        [Command("build-table-of-contents")]
        public void BuildTableOfContents([Argument] string readMePath)
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