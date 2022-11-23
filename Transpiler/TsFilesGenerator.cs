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

        var classesToSave = types.ToDictionary(ToFileName(), t => ToTsFileContent(t));

        return classesToSave;
    }

    private Func<TypeModel, string> ToFileName()
    {
        return t =>
        {
            var withoutGenericParams = t.FullName.Split("<")[0];
            return $"{_root}/{withoutGenericParams.Replace(".", "/")}.ts";
        };
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

    private string ToTsFileContent(TypeModel t)
    {
        return t switch
        {
            ClassModel classModel => ToTsClassFileContent(classModel),
            EnumModel enumModel => ToTsEnumFileContent(enumModel),
            _ => throw new Exception("4")
        };
    }

    private string ToTsEnumFileContent(EnumModel enumModel)
    {
        var builder = new StringBuilder();
        var hasValues = enumModel.Pairs.Count > 0;
        builder.Append($"export default enum {enumModel.FullName.Split(".").Last()}");
        var ddd = hasValues ? "{" : "{}";
        builder.AppendLine($" {ddd}");

        if(!hasValues)
        {
            return builder.ToString(); ;
        }

        foreach(var (valueLabel, value) in enumModel.Pairs)
        {
            builder.AppendLine($"\t{valueLabel} = {value},");
        }

        builder.AppendLine("}");

        return builder.ToString();
    }

    private string ToTsClassFileContent(ClassModel t)
    {
        var properties = new StringBuilder();
        var usings = new HashSet<string>();
        var className = GetClassName(t.FullName).typeName;

        foreach(var (propertyName, model) in t.Properties)
        {
            var fileType = ToFileType(model);
            if(fileType.isCustomType && fileType.typeFullName != t.FullName && !model.IsGeneric)
            {
                var usingValue = GetUsingLine(t.FullName, model.TypeFullName);
                usings.Add(usingValue);
            }

            properties.AppendLine($"\t{propertyName}: {fileType.typeName};");
        }

        var propertiesText = properties.ToString();

        var builder = new StringBuilder();

        foreach(var usi in usings) { builder.AppendLine(usi); }
        if(usings.Any()) { builder.AppendLine(); }
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
        if(String.IsNullOrWhiteSpace(goToRoot))
        {
            goToRoot = "./";
        }

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
