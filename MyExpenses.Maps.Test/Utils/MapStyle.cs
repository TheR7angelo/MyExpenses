using System.IO;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Styles;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;

namespace MyExpenses.Maps.Test.Utils;

public static class MapStyle
{
    public static SymbolStyle RedMarkerStyle { get; private set; }

    static MapStyle()
    {
        RedMarkerStyle = SetMarkerStyle();
    }

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

    private static SymbolStyle SetMarkerStyle()
    {
        var path = Path.GetFullPath("Ressources");
        var icon = Path.Join(path, "Sans titre - 1.png");

        var fileStream = new FileStream(icon, FileMode.Open);
        var bitmapId = BitmapRegistry.Instance.Register(fileStream);

        return new SymbolStyle
        {
            BitmapId = bitmapId,
            SymbolScale = 0.02
        };
    }
}