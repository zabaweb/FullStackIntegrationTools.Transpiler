namespace Transpiler.Models;

public class MethodModel
{
    public string MethodName { get; init; } = "";

    public Type ReturnType { get; init; } = typeof(object);

    public Type[] Parameters { get; init; } = new Type[0];

    public HttpMethodType HttpMethodType { get; init; }
    public string Route { get; set; } = "";
}
