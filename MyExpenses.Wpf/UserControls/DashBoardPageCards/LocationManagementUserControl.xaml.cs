using Mapsui.Layers;
using Mapsui.Styles;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Maps;

namespace MyExpenses.Wpf.UserControls.DashBoardPageCards;

public partial class LocationManagementUserControl
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    private WritableLayer PlaceLayer { get; } = new() { Style = null };

    public LocationManagementUserControl()
    {
        var mapper = Mapping.Mapper;

        using var context = new DataBaseContext();
        var places = context.TPlaces.ToList();

        foreach (var place in places.Where(s => s.Latitude is not null && s.Latitude is not 0 && s.Longitude is not null &&
                                                s.Longitude is not 0))
        {
            var feature = mapper.Map<PointFeature>(place);
            feature.Styles = place.IsOpen
                ? [MapsuiStyleExtensions.RedMarkerStyle]
                : [MapsuiStyleExtensions.BlueMarkerStyle];
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
                MapControl.Map.Navigator.CenterOnAndZoomTo(points[0], 1);
                break;
            case > 1:
                var mRect = points.ToMRect();
                MapControl.Map.Navigator.ZoomToBox(mRect);
                break;
        }
    }
}