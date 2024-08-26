using System.Reflection;
using Markdig;
using MyExpenses.Utils;

namespace MyExpenses.IO.Test.Html;

public class ToHtmlTest
{
    [Fact]
    public void HtmlToHtmlTest()
    {
        var executablePath = Assembly.GetExecutingAssembly().Location;
        var directory = executablePath.GetParentDirectory(6);

        var mdPath = Path.Join(directory, "TODO LIST.md");

        var content = File.ReadAllText(mdPath);

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        var htmlText = Markdown.ToHtml(content, pipeline);

        const string backgroundColor = "#1e1f22";
        const string foregroundColor = "#bcbec4";
        const string huePrimaryColor = "#FFFFFF";

        var fullHtml = $$"""
                         <html>
                         <head>
                             <meta charset="UTF-8">
                             <style>
                         
                            :root{
                                  --backgroundColor: {{backgroundColor}};
                                  --foregroundColor: {{foregroundColor}};
                                  --huePrimaryColor: {{huePrimaryColor}};
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
                              }
                         
                              blockquote{
                                  box-shadow: -5px 0 0 rgba(94, 158, 255, 100%);
                                  padding: 5px;
                                  background-color: background-color: var(--backgroundColor);
                                  color: var(--foregroundColor);
                                  border-radius: 5px;
                                  border-top: 2px solid var(--huePrimaryColor);
                                  border-right: 2px solid var(--huePrimaryColor);
                                  border-bottom: 2px solid var(--huePrimaryColor);
                                  font-size: 12px;
                              }
                          </style>
                         </head>
                         <body>
                             {{htmlText}}
                         </body>
                         </html>
                         """;

        File.WriteAllText("test.html", fullHtml);
    }
}