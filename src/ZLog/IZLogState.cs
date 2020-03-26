namespace ZLog
{
    public interface IZLogState
    {
        IZLogEntry CreateLogEntry(LogInfo logInfo);
    }
}
