using MyExpenses.Models.Maui.Sensor.Location;

namespace MyExpenses.Maui.Utils;

public static class SensorRequestUtils
{
    public static async Task<Location?> GetLocation(GeolocationAccuracy geolocationAccuracy = GeolocationAccuracy.Default)
    {
        var request = new GeolocationRequest(geolocationAccuracy);
        var location = await Geolocation.GetLocationAsync(request);

        return location;
    }

    public static EHemisphere? GetHemisphere(this Location? location)
    {
        if (location is null) return null;

        return location.Latitude >= 0
            ? EHemisphere.Northern
            : EHemisphere.Southern;
    }
}