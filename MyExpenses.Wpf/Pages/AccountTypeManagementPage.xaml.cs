using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.Pages.AccountTypeManagementPage;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class AccountTypeManagementPage
{
    public ObservableCollection<TAccountType> AccountTypes { get; }

    public required DashBoardPage DashBoardPage { get; init; }

    public AccountTypeManagementPage()
    {
        using var context = new DataBaseContext();
        AccountTypes = [..context.TAccountTypes.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    private void ButtonAddNewAccountType_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditAccountType = new AddEditAccountTypeWindow();
        var result = addEditAccountType.ShowDialog();
        if (result != true) return;

        var newAccountType = addEditAccountType.AccountType;

        Log.Information("Attempting to inject the new account type \"{NewAccountTypeName}\"", newAccountType.Name);
        var (success, exception) = newAccountType.AddOrEdit();
        if (success)
        {
            AccountTypes.AddAndSort(newAccountType, s => s.Name!);
            Log.Information("Account type was successfully added");
            MsgBox.Show(AccountTypeManagementPageResources.MessageBoxAddAccountTypeSuccess, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(AccountTypeManagementPageResources.MessageBoxAddAccountTypeError, MsgBoxImage.Error);
        }
    }

    private void ButtonAccountType_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not TAccountType accountType) return;

        var addEditAccountType = new AddEditAccountTypeWindow();
        addEditAccountType.SetTAccountType(accountType);

        var result = addEditAccountType.ShowDialog();
        if (result != true) return;

        var editedAccountType = addEditAccountType.AccountType;
        if (addEditAccountType.AccountTypeDeleted) AccountTypes.Remove(accountType);
        else
        {
            Log.Information("Attempting to update account type id:\"{EditedAccountTypeId}\", name:\"{EditedAccountTypeName}\"",editedAccountType.Id, editedAccountType.Name);
            var (success, exception) = editedAccountType.AddOrEdit();
            if (success)
            {
                AccountTypes.Remove(accountType);
                AccountTypes.AddAndSort(editedAccountType, s => s.Name!);
                Log.Information("Account type was successfully edited");
                MsgBox.Show(AccountTypeManagementPageResources.MessageBoxEditAccountTypeSuccess, MsgBoxImage.Check);
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.Show(AccountTypeManagementPageResources.MessageBoxEditAccountTypeError, MsgBoxImage.Error);
            }
        }

        DashBoardPage.RefreshAccountTotal();
        DashBoardPage.RefreshRadioButtonSelected();
    }
}