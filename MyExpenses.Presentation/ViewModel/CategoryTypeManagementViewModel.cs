using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public class CategoryTypeManagementViewModel : ViewModelBase
{
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly IAccountActionService _accountActionService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<CategoryTypeManagementViewModel> _logger;

    public ObservableCollection<CategoryTypeViewModel> CategoryTypeViewModels { get; } = [];

    public IAsyncRelayCommand LoadCommand { get; }
    public AsyncRelayCommand<CategoryTypeViewModel> CreateCategoryTypeCommand { get; }

    public CategoryTypeManagementViewModel(IExpensePresentationService expensePresentationService,
        IAccountActionService accountActionService,
        IDialogService dialogService,
        ILogger<CategoryTypeManagementViewModel> logger)
    {
        _expensePresentationService = expensePresentationService;
        _accountActionService = accountActionService;
        _dialogService = dialogService;
        _logger = logger;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        CreateCategoryTypeCommand = new AsyncRelayCommand<CategoryTypeViewModel>(ManageCategoryTypeAction);
    }

    private async Task ManageCategoryTypeAction(CategoryTypeViewModel? categoryTypeViewModel)
    {
        try
        {
            // TODO correct replace inputbox by real window call
            await _accountActionService.ManageCategoryTypeAction(categoryTypeViewModel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while managing category type action");
            _dialogService.ShowMessageBox(AccountResources.MessageBoxAddEditAccountTypeErrorCaption,
                AccountResources.MessageBoxAddEditAccountTypeErrorContent, MsgBoxImage.Error);
        }
    }

    private async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var categoryType = await _expensePresentationService.GetAllCategoryTypeViewModelAsync(cancellationToken);
        CategoryTypeViewModels.AddRangeAndSort(categoryType, vm => vm.Name!);
    }
}