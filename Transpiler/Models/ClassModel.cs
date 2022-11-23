namespace Transpiler.Models;
public class ClassModel: TypeModel
{
    public string FullName { get; init; } = "";

    public IReadOnlyDictionary<string, PropertyModel> Properties { get; init; } = new Dictionary<string, PropertyModel>();
}
