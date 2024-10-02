using Serilog;
using Transpiler.Helpers;
using Transpiler.Models;

namespace Transpiler;

public class RuntimeProcessor
{
    public async Task Run(Config config)
    {
        Log.Information("Run init");

        var assembly = await AssemblyUtils.GetAssembly(config.AssemblyPath);


        var typesExtractor = new TypeExtractor();
        var types = typesExtractor.ExtractTypes(assembly, config);
  
        Console.WriteLine($"{types.Length} types found: {String.Join(", ", types.Select(x => x.FullName))}");

        var generator = new TsFilesGenerator(config.OutputPath);
        await generator.Save(types);

        Log.Information("Saved files");
    }
}

