using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountTypeWindow
{
    #region Property

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TAccountType AccountType { get; } = new();

    private List<TAccountType> AccountTypes { get; }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditAccountTypeProperty =
        DependencyProperty.Register(nameof(EditAccountType), typeof(bool), typeof(AddEditAccountTypeWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditAccountType
    {
        get => (bool)GetValue(EditAccountTypeProperty);
        set => SetValue(EditAccountTypeProperty, value);
    }

    public bool AccountTypeDeleted { get; private set; }

    #endregion

    public AddEditAccountTypeWindow()
    {
        // TODO injector DTO MODEL VIEW

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        using var context = new DataBaseContextOld();
        AccountTypes = [..context.TAccountTypes];

        InitializeComponent();

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

    #endregion

    #region Function

    private bool CheckAccountTypeName(string accountName)
        => AccountTypes.Select(s => s.Name).Contains(accountName);

    private static void ShowErrorMessage()
        => MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorAlreadyExistMessage,
            MsgBoxImage.Warning);

    #endregion
}