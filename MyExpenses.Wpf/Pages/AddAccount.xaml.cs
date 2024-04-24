using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class AddAccount
{
    public TAccount Account { get; } = new();
    public List<TAccount> Accounts { get; }

    public string DisplayMemberPathAccountType => nameof(TAccountType.Name);
    public string DisplayMemberPathCurrencie => nameof(TCurrency.Currency);
    public List<TAccountType> AccountTypes { get; }
    public List<TCurrency> Currencies { get; }
    public double StartingBalance { get; set; }

    public AddAccount()
    {
        using var context = new DataBaseContext();
        Accounts =  [..context.TAccounts];
        AccountTypes = [..context.TAccountTypes];
        Currencies = [..context.TCurrencies];

        InitializeComponent();
    }

    private void TextBoxAccountName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        var accountName = Account.Name;
        if (string.IsNullOrEmpty(accountName)) return;

        var alreadyExist = CheckAccountName(accountName);
        if (alreadyExist) DisplayErrorAccountName();
    }

    private void DisplayErrorAccountName()
        => MessageBox.Show("An account already exists with this name");

    private bool CheckAccountName(string accountName)
        => Accounts.Select(s => s.Name).Contains(accountName);

    private void TextBoxStartingBalance_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$|^[0-9]*[,]{0,1}[0-9]*$");
        var textBox = (TextBox)sender;
        e.Handled = !regex.IsMatch(textBox.Text.Insert(textBox.SelectionStart,e.Text));
    }
}