namespace Transpiler;
using CommandLine;

internal class CommandLineOptions
{
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option('a', "assembly", Required = true, HelpText = "Path to compiled assembly")]
    public string? AssemblyPath { get; set; }
}
