using System.Diagnostics;
using System.Net;
using System.Reflection;
using MyExpenses.Models.WebApi.DropBox;
using Newtonsoft.Json;

namespace MyExpenses.WebApi.Dropbox;

public class DropboxService
{
    public AccessTokenAuthentication? AccessTokenAuthentication { get; set; }

    private string FilePathSecretKeys { get; }

    public DropboxService()
    {
        var directorySecretKeys = GenerateDirectorySecretKeys();
        FilePathSecretKeys = Path.Join(directorySecretKeys, "AccessTokenAuthentication.json");

        if (!File.Exists(FilePathSecretKeys)) return;

        var jsonStr = File.ReadAllText(FilePathSecretKeys);
        AccessTokenAuthentication = JsonConvert.DeserializeObject<AccessTokenAuthentication>(jsonStr);
    }

    public void AuthorizeApplication()
    {
        var assembly = Assembly.GetAssembly(typeof(DropboxService))!;
        var resources = assembly.GetManifestResourceNames();
        var resourceName = resources.First(s => s.Contains("DropboxKeys.json"));

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var jsonStr = reader.ReadToEnd();

        var dropboxKeys = JsonConvert.DeserializeObject<DropboxKeys>(jsonStr)!;

        var tempToken = GetTempToken(dropboxKeys)!;
        var taskAccessTokenAuthentication = GetAccessTokenAuthentication(tempToken, dropboxKeys);
        var accessTokenAuthentication = taskAccessTokenAuthentication.GetAwaiter().GetResult();

        if (accessTokenAuthentication is not null)
        {
            accessTokenAuthentication.DateCreated = DateTime.Now;
            accessTokenAuthentication.DateExpiration = DateTime.Now.AddSeconds(accessTokenAuthentication.ExpiresIn ?? 0);
        }

        File.WriteAllText(FilePathSecretKeys, JsonConvert.SerializeObject(accessTokenAuthentication, Formatting.Indented));
    }

    private static async Task<AccessTokenAuthentication?> GetAccessTokenAuthentication(string tempToken, DropboxKeys dropboxKeys)
    {
        using var httpClient = new HttpClient();
        var requestData = new Dictionary<string, string>
        {
            { "code", tempToken },
            { "grant_type", "authorization_code" },
            { "client_id", dropboxKeys.AppKey! },
            { "client_secret", dropboxKeys.AppSecret! },
            { "redirect_uri", dropboxKeys.RedirectUri! }
        };

        var requestContent = new FormUrlEncodedContent(requestData);
        var response = await httpClient.PostAsync("https://api.dropbox.com/oauth2/token", requestContent);
        var responseContent = await response.Content.ReadAsStringAsync();

        var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenAuthentication>(responseContent);
        return accessTokenResponse;

    }

    private static string? GetTempToken(DropboxKeys dropboxKeys)
    {
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add(dropboxKeys.RedirectUri!);
        httpListener.Start();

        var uri = $"https://www.dropbox.com/oauth2/authorize?client_id={dropboxKeys.AppKey}&redirect_uri={dropboxKeys.RedirectUri}&response_type=code&token_access_type=offline";
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