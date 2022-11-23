namespace Transpiler.Models;

public class PropertyModel
{
    public uint ArrayNesting { get; init; }

    public bool IsGeneric { get; init; }

    public string TypeFullName { get; init; } = "";

    public TypeCategory TypeCategory { get; init; }

    public bool IsNullable { get; set; }
}
