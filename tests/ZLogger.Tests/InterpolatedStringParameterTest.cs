using FluentAssertions;
using ZLogger.Internal;

namespace ZLogger.Tests
{
    public class InterpolatedStringParameterTest
    {
        [Fact]
        public void GetParameterizeName()
        {
            var result1 = CallerArgumentExpressionParser.GetParameterizedName("id");
            result1.ToString().Should().Be("id");

            var result2 = CallerArgumentExpressionParser.GetParameterizedName("HttpContext.RequestId");
            result2.ToString().Should().Be("RequestId");

            var result3 = CallerArgumentExpressionParser.GetParameterizedName("MyNamespace.MyClass.Foo.Bar.Buz");
            result3.ToString().Should().Be("Buz");

            var result4 = CallerArgumentExpressionParser.GetParameterizedName("myInstance.DoSomething(1, 2, 3)");
            result4.ToString().Should().Be("DoSomething");
        }
        
        [Fact]
        public void GetParameterizeName_WithStringLiteral()
        {
            var result1 = CallerArgumentExpressionParser.GetParameterizedName("""Hoge.Moge("aaa.aaa(\"123\")")""");
            result1.ToString().Should().Be("Moge");
            
            var result2 = CallerArgumentExpressionParser.GetParameterizedName("""Hoge.Moge("aaa.aaa(\"123\")").Fuga("bbb")""");
            result2.ToString().Should().Be("Fuga");
        }
        
        [Fact]
        public void GetLastPropertyName_WithVerbatimStringLiteral()
        {
            var result1 = CallerArgumentExpressionParser.GetParameterizedName("""Hoge.Moge(@"aaa.aaa(""123"")")""");
            result1.ToString().Should().Be("Moge");
            
            var result2 = CallerArgumentExpressionParser.GetParameterizedName("""Hoge.Moge(@"aaa.aaa(""123"")").Fuga("bbb")""");
            result2.ToString().Should().Be("Fuga");
        }
        
        [Fact]
        public void GetLastPropertyName_WithRawStringLiteral()
        {
            var result1 = CallerArgumentExpressionParser.GetParameterizedName(""""Hoge.Moge("""aaa...""bbb(((()))""..ccc""")"""");
            result1.ToString().Should().Be("Moge");
            
            var result2 = CallerArgumentExpressionParser.GetParameterizedName("""""
Hoge.Moge(""""
aaa...
bbb(((()))
..ccc
""""
""""");
            result2.ToString().Should().Be("Moge");
        }
    }
}
