#nullable disable

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace System.Text.Json
{
    public class Utf8JsonWriter
    {
        public Utf8JsonWriter(IBufferWriter<byte> writer, JsonWriterOptions options)
        {
            
        }

        public void Flush()
        {
        }

        public void Reset()
        {
        }

        public void Reset(IBufferWriter<byte> writer)
        {
        }

        public void WriteString(JsonEncodedText text, ReadOnlySpan<byte> span) { }
        public void WriteString(JsonEncodedText text, JsonEncodedText value) { }
        public void WriteString(JsonEncodedText text, DateTimeOffset timestamp) { }
        public void WriteString(JsonEncodedText text, string str) { }
        public void WritePropertyName(JsonEncodedText text) { }
        public void WriteNumber(JsonEncodedText text, int x) { }
        public void WriteNullValue() { }
        public void WriteStartObject() { }
        public void WriteEndObject() { }
        public void WriteNull(JsonEncodedText text) { }
    }

    public static class JsonSerializer
    {
        public static void Serialize<T>(Utf8JsonWriter writer, T payload, JsonSerializerOptions options)
        {
        }
    }

    public readonly struct JsonEncodedText
    {
        public static JsonEncodedText Encode(string text)
        {
            return default(JsonEncodedText);
        }
    }

    public struct JsonWriterOptions
    {
        public bool Indented { get; set; }
        public bool SkipValidation { get; set; }
        public JavaScriptEncoder Encoder { get; set; }
    }

    public sealed class JsonSerializerOptions
    {
        public bool WriteIndented { get; set; }
        public System.Text.Json.Serialization.JsonIgnoreCondition DefaultIgnoreCondition { get; set; }
        public JavaScriptEncoder Encoder { get; set; }
    }
}

namespace System.Text.Json.Serialization
{
    public enum JsonIgnoreCondition
    {
        Never,
        Always,
        WhenWritingDefault,
        WhenWritingNull
    }
}

namespace System.Text.Encodings.Web
{
    public class JavaScriptEncoder
    {
        public static JavaScriptEncoder Create(UnicodeRange range)
        {
            return new JavaScriptEncoder();
        }
    }
}

namespace System.Text.Unicode
{
    public static class UnicodeRanges
    {
        public static UnicodeRange All { get; set; }
    }

    public sealed class UnicodeRange
    {
    }
}
