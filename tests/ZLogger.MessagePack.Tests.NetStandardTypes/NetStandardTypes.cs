using System;
using MessagePack;
using SerializerFoundation;

namespace ZLogger.MessagePack.Tests.NetStandardTypes;

[MessagePackObject]
public partial class NetStandardPayload
{
    [Key("x")]
    public int X { get; set; }
}

// Writes a DateTimeOffset as its UTC ticks, unlike the built-in formatter, so that a test can tell which formatter wrote it.
public sealed partial class UtcTicksDateTimeOffsetFormatter<TWriteBuffer, TReadBuffer> : IMessagePackFormatter<TWriteBuffer, TReadBuffer, DateTimeOffset>
{
    public void Initialize(MessagePackFormatterResolver resolver)
    {
    }

    public void Serialize(ref TWriteBuffer buffer, ref SerializeState state, DateTimeOffset value)
    {
        buffer.WriteInt64(value.UtcTicks);
    }

    public void Deserialize(ref TReadBuffer buffer, ref DeserializeState state, ref DateTimeOffset value)
    {
        value = new DateTimeOffset(buffer.ReadInt64(), TimeSpan.Zero);
    }
}

public sealed partial class UtcTicksDateTimeOffsetFormatterFactory : MessagePackFormatterFactory
{
    public object? CreateFormatter<TWriteBuffer, TReadBuffer>(Type type)
        where TWriteBuffer : struct, IWriteBuffer
        where TReadBuffer : struct, IReadBuffer
        => type == typeof(DateTimeOffset) ? new UtcTicksDateTimeOffsetFormatter<TWriteBuffer, TReadBuffer>() : null;
}
