using BruTile.Predefined;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Mapsui.Widgets;
using Mapsui.Widgets.ButtonWidgets;
using Mapsui.Widgets.InfoWidgets;
using Mapsui.Widgets.ScaleBar;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Mapsui.PointFeatures;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Utils.Maps;

public static class MapsuiMapExtensions
{
    public static IEnumerable<KnownTileSource> GetAllKnowTileSource()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Memory allocation is currently required due to limitations with Span<T> and lambda capturing.
        // Span<T> cannot be captured in a lambda or delegate because it is a byref-like type and
        // cannot live beyond the scope of the current stack frame.
        // Once the lambda usage is refactored or removed, this array can be replaced with Span<T>
        // or ReadOnlySpan<T> to avoid heap allocation and optimize memory usage.
        // ReSharper disable once HeapView.ClosureAllocation
        var blackList = new []
        {
            KnownTileSource.OpenCycleMap, KnownTileSource.OpenCycleMapTransport,
            KnownTileSource.StamenToner, KnownTileSource.StamenTonerLite, KnownTileSource.StamenWatercolor,
            KnownTileSource.StamenTerrain, KnownTileSource.EsriWorldReferenceOverlay,
            KnownTileSource.EsriWorldBoundariesAndPlaces, KnownTileSource.HereHybrid,
            KnownTileSource.HereTerrain
        };

        // ReSharper disable once HeapView.DelegateAllocation
        var knownTileSources = Enum.GetValues<KnownTileSource>().Where(s => !blackList.Contains(s)).ToList();
        return knownTileSources;
    }

    public static Map GetMap(bool widget, Color? backColor = null)
    {
        backColor ??= Color.Black;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Map initialization allocates memory for persistent objects like CRS and BackColor.
        var map = new Map { CRS = "EPSG:3857", BackColor = (Color)backColor };
        if (widget)
        {
            // ReSharper disable HeapView.ObjectAllocation.Evident
            // Widgets are added to the map using a heap-allocated List<IWidget> to ensure persistence and support MapsUI's dynamic behavior.
            map.Widgets.AddRange(new List<IWidget>
                {
                new MapInfoWidget(map, s => s is not TileLayer),
                new ZoomInOutWidget(),
                new ScaleBarWidget(map)
            });
            // ReSharper restore HeapView.ObjectAllocation.Evident
        }

        return map;
    }

    public static TemporaryPointFeature ToTemporaryFeature(this TPlace place, SymbolStyle? symbolStyle = null)
    {
        var feature = place.ToSingleFeature(symbolStyle, false);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Memory allocation is necessary here as the method creates a new TemporaryPointFeature
        // to encapsulate the generated SingleFeature with its associated style.
        return new TemporaryPointFeature(feature);
    }


    public static PointFeature ToFeature(this TPlace place, SymbolStyle? symbolStyle = null)
        => place.ToSingleFeature(symbolStyle);

    // public static IEnumerable<PointFeature> ToFeature(this IEnumerable<TPlace> places, SymbolStyle? symbolStyle = null)
    //     => places.Select(place => place.ToSingleFeature(symbolStyle));

    private static PointFeature ToSingleFeature(this TPlace place, SymbolStyle? symbolStyle = null, bool labelStyle = true)
    {
        var mapper = Mapping.Mapper;
        var feature = mapper.Map<PointFeature>(place);

        feature.Styles = [];
        if (labelStyle)
        {
            // ReSharper disable HeapView.ObjectAllocation.Evident
            // Allocates a List<IStyle> to define rendering styles for the feature,
            // as required by MapsUI's design to handle visual representation dynamically.
            feature.Styles.Add(new LabelStyle
            {
                Text = place.Name, Offset = new Offset { X = 0, Y = 11 },
                Font = new Font { FontFamily = "Arial", Size = 12 },
                Halo = new Pen { Color = Color.White, Width = 2 }
            });
            // ReSharper restore HeapView.ObjectAllocation.Evident
        }

        if (symbolStyle is not null) feature.Styles.Add(symbolStyle);

        return feature;
    }
}