using System.Reflection;

namespace Transpiler.Helpers;
internal static class AssemblyUtils
{
    private static Assembly? mvcAssembly;

    public static async Task<Assembly> GetAssembly(string assemblyPath)
    {
        var pathWithEnv = @"%USERPROFILE%\.nuget\packages\microsoft.aspnetcore.mvc.core\2.2.5\lib\netstandard2.0\Microsoft.AspNetCore.Mvc.Core.dll";
        var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);

        mvcAssembly = Assembly.LoadFrom(filePath);
        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        var assembly = Assembly.LoadFrom(assemblyPath);
        return assembly;
    }

    private static Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        return mvcAssembly;
    }
}
