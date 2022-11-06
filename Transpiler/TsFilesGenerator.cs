using System.Text;
using Transpiler.Models;

namespace Transpiler;
public class TsFilesGenerator
{

    const string root = "gen/";
    public Dictionary<string, string> GenerateFilesToSave(TypeModel[] types)
    {
        var uniquenessCheck = types.ToDictionary(t => t.FullName);

        var classesToSave = types.ToDictionary(t => $"{root}/{t.FullName.Replace(".", "//")}.ts", t => ToTsClassFileContent(t));

        return classesToSave;
    }

    public async Task Save(TypeModel[] types)
    {
        var classesToSave = GenerateFilesToSave(types);

        foreach(var (path, content) in classesToSave)
        {
            using var fileHandler = File.CreateText(path);
            await fileHandler.WriteAsync(content);
        }

    }

    private string ToTsClassFileContent(TypeModel t)
    {
        var properties = new StringBuilder();
        var usings = new HashSet<string>();
        var className = GetClassName(t.FullName);

        foreach(var prop in t.Properties)
        {
            var propClassName = GetClassName(prop.Value.TypeFullName);
            var arrayPostfix = GetArrayPostfix(prop.Value.ArrayNesting);
            properties.Append($"{prop.Key}:{propClassName}{arrayPostfix};");
        }

        var usingsText = String.Join(";", usings.ToArray());
        var propertiesText = properties.ToString();

        return $"{usingsText}class {className}{{{propertiesText}}}";
    }

    private string GetArrayPostfix(uint arrayNesting) => arrayNesting switch
    {
        0 => "",
        1 => "[]",
        2 => "[][]",
        3 => "[][][]",
        4 => "[][][][]",
        5 => "[][][][][]",
        _ => string.Concat(Enumerable.Repeat("[]", (int)arrayNesting))
    };

    private static string GetClassName(string fullName) => fullName switch
    {
        "System.String" => "string",
        "System.Int16" => "number",
        "System.Int32" => "number",
        "System.Boolean" => "boolean",
        _ => fullName.Split(".").Last()
    };
}
