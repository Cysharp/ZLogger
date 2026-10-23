using System;
using MessagePack;

namespace ZLogger.MessagePack;

public readonly struct MessagePackEncodedText : IEquatable<MessagePackEncodedText>
{
    public string Value { get; private init; }

    public ReadOnlySpan<byte> Utf8EncodedValue => utf8EncodedValue;

    readonly byte[] utf8EncodedValue;

    MessagePackEncodedText(string value, byte[] utf8EncodedValue)
    {
        Value = value;
        this.utf8EncodedValue = utf8EncodedValue;
    }

    public static MessagePackEncodedText Encode(string stringValue)
    {
        return new MessagePackEncodedText(stringValue, MessagePackSerializer.Serialize(stringValue, MessagePackSerializerOptions.DefaultAot));
    }

    public bool Equals(MessagePackEncodedText other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is MessagePackEncodedText other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
}
