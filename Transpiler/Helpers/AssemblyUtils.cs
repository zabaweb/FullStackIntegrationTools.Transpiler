using System.Reflection;
using Serilog;

namespace Transpiler.Helpers;
internal static class AssemblyUtils
{
    public static string? RootAssemblyDirectory;
    public static Task<Assembly> GetAssembly(string assemblyPath)
    {
        Log.Information($"Retriving assembly from path {assemblyPath}");

        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        var assembly = Assembly.LoadFrom(assemblyPath);

        RootAssemblyDirectory = new FileInfo(assemblyPath).Directory?.FullName;
        return Task.FromResult(assembly);
    }

    private static Assembly CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        Log.Information($"Resolving assembly {args.Name}");
        var assemblyName = args.Name.Split(",").First();
        var assemblyToLoadPath = @$"{RootAssemblyDirectory}\{assemblyName}.dll";
        if(DllExists())
        {
            return LoadAndReturnAssembly();
        }

        var nugetDiectory = @"%USERPROFILE%\.nuget\packages\";
        var pathWithEnv = $@"{nugetDiectory}{assemblyName.ToLower()}\";// 2.2.5\lib\netstandard2.0\Microsoft.AspNetCore.Mvc.Core.dll";

        var rootNugetPath = Environment.ExpandEnvironmentVariables(pathWithEnv);
        if(!Directory.Exists(rootNugetPath))
        {
            throw new Exception("3");
        }

        var nugetVersion = Directory.GetDirectories(rootNugetPath).OrderBy(x => x).Last();
        var lib = Directory.GetDirectories(nugetVersion).First();
        var frameworkVersion = Directory.GetDirectories(lib).First();
        assemblyToLoadPath = Directory.GetFiles(frameworkVersion, "*.dll").First();

        return LoadAndReturnAssembly();

        bool DllExists() => File.Exists(assemblyToLoadPath);
        Assembly LoadAndReturnAssembly()
        {
            var assembly = Assembly.LoadFrom(assemblyToLoadPath);
            return assembly;
        }
    }
}
