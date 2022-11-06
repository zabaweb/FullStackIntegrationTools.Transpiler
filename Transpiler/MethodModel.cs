namespace Transpiler;

public class MethodModel
{
    public Type ReturnType { get; init; }

    public Type[] Parameters { get; init; }

    public HttpMethodType HttpMethodType { get; init; }
    public string Route { get; set; }
}
