using SimpleWebApi;
using SimpleWebApi.Controllers;
using Transpiler;

namespace TranspilerIntegrationTests;

public class NaiveDllEndpointsExtractorTests
{
    [Fact]
    public void GetEndpoints_ForSimpleWebApi_ShouldReturnAllEndpoints()
    {
        var assembly = typeof(WeatherForecastController).Assembly;

        var analyzer = new NaiveDllEndpointsExtractor();
        var result = analyzer.GetEndpoints(assembly);

        var expectedResult = new ControllerModel[] {
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

        result.Should().BeEquivalentTo(expectedResult);
    }
}
