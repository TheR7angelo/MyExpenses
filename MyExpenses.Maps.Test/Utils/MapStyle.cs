using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Maps.Test.Utils;

public static class MapStyle
{
    public static SymbolStyle RedMarkerStyle { get; private set; }
    public static SymbolStyle GreenMarkerStyle { get; private set; }

    static MapStyle()
    {
        RedMarkerStyle = SetRedMarkerStyle();
        GreenMarkerStyle = SetGreenMarkerStyle();
    }

    private static Offset Offset => new() { IsRelative = false, X = 0, Y = 1000 };
    private static double Scale => 0.02;

    public static Map GetMap(bool widget)
    {
        var map = new Map { CRS = "EPSG:3857", BackColor = Color.Black };
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

    private static SymbolStyle SetGreenMarkerStyle()
    {
        var path = Path.GetFullPath("Ressources");
        var icon = Path.Join(path, "GreenMarker.png");
        var bitmapId = RegisterBitmap(icon);

        return new SymbolStyle
        {
            BitmapId = bitmapId,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }

    private static SymbolStyle SetRedMarkerStyle()
    {
        var path = Path.GetFullPath("Ressources");
        var icon = Path.Join(path, "RedMarker.png");
        var bitmapId = RegisterBitmap(icon);

        return new SymbolStyle
        {
            BitmapId = bitmapId,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }

    private static int RegisterBitmap(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Open);
        var bitmapId = BitmapRegistry.Instance.Register(fileStream);

        return bitmapId;
    }
}