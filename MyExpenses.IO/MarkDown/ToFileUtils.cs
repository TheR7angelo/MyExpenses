using Markdig;
using MyExpenses.Models.WebApi.Github.Soft;

namespace MyExpenses.IO.MarkDown;

public static class ToFileUtils
{
    /// <summary>
    /// Converts a list of GitHub release notes into a Markdown formatted string.
    /// </summary>
    /// <param name="releases">The list of release notes to be formatted in Markdown.</param>
    /// <returns>A single string containing all the release notes formatted in Markdown, separated by section dividers.</returns>
    public static string ToMarkDown(this List<Release> releases)
    {
        var markdownSections = releases.Select(FormatReleaseAsMarkdown);

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

    public static string ToHtml(this string file, string backgroundColor, string foregroundColor)
    {
        var content = File.Exists(file) ? File.ReadAllText(file) : file;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The MarkdownPipeline instance is intentionally created here as it is needed
        // only once during the application lifecycle. Using a static reference would
        // cause unnecessary memory retention, so this allocation is acceptable and
        // will be collected by the Garbage Collector shortly after use.
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        var htmlBody = Markdown.ToHtml(content, pipeline);

        var fullHtml = $$"""
                         <html>
                            <head>
                                <meta charset="UTF-8">
                                <style>
                         
                                    :root{
                                        --backgroundColor: {{backgroundColor}};
                                        --foregroundColor: {{foregroundColor}};
                                    }
                              
                                    ::-webkit-scrollbar {
                                        width: 10px;
                                    }
                              
                                    ::-webkit-scrollbar-track {
                                        background: var(--backgroundColor);
                                    }
                              
                                    ::-webkit-scrollbar-thumb {
                                        background: var(--foregroundColor);
                                    }
                              
                                    ::-webkit-scrollbar-thumb:hover {
                                        background: var(--foregroundColor);
                                    }
                         
                                    body{
                                        background-color: var(--backgroundColor);
                                        color: var(--foregroundColor);
                                    }
                         
                                    blockquote{
                                        box-shadow: -5px 0 0 rgba(94, 158, 255, 100%);
                                        padding: 5px;
                                        background-color: var(--backgroundColor);
                                        color: var(--foregroundColor);
                                        border-radius: 5px;
                                        border-top: 2px solid var(--foregroundColor);
                                        border-right: 2px solid var(--foregroundColor);
                                        border-bottom: 2px solid var(--foregroundColor);
                                        font-size: 12px;
                                    }
                                    
                                    table {
                                         border-collapse: collapse;
                                         width: 100%;
                                     }
                                     th, td {
                                         border: 2px solid var(--foregroundColor);
                                         text-align: left;
                                         padding: 8px;
                                     }
                                     th {
                                         background-color: var(--backgroundColor); /* Fond de l'en-tête du tableau */
                                         color: var(--foregroundColor); /* Couleur du texte de l'en-tête */
                                     }
                                </style>
                            </head>
                            <body>
                             {{htmlBody}}
                            </body>
                         </html>
                         """;

        return fullHtml;
    }
}