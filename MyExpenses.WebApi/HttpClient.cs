namespace MyExpenses.WebApi;

public abstract class Http
{
    protected internal static HttpClient GetHttpClient(string baseUrl, string? userAgent=null)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl)};
        userAgent ??= "Test";
        httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        return httpClient;
    }

    protected internal static string ParseToUrlFormat(string str) => str.Replace(" ", "+");
}