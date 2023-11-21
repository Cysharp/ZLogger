using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZLogger;
using ZLogger.Formatters;

namespace ConsoleApp;

using static IncludeProperties;
using static JsonEncodedText; // JsonEncodedText.Encode

public static class CloudLoggingExtensions
{
    // Cloud Logging Json Field
    // https://cloud.google.com/logging/docs/structured-logging?hl=en
    public static void UseCloudLoggingJsonFormat(this ZLoggerOptions options)
    {
        _ = options.UseJsonFormatter(formatter =>
        {
            // Category is manually write in AdditionalFormatter at labels
            formatter.IncludeProperties = Timestamp | LogLevel | Message | PropertyKeyValues;

            formatter.JsonPropertyNames = JsonPropertyNames.Default with
            {
                LogLevel = Encode("severity"),
                LogLevelNone = Encode("DEFAULT"),
                LogLevelTrace = Encode("DEBUG"),
                LogLevelDebug = Encode("DEBUG"),
                LogLevelInformation = Encode("INFO"),
                LogLevelWarning = Encode("WARNING"),
                LogLevelError = Encode("ERROR"),
                LogLevelCritical = Encode("CRITICAL"),

                Message = Encode("messsage"),
                Timestamp = Encode("timestamp"),
            };

            formatter.PropertyKeyValuesObjectName = Encode("jsonPayload");

            // cache JsonENcodedText outside of AdditionalFormatter
            var labels = Encode("logging.googleapis.com/labels");
            var category = Encode("category");
            var eventId = Encode("eventId");
            var userId = Encode("userId");

            formatter.AdditionalFormatter = (writer, logInfo) =>
            {
                writer.WriteStartObject(labels);
                writer.WriteString(category, logInfo.Category.JsonEncoded);
                writer.WriteString(eventId, logInfo.EventId.Name);

                // TODO: additional, get label(for example, userId)
                writer.WriteString(userId, "get from scope...");
                writer.WriteEndObject();
            };
        });
    }
}
