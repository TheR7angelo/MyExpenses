namespace MyExpenses.Presentation.Resources.Resx.ExpenseResources;

public class ExpenseResourceManager
{
    private static string RessourceManagerNameExpenseRessources => nameof(ExpenseResources);

    public static string ComboBoxFromAccountHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ComboBoxFromAccountHintAssist)}";
    public static string ComboBoxToAccountHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ComboBoxToAccountHintAssist)}";
    public static string ComboBoxCategoryTypeHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ComboBoxCategoryTypeHintAssist)}";
    public static string ComboBoxModePaymentHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ComboBoxModePaymentHintAssist)}";
    public static string DatePickerWhenHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.DatePickerWhenHintAssist)}";
    public static string TextBoxValueHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.TextBoxValueHintAssist)}";
    public static string TextBoxMainReasonHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.TextBoxMainReasonHintAssist)}";
    public static string TextBoxAdditionalReasonHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.TextBoxAdditionalReasonHintAssist)}";
    public static string ButtonPrepareValidContent { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ButtonPrepareValidContent)}";
    public static string ButtonPrepareCancelContent { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ButtonPrepareCancelContent)}";
    public static string ButtonPreviewValidContent { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ButtonPreviewValidContent)}";
    public static string ButtonPreviewCancelContent { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ButtonPreviewCancelContent)}";

    public static string TitleWindowAddCategoryTypeName { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.TitleWindowAddCategoryTypeName)}";
    public static string TitleWindowEditCategoryTypeName { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.TitleWindowEditCategoryTypeName)}";
    public static string TextBoxCategoryTypeNameHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.TextBoxCategoryTypeNameHintAssist)}";
    public static string ComboBoxColorValueHintAssist { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ComboBoxColorValueHintAssist)}";
    public static string ButtonValidContentCategoryType { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ButtonValidContentCategoryType)}";
    public static string ButtonCancelContentCategoryType { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ButtonCancelContentCategoryType)}";
    public static string ButtonDeleteContentCategoryType { get; } = $"{RessourceManagerNameExpenseRessources}:{nameof(ExpenseResources.ButtonDeleteContentCategoryType)}";
}