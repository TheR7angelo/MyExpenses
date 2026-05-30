using System.Globalization;
using System.Text.Json;
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
    public async Task<Result<IEnumerable<NominatimSearchResultDomain>>> SearchAsync(double latitude, double longitude,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ServiceExtensions.NominatiumApi);

        try
        {
            var url = $"reverse?format=json&lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}&addressdetails=1&polygon=1&polygon_geojson=1";

            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Nominatim search with url: {BaseUrl}/{Url}", client.BaseAddress, url);

            var httpResult = await client
                .GetAsync(url, cancellationToken);

            if (!httpResult.IsSuccessStatusCode)
            {
                logger.LogError("Nominatium search failed with status code {StatusCode} and message {Message}", httpResult.StatusCode, httpResult.ReasonPhrase);
                return Result<IEnumerable<NominatimSearchResultDomain>>.Failure(ErrorCode.HttpClientRequestFailed, "Failed to retrieve Nominatim data");
            }

            var rawJson = await httpResult.Content.ReadAsStringAsync(cancellationToken);

            List<NominatimSearchResult> resultEntities;
            using var jsonDocument = JsonDocument.Parse(rawJson);
            if (jsonDocument.RootElement.ValueKind == JsonValueKind.Array)
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

    [LoggerMessage(Level = LogLevel.Information, Message = "Nominatim search result: {@NominatimSearchResults}")]
    private static partial void LogNominatimResult(ILogger logger, NominatimSearchResultDomain[] nominatimSearchResults);
}