using System.Windows;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class AddEditAccountType
{
    public TAccountType AccountType { get; } = new();

    public List<TAccountType> AccountTypes { get; }

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