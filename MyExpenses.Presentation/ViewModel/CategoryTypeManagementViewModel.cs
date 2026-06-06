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

/// <summary>
/// Represents the view model for managing category types within the application.
/// </summary>
/// <remarks>
/// This view model serves as the data context for the associated UI page and provides
/// properties, commands, and logic for managing category types and their associated actions.
/// Key responsibilities include handling CRUD operations for category types, managing related
/// color configurations, and facilitating interaction between the presentation layer and
/// service layers.
/// </remarks>
public partial class CategoryTypeManagementViewModel : ViewModelBase
{
    /// <summary>
    /// Provides access to a service responsible for handling presentation-related logic for expenses.
    /// This service is used to interact with and retrieve expense-related data to support view models
    /// and presentation logic in the application.
    /// </summary>
    private readonly IExpensePresentationService _expensePresentationService;

    /// <summary>
    /// Provides access to a service responsible for handling system-related presentation logic.
    /// This service is used to manage and retrieve data necessary for presenting information
    /// about various system components, such as colors or other system-level configurations.
    /// </summary>
    private readonly ISystemPresentationService _systemPresentationService;

    /// <summary>
    /// Provides access to a service responsible for executing business actions related to expense management.
    /// This service is used for creating, updating, and deleting expense-related entities, ensuring
    /// the application maintains a consistent state during these operations.
    /// </summary>
    private readonly IExpenseActionService _expenseActionService;

    /// <summary>
    /// Provides functionality to map and merge data between expense-related DTOs and ViewModels.
    /// This mapper is used to ensure synchronization and cohesive data representation
    /// between different layers within the application.
    /// </summary>
    private readonly IExpenseDtoViewModelMapper _expenseDtoViewModelMapper;

    /// <summary>
    /// Provides functionality for mapping and merging data between system DTOs and view models.
    /// This mapper is used to synchronize data between the application's data transfer objects (DTOs)
    /// and its presentation layer, ensuring consistency and reducing duplication across the ViewModel layer.
    /// </summary>
    private readonly ISystemDtoViewModelMapper _systemDtoViewModelMapper;

    /// <summary>
    /// Provides access to a service responsible for handling navigation and window management
    /// within the application. This service is utilized for opening and managing specific windows
    /// or views, ensuring seamless navigation across different parts of the user interface.
    /// </summary>
    private readonly INavigationWindowService _navigationWindowService;

    /// <summary>
    /// Represents a collection of category type view models used for managing and organizing category types.
    /// Provides data binding support for category type-related operations in the view, enabling manipulation
    /// and display of category type information within the application.
    /// </summary>
    public ObservableCollection<CategoryTypeViewModel> CategoryTypeViewModels { get; } = [];

    /// <summary>
    /// Represents a collection of available color view models used for managing and assigning
    /// colors within the application. This property is dynamically populated and interacts
    /// with various components, such as dropdowns or selection elements, to provide color options.
    /// </summary>
    public ObservableCollection<ColorViewModel> ColorViewModels { get; } = [];

    /// <summary>
    /// Indicates whether the current operation is to edit an existing category type.
    /// This property is used to determine the application's behavior or UI presentation related to
    /// category type management, such as enabling or disabling controls for modification or display purposes.
    /// </summary>
    [ObservableProperty]
    public partial bool IsEditCategoryType { get; private set; }

    /// <summary>
    /// Represents the view model for a specific category type.
    /// This view model includes properties and logic that describe and support the binding
    /// of category type-related data within the application's user interface.
    /// </summary>
    public CategoryTypeViewModel CategoryTypeViewModel { get; } = new();

    /// <summary>
    /// Represents the instance of <see cref="CategoryTypeViewModel"/> to be loaded and merged
    /// into the current view model during data initialization or update.
    /// This variable is dynamically set when specific category type data needs to be processed
    /// or edited within the view model context.
    /// </summary>
    private CategoryTypeViewModel? _categoryTypeViewModelToLoad;

    /// <summary>
    /// Provides access to a service responsible for logging messages in the CategoryTypeManagementViewModel.
    /// This logger is used to record information, warnings, and errors that occur during the execution of the ViewModel's logic.
    /// It helps in debugging and monitoring the application's behavior by providing insights into its operations.
    /// </summary>
    private readonly ILogger<CategoryTypeManagementViewModel> _logger;

    /// <summary>
    /// Represents an entity that facilitates the management of a closeable resource or dialog
    /// within the context of category type management.
    /// This property is used to interact with and control the lifecycle of a dialog-like
    /// resource, allowing for actions such as closing or setting dialog results.
    /// </summary>
    private IClosable? _closeable;

    public static string TextSearchColorName { get; } = nameof(ColorViewModel.Name);

