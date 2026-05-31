using System.Globalization;
using System.Text.Json;
using System.Web;
using Domain.Models.Nominatim;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.WebApi.DependencyInjections;
using MyExpenses.WebApi.Entities;
using MyExpenses.WebApi.Mappings;

namespace MyExpenses.WebApi.Repositories;

/// <summary> Manages communication with the Nominatim geocoding service to retrieve location information based on latitude and longitude. </summary>
public partial class NominatimRepository(IHttpClientFactory httpClientFactory, ILogger<NominatimRepository> logger) : INominatimRepository
{
    /// <summary>
    /// Searches for locations near the specified latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latitude of the location to search around.</param>
    /// <param name="longitude">The longitude of the location to search around.</param>
    /// <param name="cancellationToken">A token to allow cancellation of the request.</param>
    /// <returns>A task that represents the asynchronous operation. The result is a Result containing an IEnumerable of NominatimSearchResultDomain objects if successful, or an error message if it fails.</returns>
    public Task<Result<IEnumerable<NominatimSearchResultDomain>>> SearchAsync(double latitude, double longitude,
        CancellationToken cancellationToken = default)
    {
        var uri = FormatUri(latitude: latitude, longitude: longitude);
        return InternalSearchAsync(uri, cancellationToken);
    }

    public Task<Result<IEnumerable<NominatimSearchResultDomain>>> SearchAsync(string address, CancellationToken cancellationToken = default)
    {
        var uri = FormatUri(address: address);
        return InternalSearchAsync(uri, cancellationToken);
    }

    private async Task<Result<IEnumerable<NominatimSearchResultDomain>>> InternalSearchAsync(string uri, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ServiceExtensions.NominatiumApi);

        try
        {
            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Nominatim search with url: {BaseUrl}/{Url}", client.BaseAddress, uri);

            var httpResult = await client.GetAsync(uri, cancellationToken);

            if (!httpResult.IsSuccessStatusCode)
            {
                logger.LogError("Nominatium search failed with status code {StatusCode} and message {Message}", httpResult.StatusCode, httpResult.ReasonPhrase);
                return Result<IEnumerable<NominatimSearchResultDomain>>.Failure(ErrorCode.HttpClientRequestFailed, "Failed to retrieve Nominatim data");
            }

            var rawJson = await httpResult.Content.ReadAsStringAsync(cancellationToken);

            List<NominatimSearchResult> resultEntities;
            using var jsonDocument = JsonDocument.Parse(rawJson);
            if (jsonDocument.RootElement.ValueKind is JsonValueKind.Array)
            {
                resultEntities = JsonSerializer.Deserialize<List<NominatimSearchResult>>(rawJson) ?? [];
            }
            else
            {
                var single = JsonSerializer.Deserialize<NominatimSearchResult>(rawJson);
                resultEntities = single != null ? [single] : [];
            }

            var result = resultEntities.Select(s => s.MapToDomain()).ToArray();
            if (result.Length is 0)
            {
                logger.LogWarning("No Nominatim search results returned");
                return Result<IEnumerable<NominatimSearchResultDomain>>.Failure(ErrorCode.NotFound, "No Nominatim search results found");
            }

            LogNominatimResult(logger, result);
            return Result<IEnumerable<NominatimSearchResultDomain>>.Success(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static string FormatUri(double? latitude = null, double? longitude = null, string? address = null)
    {
        var hasCoordinates = latitude is not null && longitude is not null;
        var hasAddress = !string.IsNullOrWhiteSpace(address);

        if (!(hasCoordinates ^ hasAddress))
        {
            throw new ArgumentException("Either latitude and longitude or address must be provided, but not both.");
        }

        var uri = string.Empty;

        if (hasCoordinates)
        {
            uri += $"reverse?lat={latitude!.Value.ToString(CultureInfo.InvariantCulture)}&lon={longitude!.Value.ToString(CultureInfo.InvariantCulture)}";
        }

        if (hasAddress)
        {
            uri += $"search?q={HttpUtility.UrlEncode(address)}";
        }

        uri += "&addressdetails=1&polygon=1&polygon_geojson=1&format=json";

        return uri;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Nominatim search result: {@NominatimSearchResults}")]
    private static partial void LogNominatimResult(ILogger logger, NominatimSearchResultDomain[] nominatimSearchResults);
}