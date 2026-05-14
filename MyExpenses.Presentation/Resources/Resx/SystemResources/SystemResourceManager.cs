namespace MyExpenses.Presentation.Resources.Resx.SystemResources;

public static class SystemResourceManager
{
    private static string RessourceManagerNameSystemRessources => nameof(SystemResources);

    public static string TitleWindowAddColor { get; } = $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.TitleWindowAddColor)}";
    public static string TitleWindowEditColor { get; } = $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.TitleWindowEditColor)}";
    public static string TextBoxColorNameHintAssist { get; } = $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.TextBoxColorNameHintAssist)}";
    public static string ButtonValidContentColor { get; } = $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.ButtonValidContentColor)}";
    public static string ButtonCancelContentColor { get; } = $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.ButtonCancelContentColor)}";
    public static string ButtonDeleteContentColor { get; } = $"{RessourceManagerNameSystemRessources}:{nameof(SystemResources.ButtonDeleteContentColor)}";
}