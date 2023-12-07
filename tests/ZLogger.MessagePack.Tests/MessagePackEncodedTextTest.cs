using FluentAssertions;
using MessagePack;
using Xunit;

namespace ZLogger.MessagePack.Tests;

public class MessagePackEncodedTextTest
{
    [Fact]
    public void Encode()
    {
        var result1 = MessagePackSerializer.Serialize<string>("CategoryName");
        MessagePackEncodedText.Encode("CategoryName").Should().BeEquivalentTo(result1);

        var str8 = new string('a', 512);
        var result2 = MessagePackEncodedText.Encode(str8);
        MessagePackEncodedText.Encode(str8).Should().BeEquivalentTo(result2);
        
        var str16 = new string('a', 65535);
        var result3 = MessagePackEncodedText.Encode(str16);
        MessagePackEncodedText.Encode(str16).Should().BeEquivalentTo(result3);
        
        var str32 = new string('a', 65536);
        var result4 = MessagePackEncodedText.Encode(str32);
        MessagePackEncodedText.Encode(str32).Should().BeEquivalentTo(result4);
    }
}