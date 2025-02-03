using System.Reflection;
using BenchmarkDotNet.Attributes;
using MyExpenses.SharedUtils.Objects;

namespace MyExpenses.Benchmark.Utils.Objects;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class PropertySetterBenchmark
{
    private class SampleClass
    {
        public int IntProperty { get; set; }
    }

    private static readonly PropertyInfo IntPropertyInfo = typeof(SampleClass).GetProperty(nameof(SampleClass.IntProperty))!;
    private static readonly SampleClass Instance = new() { IntProperty = 0 };

    private static readonly Action<SampleClass, object?> CachedSetter =
        PropertyAccessorCache<SampleClass>.CreateSetter(IntPropertyInfo);

    [Benchmark]
    public void ReflectionSetter()
    {
        // Modification de la propriété via réflexion (SetValue)
        IntPropertyInfo.SetValue(Instance, 99);
    }

    [Benchmark]
    public void CachedSetterBenchmark()
    {
        CachedSetter(Instance, 99);
    }
}