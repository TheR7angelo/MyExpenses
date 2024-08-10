using System.Globalization;
using MyExpenses.Models.WebApi.Nominatim;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace MyExpenses.WebApi.Nominatim;

public static class Nominatim
{
    private static HttpClient HttpClient { get; }

    static Nominatim()
    {
        HttpClient = Http.GetHttpClient("https://nominatim.openstreetmap.org");
    }

    public static NominatimSearchResult? ToGeoNominatim(this Point position)
        => position._toGeoNominatim().Result;

    private static async Task<NominatimSearchResult?> _toGeoNominatim(this Point position)
    {
        try
        {
            var httpResult = await HttpClient
                .GetAsync(
                    $"reverse?format=json&lat={position.X.ToString(CultureInfo.InvariantCulture)}&lon={position.Y.ToString(CultureInfo.InvariantCulture)}&polygon_geojson=1")
                .ConfigureAwait(false);
            var result = await httpResult.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<NominatimSearchResult>(result);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static IEnumerable<NominatimSearchResult>? ToNominatim(this string address)
        => _ToNominatim(address).Result;

    private static async Task<List<NominatimSearchResult>?> _ToNominatim(string address)
    {
        try
        {
            var httpResult = await HttpClient
                .GetAsync($"search?q={Http.ParseToUrlFormat(address)}&format=json&polygon=1&addressdetails=1")
                .ConfigureAwait(false);
            var result = await httpResult.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<NominatimSearchResult>>(result);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static NominatimSearchResult? ToNominatim(this Point position)
        => _ToNominatim(position).Result;

    private static async Task<NominatimSearchResult?> _ToNominatim(Point position)
    {
        try
        {
            var httpResult = await HttpClient
                .GetAsync(
                    $"reverse?format=json&lat={position.X.ToString(CultureInfo.InvariantCulture)}&lon={position.Y.ToString(CultureInfo.InvariantCulture)}")
                .ConfigureAwait(false);
            var result = await httpResult.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<NominatimSearchResult>(result);
        }
        catch (Exception)
        {
            return null;
        }
    }
}