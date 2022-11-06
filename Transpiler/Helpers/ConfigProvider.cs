using CommandLine;

namespace Transpiler.Helpers;

internal class ConfigProvider
{
    public static Config FromCommandLineArgs(string[] args) => Parser.Default
    .ParseArguments<Config>(args)
    .WithNotParsed(HandleParseError).Value;

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        foreach(var e in errs)
        {
            Console.WriteLine(e.ToString());
        }

        throw new ArgumentException("Invalid command line arguments");
    }
}
