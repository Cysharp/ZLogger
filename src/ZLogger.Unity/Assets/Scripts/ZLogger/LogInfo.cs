using System;
using Microsoft.Extensions.Logging;

namespace ZLogger
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
}