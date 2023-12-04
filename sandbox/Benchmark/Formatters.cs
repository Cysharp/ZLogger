using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Benchmark;

class BenchmarkPlainTextConsoleFormatter : ConsoleFormatter
{
    internal class Options : ConsoleFormatterOptions
    {
    }
    
    public BenchmarkPlainTextConsoleFormatter() : base("BenchmarkPlainText")
    {
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter.Invoke(logEntry.State, logEntry.Exception);
        textWriter.Write(DateTime.Now);
        textWriter.Write(" [");
        textWriter.Write(logEntry.LogLevel);
        textWriter.Write("] ");
        textWriter.Write(message);
        textWriter.WriteLine();
    }
}