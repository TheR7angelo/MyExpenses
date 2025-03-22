using System.Reflection;

namespace MyExpenses.SharedUtils.GlobalInfos;

public static class MapsAssetsInfos
{
    public static string BlueMarkerFilename => "BlueMarker.svg";
    public static string GreenMarkerFilename => "GreenMarker.svg";
    public static string RedMarkerFilename => "RedMarker.svg";

    private static string MapsAssetsBasePath => Path.Join(OsInfos.OsBasePath, "Resources", "Assets", "Maps");

    public static string BleuMarkerFilePath => Path.Join(MapsAssetsBasePath, "BlueMarker.svg");
    public static string GreenMarkerFilePath => Path.Join(MapsAssetsBasePath, "GreenMarker.svg");
    public static string RedMarkerFilePath => Path.Join(MapsAssetsBasePath, "RedMarker.svg");

    public static string EmbeddedBleuMarkerFilePath { get; }
    public static string EmbeddedGreenMarkerFilePath { get; }
    public static string EmbeddedRedMarkerFilePath { get; }

    static MapsAssetsInfos()
    {
        // var assembly = Assembly.GetEntryAssembly()
        //                ?? AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetManifestResourceNames().Any(r =>
        //                    r.EndsWith(BlueMarkerFilename) ||
        //                    r.EndsWith(GreenMarkerFilename) ||
        //                    r.EndsWith(RedMarkerFilename)));
        //
        // var resources = assembly.GetManifestResourceNames();
        //
        // EmbeddedBleuMarkerFilePath = resources.First(s => s.EndsWith(BlueMarkerFilename));
        // EmbeddedGreenMarkerFilePath = resources.First(s => s.EndsWith(GreenMarkerFilename));
        // EmbeddedRedMarkerFilePath = resources.First(s => s.EndsWith(RedMarkerFilename));0

        EmbeddedBleuMarkerFilePath = "embedded://MyExpenses.SharedUtils.Resources.Assets.Maps.BlueMarker.svg";
        EmbeddedGreenMarkerFilePath = "embedded://MyExpenses.SharedUtils.Resources.Assets.Maps.GreenMarker.svg";
        EmbeddedRedMarkerFilePath = "embedded://MyExpenses.SharedUtils.Resources.Assets.Maps.RedMarker.svg";
    }
}