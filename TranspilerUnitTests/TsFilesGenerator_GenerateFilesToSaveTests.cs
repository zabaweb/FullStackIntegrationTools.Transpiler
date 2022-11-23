using System.Numerics;
using SimpleWebApi;
using Transpiler;
using Transpiler.Helpers;
using Transpiler.Models;

namespace TranspilerIntegrationTests;

public class TsFilesGenerator_GenerateFilesToSaveTests
{
    [Fact]
    public void ForSimpleWebApi()
    {
        var analyzer = new TsFilesGenerator("rootDir");
        var input = new TypeModel[] {
            new ClassModel{
                FullName = "SimpleWebApi.WeatherForecast",
                Properties = new Dictionary<string,PropertyModel> {
                    { "TemperatureC", new PropertyModel { TypeFullName = typeof(int).FullNameSupress() } },
                    { "Summary", new PropertyModel { TypeFullName = typeof(WeatherForecastSummary).FullNameSupress() } },
                }
            },
            new ClassModel{
                FullName = "SimpleWebApi.WeatherForecastSummary",
                Properties = new Dictionary<string,PropertyModel> {
                    { "OneProperty", new PropertyModel { TypeFullName = typeof(string).FullNameSupress() } },
                    { "TwoProperty", new PropertyModel { TypeFullName = typeof(bool).FullNameSupress() } },
                    { "ThreeProperty", new PropertyModel { TypeFullName = typeof(string).FullNameSupress(), ArrayNesting = 2 } },
                }
            }
        };
        var result = analyzer.GenerateFilesToSave(input);

        result = result.ToDictionary(x => x.Key, x => x.Value.ReplaceLineEndings());

        var expectedResult = new Dictionary<string, string>
        {
            {
                @"rootDir/SimpleWebApi/WeatherForecast.ts",
               @"import WeatherForecastSummary from ""./WeatherForecastSummary"";

export default class WeatherForecast{
	TemperatureC: number;
	Summary: WeatherForecastSummary;
}
".ReplaceLineEndings()
            },
            { @"rootDir/SimpleWebApi/WeatherForecastSummary.ts",
            @"export default class WeatherForecastSummary{
	OneProperty: string;
	TwoProperty: boolean;
	ThreeProperty: string[][];
}
".ReplaceLineEndings()
            },
        };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ForGenericClass()
    {
        var analyzer = new TsFilesGenerator("rootDir");
        var input = new TypeModel[] {
            new ClassModel{
                FullName = "TestNamespace.GenericClass<T1>",
                Properties = new Dictionary<string,PropertyModel> {
                    { "GenericParam", new PropertyModel { TypeFullName = "T1", IsGeneric = true } }
                }
            },
        };
        var result = analyzer.GenerateFilesToSave(input);

        var expectedResult = new Dictionary<string, string>
        {
            {
                @"rootDir/TestNamespace/GenericClass.ts",
               @"export default class GenericClass<T1>{
	GenericParam: T1;
}
"
            },
        };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ForEnumClass()
    {
        var analyzer = new TsFilesGenerator("rootDir");
        var input = new TypeModel[] {
            new EnumModel{
                FullName = "Some.Path.To.TestEnum",
                Pairs = new Dictionary<string, BigInteger> {
                    { "V1", 1 },
                    { "V2", 2 },
                    { "V3", 3},
                },
            },
        };
        var result = analyzer.GenerateFilesToSave(input);

        var expectedResult = new Dictionary<string, string>
        {
            {
                @"rootDir/Some/Path/To/TestEnum.ts",
"export default enum TestEnum {\r\n\tV1 = 1,\r\n\tV2 = 2,\r\n\tV3 = 3,\r\n}\r\n"
            },
        };

        result.Should().BeEquivalentTo(expectedResult);
    }
}
