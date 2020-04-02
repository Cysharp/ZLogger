namespace ZLogger
{
    public interface IZLoggerState
    {
        IZLoggerEntry CreateLogEntry(LogInfo logInfo);
    }
}