    /// <summary>
    /// Represents the ViewModel responsible for managing category types and related operations
    /// within the application. This ViewModel is dynamically instantiated and bound to the View via DataContext.
    /// </summary>
    /// <remarks>
    /// The CategoryTypeManagementViewModel handles the interaction with the services and commands
    /// necessary for loading, managing, creating, updating, and deleting category types as well as managing colors.
    /// It serves as the mediator between the UI and the core business logic, leveraging dependency injection
    /// to access required operations and mappings.
    /// </remarks>
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
        _logger = logger;

        RegisterMessages();
    }

    /// <summary>
    /// Executes the action to manage color settings associated with a category type by opening
    /// the color management window. The method also tracks the provided closable context for
    /// future operations.
    /// </summary>
    /// <param name="closeable">
    /// An optional instance of an object implementing the <see cref="IClosable"/> interface,
    /// used to manage the close operation of the invoking context.
    /// </param>
    [RelayCommand]
    private void OnManageColor(IClosable? closeable)
    {
        _closeable = closeable;
        _navigationWindowService.ShowColorManagementWindow(CategoryTypeViewModel.Color);
    }

    /// <summary>
    /// Removes the specified category type from the collection of category types,
    /// if it exists within the collection.
    /// </summary>
    /// <param name="item">
    /// The <see cref="CategoryTypeViewModel"/> instance to be removed. If the value is null,
    /// the method will exit without modifying the collection.
    /// </param>
    [RelayCommand]
    private void OnRemove(CategoryTypeViewModel? item)
    {
        if (item is null) return;

        CategoryTypeViewModels.Remove(item);
    }

    /// <summary>
    /// Handles the creation or updating of a category type based on the current operation state
    /// (create or edit). If the operation is successful, the provided dialog is closed.
    /// </summary>
    /// <param name="dialog">An instance of a closable dialog that will be closed upon successful operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [RelayCommand]
    private async Task OnCreateOrUpdate(IClosable? dialog, CancellationToken cancellationToken = default)
    {
        var result = IsEditCategoryType
            ? await _expenseActionService.UpdateCategoryType(CategoryTypeViewModel, cancellationToken)
            : await _expenseActionService.CreateCategoryType(CategoryTypeViewModel, cancellationToken);

        if (result) dialog?.Close();
    }

    [RelayCommand]
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

    /// <summary>
    /// Handles the event triggered when a color entity changes. This method processes the
    /// action (add or update) and applies the necessary updates to the current state of color-related data.
    /// </summary>
    /// <param name="recipient">The recipient object that registered for the message. Typically, this is the ViewModel instance.</param>
    /// <param name="message">The message containing information about the changed <see cref="ColorViewModel"/> entity, including the action type and associated data.</param>
    private void OnColorChanged(object recipient, EntityChangedMessage<ColorViewModel> message)
    {
        if (message.Value.EntityType is not DependencyType.Color) return;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
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

    /// <summary>
    /// Adds a new <see cref="ColorViewModel"/> item to the collection of colors and updates the color property
    /// in the associated category type view model.
    /// </summary>
    /// <param name="item">
    /// The <see cref="ColorViewModel"/> instance to be added to the collection.
    /// </param>
    private void ApplyAddAsync(ColorViewModel item)
    {
        ColorViewModels.AddAndSort(item, vm => vm.Name!);
        CategoryTypeViewModel.Color = item;
    }

    /// <summary>
    /// Updates an existing <see cref="ColorViewModel"/> in the collection with new data from the specified source.
    /// </summary>
    /// <param name="vm">
    /// The <see cref="ColorViewModel"/> containing updated data that will be
    /// merged into the corresponding model in the <see cref="CategoryTypeManagementViewModel.ColorViewModels"/> collection.
    /// </param>
    /// <remarks>
    /// This method locates the target <see cref="ColorViewModel"/> in the collection based on its <see cref="ColorViewModel.Id"/>.
    /// If a match is found, the mapped data is merged into the existing model using the <see cref="ISystemDtoViewModelMapper.Merge"/> method.
    /// If no matching model is found, the method terminates without any changes.
    /// </remarks>
    private void ApplyUpdate(ColorViewModel vm)
    {
        var item = ColorViewModels.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _systemDtoViewModelMapper.Merge(vm, item);
    }

    /// <summary>
    /// Handles the changes in the category type by processing incoming messages
    /// containing information about the data action (add or update) and applying
    /// the corresponding updates to the list of category types.
    /// </summary>
    /// <param name="recipient">
    /// The recipient object that registered to listen for category type change messages. Typically, this will
    /// be the instance of the view model itself.
    /// </param>
    /// <param name="message">
    /// The message containing details of the category type change. This includes the entity type, the action
    /// performed (add or update), and the content that is either being added or updated.
    /// </param>
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

    /// <summary>
    /// Adds a new <see cref="CategoryTypeViewModel"/> to the collection of category type view models and ensures
    /// the collection remains sorted based on the category type name.
    /// </summary>
    /// <param name="item">The <see cref="CategoryTypeViewModel"/> instance to be added to the collection.</param>
    private void ApplyAddAsync(CategoryTypeViewModel item)
        => CategoryTypeViewModels.AddAndSort(item, vm => vm.Name!);

    /// <summary>
    /// Updates an existing category type in the collection using the provided ViewModel.
    /// </summary>
    /// <remarks>
    /// This method finds the corresponding category type in the CategoryTypeViewModels collection
    /// based on the Id of the specified ViewModel. If the matching category type exists,
    /// it merges the changes from the given ViewModel into the existing item using the
    /// IExpenseDtoViewModelMapper implementation.
    /// </remarks>
    /// <param name="vm">
    /// The ViewModel containing the updated category type data. The `Id` property of this
    /// ViewModel is used to locate the corresponding category type in the collection.
    /// </param>
    private void ApplyUpdate(CategoryTypeViewModel vm)
    {
        var item = CategoryTypeViewModels.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _expenseDtoViewModelMapper.Merge(vm, item);
    }

    /// <summary>
    /// Handles the deletion of entities based on the received message, updating or removing the corresponding
    /// items in the respective collections within the ViewModel.
    /// </summary>
    /// <param name="recipient">
    /// The recipient of the message, typically the instance of the ViewModel subscribing to the notification.
    /// </param>
    /// <param name="message">
    /// The message containing details about the deletion, including the entity type, the action performed,
    /// and the list of affected entity IDs.
    /// </param>
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
            _closeable?.DialogResult = false;
            _closeable?.Close();
        }
    }

    /// <summary>
    /// Deletes the currently selected category type and handles the associated dialog closure upon success.
    /// </summary>
    /// <param name="dialog">The dialog interface instance allowing for closure after the delete operation completes successfully.</param>
    /// <param name="cancellationToken">An optional cancellation token to observe for canceling the operation.</param>
    [RelayCommand]
    private async Task OnDelete(IClosable? dialog, CancellationToken cancellationToken = default)
    {
        var result = await _expenseActionService.DeleteCategoryType(CategoryTypeViewModel, cancellationToken);
        if (result) dialog?.Close();
    }

    /// <summary>
    /// Opens the category type management window, allowing the user to create or update a category type.
    /// </summary>
    /// <param name="categoryTypeViewModel">
    /// The <see cref="CategoryTypeViewModel"/> instance representing the category type to be managed.
    /// If null, a new category type is created.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    [RelayCommand]
    private Task ManageCategoryTypeAction(CategoryTypeViewModel? categoryTypeViewModel)
    {
        _navigationWindowService.ShowManageCategoryType(categoryTypeViewModel);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously loads all category types and populates the <see cref="CategoryTypeViewModels"/> collection.
    /// Sorts the category types by their names after loading.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [RelayCommand]
    private async Task OnLoadAllCategoryTypeAsync(CancellationToken cancellationToken = default)
    {
        var categoryType = await _expensePresentationService.GetAllCategoryTypeViewModelAsync(cancellationToken);
        CategoryTypeViewModels.AddRangeAndSort(categoryType, vm => vm.Name!, logger: _logger);
    }

    /// <summary>
    /// Asynchronously loads all color ViewModels and adds them to the ColorViewModels collection.
    /// The collection is then sorted based on the Name property of each color ViewModel.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [RelayCommand]
    private async Task OnLoadAllColorAsync(CancellationToken cancellationToken = default)
    {
        var colors = await _systemPresentationService.GetAllColorViewModelAsync(cancellationToken);
        ColorViewModels.AddRangeAndSort(colors, vm => vm.Name!);

        if (_categoryTypeViewModelToLoad is not null) LoadCategoryTypeViewModel();
    }

    /// <summary>
    /// Loads a specific CategoryTypeViewModel into the CategoryTypeManagementViewModel
    /// to be used for further operations, such as editing or viewing.
    /// </summary>
    /// <param name="categoryTypeViewModel">
    /// An instance of CategoryTypeViewModel to be loaded and managed within the context
    /// of the CategoryTypeManagementViewModel. This parameter provides the data and state
    /// required for subsequent operations.
    /// </param>
    public void LoadCategoryTypeViewModel(CategoryTypeViewModel categoryTypeViewModel)
        => _categoryTypeViewModelToLoad = categoryTypeViewModel;

    /// <summary>
    /// Loads and maps the details of a specific category type into the ViewModel for editing purposes.
    /// </summary>
    /// <remarks>
    /// This method is responsible for transferring and merging data from a source CategoryTypeViewModel
    /// into the main ViewModel. Upon execution, it synchronizes the associated color from the available
    /// colors collection, ensuring the color is correctly selected in the ViewModel. Additionally, it
    /// marks the ViewModel as being in an editing state.
    /// </remarks>
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