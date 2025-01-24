using Mapsui.Styles;
using MyExpenses.SharedUtils.GlobalInfos;

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

    private static SymbolStyle SetGreenMarkerStyle()
    {
        return new SymbolStyle
        {
            ImageSource = MapsAssetsInfos.GreenMarkerFilePath,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }

    private static SymbolStyle SetRedMarkerStyle()
    {
        return new SymbolStyle
        {
            ImageSource = MapsAssetsInfos.RedMarkerFilePath,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }

    private static SymbolStyle SetBlueMarkerStyle()
    {
        return new SymbolStyle
        {
            ImageSource = MapsAssetsInfos.BleuMarkerFilePath,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }
}