using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Resources.Resx.SystemResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.ViewModel;

/// <summary>
/// Represents the ViewModel for managing color data in the application.
/// This class provides functionality to manage color records, including
/// adding, editing, deleting, and validating color configurations.
/// Integrates services for system actions, dialogs, mapping, and logging.
/// </summary>
public partial class ColorManagementViewModel : ViewModelBase
{
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

    /// Represents a command that cancels any ongoing color modification process
    /// in the color management view model. This command is typically invoked
    /// when the user chooses to discard changes and exit the context of editing
    /// or creating a color configuration.
    /// The command uses the IClosable parameter, enabling interaction with
    /// window-like components such as dialogs or other closable views. When executed,
    /// it is expected to perform the following actions:
    /// - Discard unsaved color changes or edits.
    /// - Optionally close the associated UI component, such as a dialog, to exit
    /// the editing context.
    /// - Reset the current state of the color management view model if needed to
    /// prepare it for later operations.
    /// This command is fundamental to ensuring that users can abandon their changes
    /// safely and return to prior states without risk of unintended data persistence,
    /// enhancing the usability and workflow consistency within the application.
    public IRelayCommand<IClosable?> CancelCommand { get; }

    /// Represents an asynchronous command that validates the current changes to a color within the color management context.
    /// This property is typically bound to a UI control, such as a validation button, and is executed when the user
    /// confirms their intent to save or apply the changes.
    /// The command uses the IClosable parameter, enabling interaction with window-like components such as dialogs
    /// or other closable views. When executed, it is expected to perform the following actions:
    /// - Validate the current state of the color configuration.
    /// - Persist the validated color settings, interacting with underlying services or data stores as necessary.
    /// - Optionally close the associated UI, indicating successful validation.
    /// This command is essential for ensuring users can commit changes to a color configuration, following the MVVM pattern
    /// and maintaining a seamless user experience.
    public IAsyncRelayCommand<IClosable?> ValidCommand { get; }

    /// Represents an asynchronous command that handles the deletion of a color within the color management context.
    /// This property is typically bound to a UI control, such as a delete button, and is executed when the user
    /// initiates the delete action.
    /// The command uses the IClosable parameter, allowing interaction with window-like elements
    /// such as dialogs or other closable views. When executed, it is expected to perform the following steps:
    /// - Initiate the deletion of the specified color.
    /// - Interact with additional services (e.g., dialog or messaging services) to confirm the action if needed.
    /// - Optionally close the associated UI component.
    /// This command is critical for enabling users to remove a color, while ensuring the
    /// interaction follows the MVVM pattern and adheres to the application's user experience design.
    public IAsyncRelayCommand<IClosable?> DeleteCommand { get; }

    /// <summary>
    /// Represents the ViewModel for managing color records. This class provides
    /// commands and methods to handle color data, including adding, editing, deleting,
    /// and loading color configurations. It integrates dialog services, logging,
    /// and system-specific mapping functionality.
    /// </summary>
    public ColorManagementViewModel(ISystemActionService systemActionService,
        IDialogService dialogService,
        ISystemDtoViewModelMapper systemDtoViewModelMapper,
        ILogger<ColorManagementViewModel> logger)
    {
        _systemActionService = systemActionService;
        _dialogService = dialogService;
        _systemDtoViewModelMapper = systemDtoViewModelMapper;
        _logger = logger;

        CancelCommand = new RelayCommand<IClosable?>(OnCancel);
        ValidCommand = new AsyncRelayCommand<IClosable?>(OnValidAsync);
        DeleteCommand = new AsyncRelayCommand<IClosable?>(OnDeleteAsync);
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
    private async Task OnDeleteAsync(IClosable? dialog, CancellationToken cancellationToken = default)
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
    private async Task OnValidAsync(IClosable? dialog, CancellationToken cancellationToken = default)
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