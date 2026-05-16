using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public partial class CategoryTypeManagementViewModel : ViewModelBase
{
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly ISystemPresentationService _systemPresentationService;
    private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;
    private readonly IDialogService _dialogService;
    private readonly INavigationWindowService _navigationWindowService;
    private readonly ILogger<CategoryTypeManagementViewModel> _logger;

    public ObservableCollection<CategoryTypeViewModel> CategoryTypeViewModels { get; } = [];

    public ObservableCollection<ColorViewModel> ColorViewModels { get; } = [];

    [ObservableProperty]
    public partial bool IsEditCategoryType { get; private set; }

    public CategoryTypeViewModel CategoryTypeViewModel { get; private set; } = new();

    private CategoryTypeViewModel? _categoryTypeViewModelToLoad;

    public IAsyncRelayCommand LoadAllCategoryTypeCommand { get; }
    public IAsyncRelayCommand LoadAllColorCommand { get; }
    public AsyncRelayCommand<CategoryTypeViewModel?> ManageCategoryTypeActionCommand { get; }

    public IRelayCommand<CategoryTypeViewModel> DeleteCommand { get; }

    public CategoryTypeManagementViewModel(IExpensePresentationService expensePresentationService,
        ISystemPresentationService systemPresentationService,
        IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
        IDialogService dialogService,
        INavigationWindowService navigationWindowService,
        ILogger<CategoryTypeManagementViewModel> logger)
    {
        _expensePresentationService = expensePresentationService;
        _systemPresentationService = systemPresentationService;
        _expenseDtoViewModelMapper = expenseDtoViewModelMapper;
        _dialogService = dialogService;
        _navigationWindowService = navigationWindowService;
        _logger = logger;

        LoadAllCategoryTypeCommand = new AsyncRelayCommand(LoadAllCategoryTypeAsync);
        LoadAllColorCommand = new AsyncRelayCommand(LoadAllColorAsync);

        ManageCategoryTypeActionCommand = new AsyncRelayCommand<CategoryTypeViewModel?>(ManageCategoryTypeAction);
        DeleteCommand = new RelayCommand<CategoryTypeViewModel>(DeleteCategoryType);
    }

    private void DeleteCategoryType(CategoryTypeViewModel? item)
    {
        if (item is null) return;

        CategoryTypeViewModels.Remove(item);
    }

    private Task ManageCategoryTypeAction(CategoryTypeViewModel? categoryTypeViewModel)
    {
        _navigationWindowService.ShowManageCategoryType(categoryTypeViewModel);
        return Task.CompletedTask;
    }

    private async Task LoadAllCategoryTypeAsync(CancellationToken cancellationToken = default)
    {
        var categoryType = await _expensePresentationService.GetAllCategoryTypeViewModelAsync(cancellationToken);
        CategoryTypeViewModels.AddRangeAndSort(categoryType, vm => vm.Name!);
    }

    private async Task LoadAllColorAsync(CancellationToken cancellationToken = default)
    {
        var colors = await _systemPresentationService.GetAllColorViewModelAsync(cancellationToken);
        ColorViewModels.AddRangeAndSort(colors, vm => vm.Name!);

        if (_categoryTypeViewModelToLoad is not null) LoadCategoryTypeViewModel();
    }

    public void LoadCategoryTypeViewModel(CategoryTypeViewModel categoryTypeViewModel)
        => _categoryTypeViewModelToLoad = categoryTypeViewModel;

    private void LoadCategoryTypeViewModel()
    {
        if (_categoryTypeViewModelToLoad is null) return;

        _expenseDtoViewModelMapper.Merge(_categoryTypeViewModelToLoad, CategoryTypeViewModel);

        var color = ColorViewModels.FirstOrDefault(c => c.Id == _categoryTypeViewModelToLoad.Color?.Id);
        if (color is not null) CategoryTypeViewModel.Color = color;

        IsEditCategoryType = true;
    }
}