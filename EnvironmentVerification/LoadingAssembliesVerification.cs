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

        Assembly.LoadFrom(mvcCoreFilePath);
    }
}
