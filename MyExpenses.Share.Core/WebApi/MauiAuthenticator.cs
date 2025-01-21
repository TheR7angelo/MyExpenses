using Microsoft.Maui.Authentication;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;

namespace MyExpenses.Share.Core.WebApi;

public class MauiAuthenticator : IAuthenticator
{
    public async Task<string?> AuthenticateAsync(DropboxKeys dropboxKeys, Pkce pkceData)
    {
        // ReSharper disable HeapView.ObjectAllocation.Evident
        // Normal allocation of an authorization URI for Dropbox OAuth2
        var authUri = new Uri($"https://www.dropbox.com/oauth2/authorize?client_id={dropboxKeys.AppKey}&redirect_uri={dropboxKeys.RedirectUri}&response_type=code&code_challenge={pkceData.CodeChallenge}&code_challenge_method=S256&token_access_type=offline");

        // Normal allocation of a callback URI
        var callBackUrl = new Uri(dropboxKeys.RedirectUri!);
        // ReSharper restore HeapView.ObjectAllocation.Evident

        var result = await WebAuthenticator.Default.AuthenticateAsync(
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Usage of WebAuthenticatorOptions is required to configure the authentication process.
            // This class allows specifying the authentication URL, the callback URL (redirect URI),
            // and additional parameters such as ephemeral browser sessions
            new WebAuthenticatorOptions
            {
                Url = authUri,
                CallbackUrl = callBackUrl,
                PrefersEphemeralWebBrowserSession = true
            });

        var tempToken = result.Properties["code"];
        return tempToken;
    }
}