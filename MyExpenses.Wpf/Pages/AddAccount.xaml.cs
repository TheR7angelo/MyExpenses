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
    public static readonly DependencyProperty EnableStartingBalanceProperty =
        DependencyProperty.Register(nameof(EnableStartingBalance), typeof(bool), typeof(AddAccount),
            new PropertyMetadata(default(bool)));

    #region Resx

    public string TextBoxAccountName { get; } = AddAccountResources.TextBoxAccountName;
    public string ComboBoxAccountType { get; } = AddAccountResources.ComboBoxAccountType;
    public string ComboBoxAccountCurrency { get; } = AddAccountResources.ComboBoxAccountCurrency;
    public string LabelIsAccountActive { get; } = AddAccountResources.LabelIsAccountActive;
    public string TextBoxAccountStartingBalance { get; } = AddAccountResources.TextBoxAccountStartingBalance;

    public string TextBoxAccountStartingBalanceDescription { get; } = AddAccountResources.TextBoxAccountStartingBalanceDescription;
    public string ComboBoxAccountCategoryType { get; } = AddAccountResources.ComboBoxAccountCategoryType;
    private string MsgBoxErrorAccountNameAlreadyExists { get; } = AddAccountResources.MsgBoxErrorAccountNameAlreadyExists;

    #endregion

    public TAccount Account { get; } = new();
    public THistory History { get; } = new() { Pointed = true };

    public string DisplayMemberPathAccountType => nameof(TAccountType.Name);
    public string SelectedValuePathAccountType => nameof(TAccountType.Id);
    public string DisplayMemberPathCurrency => nameof(TCurrency.Currency);
    public string SelectedValuePathCurrency => nameof(TCurrency.Id);
    public string DisplayMemberPathCategoryType => nameof(TCategoryType.Name);
    public string SelectedValuePathCategoryType => nameof(TCategoryType.Id);
    public List<TAccountType> AccountTypes { get; }
    public List<TCurrency> Currencies { get; }
    public List<TCategoryType> CategoryTypes { get; }

    private List<TAccount> Accounts { get; }

    public bool EnableStartingBalance
    {
        get => (bool)GetValue(EnableStartingBalanceProperty);
        set => SetValue(EnableStartingBalanceProperty, value);
    }

    public AddAccount()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts];
        AccountTypes = [..context.TAccountTypes];
        Currencies = [..context.TCurrencies];
        CategoryTypes = [..context.TCategoryTypes];

        InitializeComponent();
    }

    private void TextBoxAccountName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        var accountName = Account.Name;
        if (string.IsNullOrEmpty(accountName)) return;

        var alreadyExist = CheckAccountName(accountName);
        if (alreadyExist) DisplayErrorAccountName();
    }

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
        var error = CheckError();
        if (error) return;

        DialogResult = true;
        Close();
    }

    private bool CheckError()
    {
        if (string.IsNullOrEmpty(Account.Name))
        {
            MessageBox.Show(AddAccountResources.MsgBoxErrorAccountCannotByEmpty);
            return true;
        }

        var errorName = CheckAccountName(Account.Name);
        if (errorName)
        {
            MessageBox.Show(MsgBoxErrorAccountNameAlreadyExists);
            return true;
        }

        if (Account.AccountTypeFk is null)
        {
            MessageBox.Show(AddAccountResources.MsgBoxErrorAccountTypeCannotByEmpty);
            return true;
        }

        if (Account.CurrencyFk is null)
        {
            MessageBox.Show(AddAccountResources.MsgBoxErrorAccountCurrencyCannotByEmpty);
            return true;
        }

        if (EnableStartingBalance is false) return false;

        if (string.IsNullOrEmpty(History.Description))
        {
            MessageBox.Show("La description du départ du solde ne peut pas etre vide");
            return true;
        }

        return false;
    }

    private void DisplayErrorAccountName()
        => MessageBox.Show(MsgBoxErrorAccountNameAlreadyExists);

    private bool CheckAccountName(string accountName)
        => Accounts.Select(s => s.Name).Contains(accountName);
}