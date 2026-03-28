namespace MyExpenses.SharedUtils.Resources.Resx;

public static class DialogResourceManager
{
    private static string RessourceManagerDialog => nameof(Dialogs.DialogResources);

    public static string ButtonValidContent { get; } = $"{RessourceManagerDialog}:{nameof(Dialogs.DialogResources.ValidButton)}";
    public static string ButtonCancelContent { get; } = $"{RessourceManagerDialog}:{nameof(Dialogs.DialogResources.CancelButton)}";
    public static string ButtonDeleteContent { get; } = $"{RessourceManagerDialog}:{nameof(Dialogs.DialogResources.DeleteButton)}";

}