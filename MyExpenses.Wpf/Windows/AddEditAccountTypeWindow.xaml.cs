using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.WindowStyle;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditAccountTypeWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountTypeWindow
{
    public static readonly DependencyProperty EditAccountTypeProperty =
        DependencyProperty.Register(nameof(EditAccountType), typeof(bool), typeof(AddEditAccountTypeWindow),
            new PropertyMetadata(default(bool)));

    #region Property

    public TAccountType AccountType { get; } = new();

    private List<TAccountType> AccountTypes { get; }

    public bool EditAccountType
    {
        get => (bool)GetValue(EditAccountTypeProperty);
        set => SetValue(EditAccountTypeProperty, value);
    }

    public bool AccountTypeDeleted { get; set; }

    #endregion

    #region Resx

    public string TextBoxAccountTypeName { get; } = AddEditAccountTypeWindowResources.TextBoxAccountTypeName;
    public string ButtonValidContent { get; } = AddEditAccountTypeWindowResources.ButtonValidContent;
    public string ButtonDeleteContent { get; } = AddEditAccountTypeWindowResources.ButtonDeleteContent;
    public string ButtonCancelContent { get; } = AddEditAccountTypeWindowResources.ButtonCancelContent;

    #endregion

    public AddEditAccountTypeWindow()
    {
        using var context = new DataBaseContext();
        AccountTypes = [..context.TAccountTypes];

        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);

        TextBoxAccountType.Focus();
    }

        #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(AddEditAccountTypeWindowResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the account type \"{AccountToDeleteName}\"", AccountType.Name);
        var (success, exception) = AccountType.Delete();

        if (success)
        {
            Log.Information("Account was successfully removed");
            MsgBox.MsgBox.Show(AddEditAccountTypeWindowResources.MessageBoxDeleteAccountTypeNoUseSuccess, MsgBoxImage.Check);

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

            response = MsgBox.MsgBox.Show(AddEditAccountTypeWindowResources.MessageBoxDeleteAccountTypeUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the account type \"{AccountTypeToDeleteName}\" with all relative element",
                AccountType.Name);
            AccountType.Delete(true);
            Log.Information("Account type and all relative element was successfully removed");
            MsgBox.MsgBox.Show(AddEditAccountTypeWindowResources.MessageBoxDeleteAccountTypeUseSuccess, MsgBoxImage.Check);

            AccountTypeDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AddEditAccountTypeWindowResources.MessageBoxDeleteAccountTypeError, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var accountTypeName = AccountType.Name;

        if (string.IsNullOrEmpty(accountTypeName))
        {
            MsgBox.MsgBox.Show(AddEditAccountTypeWindowResources.MessageBoxAccountTypeNameCannotEmptyError, MsgBoxImage.Error);
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

    public void SetTAccountType(TAccountType accountType)
    {
        accountType.CopyPropertiesTo(AccountType);
        EditAccountType = true;

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

    #endregion

    #region Function

    private bool CheckAccountTypeName(string accountName)
        => AccountTypes.Select(s => s.Name).Contains(accountName);

    private void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AddEditAccountTypeWindowResources.MessageBoxAccountTypeNameAlreadyExists,
            MsgBoxImage.Warning);

    #endregion
}