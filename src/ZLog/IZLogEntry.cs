using Microsoft.Extensions.Logging;
using System;
using System.Buffers;

namespace ZLog
{
    public readonly struct LogInfo
    {
        public readonly string CategoryName;
        public readonly DateTimeOffset Timestamp;
        public readonly LogLevel LogLevel;
        public readonly EventId EventId;
        public readonly Exception? Exception;

        public LogInfo(string categoryName, DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, Exception? exception)
        {
            EventId = eventId;
            CategoryName = categoryName;
            Timestamp = timestamp;
            LogLevel = logLevel;
            Exception = exception;
        }
    }

    public interface IZLogEntry
    {
        public LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer);
        void Return();
    }
}
