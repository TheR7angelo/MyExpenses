using MyExpenses.WebApi.Nominatim;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Test.NominatimTest;

public class NominatimTest
{
    [Fact]
    public void StringToNominatimTest()
    {
        const string address = "Avenue des Champs-Élysées, 75008, Paris";
        var nominatim = address.ToNominatim();

        Assert.NotNull(nominatim);
        Assert.NotEmpty(nominatim);
    }

    [Fact]
    public void PointToNominatimTest()
    {
        var point = new Point(48.87378756079077, 2.2950201847722433);

        var nominatim = point.ToNominatim();

        Assert.NotNull(nominatim);
    }

}