using Transpiler.Helpers;

namespace TranspilerIntegrationTests;

public class AssemblyUtilsTests
{
    [Fact]
    public void GetFromFramework()
    {
        var args = new ResolveEventArgs("Microsoft.AspNetCore.Mvc.Core, Version=6.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60");
        var asm = AssemblyUtils.GetFromFrameworks(args);

        Assert.NotNull(asm);
    }
}
