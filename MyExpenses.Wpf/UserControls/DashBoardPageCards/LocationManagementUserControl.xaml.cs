using Mapsui.Layers;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Maps;
using MyExpenses.Wpf.Utils.Maps;

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
        MapControl.Map.Navigator.SetZoom(PlaceLayer);
    }
}