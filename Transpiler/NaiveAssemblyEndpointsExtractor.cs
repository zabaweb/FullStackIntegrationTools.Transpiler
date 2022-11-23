using System.Reflection;
using Transpiler.Models;

namespace Transpiler;

public class NaiveAssemblyEndpointsExtractor
{
    public EndpointModel[] GetEndpoints(Assembly asm)
    {
        var types = asm.GetTypes();
        var controllers = types.Where(t => t.Name.EndsWith("Controller"));
        var methodMap = controllers.ToDictionary(x => x.Name ?? "", x => GetMethods(x));
        var result = methodMap.Select(ToControllerModel).ToArray();

        return result;
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
    {
        try
        {
            var type = attr.AttributeType;
            while(type != null)
            {
                if(type.Name == "HttpMethodAttribute")
                {

                    return true;
                }
                type = type.BaseType;
            }
            return false;
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
        MethodName = methodInfo.Name,
    };

    private EndpointModel ToControllerModel(KeyValuePair<string, MethodInfo[]> x) => new EndpointModel
    {
        Name = x.Key,
        Methods = x.Value.Select(x => Map(x)).ToArray()
    };
}
