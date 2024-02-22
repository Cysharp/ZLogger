using System.Text.Json;
using ZLogger;
using ZLogger.Formatters;

namespace ConsoleApp;

using static IncludeProperties;
using static JsonEncodedText; // JsonEncodedText.Encode

public static class CloudLoggingExtensions
{
    // Cloud Logging Json Field
    // https://cloud.google.com/logging/docs/structured-logging?hl=en
    public static ZLoggerOptions UseCloudLoggingJsonFormat(this ZLoggerOptions options)
    {
        return options.UseJsonFormatter(formatter =>
        {
            // Category and ScopeValues is manually write in AdditionalFormatter at labels so remove from include properties.
            formatter.IncludeProperties = Timestamp | LogLevel | Message | ParameterKeyValues;

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

                Message = Encode("message"),
                Timestamp = Encode("timestamp"),
            };

            formatter.PropertyKeyValuesObjectName = Encode("jsonPayload");

            // cache JsonENcodedText outside of AdditionalFormatter
            var labels = Encode("logging.googleapis.com/labels");
            var category = Encode("category");
            var eventId = Encode("eventId");
            var userId = Encode("userId");

            formatter.AdditionalFormatter = (Utf8JsonWriter writer, in LogInfo logInfo) =>
            {
                writer.WriteStartObject(labels);
                writer.WriteString(category, logInfo.Category.JsonEncoded);
                writer.WriteString(eventId, logInfo.EventId.Name);

                if (logInfo.ScopeState != null && !logInfo.ScopeState.IsEmpty)
                {
                    foreach (var item in logInfo.ScopeState.Properties)
                    {
                        if (item.Key == "userId")
                        {
                            writer.WriteString(userId, item.Value!.ToString());
                            break;
                        }
                    }
                }
                writer.WriteEndObject();
            };
        });
    }
}
