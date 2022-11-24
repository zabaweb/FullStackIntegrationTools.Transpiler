namespace Transpiler.Helpers;
using CommandLine;
public class Config
{
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option('a', "assembly", Required = true, HelpText = "Path to compiled assembly")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string AssemblyPath { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Option('o', "output", Required = false, HelpText = "Output directory")]
    public string OutputPath { get; set; } = "./";
}
