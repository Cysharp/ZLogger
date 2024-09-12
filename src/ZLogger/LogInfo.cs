using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ZLogger;

public readonly struct LogInfo(LogCategory category, Timestamp timestamp, LogLevel logLevel, EventId eventId, Exception? exception, LogScopeState? scopeState, ThreadInfo threadInfo, object? context = null, string? memberName = null, string? filePath = null, int lineNumber = 0)
{
    public readonly LogCategory Category = category;
    public readonly Timestamp Timestamp = timestamp;
    public readonly LogLevel LogLevel = logLevel;
    public readonly EventId EventId = eventId;
    public readonly Exception? Exception = exception;
    public readonly LogScopeState? ScopeState = scopeState;
    public readonly ThreadInfo ThreadInfo = threadInfo;
    public readonly object? Context = context;
    public readonly string? MemberName = memberName;
    public readonly string? FilePath = filePath;
    public readonly int LineNumber = lineNumber;
}

public readonly struct LogCategory
{
    readonly byte[] utf8;
    public readonly string Name;
    public readonly JsonEncodedText JsonEncoded;
    public readonly ReadOnlySpan<byte> Utf8Span => utf8;
    public readonly ReadOnlyMemory<byte> Utf8Memory => utf8;

    public LogCategory(string category)
    {
        this.Name = category;
        this.utf8 = Encoding.UTF8.GetBytes(category);
        this.JsonEncoded = JsonEncodedText.Encode(utf8);
    }

    public override string ToString()
    {
        return Name;
    }
}

public readonly struct ThreadInfo(int threadId, string? threadName, bool isThreadPoolThread)
{
    internal static readonly ThreadInfo Null = new ThreadInfo(-1, null, false);
    internal static ThreadInfo FromCurrentThread()
    {
        var currentThread = Thread.CurrentThread;
        return new ThreadInfo(currentThread.ManagedThreadId, currentThread.Name, currentThread.IsThreadPoolThread);
    }

    public readonly int ThreadId = threadId;
    public readonly string? ThreadName = threadName;
    public readonly bool IsThreadPoolThread = isThreadPoolThread;
}
