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