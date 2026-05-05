using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Wpf.Windows;

namespace MyExpenses.Wpf.Pages;

public partial class AccountManagementPage
{
    public ObservableCollection<TotalByAccountViewModel> TotalByAccounts { get; } = [];

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

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, async (_, m) =>
        {
            if (m.Value is not {EntityType: DependencyType.Account, Content: var accountViewModel }) return;

            switch (m.Value.DataAction)
            {
                case DataAction.Update:
                {
                    var item = TotalByAccounts.FirstOrDefault(s => s.Id == accountViewModel.Id);
                    if (item is null) return;
                    item.Name = accountViewModel.Name ?? string.Empty;
                    item.Symbol = accountViewModel.CurrencyViewModel?.Symbol ?? string.Empty;
                    break;
                }
                case DataAction.Add:
                {
                    var item = await accountPresentationService.GetTotalByAccountViewModelAsync(accountViewModel);
                    if (item is null) return;
                    TotalByAccounts.AddAndSort(item, s => s.Name);
                    break;
                }
            }
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