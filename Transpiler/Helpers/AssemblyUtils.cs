using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using NuGet.Configuration;
using Serilog;

namespace Transpiler.Helpers;
internal static class AssemblyUtils
{
    public static string? RootAssemblyDirectory;
    public static string RuntimeFilePattern = "*.runtimeconfig.json";
    public static string[] DotnetDirectories = { @"C:\Program Files\dotnet", "/usr/bin/dotnet", "/usr/local/share/dotnet" };

    public static Task<Assembly?> GetAssembly(string assemblyPath)
    {
        Log.Information($"Retriving assembly from path {assemblyPath}");

        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        var assembly = LoadAssembly(assemblyPath);

        RootAssemblyDirectory = new FileInfo(assemblyPath).Directory?.FullName;
        return Task.FromResult(assembly);
    }

    public static Assembly CurrentDomain_AssemblyResolve(object? _, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name);

        Log.Information($"Resolving assembly {assemblyName}");

        var binResult = GetFromBin(args);
        if(binResult != null)
        {
            Log.Information($"Resolved from bin {assemblyName}");
            return binResult;
        }

        var nugetResult = GetFromNuget(args);
        if(nugetResult != null)
        {
            Log.Information($"Resolved from NuGet {assemblyName}");
            return nugetResult;
        }

        var sdkResult = GetFromFrameworks(args);
        if(sdkResult != null)
        {
            Log.Information($"Resolved from SDK {assemblyName}");
            return sdkResult;
        }

        Log.Information($"Assembly couldn't be resolved {assemblyName}");
        throw new Exception("3");
    }

    private static Assembly? GetFromNuget(ResolveEventArgs args)
    {
        var rootNugetPath = GetNugetPath();

        var nugetVersion = Directory.GetDirectories(rootNugetPath);
        var lib = Directory.GetDirectories(nugetVersion.OrderBy(x => x).Last()).First();
        var frameworkVersion = Directory.GetDirectories(lib).First();
        var assemblyToLoadPath = Directory.GetFiles(frameworkVersion, "*.dll").FirstOrDefault();

        if(assemblyToLoadPath == null)
        {
            return null;
        }

        return LoadAssembly(assemblyToLoadPath);
    }

    private static Assembly? GetFromBin(ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name);
        var assemblyToLoadPath = @$"{RootAssemblyDirectory}\{assemblyName.Name}.dll";
        if(!File.Exists(assemblyToLoadPath))
        {
            return null;
        }

        return LoadAssembly(assemblyToLoadPath);
    }


    private static Assembly? GetFromFrameworks(ResolveEventArgs args)
    {
        // https://natemcmaster.com/blog/2017/12/21/netcore-primitives/
        var assemblyName = new AssemblyName(args.Name);
        var frameworks = GetFrameworks(assemblyName);
        foreach(var DotnetDirectory in DotnetDirectories)
        {
            foreach(var (frameworkName, versions) in frameworks)
            {
                var frameworkDirectory = @$"{DotnetDirectory}\shared\{frameworkName}";

                var dirVersions = Directory.EnumerateDirectories(frameworkDirectory).Select(x => x.Replace(frameworkDirectory, "").Replace("\\", ""));

                foreach(var (majorMinor, patch) in versions)
                {
                    var maxPatchVersion = dirVersions
                        .Where(dv => dv.StartsWith(majorMinor))
                        .Select(dv => uint.Parse(dv.Split(".")[2]))
                        .Max();

                    if(maxPatchVersion < patch)
                    {
                        Log.Warning($"Max patch version {maxPatchVersion} is lower than expected ({frameworkName}, {patch}).");
                        continue;
                    }

                    var potentialPath = @$"{frameworkDirectory}\{majorMinor}.{maxPatchVersion}\{assemblyName.Name}.dll";
                    if(!File.Exists(potentialPath))
                    {
                        Log.Warning($"Expected path {potentialPath} does not exist ({frameworkName}, {patch}).");

                        continue;
                    }

                    try
                    {
                        return Assembly.UnsafeLoadFrom(potentialPath);
                    }
                    catch(Exception ex)
                    {
                        Log.Warning($"Failed to load assembly from {potentialPath}.", ex);
                        throw;
                        return null;
                    }
                }
            }
        }
        return null;
    }

    private static Dictionary<string, Dictionary<string, uint>> GetFrameworks(AssemblyName assemblyName)
    {
        var pathToRuntimeFile = @$"{RootAssemblyDirectory}\{assemblyName.Name}.json";
        var runtimeFiles = Directory.EnumerateFiles(RootAssemblyDirectory, RuntimeFilePattern);

        var frameworks = new Dictionary<string, Dictionary<string, uint>>();

        foreach(var runtimeFile in runtimeFiles)
        {
            var jsonString = File.ReadAllText(runtimeFile);
            var jsonSerializerOption = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var runtime = JsonNode.Parse(jsonString);

            var ddd = runtime["runtimeOptions"]["frameworks"]
                .AsArray()
                .ToArray();

            var frmw = ddd
                .Select(x => new Framework { Name = x["name"].ToString(), Version = new Version(x["version"].ToString()) })
                .ToArray();

            foreach(var framework in frmw)
            {
                if(!frameworks.ContainsKey(framework.Name))
                {
                    frameworks.Add(framework.Name, new Dictionary<string, uint>());
                }

                if(!frameworks[framework.Name].ContainsKey(framework.Version.MajorMinor))
                {
                    frameworks[framework.Name].Add(framework.Version.MajorMinor, 0);
                }

                if(frameworks[framework.Name][framework.Version.MajorMinor] < framework.Version.Patch)
                {
                    frameworks[framework.Name][framework.Version.MajorMinor] = framework.Version.Patch;
                }
            }
        }

        return frameworks;
    }

    private static Assembly? LoadAssembly(string assemblyToLoadPath)
    {
        try
        {
            return Assembly.UnsafeLoadFrom(assemblyToLoadPath);
        }
        catch(Exception ex)
        {
            Log.Error($"Failed to load assembly from {assemblyToLoadPath}", ex);
            return null;
        }
    }


    private static string GetNugetPath()
    {
        var settings = Settings.LoadDefaultSettings(root: null);
        var nugetDiectory = SettingsUtility.GetGlobalPackagesFolder(settings);
        var pathWithEnv = $@"{nugetDiectory}\";

        var rootNugetPath = Environment.ExpandEnvironmentVariables(pathWithEnv);
        return rootNugetPath;
    }
}

struct RuntimeConfig
{
    public RuntimeOptions RuntimeOptions;
}

struct RuntimeOptions
{
    public string Tfm;
    public Framework[] Frameworks;
}

struct Framework
{
    public string Name;
    public Version Version;
}

struct Version
{
    public uint Major = 0;
    public uint Minor = 0;
    public uint Patch = 0;

    public string MajorMinor => $"{Major}.{Minor}";

    public Version()
    {

    }

    public Version(string? versionsString)
    {
        if(versionsString == null)
        {
            return;
        }

        var splitted = versionsString.Split(".");
        if(splitted.Length != 3)
        {
            return;
        }

        Major = uint.Parse(splitted[0]);
        Minor = uint.Parse(splitted[1]);
        Patch = uint.Parse(splitted[2]);
    }
}
