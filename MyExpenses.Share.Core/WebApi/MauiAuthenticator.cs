using Microsoft.Maui.Authentication;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;

namespace MyExpenses.Share.Core.WebApi;

public class MauiAuthenticator : IAuthenticator
{
    public async Task<string?> AuthenticateAsync(DropboxKeys dropboxKeys)
    {
        var authUri = new Uri($"https://www.dropbox.com/oauth2/authorize?client_id={dropboxKeys.AppKey}&redirect_uri={dropboxKeys.RedirectUri}&response_type=code&token_access_type=offline");

        var result = await WebAuthenticator.Default.AuthenticateAsync(
            new WebAuthenticatorOptions
            {
                Url = authUri,
                CallbackUrl = new Uri(dropboxKeys.RedirectUri!)
            });

        var tempToken = result.Properties["code"];
        return tempToken;
    }
}