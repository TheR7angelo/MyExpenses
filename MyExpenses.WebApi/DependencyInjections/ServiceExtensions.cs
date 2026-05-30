using Microsoft.Extensions.DependencyInjection;

namespace MyExpenses.WebApi.DependencyInjections;

public static class ServiceExtensions
{
    public const string NominatiumApi = "NominatiumApi";

    public static IServiceCollection AddHttpClientBuilder(this IServiceCollection services)
    {
        var userAgent = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);

        services.AddHttpClient(NominatiumApi, client =>
        {
            client.BaseAddress = new Uri("https://nominatim.openstreetmap.org");
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        });

        return services;
    }
}