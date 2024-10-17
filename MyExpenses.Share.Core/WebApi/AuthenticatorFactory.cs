using MyExpenses.Models.WebApi;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;

namespace MyExpenses.Share.Core.WebApi;

public static class AuthenticatorFactory
{
    public static IAuthenticator CreateAuthenticator(this ProjectSystem projectSystem)
    {
        return projectSystem switch
        {
            ProjectSystem.Wpf => new WpfAuthenticator(),
            ProjectSystem.Maui => new MauiAuthenticator(),
            _ => throw new ArgumentOutOfRangeException(nameof(projectSystem), projectSystem, null)
        };
    }
}