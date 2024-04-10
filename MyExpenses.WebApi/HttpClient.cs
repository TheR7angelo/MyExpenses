namespace MyExpenses.WebApi;

public abstract class Http
{
    public static HttpClient GetHttpClient(string userAgent, string baseUrl)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl)};
        httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        return httpClient;
    }
    
    public static string ParseToUrlFormat(string str) => str.Replace(" ", "+");
}