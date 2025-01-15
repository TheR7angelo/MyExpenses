using System.Reflection;
using BenchmarkDotNet.Attributes;
using MyExpenses.Utils.Objects;

namespace MyExpenses.Benchmark.Utils.Objects;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class PropertyGetterBenchmark
{
    private class SampleClass
    {
        public int IntProperty { get; set; } = 42;
    }

    private static readonly PropertyInfo IntPropertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.IntProperty))!;
    private static readonly SampleClass Instance = new() { IntProperty = 42 };

    private static readonly Func<SampleClass, object?> CachedGetter =
        PropertyAccessorCache<SampleClass>.CreateGetter(IntPropertyInfo);

    [Benchmark]
    public object? ReflectionGetter()
    {
        return IntPropertyInfo.GetValue(Instance);
    }

    [Benchmark]
    public object? CachedGetterBenchmark()
    {
        return CachedGetter(Instance);
    }
}