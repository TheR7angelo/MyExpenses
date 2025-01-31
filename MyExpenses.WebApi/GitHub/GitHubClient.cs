using MyExpenses.Models.AutoMapper;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.WebApi.GitHub;

public class GitHubClient : Http
{
    /// <summary>
    /// Retrieves the release notes for a specific GitHub repository.
    /// </summary>
    /// <param name="owner">The owner of the GitHub repository.</param>
    /// <param name="repository">The name of the GitHub repository.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// The task result contains a collection of <see cref="MyExpenses.Models.WebApi.Github.Soft.Release"/> objects representing the release notes.
    /// If the GitHub API request fails or no release notes are found, the result is null.
    /// </returns>
    public static async Task<List<MyExpenses.Models.WebApi.Github.Soft.Release>?> GetReleaseNotes(string owner,
        string repository)
    {
        using var httpClient = GetHttpClient("https://api.github.com/repos/");
        var response = await httpClient.GetAsync($"{owner}/{repository}/releases");

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("GitHub API returned unexpected response\n{ResponseReasonPhrase}", response.ReasonPhrase);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var hardReleases = content.ToObject<List<MyExpenses.Models.WebApi.Github.Hard.Release>>()!
            .OrderByDescending(s => s.PublishedAt);

        var mapper = Mapping.Mapper;
        var softReleases = hardReleases.Select(s => mapper.Map<MyExpenses.Models.WebApi.Github.Soft.Release>(s)).ToList();

        return softReleases;
    }
}