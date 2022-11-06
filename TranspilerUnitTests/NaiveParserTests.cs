using SimpleWebApi;
using Transpiler;
using Transpiler.Helpers;
using Transpiler.Models;

namespace TranspilerIntegrationTests;

public class NaiveParserTests
{
    [Fact]
    public void Parse_ForSimpleWebApi_ShouldParseExpectedClasses()
    {
        var analyzer = new NaiveParser();
        var input = new EndpointModel[] {
            new EndpointModel{
                Name = "CalendarController",
                Methods = new []{
                    new MethodModel{
                        Parameters = new Type[]{typeof(int) },
                        ReturnType = typeof(bool),
                    }
                },
            },
            new EndpointModel{
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

        var expectedResult = new TypeModel[] {
            new TypeModel{
                FullName = "SimpleWebApi.WeatherForecast",
                Properties = new Dictionary<string,PropertyModel> {
                    { "TemperatureC", new PropertyModel { TypeFullName = typeof(int).FullNameSupress() } },
                    { "Summary", new PropertyModel { TypeFullName = typeof(WeatherForecastSummary).FullNameSupress() } },
                }
            },
            new TypeModel{
                FullName = "SimpleWebApi.WeatherForecastSummary",
                Properties = new Dictionary<string,PropertyModel> {
                    { "OneProperty", new PropertyModel { TypeFullName = typeof(string).FullNameSupress() } },
                    { "TwoProperty", new PropertyModel { TypeFullName = typeof(bool).FullNameSupress() } },
                    { "ThreeProperty", new PropertyModel { TypeFullName = typeof(string).FullNameSupress(), ArrayNesting = 2 } },
                }
            }
        };

        result.Should().BeEquivalentTo(expectedResult);
    }
}
