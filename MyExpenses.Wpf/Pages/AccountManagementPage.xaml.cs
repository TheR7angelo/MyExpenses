using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.Dialogs.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class AccountManagementPage
{
    public ObservableCollection<TotalByAccountViewModel> TotalByAccounts { get; } = [];
    // internal DashBoardPage? DashBoardPage { get; init; }

    private readonly IAccountPresentationService _accountPresentationService;

    public AccountManagementPage(IAccountPresentationService accountPresentationService)
    {
        _accountPresentationService = accountPresentationService;
        _ = FillTotalByAccounts();

        InitializeComponent();
    }

    private async Task FillTotalByAccounts()
    {
        var totalByAccounts = await _accountPresentationService.GetAllTotalByAccountViewModelAsync();
        TotalByAccounts.AddRangeAndSort(totalByAccounts, s => s.Name);
    }

    #region Action

    private void ButtonAddNewAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditAccountWindow  = App.ServiceProvider.GetRequiredService<AddEditAccountWindow>();
        addEditAccountWindow.ShowDialog();

        // TODO injector DTO MODEL VIEW
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

            MsgBox.Show(AddEditAccountResources.MessageBoxButtonValidSuccessTitle, AddEditAccountResources.MessageBoxButtonValidSuccessMessage, MsgBoxImage.Check);

            var newVTotalByAccount = newAccount.Id.ToISql<VTotalByAccount>();
            if (newVTotalByAccount is null) return;

            // TODO CLEAN
            // TotalByAccounts.AddAndSort(newVTotalByAccount, s => s.Name!);
            // DashBoardPage?.RefreshAccountTotal();
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(AddEditAccountResources.MessageBoxButtonValidErrorTitle, AddEditAccountResources.MessageBoxButtonValidErrorMessage, MsgBoxImage.Warning);
        }
    }

    private void ButtonVAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not VTotalByAccount vTotalByAccount) return;

        var account = vTotalByAccount.Id.ToISql<TAccount>();
        if (account is null) return;

        var addEditAccountWindow  = App.ServiceProvider.GetRequiredService<AddEditAccountWindow>();
        addEditAccountWindow.SetTAccount(account);
        addEditAccountWindow.ShowDialog();

        if (addEditAccountWindow.DialogResult is not true) return;
        if (addEditAccountWindow.DeleteAccount)
        {
            // TODO CLEAN
            // TotalByAccounts.Remove(vTotalByAccount);
            // DashBoardPage?.RefreshAccountTotal();
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

            MsgBox.Show(AddEditAccountResources.MessageBoxEditAccountSuccessMessage, MsgBoxImage.Check);

            var newVTotalByAccount = editedAccount.Id.ToISql<VTotalByAccount>();
            if (newVTotalByAccount is null) return;

            // TODO CLEAN
            // TotalByAccounts.Remove(vTotalByAccount);

            // TODO CLEAN
            // TotalByAccounts.AddAndSort(newVTotalByAccount, s => s.Name!);
            // DashBoardPage?.RefreshAccountTotal();
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(AddEditAccountResources.MessageBoxEditAccountErrorMessage, MsgBoxImage.Warning);
        }
    }

    #endregion
}