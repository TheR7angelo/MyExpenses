namespace MyExpenses.Presentation.Resources.Resx.LocationResources;

public static class LocationResourceManager
{
    private static string RessourceManagerName => nameof(LocationResources);

    public static string ComboBoxBaseMapHintAssist => $"{RessourceManagerName}:{nameof(LocationResources.ComboBoxBaseMapHintAssist)}";

    public static string MenuItemHeaderAddPoint => $"{RessourceManagerName}:{nameof(LocationResources.MenuItemHeaderAddPoint)}";
    public static string MenuItemHeaderEditFeature => $"{RessourceManagerName}:{nameof(LocationResources.MenuItemHeaderEditFeature)}";
    public static string MenuItemHeaderDeleteFeature => $"{RessourceManagerName}:{nameof(LocationResources.MenuItemHeaderDeleteFeature)}";

    public static string MenuItemHeaderMaps => $"{RessourceManagerName}:{nameof(LocationResources.MenuItemHeaderMaps)}";
    public static string MenuItemHeaderGoogleEarthWeb => $"{RessourceManagerName}:{nameof(LocationResources.MenuItemHeaderGoogleEarthWeb)}";
    public static string MenuItemHeaderGoogleMaps => $"{RessourceManagerName}:{nameof(LocationResources.MenuItemHeaderGoogleMaps)}";
    public static string MenuItemHeaderGoogleStreetView => $"{RessourceManagerName}:{nameof(LocationResources.MenuItemHeaderGoogleStreetView)}";
}