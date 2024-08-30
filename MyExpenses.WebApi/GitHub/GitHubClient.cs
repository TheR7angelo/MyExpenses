using MyExpenses.Models.AutoMapper;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.WebApi.GitHub;

public class GitHubClient : Http, IDisposable
{
    private HttpClient HttpClient { get; } = GetHttpClient("https://api.github.com/repos/");

    public async Task<List<MyExpenses.Models.WebApi.Github.Soft.Release>?> GetReleaseNotes(string owner, string repository)
    {
        var response = await HttpClient.GetAsync($"{owner}/{repository}/releases");

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("GitHub API returned unexpected response\n{ResponseReasonPhrase}", response.ReasonPhrase);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var hardReleases = content.ToObject<List<MyExpenses.Models.WebApi.Github.Hard.Release>>()!;

        var mapper = Mapping.Mapper;
        var softReleases = hardReleases.Select(s => mapper.Map<MyExpenses.Models.WebApi.Github.Soft.Release>(s)).ToList();

        return softReleases;
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}