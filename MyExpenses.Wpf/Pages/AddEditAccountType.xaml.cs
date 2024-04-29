using System.Windows;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.AddEditAccountType;

namespace MyExpenses.Wpf.Pages;

public partial class AddEditAccountType
{
    public TAccountType AccountType { get; } = new();

    public List<TAccountType> AccountTypes { get; }

    public string TextBoxAccountTypeName { get; } = AddEditAccountTypeResources.TextBoxAccountTypeName;
    public string ButtonValidContent { get; } = AddEditAccountTypeResources.ButtonValidContent;
    public string ButtonDeleteContent { get; } = AddEditAccountTypeResources.ButtonDeleteContent;
    public string ButtonCancelContent { get; } = AddEditAccountTypeResources.ButtonCancelContent;

    public AddEditAccountType()
    {
        using var context = new DataBaseContext();
        AccountTypes = [..context.TAccountTypes];

        InitializeComponent();
    }

    private void TextBoxAccountType_OnLostFocus(object sender, RoutedEventArgs e)
    {
        var accountTypeName = AccountType.Name;
        if (string.IsNullOrEmpty(accountTypeName)) return;

        var alreadyExist = CheckAccountName(accountTypeName);
        if (alreadyExist) MessageBox.Show("Account type name already exist");
    }

    private bool CheckAccountName(string accountName)
        => AccountTypes.Select(s => s.Name).Contains(accountName);
}