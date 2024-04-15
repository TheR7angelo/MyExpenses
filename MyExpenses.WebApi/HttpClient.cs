namespace MyExpenses.WebApi;

public abstract class Http
{
    protected static HttpClient GetHttpClient(string baseUrl, string? userAgent=null)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl)};
        if (!string.IsNullOrEmpty(userAgent))
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }
        return httpClient;
    }

    protected static string ParseToUrlFormat(string str) => str.Replace(" ", "+");
}