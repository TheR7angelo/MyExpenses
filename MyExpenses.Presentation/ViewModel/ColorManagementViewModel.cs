using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.SystemResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Systems;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

/// <summary>
/// Represents the ViewModel for managing color data in the application.
/// This class provides functionality to manage color records, including
/// adding, editing, deleting, and validating color configurations.
/// Integrates services for system actions, dialogs, mapping, and logging.
/// </summary>
public partial class ColorManagementViewModel : ViewModelBase
{
    private readonly INavigationWindowService _navigationWindowService;

    /// Provides functionality to display dialog boxes and message prompts within the application.
    /// This service is responsible for facilitating user interactions such as displaying error messages,
    /// confirmation dialogs, and text input prompts, ensuring a consistent communication mechanism
    /// between the user interface and the application logic.
    /// Key responsibilities include:
    /// - Displaying customizable message boxes with support for various button configurations and icons.
    /// - Handling user input through input dialogs and retrieving user-provided data.
    /// - Offering confirmation dialogs for specific operations, such as dependency removal.
    /// This service plays a critical role in enhancing user experience by managing dialog-based interactions
    /// and presenting relevant feedback or options to guide user decisions.
    private readonly IDialogService _dialogService;

    /// Facilitates the mapping and transformation between DTO (Data Transfer Object) and ViewModel representations
    /// within the context of the ColorManagementViewModel. This mapper is a critical component responsible for
    /// ensuring consistency and reliability when converting data between layers and maintaining synchronization.
    /// Core responsibilities include:
    /// - Mapping ColorDto objects to their corresponding ViewModel representations and vice versa.
    /// - Providing cloning capabilities for ViewModel instances to support independent manipulations.
    /// - Merging properties from one ViewModel instance into another, enabling seamless updates and synchronization across instances.
    /// This mapper plays a vital role in supporting the functionality of the ViewModel by bridging the gap
    /// between data structures and user-interface models.
    private readonly ISystemDtoViewModelMapper _systemDtoViewModelMapper;

    /// Provides logging capabilities for the ColorManagementViewModel class.
    /// This logger is used to record information, warnings, and error messages related to the operations
    /// performed in the color management process. It plays a critical role in helping to troubleshoot
    /// issues and monitor the execution flow within the ViewModel.
    /// Key responsibilities include:
    /// - Logging error details during operations such as deleting or creating colors.
    /// - Supporting the debugging process by recording critical events and exceptions.
    /// - Enhancing maintainability by providing diagnostic insights for developers.
    private readonly ILogger<ColorManagementViewModel> _logger;

    /// Provides access to the core system actions related to color management within the application.
    /// This service is responsible for handling operations such as creating, updating, and deleting
    /// color data. It serves as an abstraction layer to perform these actions effectively, ensuring
    /// proper interaction between the ViewModel and the underlying business logic.
    /// Key responsibilities include:
    /// - Creating new color entries using the `CreateColorAsync` method.
    /// - Updating existing color entries with the `UpdateColorAsync` method.
    /// - Deleting color entries via the `DeleteColorAsync` method.
    /// By using this service, the ViewModel delegates the execution of business operations and
    /// focuses solely on managing state and user interactions. This separation of concerns enhances
    /// modularity, maintainability, and testability within the application.
    private readonly ISystemActionService _systemActionService;

    /// Provides functionality to manage system-related presentation concerns within the application.
    /// This service is responsible for retrieving, organizing, and preparing data models for UI consumption,
    /// while ensuring that the system components interact seamlessly with presentation-level operations.
    /// Key responsibilities include:
    /// - Managing and retrieving dependencies relevant to various view models, such as account types, categories, and currencies.
    /// - Accessing, preparing, and delivering presentation data in a way that supports the application's user interface requirements.
    /// - Enhancing the separation of concerns by abstracting presentation logic and data preparation from core application logic.
    /// This service plays a critical role in coordinating between system-level data and the user interface,
    /// thereby contributing to a more functional and organized presentation layer.
    private readonly ISystemPresentationService _systemPresentationService;

