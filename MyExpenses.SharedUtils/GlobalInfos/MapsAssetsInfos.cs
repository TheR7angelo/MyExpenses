using System.Reflection;

namespace MyExpenses.SharedUtils.GlobalInfos;

public static class MapsAssetsInfos
{
    public static string BlueMarkerFilename => "BlueMarker.svg";
    public static string GreenMarkerFilename => "GreenMarker.svg";
    public static string RedMarkerFilename => "RedMarker.svg";
    public static string EmbeddedBleuMarkerFilePath { get; }
    public static string EmbeddedGreenMarkerFilePath { get; }
    public static string EmbeddedRedMarkerFilePath { get; }

    static MapsAssetsInfos()
    {
        var assembly = Assembly.GetEntryAssembly()
                       ?? AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetManifestResourceNames().Any(r =>
                           r.EndsWith(BlueMarkerFilename) ||
                           r.EndsWith(GreenMarkerFilename) ||
                           r.EndsWith(RedMarkerFilename)));

        var resources = assembly.GetManifestResourceNames();

        EmbeddedBleuMarkerFilePath = resources.First(s => s.EndsWith(BlueMarkerFilename));
        EmbeddedGreenMarkerFilePath = resources.First(s => s.EndsWith(GreenMarkerFilename));
        EmbeddedRedMarkerFilePath = resources.First(s => s.EndsWith(RedMarkerFilename));
    }
}