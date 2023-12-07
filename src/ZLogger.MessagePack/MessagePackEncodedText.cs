using System;
using System.Text;
using MessagePack;

namespace ZLogger.MessagePack;

public readonly struct MessagePackEncodedText : IEquatable<MessagePackEncodedText>
{
    static readonly Encoding StringEncoding = new UTF8Encoding(false);
    
    public string Value { get; private init; }
    public ReadOnlySpan<byte> Utf8EncodedValue => utf8EncodedValue;

    readonly byte[] utf8EncodedValue;

    MessagePackEncodedText(string value, byte[] utf8EncodedValue)
    {
        Value = value;
        this.utf8EncodedValue = utf8EncodedValue;
    }
    
    public static MessagePackEncodedText Encode(string stringValue, MessagePackSerializerOptions? options = null)
    {
        var oldSpec = options?.OldSpec ?? false;
        var characterLength = stringValue.Length;
        var bufferSize = StringEncoding.GetMaxByteCount(characterLength) + 5;
        
        var useOffset = characterLength switch
        {
            <= MessagePackRange.MaxFixStringLength => 1,
            <= byte.MaxValue when !oldSpec => 2,
            <= ushort.MaxValue => 3,
            _ => 5
        };

        Span<byte> buffer = stackalloc byte[bufferSize];
        var byteCount = StringEncoding.GetBytes(stringValue, buffer[useOffset..]);
        // move body and write prefix
        if (byteCount <= MessagePackRange.MaxFixStringLength)
        {
            buffer[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
        }
        else if (byteCount <= byte.MaxValue && !oldSpec)
        {
            buffer[0] = MessagePackCode.Str8;
            buffer[1] = unchecked((byte)byteCount);
        }
        else if (byteCount <= ushort.MaxValue)
        {
            buffer[0] = MessagePackCode.Str16;
            // Write big-endian
            buffer[1] = (byte)(byteCount >> 8);
            buffer[2] = (byte)byteCount;
        }
        else
        {
            buffer[0] = MessagePackCode.Str32;
            buffer[1] = (byte)(byteCount >> 24);
            buffer[2] = (byte)(byteCount >> 16);
            buffer[3] = (byte)(byteCount >> 8);
            buffer[4] = (byte)byteCount;
        }

        var encodedValue = buffer[..(byteCount + useOffset)].ToArray();
        return new MessagePackEncodedText(stringValue, encodedValue);
    }

    public bool Equals(MessagePackEncodedText other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is MessagePackEncodedText other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
}