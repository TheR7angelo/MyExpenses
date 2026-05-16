using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public partial class CategoryTypeManagementViewModel : ViewModelBase
{
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly ISystemPresentationService _systemPresentationService;
    private readonly IExpenseActionService _expenseActionService;
    private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;
    private readonly INavigationWindowService _navigationWindowService;
    private readonly ILogger<CategoryTypeManagementViewModel> _logger;

    public ObservableCollection<CategoryTypeViewModel> CategoryTypeViewModels { get; } = [];

    public ObservableCollection<ColorViewModel> ColorViewModels { get; } = [];

    [ObservableProperty]
    public partial bool IsEditCategoryType { get; private set; }

    public CategoryTypeViewModel CategoryTypeViewModel { get; } = new();

    private CategoryTypeViewModel? _categoryTypeViewModelToLoad;

    public IAsyncRelayCommand LoadAllCategoryTypeCommand { get; }
    public IAsyncRelayCommand LoadAllColorCommand { get; }
    public IAsyncRelayCommand<CategoryTypeViewModel?> ManageCategoryTypeActionCommand { get; }

    public IAsyncRelayCommand<IClosable?> CreateCommand { get; }
    public IAsyncRelayCommand<IClosable?> DeleteCommand { get; }
    public IRelayCommand<CategoryTypeViewModel?> RemoveCommand { get; }

    public IRelayCommand<IClosable?> CancelCommand { get; }

    public CategoryTypeManagementViewModel(IExpensePresentationService expensePresentationService,
        ISystemPresentationService systemPresentationService,
        IExpenseActionService expenseActionService,
        IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
        INavigationWindowService navigationWindowService,
        ILogger<CategoryTypeManagementViewModel> logger)
    {
        _expensePresentationService = expensePresentationService;
        _systemPresentationService = systemPresentationService;
        _expenseActionService = expenseActionService;
        _expenseDtoViewModelMapper = expenseDtoViewModelMapper;
        _navigationWindowService = navigationWindowService;
        _logger = logger;

        LoadAllCategoryTypeCommand = new AsyncRelayCommand(LoadAllCategoryTypeAsync);
        LoadAllColorCommand = new AsyncRelayCommand(LoadAllColorAsync);

        ManageCategoryTypeActionCommand = new AsyncRelayCommand<CategoryTypeViewModel?>(ManageCategoryTypeAction);
        CancelCommand = new RelayCommand<IClosable?>(OnCancel);

        CreateCommand = new AsyncRelayCommand<IClosable?>(CreateCategoryType);
        DeleteCommand = new AsyncRelayCommand<IClosable?>(DeleteCategoryType);
        RemoveCommand = new RelayCommand<CategoryTypeViewModel?>(RemoveCategoryType);

        RegisterMessages();
    }

    private void RemoveCategoryType(CategoryTypeViewModel? item)
    {
        if (item is null) return;

        CategoryTypeViewModels.Remove(item);
    }

    private async Task CreateCategoryType(IClosable? dialog, CancellationToken cancellationToken = default)
    {
        var result = IsEditCategoryType
            ? await _expenseActionService.UpdateCategoryType(CategoryTypeViewModel, cancellationToken)
            : await _expenseActionService.CreateCategoryType(CategoryTypeViewModel, cancellationToken);

        if (result) dialog?.Close();
    }

    private void OnCancel(IClosable? dialog)
        => dialog?.Close();

    /// <summary>
    /// Registers messages that the ViewModel listens for using the messaging system.
    /// </summary>
    /// <remarks>
    /// This method is responsible for subscribing to messages related to account changes
    /// and deletions. It enables the ViewModel to respond to these events and update its
    /// state accordingly when other components in the application signal changes in account data.
    /// </remarks>
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnDeleteCategoryType);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<CategoryTypeViewModel>>(this, OnCategoryTypeChanged);
    }

    private void OnCategoryTypeChanged(object recipient, EntityChangedMessage<CategoryTypeViewModel> message)
    {
        if (message.Value.EntityType != DependencyType.CategoryType) return;

        switch (message.Value.DataAction)
        {
            case DataAction.Update:
                ApplyUpdate(message.Value.Content);
                break;

            case DataAction.Add:
                ApplyAddAsync(message.Value.Content);
                break;
        }
    }

    private void ApplyAddAsync(CategoryTypeViewModel item)
        => CategoryTypeViewModels.AddAndSort(item, vm => vm.Name!);

    private void ApplyUpdate(CategoryTypeViewModel vm)
    {
        var item = CategoryTypeViewModels.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _expenseDtoViewModelMapper.Merge(vm, item);
    }

    private void OnDeleteCategoryType(object recipient, EntityChangedMessage<int[]> message)
    {
        if (message.Value.EntityType != DependencyType.CategoryType ||
            message.Value.DataAction != DataAction.Delete)
            return;

        foreach (var item in CategoryTypeViewModels.Where(s => message.Value.Content.Contains(s.Id)))
        {
            item.IsDeleting = true;
        }
    }

    private async Task DeleteCategoryType(IClosable? dialog, CancellationToken cancellationToken = default)
    {
        var result = await _expenseActionService.DeleteCategoryType(CategoryTypeViewModel, cancellationToken);
        if (result) dialog?.Close();
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

        CategoryTypeViewModel.AcceptChanges();
        IsEditCategoryType = true;
    }
}