using System.Linq;

namespace ZLogger.Generator.Tests
{
    public class MessageParserTest
    {
        [Fact]
        public void BasicCase()
        {
            {
                Assert.True(MessageParser.TryParseFormat(null, out var segments));
                Assert.True(MessageParser.TryParseFormat("", out segments));
            }

            {
                Assert.True(MessageParser.TryParseFormat("Could not open socket to {hostName} {ipAddress}.", out var segments));
                Assert.Equal(5, segments.Length);
                Assert.Equal(MessageSegmentKind.Text, segments[0].Kind);
                Assert.Equal("Could not open socket to ", segments[0].TextSegment);
                Assert.Equal(segments[0].UnescapedTextSegment, segments[0].TextSegment);

                Assert.Equal(MessageSegmentKind.NameParameter, segments[1].Kind);
                Assert.Equal("hostName", segments[1].NameParameter);
                Assert.Equal((null, null), (segments[1].Alignment, segments[1].FormatString));

                Assert.Equal(MessageSegmentKind.Text, segments[2].Kind);
                Assert.Equal(" ", segments[2].TextSegment);
                Assert.Equal(segments[2].UnescapedTextSegment, segments[2].TextSegment);

                Assert.Equal(MessageSegmentKind.NameParameter, segments[3].Kind);
                Assert.Equal("ipAddress", segments[3].NameParameter);
                Assert.Equal((null, null), (segments[3].Alignment, segments[3].FormatString));

                Assert.Equal(MessageSegmentKind.Text, segments[4].Kind);
                Assert.Equal(".", segments[4].TextSegment);
                Assert.Equal(segments[4].UnescapedTextSegment, segments[4].TextSegment);

                Assert.Equal("Could not open socket to {hostName} {ipAddress}.", string.Concat(segments.Select(x => x.ToString())));
            }
        }

