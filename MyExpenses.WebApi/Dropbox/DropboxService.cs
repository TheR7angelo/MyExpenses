using System.Diagnostics;
using System.Net;
using System.Reflection;
using MyExpenses.Models.WebApi.DropBox;
using Newtonsoft.Json;

namespace MyExpenses.WebApi.Dropbox;

public class DropboxService
{
    public AccessTokenAuthentication? AccessTokenAuthentication { get; set; }

    private DropboxKeys DropboxKeys { get; }

    private string FilePathSecretKeys { get; }

    public DropboxService()
    {
        DropboxKeys = GetDropboxKeys();

        var directorySecretKeys = GenerateDirectorySecretKeys();
        FilePathSecretKeys = Path.Join(directorySecretKeys, "AccessTokenAuthentication.json");

        if (!File.Exists(FilePathSecretKeys)) return;

        var jsonStr = File.ReadAllText(FilePathSecretKeys);
        AccessTokenAuthentication = JsonConvert.DeserializeObject<AccessTokenAuthentication>(jsonStr);
    }

    public async Task RefreshAccessTokenAuthentication()
    {
        using var httpClient = Http.GetHttpClient();
        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", AccessTokenAuthentication!.RefreshToken! },
            { "client_id", DropboxKeys.AppKey! },
            { "client_secret", DropboxKeys.AppSecret! }
        };

        var requestContent = new FormUrlEncodedContent(requestData);
        var response = await httpClient.PostAsync("https://api.dropbox.com/oauth2/token", requestContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenAuthentication>(responseContent);
        AccessTokenAuthentication.AccessToken = accessTokenResponse!.AccessToken;
        AccessTokenAuthentication.ExpiresIn = accessTokenResponse.ExpiresIn;
        AccessTokenAuthentication.TokenType = accessTokenResponse.TokenType;

        var now = DateTime.Now;
        AccessTokenAuthentication.DateCreated = now;
        AccessTokenAuthentication.DateExpiration = now.AddSeconds(accessTokenResponse.ExpiresIn ?? 0);

        await File.WriteAllTextAsync(FilePathSecretKeys, JsonConvert.SerializeObject(AccessTokenAuthentication, Formatting.Indented));
    }

    public void AuthorizeApplication()
    {
        var tempToken = GetTempToken()!;
        var taskAccessTokenAuthentication = GetAccessTokenAuthentication(tempToken);
        var accessTokenAuthentication = taskAccessTokenAuthentication.GetAwaiter().GetResult();

        if (accessTokenAuthentication is not null)
        {
            accessTokenAuthentication.DateCreated = DateTime.Now;
            accessTokenAuthentication.DateExpiration = DateTime.Now.AddSeconds(accessTokenAuthentication.ExpiresIn ?? 0);
        }

        File.WriteAllText(FilePathSecretKeys, JsonConvert.SerializeObject(accessTokenAuthentication, Formatting.Indented));
    }

    private static DropboxKeys GetDropboxKeys()
    {
        var assembly = Assembly.GetAssembly(typeof(DropboxService))!;
        var resources = assembly.GetManifestResourceNames();
        var resourceName = resources.First(s => s.Contains("DropboxKeys.json"));

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var jsonStr = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<DropboxKeys>(jsonStr)!;
    }

    private async Task<AccessTokenAuthentication?> GetAccessTokenAuthentication(string tempToken)
    {
        using var httpClient = Http.GetHttpClient();
        var requestData = new Dictionary<string, string>
        {
            { "code", tempToken },
            { "grant_type", "authorization_code" },
            { "client_id", DropboxKeys.AppKey! },
            { "client_secret", DropboxKeys.AppSecret! },
            { "redirect_uri", DropboxKeys.RedirectUri! }
        };

        var requestContent = new FormUrlEncodedContent(requestData);
        var response = await httpClient.PostAsync("https://api.dropbox.com/oauth2/token", requestContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenAuthentication>(responseContent);
        return accessTokenResponse;

    }

    private string? GetTempToken()
    {
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add(DropboxKeys.RedirectUri!);
        httpListener.Start();

        var uri = $"https://www.dropbox.com/oauth2/authorize?client_id={DropboxKeys.AppKey}&redirect_uri={DropboxKeys.RedirectUri}&response_type=code&token_access_type=offline";
        var process = new Process();
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = false;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        process.StartInfo.FileName = uri;
        process.Start();

        var context = httpListener.GetContext();
        var response = context.Response;

        var tempToken = context.Request.QueryString["code"];

        response.Close();
        httpListener.Close();

        return tempToken;
    }

    private static string GenerateDirectorySecretKeys()
    {
        var directorySecretKeys = Path.GetFullPath("Api");
        directorySecretKeys = Path.Join(directorySecretKeys, "Dropbox");

        var directoryInfo = Directory.CreateDirectory(directorySecretKeys);
        directoryInfo = directoryInfo.Parent;
        if (directoryInfo is not null) directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        return directorySecretKeys;
    }
}