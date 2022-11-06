namespace Transpiler.Models;
public class TypeModel
{
    public string FullName { get; init; }

    public IReadOnlyDictionary<string, PropertyModel> Properties { get; init; }
}
