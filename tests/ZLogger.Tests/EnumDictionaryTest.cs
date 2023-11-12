using FluentAssertions;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using ZLogger.Internal;

namespace ZLogger.Tests
{
    public class EnumDictionaryTest
    {
        [Fact]
        public void Lookup()
        {
            var foo = EnumDictionary.Create<Foo>();
            var more = EnumDictionary.Create<More>();
            var flags = EnumDictionary.Create<FlagsEnum>();
            var duplicate = EnumDictionary.Create<Duplicate>();

            foo.GetStringName(ToBytes(Foo.AAA)).Should().Be("AAA");
            foo.GetStringName(ToBytes(Foo.BBB)).Should().Be("BBB");
            foo.GetStringName(ToBytes(Foo.CCC)).Should().Be("CCC");
            foo.GetStringName(ToBytes((Foo)33)).Should().BeNull();

            more.GetStringName(ToBytes(More.AAA)).Should().Be("AAA");
            more.GetStringName(ToBytes(More.BBB)).Should().Be("BBB");
            more.GetStringName(ToBytes(More.CCC)).Should().Be("CCC");
            more.GetStringName(ToBytes((More)33)).Should().BeNull();

            flags.GetStringName(ToBytes(FlagsEnum.A)).Should().Be("A");
            flags.GetStringName(ToBytes(FlagsEnum.D)).Should().Be("D");
            flags.GetStringName(ToBytes(FlagsEnum.All)).Should().Be("All");
            flags.GetStringName(ToBytes(FlagsEnum.None)).Should().Be("None");
            flags.GetStringName(ToBytes(FlagsEnum.A | FlagsEnum.D)).Should().BeNull();

            duplicate.GetStringName(ToBytes(Duplicate.Foo)).Should().Be("Foo");
            (duplicate.GetStringName(ToBytes(Duplicate.Bar)) is "MoreBar" or "Bar").Should().BeTrue();
            duplicate.GetStringName(ToBytes(Duplicate.Baz)).Should().Be("Baz");
            (duplicate.GetStringName(ToBytes(Duplicate.MoreBar)) is "MoreBar" or "Bar").Should().BeTrue();
            duplicate.GetStringName(ToBytes(Duplicate.End)).Should().Be("End");
        }

        [Fact]
        public void HttpStatusCodeTest()
        {
            var statuscode = EnumDictionary.Create<System.Net.HttpStatusCode>();
            statuscode.GetStringName(ToBytes(HttpStatusCode.InternalServerError)).Should().Be("InternalServerError");
            statuscode.GetStringName(ToBytes(HttpStatusCode.RequestEntityTooLarge)).Should().Be("RequestEntityTooLarge");
            statuscode.GetUtf8Name(ToBytes(HttpStatusCode.NetworkAuthenticationRequired)).ToArray().Should().Equal(Encoding.UTF8.GetBytes("NetworkAuthenticationRequired"));
            statuscode.GetJsonEncodedName(ToBytes(HttpStatusCode.UnprocessableEntity)).Should().Be(JsonEncodedText.Encode("UnprocessableEntity"));
        }

        byte[] ToBytes<T>(T e)
            where T : struct, Enum
        {
            Span<byte> s = stackalloc byte[Unsafe.SizeOf<T>()];
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(s), e);
            return s.ToArray();
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
