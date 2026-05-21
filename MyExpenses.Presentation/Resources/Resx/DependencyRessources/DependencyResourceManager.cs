namespace MyExpenses.Presentation.Resources.Resx.DependencyRessources;

public static class DependencyResourceManager
{
    private static string RessourceManagerNameDependencyRessources => nameof(DependencyRessources);

    public static string RessourceTitleWindow => $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.DependenciesWindowTitle)}";
    public static string ConfirmDeleteTitle => $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.ConfirmDeleteTitle)}";
    public static string DeleteEverything => $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.DeleteEverything)}";

    public static string CancelButton => $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.CancelButton)}";
    public static string DeleteQuestion => $"{RessourceManagerNameDependencyRessources}:{nameof(DependencyRessources.DeleteQuestion)}";
}