using System.Reflection;
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
    public async Task GenerateFilesToSave_ForSimpleWebApi_ShouldGenerateExpectedClassess()
    {
        var path = "/usr/share/dotnet/shared/Microsoft.AspNetCore.App/6.0.12/Microsoft.AspNetCore.Mvc.Core.dll";
        var asembly = Assembly.LoadFile(path);

        Assert.NotNull(asembly);
    }

}
