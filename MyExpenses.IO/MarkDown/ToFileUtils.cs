using Markdig;
using MyExpenses.Models.WebApi.Github;

namespace MyExpenses.IO.MarkDown;

public static class ToFileUtils
{
    public static string ToMarkDown(this List<Release> releases)
    {
        var xmls = new List<string>();
        foreach (var release in releases!)
        {
            var version = release.TagName;
            var date = release.PublishedAt;

            var bodies = release.Body?.Split('\n').Select(t => $"> {t}").Select(s => s.Trim())!;

            var body = string.Join("\n", bodies);
            var xml = $"# {version}\t\t{date.ToShortDateString()}\n\n{body}";
            xmls.Add(xml);
        }

        var md = string.Join("\n\n___\n\n", xmls);
        return md;
    }

    public static string ToHtml(this string file, string backgroundColor, string foregroundColor)
    {
        var content = File.Exists(file) ? File.ReadAllText(file) : file;

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