using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;

namespace MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;

public static class AddEditAccountResourceManager
{
    #region AddEditAccount

    private static string RessourceManagerNameAddEditAccount => nameof(AddEditAccountResources);

    public static string RessourceAddEditAccountButtonCancelContent { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.ButtonCancelContent)}";
    public static string RessourceAddEditAccountButtonDeleteContent { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.ButtonDeleteContent)}";
    public static string RessourceAddEditAccountButtonValidContent { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.ButtonValidContent)}";
    public static string RessourceAddEditAccountHintAssistComboBoxAccountType { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.ComboBoxAccountType)}";
    public static string RessourceAddEditAccountHintAssistComboBoxAccountCategoryType { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.ComboBoxAccountCategoryType)}";
    public static string RessourceAddEditAccountHintAssistComboBoxAccountCurrency { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.ComboBoxAccountCurrency)}";
    public static string RessourceAddEditAccountHintAssistTextBoxAccountStartingBalance { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.TextBoxAccountStartingBalance)}";
    public static string RessourceAddEditAccountHintAssistTextBoxAccountStartingBalanceDescription { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.TextBoxAccountStartingBalanceDescription)}";
    public static string RessourceAddEditAccountLabelIsAccountActive { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.LabelIsAccountActive)}";
    public static string RessourceAddEditAccountTextBoxAccountName { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.TextBoxAccountName)}";
    public static string RessourceAddEditAccountTitleWindow { get; } = $"{RessourceManagerNameAddEditAccount}:{nameof(AddEditAccountResources.TitleWindow)}";

    #endregion

    #region AccountType

    private static string RessourceManagerNameAddEditAccountType => nameof(AccountTypeManagementResources);

    public static string RessourceAccountTypeTitleWindow { get; } = $"{RessourceManagerNameAddEditAccountType}:{nameof(AccountTypeManagementResources.TitleWindow)}";
    public static string RessourceAccountTypeTextBoxAccountTypeName { get; } = $"{RessourceManagerNameAddEditAccountType}:{nameof(AccountTypeManagementResources.TextBoxAccountTypeName)}";
    public static string RessourceAccountTypeButtonValidContent { get; } = $"{RessourceManagerNameAddEditAccountType}:{nameof(AccountTypeManagementResources.ButtonValidContent)}";
    public static string RessourceAccountTypeButtonDeleteContent { get; } = $"{RessourceManagerNameAddEditAccountType}:{nameof(AccountTypeManagementResources.ButtonDeleteContent)}";
    public static string RessourceAccountTypeButtonCancelContent { get; } = $"{RessourceManagerNameAddEditAccountType}:{nameof(AccountTypeManagementResources.ButtonCancelContent)}";

    #endregion
}