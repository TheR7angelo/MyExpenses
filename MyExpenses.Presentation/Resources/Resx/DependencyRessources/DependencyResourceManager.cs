namespace MyExpenses.Presentation.Resources.Resx.DependencyRessources;

public static class DependencyResourceManager
{
    private static string RessourceManagerNameDependencyRessources => nameof(DependencyRessources);

    public static string RessourceTitleWindow { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.DependenciesWindowTitle)}";
    public static string ConfirmDeleteTitle { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.ConfirmDeleteTitle)}";
    public static string DeleteEverything { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.DeleteEverything)}";

    public static string CancelButton { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.CancelButton)}";
    public static string DeleteQuestion { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.DeleteQuestion)}";
}