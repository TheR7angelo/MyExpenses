using BenchmarkDotNet.Attributes;
using MyExpenses.Models.WebApi.Github.Soft;
using Newtonsoft.Json;

namespace MyExpenses.Benchmark.IO.Markdown;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class ToFileUtilsBenchmark
{
    private List<Release> Releases;

    [GlobalSetup]
    public void Setup()
    {
        Releases = Enumerable.Range(0, 1000).Select(i =>
            new Release
            {
                TagName = $"v1.{i}",
                PublishedAt = DateTime.Now.AddDays(-i),
                Body = $"This is release {i}\nWith multiple lines of description."
            }
        ).ToList();
    }

    [Benchmark]
    public string LinqBasedToMarkDown()
    {
        var markdownSections = Releases.Select(FormatReleaseAsMarkdown);

        const string sectionSeparator = "\n\n___\n\n";
        return string.Join(sectionSeparator, markdownSections);
    }

    private static string FormatReleaseAsMarkdown(Release release)
    {
        var version = release.TagName ?? "Unknown Version";
        var date = release.PublishedAt.ToShortDateString();
        var body = string.Join("\n", release.Body?.Split('\n')
            .Select(line => $"> {line.Trim()}") ?? []);

        return $"# {version}\t\t{date}{Environment.NewLine}{Environment.NewLine}{body}";
    }

    [Benchmark]
    public string LoopBasedToMarkDown()
    {
        var xmls = new List<string>();
        foreach (var release in Releases)
        {
            var version = release.TagName;
            var date = release.PublishedAt;

            var bodies = release.Body?.Split('\n').Select(t => $"> {t}").Select(s => s.Trim())!;

            var body = string.Join("\n", bodies);
            var xml = $"# {version}\t\t{date.ToShortDateString()}{Environment.NewLine}{Environment.NewLine}{body}";

            var json = JsonConvert.SerializeObject(release, Formatting.Indented);
            xml = $"{xml}{Environment.NewLine}<!--{Environment.NewLine}{json}{Environment.NewLine}-->";
            xmls.Add(xml);
        }

        var md = string.Join("\n\n___\n\n", xmls);
        return md;
    }

    [Benchmark]
    public string ToMarkDownHybrid()
    {
        return string.Join(
            "\n\n___\n\n",
            Releases.Select(release => new
                {
                    Version = release.TagName ?? "Unknown",
                    Date = release.PublishedAt.ToShortDateString(),
                    Body = string.Join(
                        "\n",
                        release.Body?.Split('\n').Select(line => $"> {line.Trim()}") ?? Enumerable.Empty<string>()
                    ),
                    JsonComment = JsonConvert.SerializeObject(release, Formatting.Indented)
                })
                .Select(r => $$"""
                                   # {{r.Version}}\t\t{{r.Date}}
                                   
                                   {{r.Body}}
                                   <!--
                                   {{r.JsonComment}}
                                   -->
                               """)
        );
    }
}