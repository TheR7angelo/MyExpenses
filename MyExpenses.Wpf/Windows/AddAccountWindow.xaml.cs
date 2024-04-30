using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Regex;
using MyExpenses.Wpf.Resources.Resx.AddAccount;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddAccountWindow
{
    public static readonly DependencyProperty EnableStartingBalanceProperty =
        DependencyProperty.Register(nameof(EnableStartingBalance), typeof(bool), typeof(AddAccountWindow),
            new PropertyMetadata(default(bool)));

    #region Resx

    public string HintAssistTextBoxAccountName { get; } = AddAccountResources.TextBoxAccountName;
    public string HintAssistComboBoxAccountType { get; } = AddAccountResources.ComboBoxAccountType;
    public string HintAssistComboBoxAccountCurrency { get; } = AddAccountResources.ComboBoxAccountCurrency;
    public string LabelIsAccountActive { get; } = AddAccountResources.LabelIsAccountActive;
    public string HintAssistTextBoxAccountStartingBalance { get; } = AddAccountResources.TextBoxAccountStartingBalance;

    public string HintAssistTextBoxAccountStartingBalanceDescription { get; } = AddAccountResources.TextBoxAccountStartingBalanceDescription;
    public string HintAssistComboBoxAccountCategoryType { get; } = AddAccountResources.ComboBoxAccountCategoryType;
    private string MsgBoxErrorAccountNameAlreadyExists { get; } = AddAccountResources.MsgBoxErrorAccountNameAlreadyExists;

    #endregion

    public TAccount Account { get; } = new();
    public THistory History { get; } = new() { Pointed = true };

    public string DisplayMemberPathAccountType => nameof(TAccountType.Name);
    public string SelectedValuePathAccountType => nameof(TAccountType.Id);
    public string DisplayMemberPathCurrency => nameof(TCurrency.Symbol);
    public string SelectedValuePathCurrency => nameof(TCurrency.Id);
    public string DisplayMemberPathCategoryType => nameof(TCategoryType.Name);
    public string SelectedValuePathCategoryType => nameof(TCategoryType.Id);
    public ObservableCollection<TAccountType> AccountTypes { get; }
    public ObservableCollection<TCurrency> Currencies { get; }
    public ObservableCollection<TCategoryType> CategoryTypes { get; }

    private List<TAccount> Accounts { get; }

    public bool EnableStartingBalance
    {
        get => (bool)GetValue(EnableStartingBalanceProperty);
        set => SetValue(EnableStartingBalanceProperty, value);
    }

    public AddAccountWindow()
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
            MessageBox.Show(AddAccountResources.MsgBoxErrorAccountNameCannotByEmpty);
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
            MessageBox.Show(AddAccountResources.MsgBoxErrorAccountStartingBalanceDescriptionCannotByEmpty);
            return true;
        }

        return false;
    }

    private void DisplayErrorAccountName()
        => MessageBox.Show(MsgBoxErrorAccountNameAlreadyExists);

    private bool CheckAccountName(string accountName)
        => Accounts.Select(s => s.Name).Contains(accountName);

    private void ButtonAddAccountType_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditAccountType = new AddEditAccountTypeWindow();
        var result = addEditAccountType.ShowDialog();
        if (result != true) return;

        var newAccountType = addEditAccountType.AccountType;

        Log.Information("Attempting to inject the new account type \"{NewAccountTypeName}\"", newAccountType.Name);
        var (success, exception) = newAccountType.AddOrEdit();
        if (success)
        {
            AccountTypes.Add(newAccountType);
            Account.AccountTypeFk = newAccountType.Id;
            Log.Information("Account type was successfully added");
            MessageBox.Show(AddAccountResources.MessageBoxAddAccountTypeSuccess);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MessageBox.Show(AddAccountResources.MessageBoxAddAccountTypeError);
        }
    }

    private void ButtonAddCurrency_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditCurrency = new AddEditCurrencyWindow();
        var result = addEditCurrency.ShowDialog();
        if (result != true) return;

        var newCurrency = addEditCurrency.Currency;

        Log.Information("Attempting to inject the new currency symbole \"{NewCurrencySymbole}\"", newCurrency.Symbol);
        var (success, exception) = newCurrency.AddOrEdit();
        if (success)
        {
            Currencies.Add(newCurrency);
            Account.CurrencyFk = newCurrency.Id;
            Log.Information("Account type was successfully added");
            MessageBox.Show(AddAccountResources.MessageBoxAddCurrencySuccess);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MessageBox.Show(AddAccountResources.MessageBoxAddCurrencyError);
        }
    }

    private void ButtonAddCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditCategoryType = new AddEditCategoryTypeWindow();
        var result = addEditCategoryType.ShowDialog();
        if (result != true) return;

        var newCategoryType = addEditCategoryType.CategoryType;

        Log.Information("Attempting to inject the new category type \"{NewCategoryTypeName}\"", newCategoryType.Name);
        var (success, exception) = newCategoryType.AddOrEdit();
        if (success)
        {
            CategoryTypes.Add(newCategoryType);
            History.CategoryTypeFk = newCategoryType.Id;
            Log.Information("Account type was successfully added");
            MessageBox.Show(AddAccountResources.MessageBoxAddCurrencySuccess);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MessageBox.Show(AddAccountResources.MessageBoxAddCurrencyError);
        }
    }
}