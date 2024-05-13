using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Regex;
using MyExpenses.Wpf.Resources.Resx.Windows.AddAccountWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountWindow
{
    #region DependecyProperty

    public static readonly DependencyProperty EnableStartingBalanceProperty =
        DependencyProperty.Register(nameof(EnableStartingBalance), typeof(bool), typeof(AddEditAccountWindow),
            new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty EditAccountProperty =
        DependencyProperty.Register(nameof(EditAccount), typeof(bool), typeof(AddEditAccountWindow),
            new PropertyMetadata(default(bool)));

    #endregion

    #region Resx

    #region HintAssist

    public string HintAssistTextBoxAccountName { get; } = AddEditAccountWindowResources.TextBoxAccountName;
    public string HintAssistComboBoxAccountType { get; } = AddEditAccountWindowResources.ComboBoxAccountType;

    public string HintAssistComboBoxAccountCategoryType { get; } =
        AddEditAccountWindowResources.ComboBoxAccountCategoryType;

    public string HintAssistComboBoxAccountCurrency { get; } = AddEditAccountWindowResources.ComboBoxAccountCurrency;

    public string HintAssistTextBoxAccountStartingBalance { get; } =
        AddEditAccountWindowResources.TextBoxAccountStartingBalance;

    public string HintAssistTextBoxAccountStartingBalanceDescription { get; } =
        AddEditAccountWindowResources.TextBoxAccountStartingBalanceDescription;

    #endregion

    #region Button

    public string ButtonCancelContent { get; } = AddEditAccountWindowResources.ButtonCancelContent;
    public string ButtonDeleteContent { get; } = AddEditAccountWindowResources.ButtonDeleteContent;
    public string ButtonValidContent { get; } = AddEditAccountWindowResources.ButtonValidContent;

    #endregion

    public string LabelIsAccountActive { get; } = AddEditAccountWindowResources.LabelIsAccountActive;

    private string MsgBoxErrorAccountNameAlreadyExists { get; } =
        AddEditAccountWindowResources.MsgBoxErrorAccountNameAlreadyExists;

    #endregion

    #region Property

    public bool DeleteAccount { get; private set; }

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

    public bool EditAccount
    {
        get => (bool)GetValue(EditAccountProperty);
        set => SetValue(EditAccountProperty, value);
    }

    #endregion

    public AddEditAccountWindow()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        AccountTypes = [..context.TAccountTypes.OrderBy(s => s.Name)];
        Currencies = [..context.TCurrencies.OrderBy(s => s.Symbol)];
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    #region Action

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
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxAddAccountTypeSuccess, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxAddAccountTypeError, MsgBoxImage.Error);
        }
    }

    private void ButtonAddCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO re work

        // var addEditCategoryType = new AddEditCategoryTypeWindow();
        // var result = addEditCategoryType.ShowDialog();
        // if (result != true) return;
        //
        // var newCategoryType = addEditCategoryType.CategoryType;
        //
        // Log.Information("Attempting to inject the new category type \"{NewCategoryTypeName}\"", newCategoryType.Name);
        // var (success, exception) = newCategoryType.AddOrEdit();
        // if (success)
        // {
        //     CategoryTypes.Add(newCategoryType);
        //     History.CategoryTypeFk = newCategoryType.Id;
        //     Log.Information("Account type was successfully added");
        //     MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxAddCurrencySuccess, MsgBoxImage.Check);
        // }
        // else
        // {
        //     Log.Error(exception, "An error occurred please retry");
        //     MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxAddCurrencyError, MsgBoxImage.Error);
        // }
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
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxAddCurrencySuccess, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxAddCurrencyError, MsgBoxImage.Error);
        }
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(string.Format(AddEditAccountWindowResources.MessageBoxDeleteAccountQuestion, Account.Name), MsgBoxImage.Question,
            MessageBoxButton.YesNoCancel);
        if (response != MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the account \"{AccountToDeleteName}\"", Account.Name);
        var (success, exception) = Account.Delete();

        if (success)
        {
            Log.Information("Account was successfully removed");
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxDeleteAccountNoUseSuccess, MsgBoxImage.Check);
            DeleteAccount = true;

            DialogResult = true;
            Close();
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxDeleteAccountUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response != MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the account \"{AccountToDeleteName}\" with all relative element",
                Account.Name);
            Account.Delete(true);
            Log.Information("Account and all relative element was successfully removed");
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxDeleteAccountUseSuccess, MsgBoxImage.Check);

            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AddEditAccountWindowResources.MessageBoxDeleteAccountError, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var error = CheckError();
        if (error) return;

        DialogResult = true;
        Close();
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

    #endregion

    #region Function

    private bool CheckAccountName(string accountName)
        => !EditAccount && Accounts.Select(s => s.Name).Contains(accountName);


    private bool CheckError()
    {
        if (string.IsNullOrEmpty(Account.Name))
        {
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MsgBoxErrorAccountNameCannotByEmpty, MsgBoxImage.Warning);
            return true;
        }

        var errorName = CheckAccountName(Account.Name);
        if (errorName)
        {
            MsgBox.MsgBox.Show(MsgBoxErrorAccountNameAlreadyExists, MsgBoxImage.Warning);
            return true;
        }

        if (Account.AccountTypeFk is null)
        {
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MsgBoxErrorAccountTypeCannotByEmpty, MsgBoxImage.Warning);
            return true;
        }

        if (Account.CurrencyFk is null)
        {
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MsgBoxErrorAccountCurrencyCannotByEmpty,
                MsgBoxImage.Warning);
            return true;
        }

        if (EnableStartingBalance is false) return false;

        if (string.IsNullOrEmpty(History.Description))
        {
            MsgBox.MsgBox.Show(AddEditAccountWindowResources.MsgBoxErrorAccountStartingBalanceDescriptionCannotByEmpty,
                MsgBoxImage.Warning);
            return true;
        }

        return false;
    }

    private void DisplayErrorAccountName()
        => MsgBox.MsgBox.Show(MsgBoxErrorAccountNameAlreadyExists, MsgBoxImage.Warning);


    public void SetTAccount(TAccount account)
    {
        account.CopyPropertiesTo(Account);
        EditAccount = true;
    }

    #endregion
}