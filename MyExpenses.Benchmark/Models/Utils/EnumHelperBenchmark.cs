using BenchmarkDotNet.Attributes;
using MyExpenses.Models.Utils;

namespace MyExpenses.Benchmark.Models.Utils;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class EnumHelperBenchmark
{
    private const MyTestEnum TestEnum = MyTestEnum.Value5;

    [Benchmark]
    public string UsingEnumHelperToEnumString()
    {
        return EnumHelper<MyTestEnum>.ToEnumString(TestEnum);
    }

    [Benchmark]
    public string UsingEnumGetName()
    {
        return Enum.GetName(typeof(MyTestEnum), TestEnum)!;
    }

    [Benchmark]
    public string UsingEnumGetNameOverload()
    {
        return Enum.GetName(TestEnum)!;
    }

    [Benchmark]
    public string UsingToString()
    {
        return TestEnum.ToString();
    }
}

public enum MyTestEnum
{
    Value1,
    Value2,
    Value3,
    Value4,
    Value5 = 100000000
}