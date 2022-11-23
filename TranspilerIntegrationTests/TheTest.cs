using Transpiler;
using Transpiler.Helpers;

namespace TranspilerIntegrationTests;

public class TheTest
{
    [Fact]
    public async Task GenerateFilesToSave_ForSimpleWebApi_ShouldGenerateExpectedClasses()
    {
        var procesor = new RuntimeProcessor();
        var testGuid = Guid.NewGuid();
        var outputPath = $@"..\temp\{testGuid}";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        Directory.CreateDirectory(outputPath);
        var ddd = Directory.EnumerateFiles("./");
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
}
