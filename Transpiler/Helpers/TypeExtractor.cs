using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Transpiler.Models;

namespace Transpiler.Helpers;
public class TypeExtractor
{
    public TypeModel[] ExtractTypes(Assembly assembly, Config config)
    {
        switch(config.ExtractionStrategy)
        {
            case ExtractionStrategy.Api:
                return Api(assembly, config);
            case ExtractionStrategy.AllTypes:
                return AllTypes(assembly, config);
            
        }
        var parser = new NaiveParser();
        var types1 = assembly.GetTypes() ?? new Type[0];
        return parser.Parse(types1.ToList());
    }

    private static TypeModel[] AllTypes(Assembly assembly, Config config)
    {
        var parser = new NaiveParser();
        var types1 = assembly.GetTypes() ?? new Type[0];
        var types = parser.Parse(types1.ToList());
        return types.ToArray();
    }

    private static TypeModel[] Api(Assembly assembly, Config config)
    {
        var extractor = new NaiveAssemblyEndpointsExtractor();
        var endpoints = extractor.GetEndpoints(assembly);

        Console.WriteLine($"{endpoints.Length} endpoints found: {string.Join(", ", endpoints.Select(ToLogString))}");

        var parser = new NaiveParser();

        var types = parser.Parse(endpoints);

        return types;

        static string ToLogString(EndpointModel x)
        {
            return $"{x.Name}[{string.Join(", ", x.Methods.Select(m => m.MethodName))}]";
        }
    }
}
