namespace Transpiler.Models;

public class MethodModel
{
    public string MethodName { get; init; }

    public Type ReturnType { get; init; }

    public Type[] Parameters { get; init; }

    public HttpMethodType HttpMethodType { get; init; }
    public string Route { get; set; }
}
