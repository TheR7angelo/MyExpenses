using BenchmarkDotNet.Attributes;
using NetTopologySuite.IO.Esri.Dbf.Fields;

namespace MyExpenses.Benchmark.IO.Sig.utils;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class FieldCreatorsBenchmark
{
    private static readonly Dictionary<Type, Func<string, int?, int?, DbfField>> FieldCreators = new()
    {
        [typeof(int)] = (name, maxLength, _) => maxLength == null ? new DbfNumericInt32Field(name) : new DbfNumericInt32Field(name, maxLength.Value),
        [typeof(long)] = (name, maxLength, _) => maxLength == null ? new DbfNumericInt64Field(name) : new DbfNumericInt64Field(name, maxLength.Value),
        [typeof(double)] = (name, maxLength, precision) => new DbfFloatField(name, maxLength ?? 19, precision ?? 0),
        [typeof(string)] = (name, maxLength, _) => new DbfCharacterField(name, maxLength ?? 255),
        [typeof(DateTime)] = (name, _, _) => new DbfDateField(name),
        [typeof(bool)] = (name, _, _) => new DbfLogicalField(name)
    };

    private readonly Type[] _testTypes = [typeof(int), typeof(string), typeof(double), typeof(DateTime), typeof(bool)];

    private static DbfField CreateFieldUsingDictionary(Type propertyType, string name, int? maxLength, int? precision)
    {
        if (!FieldCreators.TryGetValue(propertyType, out var fieldCreator))
        {
            throw new ArgumentOutOfRangeException(nameof(propertyType),
                @$"Unsupported type: {propertyType.Name}");
        }

        return fieldCreator(name, maxLength, precision);
    }

    private static DbfField CreateFieldUsingSwitch(Type propertyType, string name, int? maxLength = null, int? precision = null)
    {
        return propertyType switch
        {
            _ when propertyType == typeof(int) =>
                maxLength == null ? new DbfNumericInt32Field(name) : new DbfNumericInt32Field(name, maxLength.Value),

            _ when propertyType == typeof(long) =>
                maxLength == null ? new DbfNumericInt64Field(name) : new DbfNumericInt64Field(name, maxLength.Value),

            _ when propertyType == typeof(double) =>
                new DbfFloatField(name, maxLength ?? 19, precision ?? 0),

            _ when propertyType == typeof(string) =>
                new DbfCharacterField(name, maxLength ?? 255),

            _ when propertyType == typeof(DateTime) => new DbfDateField(name),
            _ when propertyType == typeof(bool) => new DbfLogicalField(name),

            _ => throw new ArgumentOutOfRangeException(nameof(propertyType), @$"Type not supported : {propertyType.Name}")
        };
    }

    [Benchmark]
    public void BenchmarkDictionary()
    {
        foreach (var type in _testTypes)
        {
            _ = CreateFieldUsingDictionary(type, "TestField", 10, 2);
        }
    }

    [Benchmark]
    public void BenchmarkSwitch()
    {
        foreach (var type in _testTypes)
        {
            _ = CreateFieldUsingSwitch(type, "TestField", 10, 2);
        }
    }
}