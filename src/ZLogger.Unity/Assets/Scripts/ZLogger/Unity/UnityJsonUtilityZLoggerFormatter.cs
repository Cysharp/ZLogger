using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace ZLogger.Formatters
{
    public static class ZLoggerOptionsUnityJsonUtilityExtensions
    {
        public static ZLoggerOptions UseUnityJsonUtilityFormatter(this ZLoggerOptions options)
        {
            return options.UseFormatter(() => new UnityJsonUtilityZLoggerFormatter());
        }
    }

    [Serializable]
    struct SerializableLogEntry
    {
        public string CategoryName;
        public string LogLevel;
        public int EventId;
        public string EventIdName;
        public string Timestamp;
        public string Message;
    }

    [Serializable]
    struct SerializableLogEntryWithException
    {
        public string CategoryName;
        public string LogLevel;
        public int EventId;
        public string EventIdName;
        public string Timestamp;
        public string Message;
        public SerializableException Exception;
    }

    [Serializable]
    struct SerializableLogEntryWithPayload<TPayload>
    {
        public string CategoryName;
        public string LogLevel;
        public int EventId;
        public string EventIdName;
        public string Timestamp;
        public string Message;
        public TPayload Payload;
    }

    [Serializable]
    struct SerializableLogEntryWithPayloadAndException<TPayload>
    {
        public string CategoryName;
        public string LogLevel;
        public int EventId;
        public string EventIdName;
        public string Timestamp;
        public string Message;
        public TPayload Payload;
        public SerializableException Exception;
    }

    [Serializable]
    class SerializableException
    {
        public string Name;
        public string Message;
        public string StackTrace;
        public SerializableException InnerException;

        public void SetInnerException(Exception ex)
        {
            if (ex != null)
            {
                InnerException = new SerializableException
                {
                    Name = ex.GetType().FullName,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                };
                InnerException.SetInnerException(ex.InnerException);
            }
        }
    }

    public class UnityJsonUtilityZLoggerFormatter : IZLoggerFormatter
    {
        public void FormatLogEntry<TEntry, TPayload>(
            IBufferWriter<byte> writer,
            TEntry entry,
            TPayload payload,
            ReadOnlySpan<byte> utf8Message) where TEntry : IZLoggerEntry
        {
            var message = Encoding.UTF8.GetString(utf8Message);
            string jsonString;
            if (entry.LogInfo.Exception is { } ex)
            {
                var serializableException = new SerializableException
                {
                    Name = ex.GetType().FullName,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                };
                serializableException.SetInnerException(ex.InnerException);

                jsonString = payload != null
                    ? JsonUtility.ToJson(new SerializableLogEntryWithPayloadAndException<TPayload>
                    {
                        LogLevel = LogLevelString(entry.LogInfo.LogLevel),
                        CategoryName = entry.LogInfo.CategoryName,
                        EventId = entry.LogInfo.EventId.Id,
                        EventIdName = entry.LogInfo.EventId.Name,
                        Timestamp = entry.LogInfo.Timestamp.ToString("O"),
                        Message = message,
                        Payload = payload,
                        Exception = serializableException
                    })
                    : JsonUtility.ToJson(new SerializableLogEntryWithException
                    {
                        LogLevel = LogLevelString(entry.LogInfo.LogLevel),
                        CategoryName = entry.LogInfo.CategoryName,
                        EventId = entry.LogInfo.EventId.Id,
                        EventIdName = entry.LogInfo.EventId.Name,
                        Message = message,
                        Timestamp = entry.LogInfo.Timestamp.ToString("O"),
                        Exception = serializableException
                    });
            }
            else
            {
                jsonString = payload != null
                    ? JsonUtility.ToJson(new SerializableLogEntryWithPayload<TPayload>
                    {
                        LogLevel = LogLevelString(entry.LogInfo.LogLevel),
                        CategoryName = entry.LogInfo.CategoryName,
                        EventId = entry.LogInfo.EventId.Id,
                        EventIdName = entry.LogInfo.EventId.Name,
                        Timestamp = entry.LogInfo.Timestamp.ToString("O"),
                        Message = message,
                        Payload = payload
                    })
                    : JsonUtility.ToJson(new SerializableLogEntry
                    {
                        LogLevel = LogLevelString(entry.LogInfo.LogLevel),
                        CategoryName = entry.LogInfo.CategoryName,
                        EventId = entry.LogInfo.EventId.Id,
                        EventIdName = entry.LogInfo.EventId.Name,
                        Timestamp = entry.LogInfo.Timestamp.ToString("O"),
                        Message = message
                    });
            }

            var memory = writer.GetMemory(Encoding.UTF8.GetMaxByteCount(jsonString.Length));
            if (MemoryMarshal.TryGetArray<byte>(memory, out var array) && array.Array != null)
            {
                var written = Encoding.UTF8.GetBytes(jsonString, 0, jsonString.Length, array.Array, array.Offset);
                writer.Advance(written);
            }
        }

        static string LogLevelString(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Information",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            LogLevel.None => "None",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }
}
