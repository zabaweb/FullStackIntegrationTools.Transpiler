using Xunit.Abstractions;

namespace TranspilerIntegrationTests;

public class ValidationTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ValidationTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Nuget()
    {
        var nugetDiectoryPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.nuget\packages\");
        var nugetDirs = Directory.EnumerateDirectories(nugetDiectoryPath);
        _testOutputHelper.WriteLine(nugetDiectoryPath);

        foreach(var dir in nugetDirs)
        {
            _testOutputHelper.WriteLine(dir);
        }
    }
}
