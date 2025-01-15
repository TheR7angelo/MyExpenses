using System.Reflection;
using BenchmarkDotNet.Attributes;
using MyExpenses.Utils.Objects;

namespace MyExpenses.Benchmark.Utils.Objects
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [AllStatisticsColumn]
    [HtmlExporter]
    [MarkdownExporter]
    [RankColumn]
    public class FieldGetterBenchmark
    {
        private class SampleClass
        {
            public int IntField = 42;
        }

        private static readonly FieldInfo IntFieldInfo = typeof(SampleClass).GetField(nameof(SampleClass.IntField))!;
        private static readonly SampleClass Instance = new();

        private static readonly Func<SampleClass, object?> CachedGetter =
            FieldAccessorCache<SampleClass>.CreateGetter(IntFieldInfo);

        [Benchmark]
        public object? ReflectionGetter()
        {
            return IntFieldInfo.GetValue(Instance);
        }

        [Benchmark]
        public object? CachedGetterBenchmark()
        {
            return CachedGetter(Instance);
        }
    }
}