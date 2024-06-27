using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using Dropbox.Api;
using Dropbox.Api.Files;
using MyExpenses.Models.WebApi.DropBox;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace MyExpenses.WebApi.Test.Dropbox;

public class DropboxApiTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    private async Task Test()
    {
        var directorySecretKeys = Path.GetFullPath("Api");
        directorySecretKeys = Path.Join(directorySecretKeys, "Dropbox");

        var directoryInfo = Directory.CreateDirectory(directorySecretKeys);
        directoryInfo = directoryInfo.Parent;
        if (directoryInfo is not null) directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

        var filePathSecretKeys = Path.Join(directorySecretKeys, "AccessTokenAuthentication.json");

        AccessTokenAuthentication? accessTokenAuthentication;
        if (!File.Exists(filePathSecretKeys))
        {
            var assembly = Assembly.GetAssembly(typeof(MyExpenses.WebApi.Nominatim.Nominatim))!;
            var resources = assembly.GetManifestResourceNames();
            var resourceName = resources.First(s => s.Contains("DropboxKeys.json"));

            await using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            var jsonStr = await reader.ReadToEndAsync();

             var dropboxKeys = JsonConvert.DeserializeObject<DropboxKeys>(jsonStr)!;

            var tempToken = GetTempToken(dropboxKeys)!;
            accessTokenAuthentication = await GetAccessTokenAuthentication(tempToken, dropboxKeys);
            if (accessTokenAuthentication is not null)
            {
                accessTokenAuthentication.DateCreated = DateTime.Now;
                accessTokenAuthentication.DateExpiration = DateTime.Now.AddSeconds(accessTokenAuthentication.ExpiresIn ?? 0);
            }

            await File.WriteAllTextAsync(filePathSecretKeys, JsonConvert.SerializeObject(accessTokenAuthentication, Formatting.Indented));
        }
        else
        {
            var jsonStr = await File.ReadAllTextAsync(filePathSecretKeys);
            accessTokenAuthentication = JsonConvert.DeserializeObject<AccessTokenAuthentication>(jsonStr);
        }

        if (accessTokenAuthentication is null) return;

        var needToRefresh = accessTokenAuthentication.DateExpiration <= DateTime.Now;
        if (needToRefresh)
        {
            testOutputHelper.WriteLine("need to refresh");
        }
        else
        {
            using var client = new DropboxClient(accessTokenAuthentication!.AccessToken);
            var content = $"Hello, World! {DateTime.Now}";
            using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await client.Files.UploadAsync("/test.txt", WriteMode.Overwrite.Instance, body: memStream);
        }
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

        using var writer = new StreamWriter(response.OutputStream);
        writer.Write("You've successfully authenticated!");

        var tempToken = context.Request.QueryString["code"];

        response.Close();
        httpListener.Close();

        return tempToken;
    }
}