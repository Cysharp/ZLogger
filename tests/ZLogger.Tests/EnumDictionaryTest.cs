using FluentAssertions;
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
            EnumDictionary<Foo>.GetStringName(Foo.AAA).Should().Be("AAA");
            EnumDictionary<Foo>.GetStringName(Foo.BBB).Should().Be("BBB");
            EnumDictionary<Foo>.GetStringName(Foo.CCC).Should().Be("CCC");
            EnumDictionary<Foo>.GetStringName((Foo)33).Should().BeNull();

            EnumDictionary<More>.GetStringName(More.AAA).Should().Be("AAA");
            EnumDictionary<More>.GetStringName(More.BBB).Should().Be("BBB");
            EnumDictionary<More>.GetStringName(More.CCC).Should().Be("CCC");
            EnumDictionary<More>.GetStringName((More)33).Should().BeNull();

            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.A).Should().Be("A");
            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.D).Should().Be("D");
            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.All).Should().Be("All");
            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.None).Should().Be("None");
            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.A | FlagsEnum.D).Should().BeNull();

            EnumDictionary<Duplicate>.GetStringName(Duplicate.Foo).Should().Be("Foo");
            (EnumDictionary<Duplicate>.GetStringName(Duplicate.Bar) is "MoreBar" or "Bar").Should().BeTrue();
            EnumDictionary<Duplicate>.GetStringName(Duplicate.Baz).Should().Be("Baz");
            (EnumDictionary<Duplicate>.GetStringName(Duplicate.MoreBar) is "MoreBar" or "Bar").Should().BeTrue();
            EnumDictionary<Duplicate>.GetStringName(Duplicate.End).Should().Be("End");
        }

        [Fact]
        public void HttpStatusCodeTest()
        {
            EnumDictionary<HttpStatusCode>.GetStringName(HttpStatusCode.InternalServerError).Should().Be("InternalServerError");
            EnumDictionary<HttpStatusCode>.GetStringName(HttpStatusCode.RequestEntityTooLarge).Should().Be("RequestEntityTooLarge");
            EnumDictionary<HttpStatusCode>.GetUtf8Name(HttpStatusCode.NetworkAuthenticationRequired).ToArray().Should().Equal(Encoding.UTF8.GetBytes("NetworkAuthenticationRequired"));
            EnumDictionary<HttpStatusCode>.GetJsonEncodedName(HttpStatusCode.UnprocessableEntity).Should().Be(JsonEncodedText.Encode("UnprocessableEntity"));
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
