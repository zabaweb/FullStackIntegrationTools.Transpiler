namespace Transpiler;

using System.Collections.Generic;
using System.Numerics;
using Transpiler.Helpers;
using Transpiler.Models;

public class NaiveParser
{
    public TypeModel[] Parse(EndpointModel[] models)
    {
        var typesToParse = models
           .SelectMany(x => x.Methods)
           .SelectMany(x => x.Parameters.Append(x.ReturnType))
           .Select(x => Unpack(x).type)
           .ToList();

        var paresdTypes = new Dictionary<string, TypeModel>();
        for(int tIndex = 0; tIndex < typesToParse.Count; tIndex++)
        {
            var (type, parameterNesting, isParameterNullable) = Unpack(typesToParse[tIndex]);

            if(paresdTypes.ContainsKey(type.FullNameSupress()))
            {
                continue;
            }

            if(type.IsEnum)
            {
                var enumModel = new EnumModel
                {
                    FullName = type.FullNameSupress(),
                    Pairs = ParseEnumDictionary(type),

                };
                paresdTypes.Add(type.FullNameSupress(), enumModel);
                continue;
            }

            if(Constants.SimpleTypes.ContainsKey(type.FullNameSupress()))
            {
                continue;
            }

            if(type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if(paresdTypes.ContainsKey(genericType.FullNameSupress()))
                {
                    continue;
                }

                foreach(var genericArgument in type.GetGenericArguments())
                {
                    if(!paresdTypes.ContainsKey(genericArgument.Name))
                    {
                        typesToParse.Add(genericArgument);
                    }
                }
                type = genericType;
            }

            var properties = new Dictionary<string, PropertyModel>();

            foreach(var prop in type.GetProperties())
            {
                var (propertyType, nesting, isNullable) = Unpack(prop.PropertyType);

                var isGeneric = type.GetGenericArguments().Select(x => x.Name).Contains(propertyType.Name);
                if(!paresdTypes.ContainsKey(propertyType.FullNameSupress()) &&
                    !isGeneric)
                {
                    typesToParse.Add(propertyType);
                }

                properties.Add(
                    prop.Name,
                    new PropertyModel
                    {
                        ArrayNesting = nesting,
                        TypeFullName = propertyType.FullNameSupress(),
                        TypeCategory = propertyType.IsEnum ? TypeCategory.Enum : TypeCategory.Class,
                        IsNullable = isNullable,
                        IsGeneric = isGeneric,
                    });
            }

            var tsType = new ClassModel
            {
                FullName = GetClassName(type),
                Properties = properties
            };

            paresdTypes.Add(type.FullNameSupress(), tsType);
        }


        return paresdTypes.Values.ToArray();
    }

    private static Dictionary<string, BigInteger> ParseEnumDictionary(Type type)
    {
        var underlyingType = type.GetEnumUnderlyingType().Name;
        Func<string, BigInteger> mapper = underlyingType switch
        {
            "SByte" => name => new BigInteger((sbyte)Enum.Parse(type, name)),
            "Byte" => name => new BigInteger((byte)Enum.Parse(type, name)),
            "Int16" => name => new BigInteger((short)Enum.Parse(type, name)),
            "Int32" => name => new BigInteger((int)Enum.Parse(type, name)),
            "Int64" => name => new BigInteger((long)Enum.Parse(type, name)),
            "UInt16" => name => new BigInteger((ushort)Enum.Parse(type, name)),
            "UInt32" => name => new BigInteger((uint)Enum.Parse(type, name)),
            "UInt64" => name => new BigInteger((ulong)Enum.Parse(type, name)),
            _ => throw new Exception("6")
        };
        var result = Enum.GetNames(type).ToDictionary(name => name, name => (mapper(name)));
        return result;
    }

    private static string GetClassName(Type type)
    {
        var genericArguments = type.GetGenericArguments().Select(x => x.Name);
        var genericPostfix = type.IsGenericType ? $"<{string.Join(", ", genericArguments)}>" : "";
        return $"{type.FullNameSupress()}{genericPostfix}";
    }

    private (Type type, uint nesting, bool isNullable) Unpack(Type type)
    {
        var isNullable = false;
        var temp = type;
        for(var i = 0u; i <= 10; i++)
        {
            if(temp == null)
            {
                throw new Exception("7");
            }

            if(temp.Name == typeof(Nullable<>).Name)
            {
                isNullable = true;
                temp = temp.GetGenericArguments()[0];
            }

            if(temp.IsGenericType &&
                (
                temp.Name == typeof(IEnumerable<>).Name ||
                temp.GetInterfaces().Any(j => j.Name == typeof(IEnumerable<>).Name)))
            {
                temp = temp.GenericTypeArguments[0];
            }
            else if(temp.IsArray)
            {
                temp = temp.GetElementType();
            }
            else
            {
                return (temp, i, isNullable && i == 0);
            }
        }

        throw new Exception("1");
    }
}
