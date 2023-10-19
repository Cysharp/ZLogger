using FluentAssertions;

namespace ZLogger.Tests
{
    public class InterpolatedStringParameterTest
    {
        [Fact]
        public void GetLastPropertyName()
        {
            var result1 = new InterpolatedStringParameter(typeof(int), "id", 0, null, 0, null).GetParameterizeNamePart();
            result1.ToString().Should().Be("id");
            
            var result2 = new InterpolatedStringParameter(typeof(int), "HttpContext.RequestId", 0, null, 0, null).GetParameterizeNamePart();
            result2.ToString().Should().Be("RequestId");
            
            var result3 = new InterpolatedStringParameter(typeof(int), "MyNamespace.MyClass.Foo.Bar.Buz", 0, null, 0, null).GetParameterizeNamePart();
            result3.ToString().Should().Be("Buz");
            
            var result4 = new InterpolatedStringParameter(typeof(int), "myInstance.DoSomething(1, 2, 3)", 0, null, 0, null).GetParameterizeNamePart();
            result4.ToString().Should().Be("DoSomething");
        }
    }
}
