using Microsoft.Extensions.Logging;

namespace ZLogger
{
    /// <summary>
    /// extension to support event ID overrides using payload interface implementation
    /// <author>OP</author>
    /// </summary>
    public interface ILogEvent
    {
        EventId GetEventId();
    }
}