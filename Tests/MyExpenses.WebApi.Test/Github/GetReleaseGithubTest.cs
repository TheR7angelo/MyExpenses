using MyExpenses.Models.WebApi.Github;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace MyExpenses.WebApi.Test.Github;

public class GetReleaseGithubTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task GetRelease()
    {
        const string url = "https://api.github.com/repos/ravibpatel/AutoUpdater.NET/releases";

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");
        var response = await httpClient.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();
        var releases = JsonConvert.DeserializeObject<List<Release>>(content)!;

        foreach (var release in releases)
        {
            var releaseName = release.Name;
            var body = release.Body;

            testOutputHelper.WriteLine($"Release: {releaseName}");
            testOutputHelper.WriteLine($"Notes de release:\n{body}");
            testOutputHelper.WriteLine("==================================");
        }
    }
}