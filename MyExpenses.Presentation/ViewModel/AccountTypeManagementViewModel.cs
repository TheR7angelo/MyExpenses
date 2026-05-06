using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
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
    }

    private void AddAccountType()
    {
        // TODO work
        throw new NotImplementedException();
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