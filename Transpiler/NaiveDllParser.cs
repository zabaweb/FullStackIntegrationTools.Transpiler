namespace Transpiler;

using System.Collections.Generic;

public class NaiveDllParser
{
    public static readonly Dictionary<string, Type> SimpleTypes = new Type[] {
        typeof(string),
        typeof(int),
        typeof(bool),
    }.ToDictionary(x => x.FullName ?? throw new Exception("2"), x => x);

    public TsType[] Parse(ControllerModel[] models)
    {
        var typesToParse = models
           .SelectMany(x => x.Methods)
           .SelectMany(x => x.Parameters.Append(x.ReturnType))
           .Select(x => UnpackArray(x).type)
           .ToList();

        var paresdTypes = new Dictionary<string, TsType>();
        for(int tIndex = 0; tIndex < typesToParse.Count; tIndex++)
        {
            var type = typesToParse[tIndex];
            if(paresdTypes.ContainsKey(type.FullNameSupress()))
            {
                continue;
            }

            if(SimpleTypes.ContainsKey(type.FullNameSupress()))
            {
                continue;
            }

            var properties = new Dictionary<string, TsProperty>();

            foreach(var prop in type.GetProperties())
            {
                var (propertyType, nesting) = UnpackArray(prop.PropertyType);

                if(!paresdTypes.ContainsKey(propertyType.FullNameSupress()))
                {
                    typesToParse.Add(propertyType);
                }

                properties.Add(
                    prop.Name,
                    new TsProperty
                    {
                        ArrayNesting = nesting,
                        TypeFullName = propertyType.FullNameSupress()
                    });
            }

            var tsType = new TsType
            {
                FullName = type.FullNameSupress(),
                Properties = properties
            };

            paresdTypes.Add(type.FullNameSupress(), tsType);
        }


        return paresdTypes.Values.ToArray();
    }

    private (Type type, uint nesting) UnpackArray(Type type)
    {
        var temp = type;
        for(var i = 0u; i <= 10; i++)
        {
            if(temp.IsGenericType && temp.GetGenericTypeDefinition().IsAssignableTo(typeof(IEnumerable<>)))
            {
                temp = temp.GenericTypeArguments[0];
            }
            else if(temp.IsArray)
            {
                temp = temp.GetElementType();
            }
            else
            {
                return (temp, i);
            }
        }

        throw new Exception("1");
    }
}
