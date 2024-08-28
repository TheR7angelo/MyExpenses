using System.Reflection;
using MyExpenses.Utils;
using MyExpenses.WebApi.GitHub;
using Xunit.Abstractions;

namespace MyExpenses.WebApi.Test.Github;

public class GetReleaseGithubTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task GetRelease()
    {
        var gitHubClient = new GitHubClient();
        var releases = await gitHubClient.GetReleaseNotes("microsoft", "PowerToys");

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

        var executablePath = Assembly.GetExecutingAssembly().Location;
        var directory = executablePath.GetParentDirectory(6);
        directory = Path.Join(directory, "Tests", "MyExpenses.IO.Test", "test.md");
        await File.WriteAllTextAsync(directory, md);

        testOutputHelper.WriteLine(md);
    }
}