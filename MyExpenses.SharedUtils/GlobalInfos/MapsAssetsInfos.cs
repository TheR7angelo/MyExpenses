namespace MyExpenses.SharedUtils.GlobalInfos;

public static class MapsAssetsInfos
{
    private static string MapsAssetsBasePath => Path.Join(OsInfos.OsBasePath, "Assets", "Maps");

    public static string BleuMarkerFilePath => Path.Join(MapsAssetsBasePath, "BlueMarker.svg");
    public static string GreenMarkerFilePath => Path.Join(MapsAssetsBasePath, "GreenMarker.svg");
    public static string RedMarkerFilePath => Path.Join(MapsAssetsBasePath, "RedMarker.svg");
}