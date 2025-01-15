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
    public class FieldSetterBenchmark
    {
        private class SampleClass
        {
            public int IntField;
        }

        private static readonly FieldInfo IntFieldInfo = typeof(SampleClass).GetField(nameof(SampleClass.IntField))!;
        private static readonly SampleClass Instance = new();

        private static readonly Action<SampleClass, object?> CachedSetter =
            FieldAccessorCache<SampleClass>.CreateSetter(IntFieldInfo);

        [Benchmark]
        public void ReflectionSetter()
        {
            IntFieldInfo.SetValue(Instance, 99);
        }

        [Benchmark]
        public void CachedSetterBenchmark()
        {
            CachedSetter(Instance, 99);
        }
    }
}