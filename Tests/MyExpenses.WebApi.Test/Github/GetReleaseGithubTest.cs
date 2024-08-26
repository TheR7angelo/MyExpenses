using MyExpenses.WebApi.GitHub;
using Xunit.Abstractions;

namespace MyExpenses.WebApi.Test.Github;

public class GetReleaseGithubTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task GetRelease()
    {
        var gitHubClient = new GitHubClient();
        var releases = await gitHubClient.GetReleaseNotes("qgis", "QGIS");

        var xmls = new List<string>();
        foreach (var release in releases)
        {
            var version = release.TagName;
            var date = release.PublishedAt;

            var body = release.Body;
            var xml = $"{version}\t\t{date.ToShortDateString()}\n\n{body}";
            xmls.Add(xml);
        }

        var str = string.Join("\n\n___\n\n", xmls);
        testOutputHelper.WriteLine(str);
    }
}