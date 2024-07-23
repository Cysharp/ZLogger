using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Utf8StringInterpolation;

namespace ZLogger.Internal
{
    // Used for Code Generator so "public".
    public static class CodeGeneratorUtil
    {
        [ThreadStatic]
        static Utf8JsonWriter? utf8JsonWriter;

        static Utf8JsonWriter GetThreadStaticUtf8JsonWriter(IBufferWriter<byte> bufferWriter)
        {
            var writer = utf8JsonWriter;
            if (writer == null)
            {
                writer = utf8JsonWriter = new Utf8JsonWriter(bufferWriter);
            }
            else
            {
                writer.Reset(bufferWriter);
            }

            return writer;
        }

        [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode")]
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode")]
        public static void AppendAsJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(ref Utf8StringWriter<IBufferWriter<byte>> stringWriter, T value)
        {
            stringWriter.ClearState();

            var utf8JsonWriter = GetThreadStaticUtf8JsonWriter(stringWriter.GetBufferWriter());
            JsonSerializer.Serialize(utf8JsonWriter, value);
            utf8JsonWriter.Flush();
            utf8JsonWriter.Reset();
        }

        [UnconditionalSuppressMessage("Trimming", "IL3050:RequiresDynamicCode")]
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode")]
        public static void AppendAsJson(ref Utf8StringWriter<IBufferWriter<byte>> stringWriter, object? value, Type inputType)
        {
            stringWriter.ClearState();

            var utf8JsonWriter = GetThreadStaticUtf8JsonWriter(stringWriter.GetBufferWriter());
            JsonSerializer.Serialize(utf8JsonWriter, value, inputType);
            utf8JsonWriter.Flush();
            utf8JsonWriter.Reset();
        }

        public static unsafe void WriteJsonEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Utf8JsonWriter writer, JsonEncodedText key, T value)
        {
            var enumValue = EnumDictionary<T>.GetJsonEncodedName(value);
            if (enumValue == null)
            {
                // fallback write srring
                var s = value!.ToString();
                writer.WriteString(key, s);
            }
            else
            {
                writer.WriteString(key, enumValue.Value);
            }
            
        }

        public static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}