namespace MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;

public static class AddEditAccountResourceManager
{
    private static string RessourceManagerName => nameof(AddEditAccountResources);
    public static string RessourceButtonCancelContent { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.ButtonCancelContent)}";
    public static string RessourceButtonDeleteContent { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.ButtonDeleteContent)}";
    public static string RessourceButtonValidContent { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.ButtonValidContent)}";
    public static string RessourceHintAssistComboBoxAccountType { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.ComboBoxAccountType)}";
    public static string RessourceHintAssistComboBoxAccountCategoryType { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.ComboBoxAccountCategoryType)}";
    public static string RessourceHintAssistComboBoxAccountCurrency { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.ComboBoxAccountCurrency)}";
    public static string RessourceHintAssistTextBoxAccountStartingBalance { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.TextBoxAccountStartingBalance)}";
    public static string RessourceHintAssistTextBoxAccountStartingBalanceDescription { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.TextBoxAccountStartingBalanceDescription)}";
    public static string RessourceLabelIsAccountActive { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.LabelIsAccountActive)}";
    public static string RessourceTextBoxAccountName { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.TextBoxAccountName)}";
    public static string RessourceTitleWindow { get; } = $"{RessourceManagerName}:{nameof(AddEditAccountResources.TitleWindow)}";
}