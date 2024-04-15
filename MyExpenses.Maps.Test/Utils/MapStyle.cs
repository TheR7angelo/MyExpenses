using System.IO;
using Mapsui.Styles;

namespace MyExpenses.Maps.Test.Utils;

public static class MapStyle
{
    public static SymbolStyle RedMarkerStyle { get; private set; }

    static MapStyle()
    {
        RedMarkerStyle = SetMarkerStyle();
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