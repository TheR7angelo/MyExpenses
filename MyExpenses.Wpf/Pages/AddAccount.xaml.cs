using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Regex;
using MyExpenses.Wpf.Resources.Resx.AddAccount;

namespace MyExpenses.Wpf.Pages;

public partial class AddAccount
{
    #region Resx

    public string TextBoxAccountName { get; } = AddAccountResources.TextBoxAccountName;
    public string ComboBoxAccountType { get; } = AddAccountResources.ComboBoxAccountType;
    public string ComboBoxAccountCurrency { get; } = AddAccountResources.ComboBoxAccountCurrency;
    public string LabelIsAccountActive { get; } = AddAccountResources.LabelIsAccountActive;
    public string TextBoxAccountStartingBalance { get; } = AddAccountResources.TextBoxAccountStartingBalance;
    private string MsgBoxErrorAccountNameAlreadyExists { get; } = AddAccountResources.MsgBoxErrorAccountNameAlreadyExists;

    #endregion

    public TAccount Account { get; } = new();

    public string DisplayMemberPathAccountType => nameof(TAccountType.Name);
    public string DisplayMemberPathCurrencie => nameof(TCurrency.Currency);
    public List<TAccountType> AccountTypes { get; }
    public List<TCurrency> Currencies { get; }
    public double StartingBalance { get; set; }

    private List<TAccount> Accounts { get; }

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
        => MessageBox.Show(MsgBoxErrorAccountNameAlreadyExists);

    private bool CheckAccountName(string accountName)
        => Accounts.Select(s => s.Name).Contains(accountName);

    private void TextBoxStartingBalance_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        e.Handled = txt.IsOnlyDecimal();
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}