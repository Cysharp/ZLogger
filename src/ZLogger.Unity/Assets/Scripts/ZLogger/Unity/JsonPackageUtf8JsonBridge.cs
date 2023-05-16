#nullable disable

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Utf8Json;
using ZLogger;

namespace System.Text.Json
{
    /// <summary>
    /// originally a shim, this is now a bridge to Utf8Json for Unity, to enable support for EnableStructuredLogging
    /// <see>
    ///     <cref>https://github.com/Cysharp/ZLogger/blob/master/src/ZLogger.Unity/Assets/Scripts/ZLogger/Unity/JsonPackageShims.cs</cref>
    /// </see>
    /// <author>OP</author>
    /// </summary>
    public class Utf8JsonWriter
    {
        private IBufferWriter<byte> _writer;
        private StreamBufferWriter _streamWriter;
        private readonly JsonWriterOptions _options;
        private bool _isFirstProperty = true;
        private static byte[] commaBytes = Encoding.UTF8.GetBytes(",");
        private static byte[] colonBytes = Encoding.UTF8.GetBytes(":");
        private static byte[] startObjectBytes = Encoding.UTF8.GetBytes("{");
        private static byte[] endObjectBytes = Encoding.UTF8.GetBytes("}");
        private static byte[] nullBytes = Encoding.UTF8.GetBytes("null");

        internal IBufferWriter<byte> GetBufferWriter()
        {
            return _writer;
        }

        public Utf8JsonWriter(IBufferWriter<byte> writer, JsonWriterOptions options)
        {
            _options = options;
            Reset(writer);
        }

        public void Flush()
        {
            _streamWriter?.Flush();
        }

        public void Reset() => Flush();

        public void Reset(IBufferWriter<byte> writer)
        {
            Flush();
            _writer = writer;
            _streamWriter = writer as StreamBufferWriter;
        }
        
        private void WriteCommaIfNeeded()
        {
            if (_isFirstProperty)
            {
                _isFirstProperty = false;
            }
            else
            {
                _writer.Write(commaBytes);
            }
        }

        public void WriteString(JsonEncodedText text, ReadOnlySpan<byte> span)
        {
            WriteCommaIfNeeded();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(text.ToString()));
            WriteColon();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(span.ToArray()));
        }

        public void WriteString(JsonEncodedText text, JsonEncodedText value)
        {
            WriteCommaIfNeeded();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(text.ToString()));
            WriteColon();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(value.ToString()));
        }

        public void WriteString(JsonEncodedText text, DateTimeOffset timestamp)
        {
            WriteCommaIfNeeded();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(text.ToString()));
            WriteColon();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(timestamp.ToString()));
        }

        public void WriteString(JsonEncodedText text, string str)
        {
            WriteCommaIfNeeded();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(text.ToString()));
            WriteColon();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(str));
        }

        public void WritePropertyName(JsonEncodedText text)
        {
            WriteCommaIfNeeded();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(text.ToString()));
            WriteColon();
        }

        public void WriteNumber(JsonEncodedText text, int x)
        {
            WriteCommaIfNeeded();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(text.ToString()));
            WriteColon();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(x));
        }

        public void WriteNullValue()
        {
            _writer.Write(nullBytes);
        }

        public void WriteStartObject()
        {
            _isFirstProperty = true;
            _writer.Write(startObjectBytes);
        }

        public void WriteEndObject()
        {
            _writer.Write(endObjectBytes);
        }
        private void WriteColon()
        {
            _writer.Write(colonBytes);
        }

        public void WriteNull(JsonEncodedText text)
        {
            WriteCommaIfNeeded();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(text.ToString()));
            WriteColon();
            _writer.Write(nullBytes);
        }

        public void WriteDateTimeOffset(JsonEncodedText text, DateTimeOffset timestamp)
        {
            WriteCommaIfNeeded();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(text.ToString()));
            WriteColon();
            _writer.Write(Utf8Json.JsonSerializer.Serialize(timestamp));
        }
    }

    public static class JsonSerializer
    {
        public static void Serialize<T>(Utf8JsonWriter writer, T payload, JsonSerializerOptions options)
        {
            if (payload == null)
                writer.WriteNullValue();
            else
                writer.GetBufferWriter().Write(Utf8Json.JsonSerializer.Serialize( payload));
        }
    }

    public readonly struct JsonEncodedText
    {
        public readonly string EncodedText;
        public JsonEncodedText(string text)
        {
            EncodedText = text;
        }
        public static JsonEncodedText Encode(string text)
        {
            return new JsonEncodedText(text);
        }

        public override string ToString() => EncodedText;
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
