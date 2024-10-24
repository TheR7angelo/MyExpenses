using MyExpenses.Models.Maui.Sensor.Location;

namespace MyExpenses.Maui.Utils;

public static class SensorRequestUtils
{
    public static async Task<Location?> GetLocation(GeolocationAccuracy geolocationAccuracy = GeolocationAccuracy.Default)
    {
        var geolocationRequest = new GeolocationRequest(geolocationAccuracy);
        var location = await Geolocation.GetLocationAsync(geolocationRequest);

        return location;
    }

    public static EHemisphere? GetHemisphere(this Location? location)
    {
        if (location is null) return null;

        return location.Latitude >= 0
            ? EHemisphere.Northern
            : EHemisphere.Southern;
    }

    public static ESeason? GetSeason(this EHemisphere? hemisphere)
    {
        if (hemisphere is null) return null;

        var now = DateTime.Now;
        var springStart = new DateTime(now.Year, 3, 21);
        var summerStart = new DateTime(now.Year, 6, 21);
        var autumnStart = new DateTime(now.Year, 9, 21);
        var winterStart = new DateTime(now.Year, 12, 21);

        var currentSeason = now switch
        {
            _ when now >= springStart && now < summerStart => ESeason.Spring,
            _ when now >= summerStart && now < autumnStart => ESeason.Summer,
            _ when now >= autumnStart && now < winterStart => ESeason.Autumn,
            _ => ESeason.Winter
        };

        if (hemisphere is not EHemisphere.Southern) return currentSeason;
        return currentSeason switch
        {
            ESeason.Spring => ESeason.Autumn,
            ESeason.Summer => ESeason.Winter,
            ESeason.Autumn => ESeason.Spring,
            ESeason.Winter => ESeason.Summer,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}