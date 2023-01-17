using System.Reflection;

namespace EnvironmentVerification;

public class LoadingAssembliesVerification
{

    [Fact]
    public void LoadMicrosoftAspNetCoreMvcCore()
    {
        var path = "";
        var dotnetDirectories = new string[] { "/usr/share/dotnet", @"C:\Program Files\dotnet" };
        var dotnetVersions = new string[] { "6.0.11", "6.0.12" };

        foreach(var dir in dotnetDirectories)
        {
            if(Directory.Exists(dir))
            {
                path = dir;
                break;
            }
        }

        Assert.True(Directory.Exists(path));


        foreach(var version in dotnetVersions)
        {
            var temp = $"{path}/shared/Microsoft.AspNetCore.App/{version}";
            if(Directory.Exists(temp))
            {
                path = temp;
                break;
            }
        }

        Assert.True(Directory.Exists(path));

        var mvcCoreFilePath = $"{path}/Microsoft.AspNetCore.Mvc.Core.dll";

        Assert.True(File.Exists(mvcCoreFilePath));

        var assembly = Assembly.LoadFrom(mvcCoreFilePath);

        Assert.NotNull(assembly);
        var assemblyName = assembly.GetName();

        Assert.Equal("Microsoft.AspNetCore.Mvc.Core, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60", assemblyName.FullName);
        Assembly.Load("Microsoft.AspNetCore.Mvc.Core, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60");
    }
}
