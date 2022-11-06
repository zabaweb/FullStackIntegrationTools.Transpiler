namespace Transpiler;
public static class Constants
{
    public static readonly IReadOnlyDictionary<string, string> SimpleTypes = new Dictionary<string, string>
    {
      { "System.Boolean", "boolean" },

      //{ "System.DateTime", "Date"},

      { "System.String", "string"},
      { "System.Guid", "string"},

      { "System.Int16", "number"},
      { "System.Int32", "number"},
      { "System.Int64", "number"},
      { "System.UInt16", "number"},
      { "System.UInt32", "number"},
      { "System.UInt64", "number"},
      { "System.Decimal", "number"},
      { "System.Double", "number"},
      { "System.Half", "number"},
      { "System.Single", "number"},
    };


}
