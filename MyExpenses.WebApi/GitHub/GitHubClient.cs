using MyExpenses.Models.WebApi.Github;
using Newtonsoft.Json;
using Serilog;

namespace MyExpenses.WebApi.GitHub;

public class GitHubClient : Http, IDisposable
{
    private HttpClient HttpClient { get; } = GetHttpClient("https://api.github.com/repos/");

    public async Task<List<Release>?> GetReleaseNotes(string owner, string repository)
    {
        var response = await HttpClient.GetAsync($"{owner}/{repository}/releases");

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("GitHub API returned unexpected response\n{ResponseReasonPhrase}", response.ReasonPhrase);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var releases = JsonConvert.DeserializeObject<List<Release>>(content)!;

        return releases;
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}