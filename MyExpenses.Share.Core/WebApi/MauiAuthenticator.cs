using Microsoft.Maui.Authentication;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;

namespace MyExpenses.Share.Core.WebApi;

public class MauiAuthenticator : AAuthenticator, IAuthenticator
{
    public async Task<string?> AuthenticateAsync(DropboxKeys dropboxKeys)
    {
        var pkceData = GeneratePkceData();

        var authUri = new Uri($"https://www.dropbox.com/oauth2/authorize?client_id={dropboxKeys.AppKey}&redirect_uri={dropboxKeys.RedirectUri}&response_type=code&code_challenge={pkceData.CodeChallenge}&code_challenge_method=S256&token_access_type=offline");

        var result = await WebAuthenticator.Default.AuthenticateAsync(
            new WebAuthenticatorOptions
            {
                Url = authUri,
                CallbackUrl = new Uri(dropboxKeys.RedirectUri!),
                PrefersEphemeralWebBrowserSession = true
            });

        var tempToken = result.Properties["code"];
        return tempToken;
    }
}