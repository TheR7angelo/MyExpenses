using BenchmarkDotNet.Attributes;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Benchmark.SharedUtils.PropertiesUtils;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class FindItemBenchmark
{
    private const string PropertyName = nameof(TColor.Id);
    private List<TColor> _colors = null!;

    [Params(100, 1000, 10000)]
    public int _collectionSize;

    [GlobalSetup]
    public void Setup()
    {
        _colors = Enumerable.Range(1, _collectionSize)
            .Select(i => new TColor
            {
                Id = i,
                Name = $"Color{i}",
                HexadecimalColorCode = $"#{i:X6}"
            })
            .ToList();
    }

    [Benchmark]
    public TColor? FindUsingLoop()
    {
        return FindItemUsingLoop(_colors, PropertyName, _collectionSize);
    }

    [Benchmark]
    public TColor? FindUsingLinq()
    {
        return FindItemUsingLinq(_colors, _collectionSize);
    }

    private static T? FindItemUsingLoop<T>(IEnumerable<T> collection, string propertyName, int value) where T : class
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property is null) return null;

        foreach (var item in collection)
        {
            var propertyValue = property.GetValue(item);
            if (propertyValue is int intValue && intValue == value)
            {
                return item;
            }
        }
        return null;
    }

    private static TColor? FindItemUsingLinq(IEnumerable<TColor> collection, int value)
    {
        return collection.FirstOrDefault(s => s.Id == value);
    }

}