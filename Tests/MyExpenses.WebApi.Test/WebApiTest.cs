using Xunit.Abstractions;

namespace MyExpenses.WebApi.Test;

public class WebApiTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    private async Task GithubTest()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Anything");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", "Nope");

        var response = await httpClient.GetAsync("https://api.github.com/repos/TheR7angelo/AutoFiberC6/releases/latest");

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        testOutputHelper.WriteLine(responseBody);
    }
}