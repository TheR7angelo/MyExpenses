using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Resources.Resx.Pages.AccountManagementPage;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class AccountManagementPage
{
    public ObservableCollection<VTotalByAccount> TotalByAccounts { get; }
    internal DashBoardPage? DashBoardPage { get; init; }

    public AccountManagementPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        TotalByAccounts = [..context.VTotalByAccounts.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    #region Action

    private void ButtonAddNewAccount_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditAccountWindow is created locally and used exclusively within this method.
        // Since it is scoped to this method and no further references to it exist after its use,
        // the Garbage Collector will automatically handle the memory cleanup once the method execution ends.
        // Therefore, explicit management of this allocation is unnecessary, and the hint can be safely ignored.
        var addEditAccountWindow = new AddEditAccountWindow();
        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult is not true) return;

        var newAccount = addEditAccountWindow.Account;

        if (addEditAccountWindow.EnableStartingBalance)
        {
            var newHistory = addEditAccountWindow.History;
            newHistory.ModePaymentFk = 1;
            newAccount.THistories = [newHistory];
        }

        Log.Information("Attempting to inject the new account \"{NewAccountName}\"", newAccount.Name);
        var (success, exception) = newAccount.AddOrEdit();
        if (success)
        {
            Log.Information("Account was successfully added");
            var json = newAccount.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(AccountManagementPageResources.MessageBoxAddAccountSuccess, MsgBoxImage.Check);

            var newVTotalByAccount = newAccount.Id.ToISql<VTotalByAccount>();
            if (newVTotalByAccount is null) return;

            TotalByAccounts.AddAndSort(newVTotalByAccount, s => s.Name!);
            DashBoardPage?.RefreshAccountTotal();
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
        if (button.DataContext is not VTotalByAccount vTotalByAccount) return;

        var account = vTotalByAccount.Id.ToISql<TAccount>();
        if (account is null) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditAccountWindow is created within the scope of this method
        // and is only used temporarily. Once the execution of the method completes,
        // and no references remain, the Garbage Collector will handle its cleanup automatically.
        // Explicit allocation management is not required here, so the hint is safely ignored.
        var addEditAccountWindow = new AddEditAccountWindow();
        addEditAccountWindow.SetTAccount(account);
        addEditAccountWindow.ShowDialog();

        if (addEditAccountWindow.DialogResult is not true) return;
        if (addEditAccountWindow.DeleteAccount)
        {
            TotalByAccounts.Remove(vTotalByAccount);
            DashBoardPage?.RefreshAccountTotal();
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

            var newVTotalByAccount = editedAccount.Id.ToISql<VTotalByAccount>();
            if (newVTotalByAccount is null) return;

            TotalByAccounts.Remove(vTotalByAccount);

            TotalByAccounts.AddAndSort(newVTotalByAccount, s => s.Name!);
            DashBoardPage?.RefreshAccountTotal();
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(AccountManagementPageResources.MessageBoxEditAccountError, MsgBoxImage.Warning);
        }
    }

    #endregion
}