using SimpleWebApi;
using SimpleWebApi.Controllers;
using Transpiler;
using Transpiler.Models;

namespace TranspilerIntegrationTests;

public class NaiveAssemblyEndpointsExtractorTests
{
    [Fact]
    public void GetEndpoints_ForSimpleWebApi_ShouldReturnAllEndpoints()
    {
        var assembly = typeof(WeatherForecastController).Assembly;

        var analyzer = new NaiveAssemblyEndpointsExtractor();
        var result = analyzer.GetEndpoints(assembly);

        var expectedResult = new EndpointModel[] {
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

        result.Should().BeEquivalentTo(expectedResult);
    }
}
