using MyExpenses.Models.WebApi.Authenticator;

namespace MyExpenses.Share.Core.WebApi;

public static class AuthenticatorFactory
{
    public static IAuthenticator CreateAuthenticator(this ProjectSystem projectSystem)
    {
        // ReSharper disable HeapView.ObjectAllocation.Evident
        return projectSystem switch
        {
            // Normal allocation of a WPF Authenticator
            ProjectSystem.Wpf => new WpfAuthenticator(),
            // Normal allocation of a MAUI Authenticator
            ProjectSystem.Maui => new MauiAuthenticator(),
            _ => throw new ArgumentOutOfRangeException(nameof(projectSystem), projectSystem, null)
        };
        // ReSharper restore HeapView.ObjectAllocation.Evident
    }
}