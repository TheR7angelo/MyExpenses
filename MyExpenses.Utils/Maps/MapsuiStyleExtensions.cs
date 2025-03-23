using Mapsui.Styles;
using MyExpenses.SharedUtils.GlobalInfos;

namespace MyExpenses.Utils.Maps;

public static class MapsuiStyleExtensions
{
    public static ImageStyle RedMarkerStyle { get; private set; }
    public static ImageStyle GreenMarkerStyle { get; private set; }
    public static ImageStyle BlueMarkerStyle { get; private set; }

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

    private static string EmbeddedString => "embedded://";
    private static string FileString => "file://";

    private static ImageStyle SetGreenMarkerStyle()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Constructs a new ImageStyle instance, defining the marker appearance
        // with a specified image source, offset, and scale for consistent styling.
        return new ImageStyle
        {
            // Image = $"{EmbeddedString}{MapsAssetsInfos.GreenMarkerFilePath}",
            Image = $"{FileString}{MapsAssetsInfos.GreenMarkerFilePath}",
            // Image = MapsAssetsInfos.EmbeddedGreenMarkerFilePath,
            Offset = Offset,
            SymbolScale = Scale
        };
    }

    private static ImageStyle SetRedMarkerStyle()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Constructs a new ImageStyle instance, defining the marker appearance
        // with a specified image source, offset, and scale for consistent styling.
        return new ImageStyle
        {
            // Image = $"{EmbeddedString}{MapsAssetsInfos.RedMarkerFilePath}",
            Image = $"{FileString}{MapsAssetsInfos.RedMarkerFilePath}",
            // Image = MapsAssetsInfos.EmbeddedRedMarkerFilePath,
            Offset = Offset,
            SymbolScale = Scale
        };
    }

    private static ImageStyle SetBlueMarkerStyle()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Constructs a new ImageStyle instance, defining the marker appearance
        // with a specified image source, offset, and scale for consistent styling.
        return new ImageStyle
        {
            // Image = $"{EmbeddedString}{MapsAssetsInfos.BleuMarkerFilePath}",
            Image = $"{FileString}{MapsAssetsInfos.BleuMarkerFilePath}",
            // Image = MapsAssetsInfos.EmbeddedBleuMarkerFilePath,
            Offset = Offset,
            SymbolScale = Scale
        };
    }
}