    /// Indicates whether the current color operation in the color management workflow
    /// is in "edit" mode. This property is used to distinguish between creating a new color
    /// and editing an existing one within the application.
    /// When `true`:
    /// - The system assumes that the user is modifying the attributes of an existing color.
    /// - The user interface is updated accordingly to reflect edit-specific actions, such as
    /// displaying delete options and setting appropriate titles or labels.
    /// When `false`:
    /// - The system treats the operation as the creation of a new color.
    /// - The interface behavior adjusts to reflect creation-related workflows, hiding elements
    /// irrelevant in this mode, such as delete buttons.
    /// In conjunction with commands and user-interface triggers, this property enables the
    /// application to dynamically adapt its functionality and presentation based on the user's
    /// intent within the color management system. It is also used to determine the appropriate
    /// service method (e.g., updating vs. creating) when persisting changes.
    [ObservableProperty]
    public partial bool IsEditColor { get; set; }

    /// Represents the core view model used for managing color-related operations
    /// within the color management system. This view model serves as a bridge
    /// between the user interface and the underlying color data model, encapsulating
    /// all properties and behaviors necessary for creating, editing, and deleting colors.
    /// The `ColorViewModel` instance within this view model holds the state and attributes
    /// of the current color being manipulated by the user. It is used across various
    /// commands and workflows, enabling centralized management of color-related data.
    /// Key responsibilities of this property include:
    /// - Reflecting the current color's state (e.g., name, attributes) during ongoing operations.
    /// - Acting as the primary data source for user-interface bindings, such as input fields
    /// and selection controls within the color management window.
    /// - Allowing modifications to its content, which are tracked for changes using dirty
    /// tracking mechanisms to facilitate validation and updates.
    /// This property plays a vital role in ensuring seamless user interactions with
    /// color-related functionalities, maintaining synchronization between the GUI and
    /// application logic, and helping enforce consistency across the color management module.
    public ColorViewModel ColorViewModel { get; } = new();

    /// Represents a collection of color view models used in the application.
    /// This property is responsible for managing the list of colors displayed or manipulated
    /// within the color management functionality. It supports adding, updating, and
    /// removing individual color records, ensuring the collection remains synchronized
    /// with user actions and application logic.
    /// Key responsibilities include:
    /// - Maintaining an observable list of `ColorViewModel` instances for data binding.
    /// - Providing support for managing and updating the list dynamically.
    /// - Serving as the primary source of color-related data for the user interface.
    public ObservableCollection<ColorViewModel> ColorViewModels { get; } = [];

    /// <summary>
    /// Represents the ViewModel for managing color records. This class provides
    /// commands and methods to handle color data, including adding, editing, deleting,
    /// and loading color configurations. It integrates dialog services, logging,
    /// and system-specific mapping functionality.
    /// </summary>
    public ColorManagementViewModel(ISystemActionService systemActionService,
        ISystemPresentationService systemPresentationService,
        INavigationWindowService navigationWindowService,
        IDialogService dialogService,
        ISystemDtoViewModelMapper systemDtoViewModelMapper,
        ILogger<ColorManagementViewModel> logger)
    {
        _systemActionService = systemActionService;
        _systemPresentationService = systemPresentationService;
        _navigationWindowService = navigationWindowService;
        _dialogService = dialogService;
        _systemDtoViewModelMapper = systemDtoViewModelMapper;
        _logger = logger;

        RegisterMessages();
    }

