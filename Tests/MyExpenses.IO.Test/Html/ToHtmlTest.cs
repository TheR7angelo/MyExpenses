using System.Reflection;
using MyExpenses.IO.MarkDown;
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

        // var content = File.ReadAllText(mdPath);

        const string backgroundColor = "#1e1f22";
        const string foregroundColor = "#bcbec4";
        const string huePrimaryColor = "#FFFFFF";

        var fullHtml = mdPath.ToHtml(backgroundColor, foregroundColor, huePrimaryColor);

        var outputFilePath = Path.GetFullPath("test.html");
        File.WriteAllText(outputFilePath, fullHtml);

        outputFilePath.StartFile();
    }
}