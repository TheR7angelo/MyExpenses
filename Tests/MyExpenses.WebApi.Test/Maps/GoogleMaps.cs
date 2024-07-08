using MyExpenses.WebApi.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Test.Maps;

public class StreetViewTest
{
    private Point Point { get; } = new(-0.5754690669950785, 44.837624634089);

    [Fact]
    private void GoToGoogleEarthWeb()
    {
        Point.GoToGoogleEarthWeb();
    }

    [Fact]
    private void GoToMaps()
    {
        Point.ToGoogleMaps();
    }

    [Fact]
    private void GoToStreetView()
    {
        Point.ToGoogleStreetView();
    }
}