namespace MyExpenses.WebApi;

public class HttpSimpleClient : Http
{
    private static readonly HttpClient HttpClient = GetHttpClient();

    public static async Task<bool> HasInternetConnectionAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync("https://google.com");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}