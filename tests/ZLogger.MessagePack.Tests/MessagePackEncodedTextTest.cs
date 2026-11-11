using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace ZLogger.MessagePack.Tests;

public class MessagePackEncodedTextTest
{
    public static TheoryData<string, byte[]> Cases => new()
    {
        { "CategoryName", [0xac] },
        { new string('a', 31), [0xbf] },
        { new string('a', 32), [0xd9, 32] },
        { new string('a', 255), [0xd9, 255] },
        { new string('a', 256), [0xda, 0x01, 0x00] },
        { new string('a', 65535), [0xda, 0xff, 0xff] },
        { new string('a', 65536), [0xdb, 0x00, 0x01, 0x00, 0x00] },
        // Non-ASCII text, where the format depends on the UTF-8 byte length, not on the character length.
        { new string('あ', 10), [0xa0 | 30] },
        { new string('あ', 11), [0xd9, 33] },
        { new string('あ', 86), [0xda, 0x01, 0x02] },
    };

    /// <summary>
    /// Ensures that Encode writes the str header that the MessagePack spec requires for the UTF-8 byte length
    /// (fixstr up to 31 bytes, str8 up to 255, str16 up to 65535 and str32 beyond), followed by the UTF-8 bytes,
    /// including for non-ASCII text whose byte length differs from its character length.
    /// </summary>
    [Theory]
    [MemberData(nameof(Cases))]
    public void Encode(string value, byte[] expectedHeader)
    {
        var expected = expectedHeader.Concat(Encoding.UTF8.GetBytes(value)).ToArray();

        MessagePackEncodedText.Encode(value).Utf8EncodedValue.ToArray().Should().Equal(expected);
    }

    /// <summary>
    /// Ensures that Encode rejects null instead of producing a nil key name, which readers do not accept as a map key.
    /// </summary>
    [Fact]
    public void EncodeRejectsNull()
    {
        var encode = () => MessagePackEncodedText.Encode(null!);
        encode.Should().Throw<System.ArgumentNullException>();
    }
}
