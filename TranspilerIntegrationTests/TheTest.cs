using NuGet.Configuration;
using Transpiler;
using Transpiler.Helpers;
using Xunit.Abstractions;

namespace TranspilerIntegrationTests;

public class TheTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    public TheTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GenerateFilesToSave_ForSimpleWebApi_ShouldGenerateExpectedClasses()
    {
        var procesor = new RuntimeProcessor();

        var testGuid = Guid.NewGuid();
        var outputPath = $@"./temp\{testGuid}";
        Directory.CreateDirectory(outputPath);

        var realPath = Path.GetDirectoryName(outputPath) ?? throw new ArgumentNullException(nameof(outputPath));
        Directory.CreateDirectory(realPath);

        const string assemblyPath = @"../../../../ProjectWithAssemblyDependency\bin\Debug\net6.0\ProjectWithAssemblyDependency.dll";
        var config = new Config
        {
            AssemblyPath = assemblyPath,
            OutputPath = outputPath,
            Verbose = true
        };
        await procesor.Run(config);
        Directory.Delete(outputPath, recursive: true);
    }

    [Fact]
    public void NugetPath()
    {
        var settings = Settings.LoadDefaultSettings(null);
        var nugetDiectory = SettingsUtility.GetGlobalPackagesFolder(settings);

        var subdirs = Directory.EnumerateDirectories(nugetDiectory);

        throw new Exception(String.Join("\r\n", subdirs));
    }

    [Fact]
    public void DirPath()
    {
        var assemblyPath = this.GetType().Assembly.Location;
        var assemblyDir = new FileInfo(assemblyPath).Directory?.FullName;


        var subdirs = Directory.EnumerateFiles(assemblyDir);

        throw new Exception(String.Join("\r\n", subdirs));
    }
}
