using MyExpenses.WebApi.Nominatim;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Test.NominatimTest;

public class NominatimTest
{
    private static readonly Point Point = new(48.87378756079077, 2.2950201847722433);
    private const string Address = "Avenue des Champs-Élysées, 75008, Paris";

    [Fact]
    public void StringToNominatimGeoJsonTest()
    {
        var nominatims = Address.ToNominatim(polygonGeojson:true)?.ToArray();

        Assert.NotNull(nominatims);
        Assert.NotEmpty(nominatims);

        foreach (var nominatim in nominatims)
        {
            Assert.NotNull(nominatim.GeoJson);
            Assert.NotNull(nominatim.GeoJson.GetLineStringCoordinates());
            Assert.Throws<InvalidOperationException>(() => nominatim.GeoJson.GetPolygonCoordinates());
        }
    }

    [Fact]
    public void StringToNominatimAllFalseTest()
    {
        var nominatim = Address.ToNominatim();

        Assert.NotNull(nominatim);
        Assert.NotEmpty(nominatim);
    }

    [Fact]
    public void PointToNominatimGeoJsonTest()
    {
        var nominatim = Point.ToNominatim(polygonGeojson:true);

        Assert.NotNull(nominatim);
        Assert.NotNull(nominatim.GeoJson);
        Assert.NotNull(nominatim.GeoJson.GetPolygonCoordinates());
        Assert.Throws<InvalidOperationException>(() => nominatim.GeoJson.GetLineStringCoordinates());
    }

    [Fact]
    public void PointToNominatimAllFalseTest()
    {
        var nominatim = Point.ToNominatim();

        Assert.NotNull(nominatim);
        Assert.Null(nominatim.GeoJson);
    }

}