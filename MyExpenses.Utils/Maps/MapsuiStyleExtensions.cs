using Mapsui.Styles;

namespace MyExpenses.Utils.Maps;

public static class MapsuiStyleExtensions
{
    public static SymbolStyle RedMarkerStyle { get; private set; }
    public static SymbolStyle GreenMarkerStyle { get; private set; }
    public static SymbolStyle BlueMarkerStyle { get; private set; }

    static MapsuiStyleExtensions()
    {
        RedMarkerStyle = SetRedMarkerStyle();
        GreenMarkerStyle = SetGreenMarkerStyle();
        BlueMarkerStyle = SetBlueMarkerStyle();
    }

    // private static Offset Offset => new() { IsRelative = false, X = 0, Y = 1000 };
    private static Offset Offset => new() { X = 0, Y = 1000 };
    private static double Scale => 0.02;

    private static readonly string IcoPath = Path.Join(AppContext.BaseDirectory, "Resources", "Maps");

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

    private static SymbolStyle SetBlueMarkerStyle()
    {
        var icon = Path.Join(IcoPath, "BlueMarker.png");
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