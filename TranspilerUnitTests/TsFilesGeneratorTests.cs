using SimpleWebApi;
using Transpiler;
using Transpiler.Helpers;
using Transpiler.Models;

namespace TranspilerIntegrationTests;

public class TsFilesGeneratorTests
{
    [Fact]
    public void GenerateFilesToSave_ForSimpleWebApi_ShouldGenerateExpectedClasses()
    {
        var analyzer = new TsFilesGenerator();
        var input = new TypeModel[] {
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
        var result = analyzer.GenerateFilesToSave(input);

        var expectedResult = new Dictionary<string, string>
        {
            { "gen//SimpleWebApi//WeatherForecast.ts", "class WeatherForecast{TemperatureC:number;Summary:WeatherForecastSummary;}" },
            { "gen//SimpleWebApi//WeatherForecastSummary.ts", "class WeatherForecastSummary{OneProperty:string;TwoProperty:boolean;ThreeProperty:string[][];}" }
        };

        result.Should().BeEquivalentTo(expectedResult);
    }
}
