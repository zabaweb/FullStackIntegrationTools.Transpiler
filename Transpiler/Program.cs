using System.Reflection;
using System.Runtime.InteropServices;
using CommandLine;
using Transpiler;

var cmd = CommandLine.Parser.Default
    .ParseArguments<CommandLineOptions>(args)
    .WithNotParsed(HandleParseError).Value;

var resolver = new PathAssemblyResolver(new List<string>(Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll"))
{
    cmd.AssemblyPath,
    "Microsoft.AspNetCore.Mvc.Core.dll"
});

var dir = new DirectoryInfo(".");

using var metadataContext = new MetadataLoadContext(resolver);
var assembly = metadataContext.LoadFromAssemblyPath(cmd.AssemblyPath);
_ = metadataContext.LoadFromAssemblyPath("Microsoft.AspNetCore.Mvc.Core.dll");

var ddd = metadataContext.GetAssemblies();

var extractor = new NaiveAssemblyEndpointsExtractor();
var endpointModels = extractor.GetEndpoints(assembly);

var parser = new NaiveParser();
var types = parser.Parse(endpointModels);

var generator = new TsFilesGenerator();
await generator.Save(types);


static void HandleParseError(IEnumerable<Error> errs)
{
    foreach(var e in errs)
    {
        Console.WriteLine(e.ToString());
    }

    throw new ArgumentException("Invalid command line arguments");
}
