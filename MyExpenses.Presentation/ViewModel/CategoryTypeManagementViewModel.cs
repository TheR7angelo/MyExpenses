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
    private readonly ISystemDtoViewModelMapper _systemDtoViewModelMapper;
    private readonly INavigationWindowService _navigationWindowService;

    public ObservableCollection<CategoryTypeViewModel> CategoryTypeViewModels { get; } = [];

    public ObservableCollection<ColorViewModel> ColorViewModels { get; } = [];

    [ObservableProperty]
    public partial bool IsEditCategoryType { get; private set; }

    public CategoryTypeViewModel CategoryTypeViewModel { get; } = new();

    private CategoryTypeViewModel? _categoryTypeViewModelToLoad;

    public IAsyncRelayCommand LoadAllCategoryTypeCommand { get; }
    public IAsyncRelayCommand LoadAllColorCommand { get; }
    public IAsyncRelayCommand<CategoryTypeViewModel?> ManageCategoryTypeActionCommand { get; }
    public IRelayCommand<IClosable?> ManageColorCommand { get; }

    public IAsyncRelayCommand<IClosable?> CreateOrUpdateCommand { get; }
    public IAsyncRelayCommand<IClosable?> DeleteCommand { get; }
    public IRelayCommand<CategoryTypeViewModel?> RemoveCommand { get; }

    public IRelayCommand<IClosable?> CancelCommand { get; }

    public IClosable? Closeable { get; private set; }

    public CategoryTypeManagementViewModel(IExpensePresentationService expensePresentationService,
        ISystemPresentationService systemPresentationService,
        IExpenseActionService expenseActionService,
        IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
        ISystemDtoViewModelMapper systemDtoViewModelMapper,
        INavigationWindowService navigationWindowService,
        ILogger<CategoryTypeManagementViewModel> logger)
    {
        _expensePresentationService = expensePresentationService;
        _systemPresentationService = systemPresentationService;
        _expenseActionService = expenseActionService;
        _expenseDtoViewModelMapper = expenseDtoViewModelMapper;
        _systemDtoViewModelMapper = systemDtoViewModelMapper;
        _navigationWindowService = navigationWindowService;

        LoadAllCategoryTypeCommand = new AsyncRelayCommand(LoadAllCategoryTypeAsync);
        LoadAllColorCommand = new AsyncRelayCommand(LoadAllColorAsync);

        ManageCategoryTypeActionCommand = new AsyncRelayCommand<CategoryTypeViewModel?>(ManageCategoryTypeAction);
        ManageColorCommand = new RelayCommand<IClosable?>(ManageColorAction);

        CancelCommand = new RelayCommand<IClosable?>(OnCancel);

        CreateOrUpdateCommand = new AsyncRelayCommand<IClosable?>(CreateCategoryType);
        DeleteCommand = new AsyncRelayCommand<IClosable?>(DeleteCategoryType);
        RemoveCommand = new RelayCommand<CategoryTypeViewModel?>(RemoveCategoryType);

        RegisterMessages();
    }

    private void ManageColorAction(IClosable? closeable)
    {
        Closeable = closeable;
        _navigationWindowService.ShowColorManagementWindow(CategoryTypeViewModel.Color);
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
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnDelete);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<CategoryTypeViewModel>>(this, OnCategoryTypeChanged);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<ColorViewModel>>(this, OnColorChanged);
    }

    private void OnColorChanged(object recipient, EntityChangedMessage<ColorViewModel> message)
    {
        if (message.Value.EntityType is not DependencyType.Color) return;

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

    private void ApplyAddAsync(ColorViewModel item)
    {
        ColorViewModels.AddAndSort(item, vm => vm.Name!);
        CategoryTypeViewModel.Color = item;
    }

    private void ApplyUpdate(ColorViewModel vm)
    {
        var item = ColorViewModels.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _systemDtoViewModelMapper.Merge(vm, item);
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

    private void OnDelete(object recipient, EntityChangedMessage<int[]> message)
    {
        if (message.Value.DataAction is not DataAction.Delete) return;

        if (message.Value.EntityType is DependencyType.CategoryType)
        {
            foreach (var item in CategoryTypeViewModels.Where(s => message.Value.Content.Contains(s.Id)))
            {
                item.IsDeleting = true;
            }
        }
        else if (message.Value.EntityType is DependencyType.Color)
        {
            foreach (var colorId in message.Value.Content)
            {
                var color = ColorViewModels.FirstOrDefault(c => c.Id == colorId);
                if (color is not null) ColorViewModels.Remove(color);
            }
            Closeable?.DialogResult = false;
            Closeable?.Close();
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