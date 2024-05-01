using System.IO;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Styles;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;

namespace MyExpenses.Wpf.Utils.Maps;

public static class MapsuiExtensions
{
    public static SymbolStyle RedMarkerStyle { get; private set; }
    public static SymbolStyle GreenMarkerStyle { get; private set; }

    static MapsuiExtensions()
    {
        RedMarkerStyle = SetRedMarkerStyle();
        GreenMarkerStyle = SetGreenMarkerStyle();
    }

    private static Offset Offset => new() { IsRelative = false, X = 0, Y = 1000 };
    private static double Scale => 0.02;

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

    private static readonly string IcoPath = Path.Join(Path.GetFullPath("Resources"), "Maps");

    private static SymbolStyle SetGreenMarkerStyle()
    {
        var icon = Path.Join(IcoPath, "GreenMarker.png");
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
        var icon = Path.Join(IcoPath, "RedMarker.png");
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