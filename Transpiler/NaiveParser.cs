namespace Transpiler;

using System.Collections.Generic;
using Transpiler.Helpers;
using Transpiler.Models;

public class NaiveParser
{
    public static readonly Dictionary<string, Type> SimpleTypes = new Type[] {
        typeof(string),
        typeof(int),
        typeof(bool),
    }.ToDictionary(x => x.FullName ?? throw new Exception("2"), x => x);

    public TypeModel[] Parse(EndpointModel[] models)
    {
        var typesToParse = models
           .SelectMany(x => x.Methods)
           .SelectMany(x => x.Parameters.Append(x.ReturnType))
           .Select(x => UnpackArray(x).type)
           .ToList();

        var paresdTypes = new Dictionary<string, TypeModel>();
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

            var properties = new Dictionary<string, PropertyModel>();

            foreach(var prop in type.GetProperties())
            {
                var (propertyType, nesting) = UnpackArray(prop.PropertyType);

                if(!paresdTypes.ContainsKey(propertyType.FullNameSupress()))
                {
                    typesToParse.Add(propertyType);
                }

                properties.Add(
                    prop.Name,
                    new PropertyModel
                    {
                        ArrayNesting = nesting,
                        TypeFullName = propertyType.FullNameSupress()
                    });
            }

            var tsType = new TypeModel
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
