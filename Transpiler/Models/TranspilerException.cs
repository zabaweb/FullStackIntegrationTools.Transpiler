namespace Transpiler.Models;
public class TranspilerException: Exception
{
    public ushort ErrorCode { get; }

    public string Description { get; }

    public TranspilerException(ushort errorCode, string description)
    {
        Description = description;
        ErrorCode = errorCode;
    }
}
