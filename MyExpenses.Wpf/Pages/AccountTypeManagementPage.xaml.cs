using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class AccountTypeManagementPage
{
    public ObservableCollection<TAccountType> AccountTypes { get; }

    public AccountTypeManagementPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        AccountTypes = [..context.TAccountTypes.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    private void ButtonAddNewAccountType_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditAccountTypeWindow is created locally within this method and is used temporarily.
        // Since there are no references to it after this scope and the Garbage Collector will handle
        // its cleanup efficiently, this allocation is intentional and does not require further optimization.
        var addEditAccountType = new AddEditAccountTypeWindow();
        var result = addEditAccountType.ShowDialog();
        if (result is not true) return;

        var newAccountType = addEditAccountType.AccountType;

        Log.Information("Attempting to inject the new account type \"{NewAccountTypeName}\"", newAccountType.Name);
        var (success, exception) = newAccountType.AddOrEdit();
        if (success)
        {
            AccountTypes.AddAndSort(newAccountType, s => s.Name!);

            Log.Information("Account type was successfully added");
            var json = newAccountType.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(AccountTypeManagementResources.MessageBoxAddNewAccountTypeSuccessTitle,
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeSuccessMessage, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(AccountTypeManagementResources.MessageBoxAddNewAccountTypeErrorTitle,
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeErrorMessage, MsgBoxImage.Error);
        }
    }

    private void ButtonAccountType_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not TAccountType accountType) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditAccountTypeWindow is created locally within this method and is used temporarily.
        // Since there are no references to it after this scope and the Garbage Collector will handle
        // its cleanup efficiently, this allocation is intentional and does not require further optimization.
        var addEditAccountType = new AddEditAccountTypeWindow();
        addEditAccountType.SetTAccountType(accountType);

        var result = addEditAccountType.ShowDialog();
        if (result is not true) return;

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
                var json = editedAccountType.ToJsonString();
                Log.Information("{Json}", json);

                MsgBox.Show(AccountTypeManagementResources.MessageBoxAccountTypeEditSuccessTitle,
                    AccountTypeManagementResources.MessageBoxAccountTypeEditSuccessMessage, MsgBoxImage.Check);
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.Show(AccountTypeManagementResources.MessageBoxAccountTypeEditErrorTitle,
                AccountTypeManagementResources.MessageBoxAccountTypeEditErrorMessage, MsgBoxImage.Error);
            }
        }
    }
}