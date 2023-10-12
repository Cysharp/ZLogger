using System.Linq;

namespace ZLogger.Generator.Tests
{
    public class MessageParserTest
    {
        [Fact]
        public void BasicCase()
        {
            {
                MessageParser.TryParseFormat(null, out var segments).Should().BeTrue();
                MessageParser.TryParseFormat("", out segments).Should().BeTrue();
            }

            {
                MessageParser.TryParseFormat("Could not open socket to {hostName} {ipAddress}.", out var segments).Should().BeTrue();
                segments.Length.Should().Be(5);
                segments[0].Kind.Should().Be(MessageSegmentKind.Text);
                segments[0].TextSegment.Should().Be("Could not open socket to ").And.Be(segments[0].UnescapedTextSegment);

                segments[1].Kind.Should().Be(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.Should().Be("hostName");
                (segments[1].Alignment, segments[1].FormatString).Should().Be((null, null));

                segments[2].Kind.Should().Be(MessageSegmentKind.Text);
                segments[2].TextSegment.Should().Be(" ").And.Be(segments[2].UnescapedTextSegment);

                segments[3].Kind.Should().Be(MessageSegmentKind.NameParameter);
                segments[3].NameParameter.Should().Be("ipAddress");
                (segments[3].Alignment, segments[3].FormatString).Should().Be((null, null));

                segments[4].Kind.Should().Be(MessageSegmentKind.Text);
                segments[4].TextSegment.Should().Be(".").And.Be(segments[4].UnescapedTextSegment);

                string.Concat(segments.Select(x => x.ToString())).Should().Be("Could not open socket to {hostName} {ipAddress}.");
            }
        }

        [Fact]
        public void Test2()
        {
            // Begin
            {
                MessageParser.TryParseFormat("{0}aaa", out var segments).Should().BeTrue();
                segments.Length.Should().Be(2);
                segments[0].Kind.Should().Be(MessageSegmentKind.IndexParameter);
                segments[0].IndexParameter.Should().Be(0);
                segments[1].Kind.Should().Be(MessageSegmentKind.Text);
                segments[1].TextSegment.Should().Be("aaa");
                string.Concat(segments.Select(x => x.ToString())).Should().Be("{0}aaa");
            }
            // End
            {
                MessageParser.TryParseFormat("aaa{0}", out var segments).Should().BeTrue();
                segments.Length.Should().Be(2);
                segments[0].Kind.Should().Be(MessageSegmentKind.Text);
                segments[0].TextSegment.Should().Be("aaa");
                segments[1].Kind.Should().Be(MessageSegmentKind.IndexParameter);
                segments[1].IndexParameter.Should().Be(0);
                string.Concat(segments.Select(x => x.ToString())).Should().Be("aaa{0}");
            }
            // Escape Begin/End
            {
                MessageParser.TryParseFormat("{{aaa{foo}bbb}}", out var segments).Should().BeTrue();
                segments.Length.Should().Be(3);
                segments[0].Kind.Should().Be(MessageSegmentKind.Text);
                segments[0].TextSegment.Should().Be("{{aaa");
                segments[0].UnescapedTextSegment.Should().Be("{aaa");
                segments[1].Kind.Should().Be(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.Should().Be("foo");
                segments[2].Kind.Should().Be(MessageSegmentKind.Text);
                segments[2].TextSegment.Should().Be("bbb}}");
                segments[2].UnescapedTextSegment.Should().Be("bbb}");
                string.Concat(segments.Select(x => x.ToString())).Should().Be("{{aaa{foo}bbb}}");
            }
            // Escape
            {
                MessageParser.TryParseFormat("aaa{{{bar}}}bbb", out var segments).Should().BeTrue();
                segments.Length.Should().Be(3);
                segments[0].Kind.Should().Be(MessageSegmentKind.Text);
                segments[0].TextSegment.Should().Be("aaa{{");
                segments[0].UnescapedTextSegment.Should().Be("aaa{");
                segments[1].Kind.Should().Be(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.Should().Be("bar");
                segments[2].Kind.Should().Be(MessageSegmentKind.Text);
                segments[2].TextSegment.Should().Be("}}bbb");
                segments[2].UnescapedTextSegment.Should().Be("}bbb");
                string.Concat(segments.Select(x => x.ToString())).Should().Be("aaa{{{bar}}}bbb");
            }
            // TextOnly
            {
                MessageParser.TryParseFormat("aaabbbccc", out var segments).Should().BeTrue();
                segments.Length.Should().Be(1);
                segments[0].Kind.Should().Be(MessageSegmentKind.Text);
                segments[0].TextSegment.Should().Be("aaabbbccc");
                string.Concat(segments.Select(x => x.ToString())).Should().Be("aaabbbccc");
            }
            // ParameterOnly
            {
                MessageParser.TryParseFormat("{0}", out var segments).Should().BeTrue();
                segments.Length.Should().Be(1);
                segments[0].Kind.Should().Be(MessageSegmentKind.IndexParameter);
                segments[0].IndexParameter.Should().Be(0);
                string.Concat(segments.Select(x => x.ToString())).Should().Be("{0}");
            }
            // ParameterOnly2
            {
                MessageParser.TryParseFormat("{0}{foo}", out var segments).Should().BeTrue();
                segments.Length.Should().Be(2);
                segments[0].Kind.Should().Be(MessageSegmentKind.IndexParameter);
                segments[0].IndexParameter.Should().Be(0);
                segments[1].Kind.Should().Be(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.Should().Be("foo");
                string.Concat(segments.Select(x => x.ToString())).Should().Be("{0}{foo}");
            }
            // AlignmentOnly
            {
                MessageParser.TryParseFormat("aaa{bbb,100}", out var segments).Should().BeTrue();
                segments.Length.Should().Be(2);
                segments[0].Kind.Should().Be(MessageSegmentKind.Text);
                segments[0].TextSegment.Should().Be("aaa");
                segments[1].Kind.Should().Be(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.Should().Be("bbb");
                segments[1].Alignment.Should().Be("100");
                string.Concat(segments.Select(x => x.ToString())).Should().Be("aaa{bbb,100}");
            }
            // FormatOnly
            {
                MessageParser.TryParseFormat("{0:X}ccc", out var segments).Should().BeTrue();
                segments.Length.Should().Be(2);
                segments[0].Kind.Should().Be(MessageSegmentKind.IndexParameter);
                segments[0].IndexParameter.Should().Be(0);
                segments[0].FormatString.Should().Be("X");
                segments[1].Kind.Should().Be(MessageSegmentKind.Text);
                segments[1].TextSegment.Should().Be("ccc");
                string.Concat(segments.Select(x => x.ToString())).Should().Be("{0:X}ccc");
            }
            // FullParameter
            {
                MessageParser.TryParseFormat("{a,10:X}{0,-1:Z}", out var segments).Should().BeTrue();
                segments.Length.Should().Be(2);
                segments[0].Kind.Should().Be(MessageSegmentKind.NameParameter);
                segments[0].NameParameter.Should().Be("a");
                segments[0].Alignment.Should().Be("10");
                segments[0].FormatString.Should().Be("X");
                segments[1].Kind.Should().Be(MessageSegmentKind.IndexParameter);
                segments[1].IndexParameter.Should().Be(0);
                segments[1].Alignment.Should().Be("-1");
                segments[1].FormatString.Should().Be("Z");
                string.Concat(segments.Select(x => x.ToString())).Should().Be("{a,10:X}{0,-1:Z}");
            }
        }

        [Fact]
        public void ParseFailPatterns()
        {
            MessageParser.TryParseFormat("{", out var segments).Should().BeFalse();
            MessageParser.TryParseFormat("{aaa", out segments).Should().BeFalse();
            MessageParser.TryParseFormat("}", out segments).Should().BeFalse();
            MessageParser.TryParseFormat("aaa}", out segments).Should().BeFalse();
            MessageParser.TryParseFormat("a      f{           ", out segments).Should().BeFalse();
            MessageParser.TryParseFormat("a  {}   b", out segments).Should().BeFalse();
        }
    }
}