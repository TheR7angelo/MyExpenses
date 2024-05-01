using Mapsui;
using Mapsui.Extensions;
using Mapsui.Styles;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;

namespace MyExpenses.Wpf.Utils.Maps;

public static class MapsuiMapExtensions
{
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
}