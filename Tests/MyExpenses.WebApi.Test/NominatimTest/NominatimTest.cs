using MyExpenses.WebApi.Nominatim;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Test.NominatimTest;

public class NominatimTest
{
    private static readonly Point Point = new(48.87378756079077, 2.2950201847722433);
    private const string Address = "Avenue des Champs-Élysées, 75008, Paris";

    [Fact]
    public void PointToGeoNominatim()
    {
        var nominatim = Point.ToGeoNominatim();

        Assert.NotNull(nominatim);
    }

    [Fact]
    public void StringToNominatimTest()
    {
        var nominatim = Address.ToNominatim();

        Assert.NotNull(nominatim);
        Assert.NotEmpty(nominatim);
    }

    [Fact]
    public void PointToNominatimTest()
    {
        var nominatim = Point.ToNominatim();

        Assert.NotNull(nominatim);
    }

}