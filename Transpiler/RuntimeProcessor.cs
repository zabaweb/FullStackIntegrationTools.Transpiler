using Transpiler.Helpers;
using Transpiler.Models;

namespace Transpiler;

public class RuntimeProcessor
{
    public async Task Run(Config config)
    {
        var assembly = await AssemblyUtils.GetAssembly(config.AssemblyPath);

        var extractor = new NaiveAssemblyEndpointsExtractor();
        var endpoints = extractor.GetEndpoints(assembly);

        Console.WriteLine($"{endpoints.Length} endpoints found: {string.Join(", ", endpoints.Select(ToLogString))}");

        var parser = new NaiveParser();
        var types = parser.Parse(endpoints);
        Console.WriteLine($"{types.Length} types found: {String.Join(", ", types.Select(x => x.FullName))}");

        var generator = new TsFilesGenerator(config.OutputPath);
        await generator.Save(types);

        static string ToLogString(EndpointModel x)
        {
            return $"{x.Name}[{string.Join(", ", x.Methods.Select(m => m.MethodName))}]";
        }
    }
}

