#if NETFRAMEWORK
using System;
using System.Text;

namespace ZLogger.Tests
{
    public class ShimNetStandardTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("hello")]
        [InlineData("Живко")]                 // Cyrillic
        [InlineData("mañana café")]           // Latin-1 accents
        [InlineData("汉字テスト")]              // CJK/Kana mix
        public void GetBytes_Matches_Framework_For_Exact_Buffer_Utf8(string text)
        {
            var enc = Encoding.UTF8;

            // What the framework would produce (reference)
            var expected = enc.GetBytes(text);

            // Exact-size buffer
            var buffer = new byte[expected.Length];

            // IMPORTANT: call the shim explicitly (avoid BCL span overload)
            int written = Shims.GetBytes(enc, text.AsSpan(), buffer.AsSpan());

            written.Should().Be(expected.Length);
            buffer.Should().Equal(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("hello")]
        [InlineData("Žužu")]                  // Latin-2
        [InlineData("Здрасти")]               // Bulgarian
        public void GetBytes_Matches_Framework_For_Exact_Buffer_Unicode(string text)
        {
            var enc = Encoding.Unicode; // UTF-16 LE
            var expected = enc.GetBytes(text);
            var buffer = new byte[expected.Length];

            int written = Shims.GetBytes(enc, text.AsSpan(), buffer.AsSpan());

            written.Should().Be(expected.Length);
            buffer.Should().Equal(expected);
        }

        [Fact]
        public void GetBytes_Returns_Zero_When_Chars_Empty()
        {
            var enc = Encoding.UTF8;
            var buf = new byte[16];

            int written = Shims.GetBytes(enc, ReadOnlySpan<char>.Empty, buf);

            written.Should().Be(0);
            buf.Should().OnlyContain(b => b == 0); // untouched
        }

        [Fact]
        public void GetBytes_Returns_Zero_When_Bytes_Empty()
        {
            var enc = Encoding.UTF8;
            var text = "whatever";

            int written = Shims.GetBytes(enc, text.AsSpan(), ReadOnlySpan<byte>.Empty);

            written.Should().Be(0);
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("Здрасти")]
        [InlineData("mañana café")]
        [InlineData("漢字テスト")]
        public void GetString_Matches_Framework(string text)
        {
            var enc = Encoding.UTF8;
            var bytes = enc.GetBytes(text);

            // Slice in the middle too, to ensure length parameter is respected.
            var span = new ReadOnlySpan<byte>(bytes);

            string shim = Shims.GetString(enc, span);
            string bcl  = enc.GetString(bytes);

            shim.Should().Be(bcl);
        }

        [Fact]
        public void GetString_Returns_Empty_On_Empty_Bytes()
        {
            var enc = Encoding.UTF8;
            Shims.GetString(enc, ReadOnlySpan<byte>.Empty).Should().Be(string.Empty);
        }

        [Theory]
        [InlineData("x", 'x', true)]
        [InlineData("xyz", 'x', true)]
        [InlineData("xyz", 'y', false)]
        [InlineData("", 'x', false)]
        [InlineData("Жоро", 'Ж', true)]
        [InlineData("жоро", 'Ж', false)]
        public void StartsWith_Behavior(string input, char value, bool expected)
        {
            // Explicit call to the shim — do NOT rely on string.StartsWith(char) on newer TFMs
            bool actual = Shims.StartsWith(input, value);
            actual.Should().Be(expected);
        }

        [Fact]
        public void GetBytes_With_Partial_Slice_Writes_For_Slice_Length()
        {
            var enc = Encoding.UTF8;
            const string text = "ABCDE";

            var full = enc.GetBytes(text);
            // Give the shim only the first 3 chars and a correctly sized byte buffer for those 3
            var chars = text.AsSpan(0, 3);

            var expected = enc.GetBytes(chars.ToArray()); // reference for 3 chars
            var buffer = new byte[expected.Length];

            int written = Shims.GetBytes(enc, chars, buffer);

            written.Should().Be(expected.Length);
            buffer.Should().Equal(expected);
        }
    }
}
#endif
