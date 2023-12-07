using FluentAssertions;
using MessagePack;
using Xunit;

namespace ZLogger.MessagePack.Tests;

public class MessagePackEncodedTextTest
{
    [Fact]
    public void Encode()
    {
        var fixStrEncoded = MessagePackSerializer.Serialize<string>("CategoryName");
        MessagePackEncodedText.Encode("CategoryName").Utf8EncodedValue.ToArray().Should().BeEquivalentTo(fixStrEncoded);

        var str8 = new string('a', 512);
        var str8Encoded = MessagePackSerializer.Serialize(str8);
        MessagePackEncodedText.Encode(str8).Utf8EncodedValue.ToArray().Should().BeEquivalentTo(str8Encoded);
        
        var str16 = new string('a', 65535);
        var str16Encoded = MessagePackSerializer.Serialize(str16);
        MessagePackEncodedText.Encode(str16).Utf8EncodedValue.ToArray().Should().BeEquivalentTo(str16Encoded);
        
        var str32 = new string('a', 65536);
        var str32Encoded = MessagePackSerializer.Serialize(str32);
        MessagePackEncodedText.Encode(str32).Utf8EncodedValue.ToArray().Should().BeEquivalentTo(str32Encoded);
    }
}