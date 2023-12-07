using FluentAssertions;
using MessagePack;
using Xunit;

namespace ZLogger.MessagePack.Tests;

public class MessagePackStringEncoderTest
{
    [Fact]
    public void Encode()
    {
        var result1 = MessagePackSerializer.Serialize<string>("CategoryName");
        MessagePackStringEncoder.Encode("CategoryName").Should().BeEquivalentTo(result1);

        var str8 = new string('a', 512);
        var result2 = MessagePackStringEncoder.Encode(str8);
        MessagePackStringEncoder.Encode(str8).Should().BeEquivalentTo(result2);
        
        var str16 = new string('a', 65535);
        var result3 = MessagePackStringEncoder.Encode(str16);
        MessagePackStringEncoder.Encode(str16).Should().BeEquivalentTo(result3);
        
        var str32 = new string('a', 65536);
        var result4 = MessagePackStringEncoder.Encode(str32);
        MessagePackStringEncoder.Encode(str32).Should().BeEquivalentTo(result4);
    }
}