using System.Windows;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Utils;
using MyExpenses.WebApi.Nominatim;
using Point = NetTopologySuite.Geometries.Point;

namespace MyExpenses.Maps.Test;

public partial class WindowEdit
{
    public TPlace TPlace { get; } = new();

    public WindowEdit()
    {
        InitializeComponent();
    }

    public void SetTplace(TPlace newTPlace)
    {
        PropertyCopyHelper.CopyProperties(newTPlace, TPlace);
    }


    private void ButtonSearchByCoordinate_OnClick(object sender, RoutedEventArgs e)
    {
        var latitude = TPlace.Latitude;
        var longitude = TPlace.Longitude;
        var point = new Point(latitude ?? 0, longitude ?? 0);

        var result = Nominatim.PointToNominatim(point);
        Console.WriteLine(result);
    }
}