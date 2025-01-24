using BenchmarkDotNet.Attributes;

namespace MyExpenses.Benchmark.Utils.Lists;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]

public class ListAllocationBenchmark
{
    private readonly string[] _filePaths = ["blue.svg", "green.svg", "red.svg"];

    [Benchmark]
    public void UsingArray()
    {
        var svgs = new[] { _filePaths[0], _filePaths[1], _filePaths[2] };
        foreach (var svg in svgs)
        {
            var filename = Path.GetFileName(svg);
        }
    }

    [Benchmark]
    public void UsingList()
    {
        var svgs = new List<string> { _filePaths[0], _filePaths[1], _filePaths[2] };
        foreach (var svg in svgs)
        {
            var filename = Path.GetFileName(svg);
        }
    }

    [Benchmark]
    public void UsingSpan()
    {
        Span<string> svgs = [_filePaths[0], _filePaths[1], _filePaths[2]];
        foreach (var svg in svgs)
        {
            var filename = Path.GetFileName(svg);
        }
    }

    [Benchmark]
    public void UsingReadOnlySpan()
    {
        ReadOnlySpan<string> svgs = [_filePaths[0], _filePaths[1], _filePaths[2]];
        foreach (var svg in svgs)
        {
            var filename = Path.GetFileName(svg);
        }
    }
}
