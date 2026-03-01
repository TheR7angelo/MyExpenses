using MyExpenses.Models.WebApi.Dropbox;

namespace MyExpenses.Models.WebApi.Authenticator;

public interface IAuthenticator
{
    Task<string?> AuthenticateAsync(DropboxKeys dropboxKeys, Pkce pkceData);
}