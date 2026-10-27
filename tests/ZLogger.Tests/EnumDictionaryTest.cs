using System;
using System.Net;
using System.Text;
using System.Text.Json;
using ZLogger.Internal;


namespace ZLogger.Tests
{

    public unsafe class EnumDictionaryTest
    {
        [Fact]
        public void Lookup()
        {
            Assert.Equal("AAA", EnumDictionary<Foo>.GetStringName(Foo.AAA));
            Assert.Equal("BBB", EnumDictionary<Foo>.GetStringName(Foo.BBB));
            Assert.Equal("CCC", EnumDictionary<Foo>.GetStringName(Foo.CCC));
            Assert.Null(EnumDictionary<Foo>.GetStringName((Foo)33));

            Assert.Equal("AAA", EnumDictionary<More>.GetStringName(More.AAA));
            Assert.Equal("BBB", EnumDictionary<More>.GetStringName(More.BBB));
            Assert.Equal("CCC", EnumDictionary<More>.GetStringName(More.CCC));
            Assert.Null(EnumDictionary<More>.GetStringName((More)33));

            Assert.Equal("A", EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.A));
            Assert.Equal("D", EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.D));
            Assert.Equal("All", EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.All));
            Assert.Equal("None", EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.None));
            Assert.Null(EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.A | FlagsEnum.D));

            Assert.Equal("Foo", EnumDictionary<Duplicate>.GetStringName(Duplicate.Foo));
            Assert.True((EnumDictionary<Duplicate>.GetStringName(Duplicate.Bar) is "MoreBar" or "Bar"));
            Assert.Equal("Baz", EnumDictionary<Duplicate>.GetStringName(Duplicate.Baz));
            Assert.True((EnumDictionary<Duplicate>.GetStringName(Duplicate.MoreBar) is "MoreBar" or "Bar"));
            Assert.Equal("End", EnumDictionary<Duplicate>.GetStringName(Duplicate.End));
        }

        [Fact]
        public void HttpStatusCodeTest()
        {
            Assert.Equal("InternalServerError", EnumDictionary<HttpStatusCode>.GetStringName(HttpStatusCode.InternalServerError));
            Assert.Equal("RequestEntityTooLarge", EnumDictionary<HttpStatusCode>.GetStringName(HttpStatusCode.RequestEntityTooLarge));
            Assert.Equal(Encoding.UTF8.GetBytes("NetworkAuthenticationRequired"), EnumDictionary<HttpStatusCode>.GetUtf8Name(HttpStatusCode.NetworkAuthenticationRequired).ToArray());
            Assert.Equal(JsonEncodedText.Encode("UnprocessableEntity"), EnumDictionary<HttpStatusCode>.GetJsonEncodedName(HttpStatusCode.UnprocessableEntity));
        }
    }

    public enum Foo
    {
        AAA,
        BBB,
        CCC
    }

    public enum More
    {
        AAA = 10,
        BBB = 20,
        CCC = 30
    }

    [Flags]
    public enum FlagsEnum
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4,
        D = 8,
        E = 16,
        All = A | B | C | D | E
    }

    public enum Duplicate
    {
        Foo = 1,
        Bar = 2,
        Baz = 3,
        MoreBar = 2,
        End = 4
    }
}