        [Fact]
        public void Test2()
        {
            // Begin
            {
                Assert.True(MessageParser.TryParseFormat("{0}aaa", out var segments));
                Assert.Equal(2, segments.Length);
                Assert.Equal(MessageSegmentKind.IndexParameter, segments[0].Kind);
                Assert.Equal(0, segments[0].IndexParameter);
                Assert.Equal(MessageSegmentKind.Text, segments[1].Kind);
                Assert.Equal("aaa", segments[1].TextSegment);
                Assert.Equal("{0}aaa", string.Concat(segments.Select(x => x.ToString())));
            }
            // End
            {
                Assert.True(MessageParser.TryParseFormat("aaa{0}", out var segments));
                Assert.Equal(2, segments.Length);
                Assert.Equal(MessageSegmentKind.Text, segments[0].Kind);
                Assert.Equal("aaa", segments[0].TextSegment);
                Assert.Equal(MessageSegmentKind.IndexParameter, segments[1].Kind);
                Assert.Equal(0, segments[1].IndexParameter);
                Assert.Equal("aaa{0}", string.Concat(segments.Select(x => x.ToString())));
            }
            // Escape Begin/End
            {
                Assert.True(MessageParser.TryParseFormat("{{aaa{foo}bbb}}", out var segments));
                Assert.Equal(3, segments.Length);
                Assert.Equal(MessageSegmentKind.Text, segments[0].Kind);
                Assert.Equal("{{aaa", segments[0].TextSegment);
                Assert.Equal("{aaa", segments[0].UnescapedTextSegment);
                Assert.Equal(MessageSegmentKind.NameParameter, segments[1].Kind);
                Assert.Equal("foo", segments[1].NameParameter);
                Assert.Equal(MessageSegmentKind.Text, segments[2].Kind);
                Assert.Equal("bbb}}", segments[2].TextSegment);
                Assert.Equal("bbb}", segments[2].UnescapedTextSegment);
                Assert.Equal("{{aaa{foo}bbb}}", string.Concat(segments.Select(x => x.ToString())));
            }
            // Escape
            {
                Assert.True(MessageParser.TryParseFormat("aaa{{{bar}}}bbb", out var segments));
                Assert.Equal(3, segments.Length);
                Assert.Equal(MessageSegmentKind.Text, segments[0].Kind);
                Assert.Equal("aaa{{", segments[0].TextSegment);
                Assert.Equal("aaa{", segments[0].UnescapedTextSegment);
                Assert.Equal(MessageSegmentKind.NameParameter, segments[1].Kind);
                Assert.Equal("bar", segments[1].NameParameter);
                Assert.Equal(MessageSegmentKind.Text, segments[2].Kind);
                Assert.Equal("}}bbb", segments[2].TextSegment);
                Assert.Equal("}bbb", segments[2].UnescapedTextSegment);
                Assert.Equal("aaa{{{bar}}}bbb", string.Concat(segments.Select(x => x.ToString())));
            }
            // TextOnly
            {
                Assert.True(MessageParser.TryParseFormat("aaabbbccc", out var segments));
                Assert.Single(segments);
                Assert.Equal(MessageSegmentKind.Text, segments[0].Kind);
                Assert.Equal("aaabbbccc", segments[0].TextSegment);
                Assert.Equal("aaabbbccc", string.Concat(segments.Select(x => x.ToString())));
            }
            // ParameterOnly
            {
                Assert.True(MessageParser.TryParseFormat("{0}", out var segments));
                Assert.Single(segments);
                Assert.Equal(MessageSegmentKind.IndexParameter, segments[0].Kind);
                Assert.Equal(0, segments[0].IndexParameter);
                Assert.Equal("{0}", string.Concat(segments.Select(x => x.ToString())));
            }
            // ParameterOnly2
            {
                Assert.True(MessageParser.TryParseFormat("{0}{foo}", out var segments));
                Assert.Equal(2, segments.Length);
                Assert.Equal(MessageSegmentKind.IndexParameter, segments[0].Kind);
                Assert.Equal(0, segments[0].IndexParameter);
                Assert.Equal(MessageSegmentKind.NameParameter, segments[1].Kind);
                Assert.Equal("foo", segments[1].NameParameter);
                Assert.Equal("{0}{foo}", string.Concat(segments.Select(x => x.ToString())));
            }
            // AlignmentOnly
            {
                Assert.True(MessageParser.TryParseFormat("aaa{bbb,100}", out var segments));
                Assert.Equal(2, segments.Length);
                Assert.Equal(MessageSegmentKind.Text, segments[0].Kind);
                Assert.Equal("aaa", segments[0].TextSegment);
                Assert.Equal(MessageSegmentKind.NameParameter, segments[1].Kind);
                Assert.Equal("bbb", segments[1].NameParameter);
                Assert.Equal("100", segments[1].Alignment);
                Assert.Equal("aaa{bbb,100}", string.Concat(segments.Select(x => x.ToString())));
            }
            // FormatOnly
            {
                Assert.True(MessageParser.TryParseFormat("{0:X}ccc", out var segments));
                Assert.Equal(2, segments.Length);
                Assert.Equal(MessageSegmentKind.IndexParameter, segments[0].Kind);
                Assert.Equal(0, segments[0].IndexParameter);
                Assert.Equal("X", segments[0].FormatString);
                Assert.Equal(MessageSegmentKind.Text, segments[1].Kind);
                Assert.Equal("ccc", segments[1].TextSegment);
                Assert.Equal("{0:X}ccc", string.Concat(segments.Select(x => x.ToString())));
            }
            // FullParameter
            {
                Assert.True(MessageParser.TryParseFormat("{a,10:X}{0,-1:Z}", out var segments));
                Assert.Equal(2, segments.Length);
                Assert.Equal(MessageSegmentKind.NameParameter, segments[0].Kind);
                Assert.Equal("a", segments[0].NameParameter);
                Assert.Equal("10", segments[0].Alignment);
                Assert.Equal("X", segments[0].FormatString);
                Assert.Equal(MessageSegmentKind.IndexParameter, segments[1].Kind);
                Assert.Equal(0, segments[1].IndexParameter);
                Assert.Equal("-1", segments[1].Alignment);
                Assert.Equal("Z", segments[1].FormatString);
                Assert.Equal("{a,10:X}{0,-1:Z}", string.Concat(segments.Select(x => x.ToString())));
            }
        }

        [Fact]
        public void ParseFailPatterns()
        {
            Assert.False(MessageParser.TryParseFormat("{", out var segments));
            Assert.False(MessageParser.TryParseFormat("{aaa", out segments));
            Assert.False(MessageParser.TryParseFormat("}", out segments));
            Assert.False(MessageParser.TryParseFormat("aaa}", out segments));
            Assert.False(MessageParser.TryParseFormat("a      f{           ", out segments));
            Assert.False(MessageParser.TryParseFormat("a  {}   b", out segments));
        }
    }
}