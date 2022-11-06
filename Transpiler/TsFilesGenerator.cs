using System.Text;
using Transpiler.Models;

namespace Transpiler;
public class TsFilesGenerator
{
    public TsFilesGenerator(string root, bool minify = false)
    {
        _root = root;
    }

    private string _root;

    public Dictionary<string, string> GenerateFilesToSave(TypeModel[] types)
    {
        var uniquenessCheck = types.ToDictionary(t => t.FullName);

        var classesToSave = types.ToDictionary(t => $"{_root}/{t.FullName.Replace(".", "/")}.ts", t => ToTsClassFileContent(t));

        return classesToSave;
    }

    public async Task Save(TypeModel[] types)
    {
        var classesToSave = GenerateFilesToSave(types);

        foreach(var (path, content) in classesToSave)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using var fileHandler = File.CreateText(path);
            await fileHandler.WriteAsync(content);
        }

    }

    private string ToTsClassFileContent(TypeModel t)
    {
        var properties = new StringBuilder();
        var usings = new HashSet<string>();
        var className = GetClassName(t.FullName).typeName;

        foreach(var (propertyName, model) in t.Properties)
        {
            if(model.TypeCategory == TypeCategory.Enum)
            {
                continue;
            }
            var fileType = ToFileType(model);
            if(fileType.isCustomType && fileType.typeFullName != t.FullName)
            {
                var usingValue = GetUsingLine(t.FullName, model.TypeFullName);
                usings.Add(usingValue);
            }

            properties.AppendLine($"\t{propertyName}: {fileType.typeName};");
        }

        var propertiesText = properties.ToString();

        var builder = new StringBuilder();

        foreach(var usi in usings) { builder.AppendLine(usi); }
        builder.AppendLine();
        builder.AppendLine($"export default class {className}{{");
        builder.Append(properties.ToString());
        builder.AppendLine("}");

        return builder.ToString();
    }

    private string GetUsingLine(string classFullName, string propertyFullName)
    {
        var classArr = classFullName.Split(".").SkipLast(1).ToArray();
        var propArr = propertyFullName.Split(".");

        var minLenght = classArr.Length > propArr.Length ? propArr.Length : classArr.Length;

        var diffIndex = 0;

        for(; diffIndex < minLenght; diffIndex++)
        {
            if(classArr[diffIndex] != propArr[diffIndex])
            {
                break;
            }
        }

        classArr = classArr.Skip(diffIndex).ToArray();
        propArr = propArr.Skip(diffIndex).ToArray();
        var goToRoot = string.Join("", classArr.Select(x => "../"));
        var pathToClass = string.Join("/", propArr);


        return $"import {propArr.Last()} from \"{goToRoot}{pathToClass}\";";

    }

    private (string typeName, string typeFullName, bool isCustomType) ToFileType(PropertyModel model)
    {
        var baseTypeName = GetClassName(model.TypeFullName);
        var arrayPostfix = GetArrayPostfix(model.ArrayNesting);

        return ($"{baseTypeName.typeName}{arrayPostfix}", model.TypeFullName, baseTypeName.isCustomType);
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

    private static (string typeName, bool isCustomType) GetClassName(string fullName)
    {
        if(Constants.SimpleTypes.ContainsKey(fullName))
        {
            var entry = Constants.SimpleTypes[fullName];
            return (entry, false);
        }

        return (fullName.Split(".").Last(), true);
    }
}
