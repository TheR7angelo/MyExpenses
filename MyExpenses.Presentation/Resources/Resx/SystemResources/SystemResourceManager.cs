namespace MyExpenses.Presentation.Resources.Resx.SystemResources;

public static class SystemResourceManager
{
    private static string RessourceManagerNameSystemRessources => nameof(SystemResources);

    public static string TitleWindowAddColor => $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.TitleWindowAddColor)}";
    public static string TitleWindowEditColor => $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.TitleWindowEditColor)}";
    public static string TextBoxColorNameHintAssist => $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.TextBoxColorNameHintAssist)}";
    public static string ButtonValidContentColor => $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.ButtonValidContentColor)}";
    public static string ButtonCancelContentColor => $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.ButtonCancelContentColor)}";
    public static string ButtonDeleteContentColor => $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.ButtonDeleteContentColor)}";
}