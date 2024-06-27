namespace MyExpenses.WebApi;

public abstract class Http
{
    protected internal static HttpClient GetHttpClient(string? baseUrl=null, string? userAgent=null)
    {
        var httpClient = new HttpClient();
        if (!string.IsNullOrWhiteSpace(baseUrl)) httpClient.BaseAddress = new Uri(baseUrl);

        userAgent ??= Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        return httpClient;
    }

    protected internal static string ParseToUrlFormat(string str) => str.Replace(" ", "+");
}