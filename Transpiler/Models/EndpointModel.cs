namespace Transpiler.Models;

public class EndpointModel
{
    public string Name { get; init; } = default!;
    public MethodModel[] Methods { get; init; } = default!;
}
