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

    private static Offset Offset => new() { X = 0, Y = 222 };
    private static double Scale => 0.1;

    private static readonly string IcoPath = Path.Join(AppContext.BaseDirectory, "Resources", "Maps");

    private static SymbolStyle SetGreenMarkerStyle()
    {
        var icon = Path.Join(IcoPath, "GreenMarker.svg");

        return new SymbolStyle
        {
            ImageSource = icon,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }

    private static SymbolStyle SetRedMarkerStyle()
    {
        var icon = Path.Join(IcoPath, "RedMarker.svg");

        return new SymbolStyle
        {
            ImageSource = icon,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }

    private static SymbolStyle SetBlueMarkerStyle()
    {
        var icon = Path.Join(IcoPath, "BlueMarker.svg");

        return new SymbolStyle
        {
            ImageSource = icon,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }
}