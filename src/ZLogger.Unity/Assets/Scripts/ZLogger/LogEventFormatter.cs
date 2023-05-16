using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utf8Json;

namespace ZLogger
{
    /// <summary>
    /// extension to support automatic deserialization of log events (by using ILogEvent)
    /// <author>OP</author>
    /// </summary>
    public sealed class LogEventFormatter : IJsonFormatter<ILogEvent?>
    {
        private static Dictionary<int,Type> logEventTypes;

        private struct LogInfoEventIdStub
        {
            public int EventId;
            public string EventIdName;
        }

        static LogEventFormatter()
        {
            #if UNITY_EDITOR
            logEventTypes = TypeCache.GetTypesDerivedFrom<ILogEvent>()
                .ToDictionary(t => ((ILogEvent)Activator.CreateInstance(t)).GetEventId().Id);
            #else
            // TODO for build (see DebugConfigLoader's implementation)
            #endif
        }
        
        public void Serialize(ref JsonWriter writer, ILogEvent? value, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }

        ILogEvent? IJsonFormatter<ILogEvent?>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            // try to read null
            if (reader.ReadIsNull())
            {
                return default;
            }
            
            // first reparse the object just to get EventId
            var eventId = JsonSerializer.Deserialize<LogInfoEventIdStub>(reader.GetBufferUnsafe());
            
            if (logEventTypes.TryGetValue(eventId.EventId, out var type))
            {
                var logEvent = (ILogEvent)JsonSerializer.NonGeneric.Deserialize(type, ref reader, formatterResolver);
                return logEvent;
            }

            Debug.LogError($"LogStream: Unknown log event type with id {eventId.EventId} ({eventId.EventIdName})");
            return default;
        }
    }
}