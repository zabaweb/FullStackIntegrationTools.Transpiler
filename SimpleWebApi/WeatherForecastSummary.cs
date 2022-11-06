using SimpleWebApi.Nesting.MoreNesting;

namespace SimpleWebApi;

public class WeatherForecastSummary
{
    public string OneProperty { get; set; }
    public bool TwoProperty { get; set; }
    public string[][] ThreeProperty { get; set; }
    public decimal[][] SomeDecimal { get; set; }
    public NestedClass NestingTest { get; set; }
}
