namespace ZLog
{
    public interface IZLogState
    {
        bool IsJson { get; }
        IZLogEntry CreateLogEntry(LogInfo logInfo);
    }
}
