using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Windows.Dialogs.MsgBox;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountTypeWindow
{
    #region Property

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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public AccountTypeViewModel AccountType { get; } = new();

    #endregion

    private readonly IAccountPresentationService _accountPresentationServiceService;

    public AddEditAccountTypeWindow(IAccountPresentationService accountPresentationService)
    {
        // TODO injector DTO MODEL VIEW

        _accountPresentationServiceService = accountPresentationService;

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
        var response = Dialogs.MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        // TODO correct
        // Log.Information("Attempting to remove the account type \"{AccountToDeleteName}\"", AccountType.Name);
        // var (success, exception) = AccountType.Delete();
        //
        // if (success)
        // {
        //     Log.Information("Account was successfully removed");
        //     MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteAccountTypeNoUseSuccess,
        //         MsgBoxImage.Check);
        //
        //     AccountTypeDeleted = true;
        //     DialogResult = true;
        //     Close();
        //     return;
        // }
        //
        // if (exception!.InnerException is SqliteException
        //     {
        //         SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
        //     })
        // {
        //     Log.Error("Foreign key constraint violation");
        //
        //     response = MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteAccountTypeUseQuestion,
        //         MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        //
        //     if (response is not MessageBoxResult.Yes) return;
        //
        //     Log.Information(
        //         "Attempting to remove the account type \"{AccountTypeToDeleteName}\" with all relative element",
        //         AccountType.Name);
        //     AccountType.Delete(true);
        //     Log.Information("Account type and all relative element was successfully removed");
        //     MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteAccountTypeUseSuccess,
        //         MsgBoxImage.Check);
        //
        //     AccountTypeDeleted = true;
        //     DialogResult = true;
        //     Close();
        //
        //     return;
        // }
        //
        // Log.Error(exception, "An error occurred please retry");
        // MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxDeleteAccountTypeNoUseSuccess, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var accountTypeName = AccountType.Name;

        if (string.IsNullOrEmpty(accountTypeName))
        {
            Dialogs.MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorEmptyMessage,
                MsgBoxImage.Error);
            return;
        }

        // TODO correct
        // var alreadyExist = CheckAccountTypeName(accountTypeName);
        // if (alreadyExist) ShowErrorMessage();
        else
        {
            DialogResult = true;
            Close();
        }
    }

    // ReSharper disable once HeapView.ClosureAllocation
    public void SetAccountType(TAccountType accountType)
    {
        accountType.CopyPropertiesTo(AccountType);
        EditAccountType = true;
    }

    public void SetAccountType(AccountTypeViewModel accountTypeViewModel)
    {
        accountTypeViewModel.CopyPropertiesTo(AccountType);
        EditAccountType = true;
    }

    private void TextBoxAccountType_OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var textBox = (TextBox)sender;

        var accountTypeName = textBox.Text;
        if (string.IsNullOrEmpty(accountTypeName)) return;

        // TODO correct
        // var alreadyExist = CheckAccountTypeName(accountTypeName);
        // if (alreadyExist) ShowErrorMessage();
    }

    #endregion

    #region Function

    // TODO correct
    // private bool CheckAccountTypeName(string accountName)
        // => AccountTypeViewModels.Select(s => s.Name).Contains(accountName);


    private static void ShowErrorMessage()
        => Dialogs.MsgBox.MsgBox.Show(AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorAlreadyExistMessage,
            MsgBoxImage.Warning);

    #endregion
}