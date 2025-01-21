using System.Net;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.SharedUtils.Utils;

namespace MyExpenses.Share.Core.WebApi;

public class WpfAuthenticator : IAuthenticator
{
    public async Task<string?> AuthenticateAsync(DropboxKeys dropboxKeys, Pkce pkceData)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary allocation of the HttpListener class to set up an HTTP listener for the callback URI.
        // This is required to capture the authorization code provided by Dropbox after user authentication.
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add(dropboxKeys.RedirectUriWpf!);
        httpListener.Start();

        var uri = $"https://www.dropbox.com/oauth2/authorize?client_id={dropboxKeys.AppKey}&redirect_uri={dropboxKeys.RedirectUriWpf}&response_type=code&code_challenge={pkceData.CodeChallenge}&code_challenge_method=S256&token_access_type=offline";
        uri.StartProcessWithParameters();

        var context = await httpListener.GetContextAsync();
        var response = context.Response;

        var tempToken = context.Request.QueryString["code"];

        response.Close();
        httpListener.Close();

        return tempToken;
    }
}