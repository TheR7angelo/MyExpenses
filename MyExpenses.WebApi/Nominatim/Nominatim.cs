using System.Globalization;
using MyExpenses.Models.WebApi.Nominatim;
using MyExpenses.Utils;
using NetTopologySuite.Geometries;

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
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The allocation of the parameters list is necessary here to build the URL for the Nominatim API.
            // This allocation is minimal and short-lived, limited to the scope of this operation, and does not significantly impact performance.
            var parameters = new List<string>
            {
                $"reverse?format=json&lat={position.Y.ToString(CultureInfo.InvariantCulture)}&lon={position.X.ToString(CultureInfo.InvariantCulture)}"
            };
            if (addressDetails) parameters.Add("addressdetails=1");
            if (polygon) parameters.Add("polygon=1");
            if (polygonGeojson) parameters.Add("polygon_geojson=1");

            var url = string.Join('&', parameters);

            var httpResult = await HttpClient
                .GetAsync(url)
                .ConfigureAwait(false);
            var result = await httpResult.Content.ReadAsStringAsync();
            return result.ToObject<NominatimSearchResult>();
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
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The allocation of the parameters list is necessary here to build the URL for the Nominatim API.
            // This allocation is minimal and short-lived, limited to the scope of this operation, and does not significantly impact performance.
            var parameters = new List<string> { $"search?q={Http.ParseToUrlFormat(address)}&format=json" };

            if (addressDetails) parameters.Add("addressdetails=1");
            if (polygon) parameters.Add("polygon=1");
            if (polygonGeojson) parameters.Add("polygon_geojson=1");

            var url = string.Join('&', parameters);

            var httpResult = await HttpClient
                .GetAsync(url)
                .ConfigureAwait(false);
            var result = await httpResult.Content.ReadAsStringAsync();

            return result.ToObject<List<NominatimSearchResult>>();
        }
        catch (Exception)
        {
            return null;
        }
    }
}