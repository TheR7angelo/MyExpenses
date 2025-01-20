using System.Text;
using BenchmarkDotNet.Attributes;

namespace MyExpenses.Benchmark.Models.Sql.Bases.Tables;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class PlaceBenchmark
{
    private const string? Number = "123";
    private const string? Street = "Main Street";
    private const string? Postal = null;
    private const string? City = "Springfield";
    private const string? Country = "USA";

    [Benchmark]
    public string UsingList()
    {
        var partAddress = new List<string>();

        if (!string.IsNullOrEmpty(Number)) partAddress.Add(Number!);
        if (!string.IsNullOrEmpty(Street)) partAddress.Add(Street!);
        if (!string.IsNullOrEmpty(Postal)) partAddress.Add(Postal!);
        if (!string.IsNullOrEmpty(City)) partAddress.Add(City!);
        if (!string.IsNullOrEmpty(Country)) partAddress.Add(Country!);

        return partAddress.Count > 0 ? string.Join(", ", partAddress) : string.Empty;
    }

    [Benchmark]
    public string UsingStringBuilder()
    {
        var builder = new StringBuilder();

        AppendPart(builder, Number);
        AppendPart(builder, Street);
        AppendPart(builder, Postal);
        AppendPart(builder, City);
        AppendPart(builder, Country);

        return builder.ToString();
    }

    private void AppendPart(StringBuilder builder, string? part)
    {
        if (string.IsNullOrEmpty(part)) return;
        if (builder.Length > 0) builder.Append(", ");
        builder.Append(part);
    }

    [Benchmark]
    public string UsingLinq()
    {
        var parts = new[] { Number, Street, Postal, City, Country }
            .Where(part => !string.IsNullOrEmpty(part));

        return string.Join(", ", parts);
    }
}