using SimpleWebApi;
using Transpiler;

namespace TranspilerIntegrationTests;

public class NaiveDllParserTests
{
    [Fact]
    public void Parse_ForSimpleWebApi_ShouldParseExpectedClasses()
    {
        var analyzer = new NaiveDllParser();
        var input = new ControllerModel[] {
            new ControllerModel{
                Name = "CalendarController",
                Methods = new []{
                    new MethodModel{
                        Parameters = new Type[]{typeof(int) },
                        ReturnType = typeof(bool),
                    }
                },
            },
            new ControllerModel{
                Name = "WeatherForecastController",
                Methods = new []{
                    new MethodModel{
                        Parameters = new Type[]{ },
                        ReturnType = typeof(IEnumerable<WeatherForecast>),
                    },
                    new MethodModel{
                        Parameters = new Type[]{ typeof(string) },
                        ReturnType = typeof(WeatherForecast),
                    }
            },
        }
        };
        var result = analyzer.Parse(input);

        var expectedResult = new TsType[] {
            new TsType{
                FullName = "SimpleWebApi.WeatherForecast",
                Properties = new Dictionary<string,TsProperty> {
                    { "TemperatureC", new TsProperty { TypeFullName = typeof(int).FullNameSupress() } },
                    { "Summary", new TsProperty { TypeFullName = typeof(WeatherForecastSummary).FullNameSupress() } },
                }
            },
            new TsType{
                FullName = "SimpleWebApi.WeatherForecastSummary",
                Properties = new Dictionary<string,TsProperty> {
                    { "OneProperty", new TsProperty { TypeFullName = typeof(string).FullNameSupress() } },
                    { "TwoProperty", new TsProperty { TypeFullName = typeof(bool).FullNameSupress() } },
                    { "ThreeProperty", new TsProperty { TypeFullName = typeof(string).FullNameSupress(), ArrayNesting = 2 } },
                }
            }
        };

        result.Should().BeEquivalentTo(expectedResult);
    }
}
