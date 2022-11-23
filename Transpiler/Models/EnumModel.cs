using System.Numerics;

namespace Transpiler.Models;

public class EnumModel: TypeModel
{
    public string FullName { get; init; }

    public IReadOnlyDictionary<string, BigInteger> Pairs { get; init; }
}

