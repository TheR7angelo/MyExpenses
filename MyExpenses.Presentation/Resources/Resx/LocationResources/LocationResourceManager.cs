using MyExpenses.Presentation.Resources.Resx.ExpenseResources;

namespace MyExpenses.Presentation.Resources.Resx.LocationResources;

public static class LocationResourceManager
{
    private static string RessourceManagerName => nameof(LocationResources);

    public static string ComboBoxBaseMapHintAssist => $"{RessourceManagerName}:{nameof(LocationResources.ComboBoxBaseMapHintAssist)}";
}