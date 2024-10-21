using MyExpenses.Models.WebApi.DropBox;

namespace MyExpenses.Models.WebApi.Authenticator;

public interface IAuthenticator
{
    Task<string?> AuthenticateAsync(DropboxKeys dropboxKeys, Pkce pkceData);
}