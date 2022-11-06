using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Transpiler;

public class NaiveDllEndpointsExtractor
{

    public ControllerModel[] GetEndpoints(Assembly asm)
    {
        return asm.GetTypes()
            .Where(t => t.Name.EndsWith("Controller"))
            .ToDictionary(x => x.Name ?? "", x => GetMethods(x))
            .Select(ToControllerModel)
            .ToArray();
    }

    private MethodInfo[] GetMethods(Type t)
    {
        var methodsAll = t.GetMethods();
        var methods = methodsAll
            .Where(m => m.CustomAttributes.Any(x => IsHttpMethodAttribute(x)));

        return methods.ToArray();
    }

    private Type[] GetParameters(MethodInfo methodInfo)
        => methodInfo
        .GetParameters()
        .Select(x => x.ParameterType)
        .ToArray();

    private bool IsHttpMethodAttribute(CustomAttributeData attr)
        => attr.AttributeType.IsAssignableTo(typeof(HttpMethodAttribute));

    private MethodModel Map(MethodInfo methodInfo) => new MethodModel
    {
        Parameters = GetParameters(methodInfo),
        ReturnType = methodInfo.ReturnType,
    };

    private ControllerModel ToControllerModel(KeyValuePair<string, MethodInfo[]> x) => new ControllerModel
    {
        Name = x.Key,
        Methods = x.Value.Select(x => Map(x)).ToArray()
    };
}