namespace MyExpenses.Presentation.Resources.Resx.AccountResources;

public static class AccountResourceManager
{
    private static string RessourceManagerName => nameof(AccountResources);

    public static string RessourceAccountTextBoxAddNewAccountTypeName => $"{RessourceManagerName}:{nameof(AccountResources.TextBoxAddNewAccountTypeName)}";
}