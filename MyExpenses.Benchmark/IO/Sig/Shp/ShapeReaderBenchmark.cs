using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace MyExpenses.Benchmark.IO.Sig.Shp;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class ShapeReaderBenchmark
{
    private readonly Person _instance = new();
    private readonly PropertyInfo _ageProperty = typeof(Person).GetProperty(nameof(Person.Age))!;
    private readonly PropertyInfo _nameProperty = typeof(Person).GetProperty(nameof(Person.Name))!;

    private readonly Action<Person, object> _compiledSetter;
    private const int TestAge = 30;
    private const string TestName = "Alice";

    public ShapeReaderBenchmark()
    {
        _compiledSetter = PropertySetterCache<Person>.CreateSetter(_ageProperty);
    }

    [Benchmark(Baseline = true)]
    public void OldSetValue_Age()
    {
        _ageProperty.SetValue(_instance, TestAge);
    }

    [Benchmark]
    public void NewCompileSetter_Age()
    {
        _compiledSetter(_instance, TestAge);
    }

    [Benchmark]
    public void OldSetValue_Name()
    {
        _nameProperty.SetValue(_instance, TestName);
    }

    [Benchmark]
    public void NewCompileSetter_Name()
    {
        _compiledSetter(_instance, TestName);
    }
}

public static class PropertySetterCache<T>
{
    private static readonly Dictionary<string, Action<T, object>> SettersCache = new();

    public static Action<T, object> CreateSetter(PropertyInfo property)
    {
        if (SettersCache.TryGetValue(property.Name, out var cachedSetter))
            return cachedSetter;

        // On compile un setter s'il n'existe pas dans le cache
        var instanceParam = Expression.Parameter(typeof(T), "instance");
        var valueParam = Expression.Parameter(typeof(object), "value");

        var castValue = Expression.Convert(valueParam, property.PropertyType); // Conversion vers le type attendu
        var propertyExpr = Expression.Property(instanceParam, property);
        var assignExpr = Expression.Assign(propertyExpr, castValue);

        // Compile le setter
        var setter = Expression.Lambda<Action<T, object>>(assignExpr, instanceParam, valueParam).Compile();
        SettersCache[property.Name] = setter;

        return setter;
    }
}

public class Person
{
    public int Age { get; set; }
    public string Name { get; set; } = string.Empty;
}