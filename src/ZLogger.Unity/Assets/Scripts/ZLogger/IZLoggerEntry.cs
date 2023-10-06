using Cysharp.Text;
using System;
using System.Buffers;

namespace ZLogger
{
    public interface IZLoggerEntry
    {
        LogInfo LogInfo { get; }
        void FormatUtf8(IBufferWriter<byte> writer, IZLoggerFormatter formatter);
        void SwitchCasePayload<TPayload>(Action<IZLoggerEntry, TPayload, object?> payloadCallback, object? state);
        object? GetPayload();
        void Return();
    }

    public static class ZLoggerEntryExtensions
    {
        public static string FormatToString(this IZLoggerEntry entry, IZLoggerFormatter formatter)
        {
            var boxedBuilder = (IBufferWriter<byte>)ZString.CreateUtf8StringBuilder();
            try
            {
                entry.FormatUtf8(boxedBuilder, formatter);
                return boxedBuilder.ToString()!;
            }
            finally
            {
                ((Utf8ValueStringBuilder)boxedBuilder).Dispose();
            }
        }
    }
}