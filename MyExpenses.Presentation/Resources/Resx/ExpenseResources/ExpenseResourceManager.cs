namespace MyExpenses.Presentation.Resources.Resx.ExpenseResources;

public class ExpenseResourceManager
{
    private static string RessourceManagerNameDependencyRessources => nameof(ExpenseResources);

    public static string ComboBoxFromAccountHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ComboBoxFromAccountHintAssist)}";
    public static string ComboBoxToAccountHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ComboBoxToAccountHintAssist)}";
    public static string ComboBoxCategoryTypeHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ComboBoxCategoryTypeHintAssist)}";
    public static string ComboBoxModePaymentHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ComboBoxModePaymentHintAssist)}";
    public static string DatePickerWhenHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.DatePickerWhenHintAssist)}";
    public static string TextBoxValueHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.TextBoxValueHintAssist)}";
    public static string TextBoxMainReasonHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.TextBoxMainReasonHintAssist)}";
    public static string TextBoxAdditionalReasonHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.TextBoxAdditionalReasonHintAssist)}";
    public static string ButtonPrepareValidContent { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ButtonPrepareValidContent)}";
    public static string ButtonPrepareCancelContent { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ButtonPrepareCancelContent)}";
    public static string ButtonPreviewValidContent { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ButtonPreviewValidContent)}";
    public static string ButtonPreviewCancelContent { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ButtonPreviewCancelContent)}";

    public static string TitleWindowAddCategoryTypeName { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.TitleWindowAddCategoryTypeName)}";
    public static string TitleWindowEditCategoryTypeName { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.TitleWindowEditCategoryTypeName)}";
    public static string TextBoxCategoryTypeNameHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.TextBoxCategoryTypeNameHintAssist)}";
    public static string ComboBoxColorValueHintAssist { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ComboBoxColorValueHintAssist)}";
    public static string ButtonValidContentCategoryType { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ButtonValidContentCategoryType)}";
    public static string ButtonCancelContentCategoryType { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ButtonCancelContentCategoryType)}";
    public static string ButtonDeleteContentCategoryType { get; } = $"{RessourceManagerNameDependencyRessources}:{nameof(ExpenseResources.ButtonDeleteContentCategoryType)}";
}