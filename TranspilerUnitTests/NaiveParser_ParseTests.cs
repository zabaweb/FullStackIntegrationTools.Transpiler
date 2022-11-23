using System.Numerics;
using SimpleWebApi;
using Transpiler;
using Transpiler.Helpers;
using Transpiler.Models;

namespace TranspilerIntegrationTests;

public class NaiveParser_ParseTests
{
    [Fact]
    public void ForSimpleWebApi()
    {
        var analyzer = new NaiveParser();
        var input = new[]{
            new MethodModel{
                Parameters = new Type[]{typeof(int) },
                ReturnType = typeof(bool),
            },
            new MethodModel{
                Parameters = new Type[]{ },
                ReturnType = typeof(IEnumerable<WeatherForecast>),
            },
            new MethodModel{
                Parameters = new Type[]{ typeof(string) },
                ReturnType = typeof(WeatherForecast),
            }
        };

        var result = analyzer.Parse(AsEndps(input));

        var expectedResult = new TypeModel[] {
            new ClassModel{
                FullName = "SimpleWebApi.WeatherForecast",
                Properties = new Dictionary<string,PropertyModel> {
                    { "TemperatureC", new PropertyModel { TypeFullName = typeof(int).FullNameSupress(), TypeCategory = TypeCategory.Class } },
                    { "Summary", new PropertyModel { TypeFullName = typeof(WeatherForecastSummary).FullNameSupress(), TypeCategory = TypeCategory.Class } },
                }
            },
            new ClassModel{
                FullName = "SimpleWebApi.Nesting.MoreNesting.NestedClass",
                Properties = new Dictionary<string,PropertyModel>()
            },
            new ClassModel
            {
                FullName = "SimpleWebApi.WeatherForecastSummary",
                Properties = new Dictionary<string, PropertyModel> {
                    { "OneProperty", new PropertyModel { TypeFullName = typeof(string).FullNameSupress(), TypeCategory = TypeCategory.Class } },
                    { "TwoProperty", new PropertyModel { TypeFullName = typeof(bool).FullNameSupress(), TypeCategory = TypeCategory.Class } },
                    { "ThreeProperty", new PropertyModel { TypeFullName = typeof(string).FullNameSupress(), ArrayNesting = 2, TypeCategory = TypeCategory.Class } },
                    { "SomeDecimal", new PropertyModel { TypeFullName = typeof(decimal).FullNameSupress(), ArrayNesting = 2, TypeCategory = TypeCategory.Class } },
                    { "NestingTest", new PropertyModel { TypeFullName = "SimpleWebApi.Nesting.MoreNesting.NestedClass", ArrayNesting = 0, TypeCategory = TypeCategory.Class } },
                }
            },
            };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ForListType()
    {
        var analyzer = new NaiveParser();
        var input = new MethodModel
        {
            Parameters = new Type[] { typeof(List<ListClass>) },
            ReturnType = typeof(bool),
        };

        var result = analyzer.Parse(AsEndps(input));

        var expectedResult = new TypeModel[] {
            new ClassModel{
                FullName = "TranspilerIntegrationTests.ListClass",
                Properties = new Dictionary<string,PropertyModel> {
                    { "ListProperty", new PropertyModel { TypeFullName = "TranspilerIntegrationTests.ListClass", TypeCategory = TypeCategory.Class, ArrayNesting = 1 } },
                }
            },
            };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ForNullableType()
    {
        var analyzer = new NaiveParser();
        var input = new MethodModel
        {
            Parameters = new Type[] { typeof(NullableClass) },
            ReturnType = typeof(bool),
        };

        var result = analyzer.Parse(AsEndps(input));

        var expectedResult = new TypeModel[] {
            new ClassModel{
                FullName = "TranspilerIntegrationTests.NullableClass",
                Properties = new Dictionary<string,PropertyModel> {
                    { "NullableProperty", new PropertyModel { TypeFullName = "System.Int32", TypeCategory = TypeCategory.Class, ArrayNesting = 0, IsNullable = true } },
                }
            },
            };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ForGenericType()
    {
        var analyzer = new NaiveParser();
        var input = new MethodModel
        {
            Parameters = new Type[] { },
            ReturnType = typeof(GenericClass3<string, int, bool>),
        };

        var result = analyzer.Parse(AsEndps(input));

        var expectedResult = new TypeModel[] {
            new ClassModel{
                FullName = "TranspilerIntegrationTests.GenericClass3<T1, T2, T3>",
                Properties = new Dictionary<string,PropertyModel> {
                    { "Type1", new PropertyModel { TypeFullName = "T1", TypeCategory = TypeCategory.Class } },
                    { "Type2", new PropertyModel { TypeFullName = "T2", TypeCategory = TypeCategory.Class } },
                    { "Type3", new PropertyModel { TypeFullName = "T3", TypeCategory = TypeCategory.Class } },
                }
            } };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ForEnum()
    {
        var analyzer = new NaiveParser();
        var input = new MethodModel
        {
            Parameters = new Type[0],
            ReturnType = typeof(TestEnum),
        };

        var result = analyzer.Parse(AsEndps(input));

        var expectedResult = new TypeModel[] {
            new EnumModel{
                FullName = "TranspilerIntegrationTests.TestEnum",
                Pairs = new Dictionary<string,BigInteger> {
                    { "Value1", 1},
                    { "Value2", 2},
                    { "Value4", 4},
                }
            } };

        result.Should().BeEquivalentTo(expectedResult);
        result.First().Should().BeAssignableTo<EnumModel>();
        var givenEnumModel = result.First() as EnumModel;

        (givenEnumModel?.Pairs.Keys).Should().BeEquivalentTo(new[] { 1, 2, 4 }.Select(x => $"Value{x}"));
        (givenEnumModel?.Pairs.Values).Should().BeEquivalentTo(new[] { 1, 2, 4 }.Select(x => new BigInteger(x)));
    }

    private EndpointModel[] AsEndps(MethodModel[] methods) => new[] { new EndpointModel { Methods = methods } };
    private EndpointModel[] AsEndps(MethodModel method) => AsEndps(new[] { method });
}

enum TestEnum
{
    Value1 = 1,
    Value2 = 2,
    Value4 = 4,
}

enum SameValueForDifferentLabeslTestEnum
{
    Value1 = 1,
    Value_1 = 1,
}
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

enum ULongTestEnum: ulong { Value1 = 1 }
enum LongTestEnum: long { Value1 = 1 }
enum IntTestEnum: int { Value1 = 1 }
enum UIntTestEnum: uint { Value1 = 1 }
enum ShortTestEnum: short { Value1 = 1 }
enum UShortTestEnum: ushort { Value1 = 1 }
enum ByteTestEnum: byte { Value1 = 1 }
enum SByteTestEnum: sbyte { Value1 = 1 }

class ListClass { public List<ListClass> ListProperty { get; set; } }
class NullableClass { public int? NullableProperty { get; set; } }
class GenericClass3<T1, T2, T3>
{
    public T1 Type1 { get; set; }
    public T2 Type2 { get; set; }
    public T3 Type3 { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
