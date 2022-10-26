using Cysharp.Text;
using System;
using System.Buffers;
using System.Text.Json;

namespace ZLogger
{
    public interface IZLoggerEntry
    {
        LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer, ZLoggerOptions options, Utf8JsonWriter? jsonWriter);
        void SwitchCasePayload<TPayload>(Action<IZLoggerEntry, TPayload, object?> payloadCallback, object? state);
        object? GetPayload();
        void Return();
    }

    public static class ZLoggerEntryExtensions
    {
        public static string FormatToString(this IZLoggerEntry entry, ZLoggerOptions options, Utf8JsonWriter? jsonWriter)
        {
            var boxedBuilder = (IBufferWriter<byte>)ZString.CreateUtf8StringBuilder();
            try
            {
                entry.FormatUtf8(boxedBuilder, options, jsonWriter);
                return boxedBuilder.ToString()!;
            }
            finally
            {
                ((Utf8ValueStringBuilder)boxedBuilder).Dispose();
            }
        }
    }
}