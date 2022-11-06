using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;
using Transpiler.Models;

namespace Transpiler;

public class NaiveAssemblyEndpointsExtractor
{
    public EndpointModel[] GetEndpoints(Assembly asm)
    {
        var types = asm.GetTypes().Select(x => x.GetTypeInfo());
        var controllers = types.Where(t => t.Name.EndsWith("Controller"));
        var methodMap = controllers.ToDictionary(x => x.Name ?? "", x => GetMethods(x));
        var result = methodMap.Select(ToControllerModel).ToArray();

        return result;
    }

    private MethodInfo[] GetMethods(TypeInfo t)
    {
        var methodsAll = t.DeclaredMethods;
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
    {
        try
        {
            var result = attr.AttributeType.IsAssignableTo(typeof(HttpMethodAttribute));
            return result;
        }
        catch(Exception)
        {
            return false;
        }
    }

    private MethodModel Map(MethodInfo methodInfo) => new MethodModel
    {
        Parameters = GetParameters(methodInfo),
        ReturnType = methodInfo.ReturnType,
    };

    private EndpointModel ToControllerModel(KeyValuePair<string, MethodInfo[]> x) => new EndpointModel
    {
        Name = x.Key,
        Methods = x.Value.Select(x => Map(x)).ToArray()
    };
}
