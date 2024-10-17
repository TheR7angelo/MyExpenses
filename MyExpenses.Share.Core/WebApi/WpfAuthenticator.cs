using System.Diagnostics;
using System.Net;
using MyExpenses.Models.WebApi;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;

namespace MyExpenses.Share.Core.WebApi;

public class WpfAuthenticator : IAuthenticator
{
    public async Task<string?> AuthenticateAsync(DropboxKeys dropboxKeys)
    {
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add(dropboxKeys.RedirectUri!);
        httpListener.Start();

        var uri =
            $"https://www.dropbox.com/oauth2/authorize?client_id={dropboxKeys.AppKey}&redirect_uri={dropboxKeys.RedirectUri}&response_type=code&token_access_type=offline";
        var process = new Process();
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = false;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        process.StartInfo.FileName = uri;
        process.Start();

        var context = await httpListener.GetContextAsync();
        var response = context.Response;

        var tempToken = context.Request.QueryString["code"];

        response.Close();
        httpListener.Close();

        return tempToken;
    }
}