using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public class AccountTypeManagementViewModel : ViewModelBase
{
    private readonly IAccountPresentationService _accountService;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialog;
    private readonly ILogger<AccountTypeManagementViewModel> _logger;

    public ObservableCollection<AccountTypeViewModel> AccountTypes { get; } = [];

    public IAsyncRelayCommand LoadCommand { get; }

    public IRelayCommand<AccountTypeViewModel> DeleteCommand { get; }
    public IAsyncRelayCommand<AccountTypeViewModel> ViewAccountTypeCommand { get; }

    public IRelayCommand AddAccountTypeCommand { get; }

    public AccountTypeManagementViewModel(IAccountPresentationService accountService,
        INavigationService navigation,
        IDialogService dialog,
        ILogger<AccountTypeManagementViewModel> logger)
    {
        _accountService = accountService;
        _navigation = navigation;
        _dialog = dialog;
        _logger = logger;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        DeleteCommand = new RelayCommand<AccountTypeViewModel>(Delete);
        ViewAccountTypeCommand = new AsyncRelayCommand<AccountTypeViewModel>(ViewAccountTypeAsync);
        AddAccountTypeCommand = new RelayCommand(AddAccountType);

        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnAccountTypeDeleted);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountTypeViewModel>>(this, OnAccountTypeChanged);
    }

    private void OnAccountTypeDeleted(object recipient, EntityChangedMessage<int[]> m)
    {
        if (m.Value.EntityType != DependencyType.AccountType ||
            m.Value.DataAction != DataAction.Delete)
            return;

        foreach (var item in AccountTypes.Where(s => m.Value.Content.Contains(s.Id)))
        {
            item.IsDeleting = true;
        }
    }

    private async void OnAccountTypeChanged(object recipient, EntityChangedMessage<AccountTypeViewModel> m)
    {
        if (m.Value.EntityType != DependencyType.AccountType) return;

        switch (m.Value.DataAction)
        {
            case DataAction.Update:
                ApplyUpdate(m.Value.Content);
                break;

            case DataAction.Add:
                await ApplyAddAsync(m.Value.Content);
                break;
        }
    }

    private void ApplyUpdate(AccountTypeViewModel vm)
    {
        var item = AccountTypes.FirstOrDefault(s => s.Id == vm.Id);
        item?.Name = vm.Name ?? string.Empty;
    }

    private async Task ApplyAddAsync(AccountTypeViewModel vm)
    {
        AccountTypes.AddAndSort(vm, s => s.Name!);
    }

    private void AddAccountType()
    {
        _navigation.ShowAddAccountType();
    }

    private Task ViewAccountTypeAsync(AccountTypeViewModel? item, CancellationToken cancellationToken = default)
    {
        // TODO work
        if (item is null) return Task.CompletedTask;



        throw new NotImplementedException();
    }

    private void Delete(AccountTypeViewModel? item)
    {
        if (item is null) return;

        AccountTypes.Remove(item);
    }

    private async Task LoadAsync()
    {
        try
        {
            await FillAccountTypes();
        }
        catch (OperationCanceledException)
        {
            // Pass
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading accounts types");
            _dialog.ShowError("An error occurred while loading account types.");
        }
    }

    private async Task FillAccountTypes()
    {
        AccountTypes.Clear();

        var items = await _accountService.GetAllAccountTypeViewModelAsync();
        AccountTypes.AddRangeAndSort(items, s => s.Name!);
    }
}