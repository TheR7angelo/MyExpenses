﻿using System.Globalization;
using MyExpenses.Models.WebApi.Nominatim;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace MyExpenses.WebApi.Nominatim;

public class Nominatim : Http
{
    private static HttpClient HttpClient { get; set; } = null!;

    static Nominatim()
    {
        HttpClient = GetHttpClient("https://nominatim.openstreetmap.org");
    }

    public static List<NominatimStruc>? AddressToNominatim(string address) => _AddressToNominatim(address).Result;
    public static NominatimStruc? PointToNominatim(Point position) => _PositionToNominatim(position).Result;
    private static async Task<List<NominatimStruc>?> _AddressToNominatim(string address)
    {
        try
        {
            var httpResult = await HttpClient.GetAsync($"search?q={ParseToUrlFormat(address)}&format=json&polygon=1&addressdetails=1").ConfigureAwait(false);
            var result = await httpResult.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<List<NominatimStruc>>(result);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    private static async Task<NominatimStruc?> _PositionToNominatim(Point position)
    {
        try
        {
            var httpResult = await HttpClient.GetAsync($"reverse?format=json&lat={position.X.ToString(CultureInfo.InvariantCulture)}&lon={position.Y.ToString(CultureInfo.InvariantCulture)}").ConfigureAwait(false);
            var result = await httpResult.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<NominatimStruc>(result);
        }
        catch (Exception)
        {
            return null;
        }
    }
}