    /// <summary>
    /// Registers message handlers to listen for entity changes related to color data.
    /// This method subscribes to notifications for deletions and updates of color-related records,
    /// enabling the ViewModel to respond dynamically to changes in the application's state.
    /// </summary>
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnColorDelete);
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
        => ColorViewModels.AddAndSort(item, vm => vm.Name!);

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
    private void OnColorDelete(object recipient, EntityChangedMessage<int[]> message)
    {
        if (message.Value.DataAction is not DataAction.Delete || message.Value.EntityType is not DependencyType.Color) return;

        foreach (var item in ColorViewModels.Where(s => message.Value.Content.Contains(s.Id)))
        {
            item.IsDeleting = true;
        }
    }

    /// <summary>
    /// Handles the action associated with managing a specific color. This method is typically invoked
    /// to open a color management window for viewing or editing the specified color data.
    /// </summary>
    /// <param name="vm">The instance of <see cref="ColorViewModel"/> representing the color to be managed.
    /// If null, the action may initialize with a default or empty state for creating a new color.</param>
    [RelayCommand]
    private void OnManageColor(ColorViewModel? vm)
        => _navigationWindowService.ShowColorManagementWindow(vm);

    /// <summary>
    /// Removes the specified color item from the collection of color view models.
    /// </summary>
    /// <param name="item">The <see cref="ColorViewModel"/> instance to be removed from the collection.
    /// If the item is null, the method execution is skipped.</param>
    [RelayCommand]
    private void OnRemove(ColorViewModel? item)
    {
        if (item is null) return;

        ColorViewModels.Remove(item);
    }

    /// <summary>
    /// Asynchronously loads all color view models into the observable collection and sorts them by name.
    /// This method fetches color data from the system presentation service and updates the
    /// <see cref="ColorViewModels"/> collection with the results.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [RelayCommand]
    private async Task OnLoad(CancellationToken cancellationToken = default)
    {
        var colors = await _systemPresentationService.GetAllColorViewModelAsync(cancellationToken);
        ColorViewModels.AddRangeAndSort(colors, s => s.Name!);
    }

    /// <summary>
    /// Deletes the current color asynchronously. If the operation is successful,
    /// the associated dialog is closed. In case of failure, an error message is displayed.
    /// </summary>
    /// <param name="dialog">
    /// An instance of <see cref="IClosable"/> representing the dialog to be closed upon successful deletion.
    /// If null, no dialog is closed.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> used to propagate the signal that the operation should be canceled.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous delete operation.
    /// </returns>
    [RelayCommand]
    private async Task OnDelete(IClosable? dialog, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _systemActionService.DeleteColorAsync(ColorViewModel, cancellationToken);
            if (result.IsSuccess) dialog?.Close();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting color");

            _dialogService.ShowMessageBox(SystemResources.MessageboxColorDeleteErrorCaption,
                SystemResources.MessageboxColorDeleteErrorContent,
                MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }

    /// <summary>
    /// Validates and processes the current color operation asynchronously.
    /// If in edit mode, updates the existing color; otherwise, creates a new color.
    /// On success, closes the dialog.
    /// </summary>
    /// <param name="dialog">
    /// The instance of <see cref="IClosable"/> representing the dialog to be closed upon successful operation.
    /// If null, no dialog is closed.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> used to propagate the signal that the operation should be canceled.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous validation operation.
    /// </returns>
    [RelayCommand]
    private async Task OnValid(IClosable? dialog, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = IsEditColor
                ? await _systemActionService.UpdateColorAsync(ColorViewModel, cancellationToken)
                : await _systemActionService.CreateColorAsync(ColorViewModel, cancellationToken);

            if (result.IsSuccess) dialog?.Close();
        }
        catch (Exception e)
        {
            if (IsEditColor)
            {
                _logger.LogError(e, "Error updating color");

                _dialogService.ShowMessageBox(SystemResources.MessageboxColorUpdateErrorCaption,
                    SystemResources.MessageboxColorUpdateErrorContent,
                    MessageBoxButton.Ok, MsgBoxImage.Error);
            }
            else
            {
                _logger.LogError(e, "Error creating color");

                _dialogService.ShowMessageBox(SystemResources.MessageboxColorCreateErrorCaption,
                    SystemResources.MessageboxColorCreateErrorContent,
                    MessageBoxButton.Ok, MsgBoxImage.Error);
            }
        }
    }

    /// <summary>
    /// Handles the cancellation of the current operation by closing the dialog
    /// and setting its <see cref="IClosable.DialogResult"/> to false.
    /// </summary>
    /// <param name="dialog">
    /// The instance of <see cref="IClosable"/> representing the dialog to be closed.
    /// If null, no action is performed.
    /// </param>
    [RelayCommand]
    private void OnCancel(IClosable? dialog)
    {
        dialog?.DialogResult = false;
        dialog?.Close();
    }

    /// <summary>
    /// Loads the specified <see cref="ColorViewModel"/> into the current instance,
    /// setting the appropriate state for color editing and merging the properties
    /// from the source view model into the target view model.
    /// </summary>
    /// <param name="colorViewModel">
    /// The source <see cref="ColorViewModel"/> containing the color properties to load.
    /// </param>
    public void LoadColorViewModel(ColorViewModel colorViewModel)
    {
        IsEditColor = true;
        _systemDtoViewModelMapper.Merge(colorViewModel, ColorViewModel);

        ColorViewModel.AcceptChanges();
    }
}