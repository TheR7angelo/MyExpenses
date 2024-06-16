using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Utils.Maps;

namespace MyExpenses.Wpf.UserControls.DashBoardPageCards;

public partial class LocationManagementUserControl
{
    private WritableLayer PlaceLayer { get; } = new() { Style = null, IsMapInfoLayer = false };

    public LocationManagementUserControl()
    {
        var mapper = Mapping.Mapper;

        using var context = new DataBaseContext();
        var places = context.TPlaces.ToList();

        foreach (var place in places.Where(s => s.Latitude is not null && s.Latitude is not 0 && s.Longitude is not null &&
                                                s.Longitude is not 0))
        {
            var feature = mapper.Map<PointFeature>(place);
            feature.Styles = place.IsOpen is true
                ? [MapsuiStyleExtensions.RedMarkerStyle]
                : new List<IStyle> { MapsuiStyleExtensions.BlueMarkerStyle };
            PlaceLayer.Add(feature);
        }

        var map = MapsuiMapExtensions.GetMap(false);
        map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        map.Layers.Add(PlaceLayer);

        InitializeComponent();

        MapControl.Map = map;

        SetInitialZoom();
    }

    private void SetInitialZoom()
    {
        var points = PlaceLayer.GetFeatures().Select(s => ((PointFeature)s).Point).ToList();

        switch (points.Count)
        {
            case 0:
                break;
            case 1:
                MapControl.Map.Home = navigator =>
                {
                    navigator.CenterOn(points[0]);
                    navigator.ZoomTo(1);
                };
                break;
            case > 1:
                double minX = points.Min(p => p.X), maxX = points.Max(p => p.X);
                double minY = points.Min(p => p.Y), maxY = points.Max(p => p.Y);

                var width = maxX - minX;
                var height = maxY - minY;

                const double marginPercentage = 10; // Change this value to suit your needs
                var marginX = width * marginPercentage / 100;
                var marginY = height * marginPercentage / 100;

                var mRect = new MRect(minX - marginX, minY - marginY, maxX + marginX, maxY + marginY);

                MapControl.Map.Home = navigator => { navigator.ZoomToBox(mRect); };
                break;
        }
    }
}