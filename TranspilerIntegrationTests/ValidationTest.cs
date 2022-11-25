using Xunit.Abstractions;

namespace TranspilerIntegrationTests;

public class validationTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    [Fact]
    public async Task Nuget()
    {
        var nugetDiectoryPath = @"%USERPROFILE%\.nuget\packages\";

        var nugetDirs = Directory.EnumerateDirectories(nugetDiectoryPath);

        foreach(var dir in nugetDirs)
        {
            _testOutputHelper.WriteLine(dir)
        }
    }
}
