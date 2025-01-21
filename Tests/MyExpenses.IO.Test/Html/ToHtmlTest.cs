using System.Reflection;
using MyExpenses.IO.MarkDown;
using MyExpenses.SharedUtils.Utils;

namespace MyExpenses.IO.Test.Html;

public class ToHtmlTest
{
    [Fact]
    public void HtmlToHtmlTest()
    {
        var executablePath = Assembly.GetExecutingAssembly().Location;
        var directory = executablePath.GetParentDirectory(4);

        var mdPath = Path.Join(directory, "test.md");

        // var content = File.ReadAllText(mdPath);

        const string backgroundColor = "#1e1f22";
        const string foregroundColor = "#bcbec4";

        var fullHtml = mdPath.ToHtml(backgroundColor, foregroundColor);

        var outputFilePath = Path.GetFullPath("test.html");
        File.WriteAllText(outputFilePath, fullHtml);

        outputFilePath.StartProcess();
    }
}