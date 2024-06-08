using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Resources.Resx.Pages.AccountManagementPage;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class AccountManagementPage
{
    public ObservableCollection<VTotalByAccount> TotalByAccounts { get; }

    public AccountManagementPage()
    {
        using var context = new DataBaseContext();
        TotalByAccounts = [..context.VTotalByAccounts.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    #region Action

    private void ButtonAddNewAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditAccountWindow = new AddEditAccountWindow();
        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult is not true) return;

        var newAccount = addEditAccountWindow.Account;

        if (addEditAccountWindow.EnableStartingBalance)
        {
            var newHistory = addEditAccountWindow.History;
            newAccount.THistories = new List<THistory> { newHistory };
        }

        Log.Information("Attempting to inject the new account \"{NewAccountName}\"", newAccount.Name);
        var (success, exception) = newAccount.AddOrEdit();
        if (success)
        {
            Log.Information("Account was successfully added");
            var json = newAccount.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(AccountManagementPageResources.MessageBoxAddAccountSuccess, MsgBoxImage.Check);

            var newVTotalByAccount = newAccount.Id.ToISqlT<VTotalByAccount>();
            if (newVTotalByAccount is null) return;

            TotalByAccounts.AddAndSort(newVTotalByAccount, s => s.Name!);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(AccountManagementPageResources.MessageBoxAddAccountError, MsgBoxImage.Warning);
        }
    }

    private void ButtonVAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var vTotalByAccount = (VTotalByAccount)button.DataContext;

        var account = vTotalByAccount.Id.ToISqlT<TAccount>();
        if (account is null) return;

        var addEditAccountWindow = new AddEditAccountWindow();
        addEditAccountWindow.SetTAccount(account);
        addEditAccountWindow.ShowDialog();

        if (addEditAccountWindow.DialogResult is not true) return;
        if (addEditAccountWindow.DeleteAccount)
        {
            TotalByAccounts.Remove(vTotalByAccount);
            return;
        }

        var editedAccount = addEditAccountWindow.Account;

        Log.Information("Attempting to edit the account \"{AccountName}\"", account.Name);
        var (success, exception) = editedAccount.AddOrEdit();
        if (success)
        {
            Log.Information("Account was successfully edited");
            var json = editedAccount.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(AccountManagementPageResources.MessageBoxEditAccountSuccess, MsgBoxImage.Check);

            var newVTotalByAccount = editedAccount.Id.ToISqlT<VTotalByAccount>();
            if (newVTotalByAccount is null) return;

            TotalByAccounts.Remove(vTotalByAccount);

            TotalByAccounts.AddAndSort(newVTotalByAccount, s => s.Name!);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(AccountManagementPageResources.MessageBoxEditAccountError, MsgBoxImage.Warning);
        }
    }

    #endregion
}