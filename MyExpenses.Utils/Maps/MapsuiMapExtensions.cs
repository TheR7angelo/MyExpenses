using BruTile.Predefined;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Widgets;
using Mapsui.Widgets.ButtonWidgets;
using Mapsui.Widgets.InfoWidgets;
using Mapsui.Widgets.ScaleBar;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Utils.Maps;

public static class MapsuiMapExtensions
{
    public static IEnumerable<KnownTileSource> GetAllKnowTileSource()
    {
        var blackList = new List<KnownTileSource>
        {
            KnownTileSource.OpenCycleMap, KnownTileSource.OpenCycleMapTransport,
            KnownTileSource.StamenToner, KnownTileSource.StamenTonerLite, KnownTileSource.StamenWatercolor, KnownTileSource.StamenTerrain,
            KnownTileSource.EsriWorldReferenceOverlay, KnownTileSource.EsriWorldBoundariesAndPlaces,
            KnownTileSource.HereHybrid, KnownTileSource.HereTerrain
        };
        var knownTileSources = Enum.GetValues<KnownTileSource>().Where(s => !blackList.Contains(s)).ToList();
        return knownTileSources;
    }

    public static Map GetMap(bool widget, Color? backColor = null)
    {
        backColor ??= Color.Black;
        var map = new Map { CRS = "EPSG:3857", BackColor = backColor };
        if (widget)
        {
            map.Widgets.AddRange(new List<IWidget>
            {
                new MapInfoWidget(map),
                new ZoomInOutWidget(),
                new ScaleBarWidget(map),
            });
        }

        return map;
    }

    public static PointFeature ToFeature(this TPlace place, SymbolStyle? symbolStyle = null)
        => place.ToSingleFeature(symbolStyle);

    public static IEnumerable<PointFeature> ToFeature(this IEnumerable<TPlace> places, SymbolStyle? symbolStyle = null)
        => places.Select(place => place.ToSingleFeature(symbolStyle));

    private static PointFeature ToSingleFeature(this TPlace place, SymbolStyle? symbolStyle = null)
    {
        var mapper = Mapping.Mapper;
        var feature = mapper.Map<PointFeature>(place);

        feature.Styles = new List<IStyle>
        {
            new LabelStyle
            {
                Text = place.Name, Offset = new Offset { X = 0, Y = 11 },
                Font = new Font { FontFamily = "Arial", Size = 12 },
                Halo = new Pen { Color = Color.White, Width = 2 }
            }
        };
        if (symbolStyle is not null) feature.Styles.Add(symbolStyle);

        return feature;
    }
}