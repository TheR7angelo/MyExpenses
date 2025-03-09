using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountTypeWindow
{
    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditAccountTypeProperty =
        DependencyProperty.Register(nameof(EditAccountType), typeof(bool), typeof(AddEditAccountTypeWindow),
            new PropertyMetadata(false));

    #region Property

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TAccountType AccountType { get; } = new();

    private List<TAccountType> AccountTypes { get; }

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditAccountType
    {
        get => (bool)GetValue(EditAccountTypeProperty);
        set => SetValue(EditAccountTypeProperty, value);
    }

    public bool AccountTypeDeleted { get; private set; }

    #endregion

    #region Resx

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxAccountTypeNameProperty =
        DependencyProperty.Register(nameof(TextBoxAccountTypeName), typeof(string), typeof(AddEditAccountTypeWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxAccountTypeName
    {
        get => (string)GetValue(TextBoxAccountTypeNameProperty);
        set => SetValue(TextBoxAccountTypeNameProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonValidContentProperty = DependencyProperty.Register(
        nameof(ButtonValidContent),
        typeof(string), typeof(AddEditAccountTypeWindow), new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonDeleteContentProperty =
        DependencyProperty.Register(nameof(ButtonDeleteContent), typeof(string), typeof(AddEditAccountTypeWindow),
            new PropertyMetadata(default(string)));

    public string ButtonDeleteContent
    {
        get => (string)GetValue(ButtonDeleteContentProperty);
        set => SetValue(ButtonDeleteContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(AddEditAccountTypeWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(AddEditAccountTypeWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    #endregion

    public AddEditAccountTypeWindow()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        using var context = new DataBaseContext();
        AccountTypes = [..context.TAccountTypes];

        UpdateLanguage();
        InitializeComponent();
        TextBoxAccountType.Focus();

        this.SetWindowCornerPreference();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the account type \"{AccountToDeleteName}\"", AccountType.Name);
        var (success, exception) = AccountType.Delete();

        if (success)
        {
            Log.Information("Account was successfully removed");
            MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteAccountTypeNoUseSuccess,
                MsgBoxImage.Check);

            AccountTypeDeleted = true;
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

            response = MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteAccountTypeUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information(
                "Attempting to remove the account type \"{AccountTypeToDeleteName}\" with all relative element",
                AccountType.Name);
            AccountType.Delete(true);
            Log.Information("Account type and all relative element was successfully removed");
            MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteAccountTypeUseSuccess,
                MsgBoxImage.Check);

            AccountTypeDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteAccountTypeNoUseSuccess, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var accountTypeName = AccountType.Name;

        if (string.IsNullOrEmpty(accountTypeName))
        {
            MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorEmptyMessage,
                MsgBoxImage.Error);
            return;
        }

        var alreadyExist = CheckAccountTypeName(accountTypeName);
        if (alreadyExist) ShowErrorMessage();
        else
        {
            DialogResult = true;
            Close();
        }
    }

    // ReSharper disable once HeapView.ClosureAllocation
    public void SetTAccountType(TAccountType accountType)
    {
        accountType.CopyPropertiesTo(AccountType);
        EditAccountType = true;

        // ReSharper disable once HeapView.DelegateAllocation
        var oldItem = AccountTypes.FirstOrDefault(s => s.Id == accountType.Id);
        if (oldItem is null) return;
        AccountTypes.Remove(oldItem);
    }

    private void TextBoxAccountType_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var accountTypeName = textBox.Text;
        if (string.IsNullOrEmpty(accountTypeName)) return;

        var alreadyExist = CheckAccountTypeName(accountTypeName);
        if (alreadyExist) ShowErrorMessage();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private bool CheckAccountTypeName(string accountName)
        => AccountTypes.Select(s => s.Name).Contains(accountName);

    private static void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorAlreadyExistMessage,
            MsgBoxImage.Warning);

    private void UpdateLanguage()
    {
        TitleWindow = AccountTypeManagementResources.TitleWindow;

        TextBoxAccountTypeName = AccountTypeManagementResources.TextBoxAccountTypeName;
        ButtonValidContent = AccountTypeManagementResources.ButtonValidContent;
        ButtonDeleteContent = AccountTypeManagementResources.ButtonDeleteContent;
        ButtonCancelContent = AccountTypeManagementResources.ButtonCancelContent;
    }

    #endregion
}