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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // Creates a static Offset instance with predefined X and Y values,
    // ensuring consistent positioning throughout the application.
    private static Offset Offset => new() { X = 0, Y = 222 };
    private static double Scale => 0.1;

    private static SymbolStyle SetGreenMarkerStyle()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Constructs a new SymbolStyle instance, defining the marker appearance
        // with a specified image source, offset, and scale for consistent styling.
        return new SymbolStyle
        {
            ImageSource = MapsAssetsInfos.GreenMarkerFilePath,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }

    private static SymbolStyle SetRedMarkerStyle()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Constructs a new SymbolStyle instance, defining the marker appearance
        // with a specified image source, offset, and scale for consistent styling.
        return new SymbolStyle
        {
            ImageSource = MapsAssetsInfos.RedMarkerFilePath,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }

    private static SymbolStyle SetBlueMarkerStyle()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Constructs a new SymbolStyle instance, defining the marker appearance
        // with a specified image source, offset, and scale for consistent styling.
        return new SymbolStyle
        {
            ImageSource = MapsAssetsInfos.BleuMarkerFilePath,
            SymbolOffset = Offset,
            SymbolScale = Scale
        };
    }
}