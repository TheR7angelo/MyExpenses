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

    public static NominatimSearchResult? ToNominatim(this Point position, bool addressDetails = false, bool polygon = false, bool polygonGeojson = false)
        => position._ToNominatim(addressDetails, polygon, polygonGeojson).Result;

    private static async Task<NominatimSearchResult?> _ToNominatim(this Point position, bool addressDetails, bool polygon, bool polygonGeojson)
    {
        try
        {
            var parameters = new List<string>
            {
                $"reverse?format=json&lat={position.X.ToString(CultureInfo.InvariantCulture)}&lon={position.Y.ToString(CultureInfo.InvariantCulture)}"
            };
            if (addressDetails) parameters.Add("addressdetails=1");
            if (polygon) parameters.Add("polygon=1");
            if (polygonGeojson) parameters.Add("polygon_geojson=1");

            var url = string.Join('&', parameters);

            var httpResult = await HttpClient
                .GetAsync(url)
                .ConfigureAwait(false);
            var result = await httpResult.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<NominatimSearchResult>(result);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static IEnumerable<NominatimSearchResult>? ToNominatim(this string address, bool addressDetails = false, bool polygon = false, bool polygonGeojson = false)
        => address._ToNominatim(addressDetails, polygon, polygonGeojson).Result;

    private static async Task<List<NominatimSearchResult>?> _ToNominatim(this string address, bool addressDetails, bool polygon, bool polygonGeojson)
    {
        try
        {
            var parameters = new List<string> { $"search?q={Http.ParseToUrlFormat(address)}&format=json" };
            if (addressDetails) parameters.Add("addressdetails=1");
            if (polygon) parameters.Add("polygon=1");
            if (polygonGeojson) parameters.Add("polygon_geojson=1");

            var url = string.Join('&', parameters);

            var httpResult = await HttpClient
                .GetAsync(url)
                .ConfigureAwait(false);
            var result = await httpResult.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<NominatimSearchResult>>(result);
        }
        catch (Exception)
        {
            return null;
        }
    }
}