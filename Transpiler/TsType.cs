namespace Transpiler;

using System.Collections.Generic;

public class TsType
{
    public string FullName { get; init; }

    public IReadOnlyDictionary<string, TsProperty> Properties { get; init; }
}
