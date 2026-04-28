using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Messages;
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

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, (_, m) =>
        {
            if (m.Value.EntityType is not DependencyType.Account || m.Value.DataAction is not DataAction.Delete) return;

            var ids = m.Value.Content;
            foreach (var item in TotalByAccounts.Where(s => ids.Contains(s.Id)))
            {
                item.IsDeleting = true;
            }
        });

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, (_, m) =>
        {
            if (m.Value is not { EntityType: DependencyType.Account, DataAction: DataAction.Update, Content: var accountViewModel }) return;
            var item = TotalByAccounts.FirstOrDefault(s => s.Id == accountViewModel.Id);
            if (item is null) return;
            item.Name = accountViewModel.Name ?? string.Empty;
            item.Symbol = accountViewModel.CurrencyViewModel?.Symbol ?? string.Empty;
        });
    }

    [RelayCommand]
    private void DeleteSelf(object parameter)
    {
        if (parameter is not TotalByAccountViewModel item) return;

        TotalByAccounts.Remove(item);
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

    private async void ButtonVAccount_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not TotalByAccountViewModel totalByAccountViewModel) return;

        var addEditAccountWindow  = App.ServiceProvider.GetRequiredService<AddEditAccountWindow>();
        await addEditAccountWindow.LoadAsync(totalByAccountViewModel);
        addEditAccountWindow.ShowDialog();
    }

    #endregion
}