namespace Transpiler.Helpers;

public static class TypeExtensions
{
    public static string FullNameSupress(this Type type) => type?.FullName?.Split("`")[0] ?? type?.Name ?? "";
}
