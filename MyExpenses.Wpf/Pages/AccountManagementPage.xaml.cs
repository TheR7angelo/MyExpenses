using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Resources.Resx.Pages.AccountManagementPage;
using MyExpenses.Wpf.Windows;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class AccountManagementPage
{
    public ObservableCollection<VTotalByAccount> TotalByAccounts { get; }

    public required DashBoardPage DashBoardPage { get; init; }

    public AccountManagementPage()
    {
        using var context = new DataBaseContext();
        TotalByAccounts = [..context.VTotalByAccounts];

        InitializeComponent();
    }

    #region Action

    private void ButtonAddNewAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditAccountWindow = new AddEditAccountWindow();
        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult != true) return;

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
            MessageBox.Show(AccountManagementPageResources.MessageBoxAddAccountSuccess);

            var newVTotalByAccount = newAccount.ToVTotalByAccount()!;

            TotalByAccounts.AddAndSort(newVTotalByAccount, s => s.Name!);

            DashBoardPage.RefreshAccountTotal();
            Application.Current.Dispatcher.InvokeAsync(DashBoardPage.RefreshRadioButtonSelected, DispatcherPriority.ContextIdle);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MessageBox.Show(AccountManagementPageResources.MessageBoxAddAccountError);
        }
    }

    private void ButtonVAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var vTotalByAccount = button.DataContext as VTotalByAccount;

        Console.WriteLine(vTotalByAccount?.Id);
    }

    #endregion
}