using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Resources.Resx.SystemResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations.Validator;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.Services;

public abstract class AActionService(IDialogService dialogService, ILogger logger,
    IServiceProvider serviceProvider)
{
    /// <summary>
    /// Manages the creation, editing, and deletion of a named entity through a series of user actions.
    /// The method handles validation, user interactions, and persistence operations for the specified entity.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of the view model used to represent the entity. Must be a class.
    /// </typeparam>
    /// <param name="viewModel">
    /// The current instance of the view model being edited. If null, the operation will be treated as creating a new entity.
    /// </param>
    /// <param name="getName">
    /// A function to retrieve the name or identifier of the entity from the view model.
    /// </param>
    /// <param name="setName">
    /// An action to set the name or identifier of the entity in the view model.
    /// </param>
    /// <param name="maxNameLength">
    /// The maximum allowed length for the name or identifier of the entity.
    /// </param>
    /// <param name="addTitle">
    /// The dialog title to display when creating a new entity.
    /// </param>
    /// <param name="editTitle">
    /// The dialog title to display when editing an existing entity.
    /// </param>
    /// <param name="addPlaceholder">
    /// The placeholder text to display in the input field when creating a new entity.
    /// </param>
    /// <param name="editPlaceholder">
    /// The placeholder text to display in the input field when editing an existing entity.
    /// </param>
    /// <param name="createValidationViewModel">
    /// A function to create a new instance of the view model used for validation during the process.
    /// </param>
    /// <param name="cloneValidationViewModel">
    /// A function to clone the existing view model for validation purposes.
    /// </param>
    /// <param name="beforeValidationAsync">
    /// An asynchronous action to be executed before performing validation.
    /// </param>
    /// <param name="validateAsync">
    /// A function to asynchronously validate the view model and return a validation result.
    /// </param>
    /// <param name="logValidationError">
    /// An action to log validation errors, allowing for diagnostic and feedback purposes.
    /// </param>
    /// <param name="deleteAsync">
    /// A function to asynchronously delete the specified entity.
    /// </param>
    /// <param name="createAsync">
    /// A function to asynchronously create a new entity using the specified name.
    /// </param>
    /// <param name="updateAsync">
    /// A function to asynchronously update the specified entity with a new name.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor or cancel the asynchronous operations.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task completes when the user finishes the interaction
    /// or the operation is canceled.
    /// </returns>
    internal async Task ManageNamedEntityAction<TViewModel>(TViewModel? viewModel,
        Func<TViewModel, string?> getName,
        Action<TViewModel, string?> setName,
        int maxNameLength, string addTitle, string editTitle, string addPlaceholder, string editPlaceholder,
        Func<TViewModel> createValidationViewModel,
        Func<TViewModel, TViewModel> cloneValidationViewModel,
        Func<TViewModel, Task> beforeValidationAsync,
        Func<TViewModel, CancellationToken, Task<ValidationResult>> validateAsync,
        Action<ValidationFailure> logValidationError,
        Func<TViewModel, CancellationToken, Task> deleteAsync,
        Func<string, CancellationToken, Task> createAsync,
        Func<TViewModel, string, CancellationToken, Task> updateAsync,
        CancellationToken cancellationToken)
        where TViewModel : class
    {
        while (true)
        {
            var dialogContext = ShowEntityInputDialog(viewModel, getName, maxNameLength, addTitle, editTitle,
                addPlaceholder, editPlaceholder);

            if (dialogContext.ShouldCancel) return;

            if (dialogContext.ShouldDelete)
            {
                await deleteAsync(viewModel!, cancellationToken);
                return;
            }

            var isValid = await ValidateEntityInputAsync(viewModel, dialogContext.Input, setName,
                createValidationViewModel, cloneValidationViewModel, beforeValidationAsync, validateAsync,
                logValidationError, cancellationToken);

            if (!isValid) continue;

            await ExecuteEntitySaveActionAsync(viewModel, dialogContext.Input!,
                createAsync, updateAsync, cancellationToken);
            return;
        }
    }

    /// <summary>
    /// Displays an input dialog for creating or editing an entity, and captures the user interaction and input.
    /// The dialog dynamically adjusts based on whether the operation is for adding or editing an entity.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of the view model associated with the entity. Must be a class.
    /// </typeparam>
    /// <param name="currentViewModel">
    /// The current instance of the view model being edited. If null, the dialog will be configured for creating a new entity.
    /// </param>
    /// <param name="getName">
    /// A function to retrieve the name or identifier of the entity from the view model.
    /// </param>
    /// <param name="maxNameLength">
    /// The maximum allowed length for the input provided in the dialog.
    /// </param>
    /// <param name="addTitle">
    /// The title of the dialog when performing an add operation.
    /// </param>
    /// <param name="editTitle">
    /// The title of the dialog when performing an edit operation.
    /// </param>
    /// <param name="addPlaceholder">
    /// The placeholder text displayed in the input field for adding a new entity.
    /// </param>
    /// <param name="editPlaceholder">
    /// The placeholder text displayed in the input field for editing an existing entity.
    /// </param>
    /// <returns>
    /// A context object containing the captured user input, a flag indicating whether the operation was canceled,
    /// and another flag indicating if the entity should be deleted (applicable only for edit operations).
    /// </returns>
    private EntityInputDialogContext ShowEntityInputDialog<TViewModel>(TViewModel? currentViewModel,
        Func<TViewModel, string?> getName,
        int maxNameLength, string addTitle, string editTitle, string addPlaceholder, string editPlaceholder)
        where TViewModel : class
    {
        var isEdit = currentViewModel is not null;
        var currentName = isEdit ? getName(currentViewModel!) ?? string.Empty : string.Empty;
        var title = isEdit ? editTitle : addTitle;
        var placeholder = isEdit ? editPlaceholder : addPlaceholder;

        var dialogResult = dialogService.ShowInputDialog(title, currentName,
            out var btnResult,
            out var input,
            maxNameLength, placeholder);

        return new EntityInputDialogContext(Input: input,
            ShouldCancel: dialogResult is not true || btnResult is MessageBoxInputResult.Cancel,
            ShouldDelete: btnResult is MessageBoxInputResult.Delete && isEdit);
    }

    /// <summary>
    /// Asynchronously validates the input for an entity using the provided validation functions and behaviors.
    /// If the input is valid, the method returns true. Otherwise, it logs validation errors and returns false.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model used for validation. Must be a class.</typeparam>
    /// <param name="currentViewModel">
    /// The current instance of the view model. If null, a new view model is created or the input is cloned for validation.
    /// </param>
    /// <param name="input">
    /// The input string provided by the user that needs validation.
    /// </param>
    /// <param name="setName">
    /// An action to set the input value onto the validation view model.
    /// </param>
    /// <param name="createValidationViewModel">
    /// A function to create a new instance of the view model for validation if no current view model exists.
    /// </param>
    /// <param name="cloneValidationViewModel">
    /// A function to clone an existing view model for validation if the current view model is provided.
    /// </param>
    /// <param name="beforeValidationAsync">
    /// A function to execute any asynchronous pre-validation logic for the view model.
    /// </param>
    /// <param name="validateAsync">
    /// A function to perform asynchronous validation logic on the view model, returning a validation result.
    /// </param>
    /// <param name="logValidationError">
    /// An action to log validation errors for further processing or debugging purposes.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests and gracefully terminate the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous validation operation. The task result is true if the input is valid; otherwise, false.
    /// </returns>
    private async Task<bool> ValidateEntityInputAsync<TViewModel>(TViewModel? currentViewModel, string? input,
        Action<TViewModel, string?> setName,
        Func<TViewModel> createValidationViewModel,
        Func<TViewModel, TViewModel> cloneValidationViewModel,
        Func<TViewModel, Task> beforeValidationAsync,
        Func<TViewModel, CancellationToken, Task<ValidationResult>> validateAsync,
        Action<ValidationFailure> logValidationError, CancellationToken cancellationToken) where TViewModel : class
    {
        var validationViewModel = CreateValidationViewModel(currentViewModel, createValidationViewModel, cloneValidationViewModel);

        setName(validationViewModel, input);
        await beforeValidationAsync(validationViewModel);
        var validationResult = await validateAsync(validationViewModel, cancellationToken);

        if (validationResult.IsValid) return true;

        ShowValidationError(validationResult, logValidationError);
        return false;
    }

    /// <summary>
    /// Creates a validation view model by either cloning the provided current view model or creating a new instance.
    /// If the current view model is not null, it will be cloned using the specified cloning function.
    /// Otherwise, a new instance will be created using the provided creation function.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model to be created or cloned. Must be a class.</typeparam>
    /// <param name="currentViewModel">
    /// The current instance of the view model to be used for cloning.
    /// If null, a new instance is created instead of cloning.
    /// </param>
    /// <param name="createValidationViewModel">
    /// A function to create a new instance of the validation view model when no current instance is provided.
    /// </param>
    /// <param name="cloneValidationViewModel">
    /// A function to clone an existing validation view model when the current instance is provided.
    /// </param>
    /// <returns>
    /// A new or cloned validation view model based on the provided input.
    /// </returns>
    private static TViewModel CreateValidationViewModel<TViewModel>(TViewModel? currentViewModel,
        Func<TViewModel> createValidationViewModel,
        Func<TViewModel, TViewModel> cloneValidationViewModel) where TViewModel : class
    {
        return currentViewModel is not null
            ? cloneValidationViewModel(currentViewModel)
            : createValidationViewModel();
    }

    /// <summary>
    /// Executes the save action for the given entity based on its current state.
    /// If an existing entity is provided, it will be updated using the specified update function.
    /// If no current entity exists, a new entity will be created using the specified create function.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the entity or view model to be saved. Must be a class.</typeparam>
    /// <param name="currentViewModel">
    /// The current instance of the entity or view model. If null, a new entity will be created.
    /// </param>
    /// <param name="input">
    /// The string input associated with the entity save operation. Typically used for naming or identifying the entity.
    /// </param>
    /// <param name="createAsync">
    /// A function to asynchronously create a new entity using the provided input.
    /// The function takes the input string and a cancellation token.
    /// </param>
    /// <param name="updateAsync">
    /// A function to asynchronously update an existing entity using the provided input.
    /// The function takes the current entity, the input string, and a cancellation token.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe cancellation requests during the save operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous save operation.
    /// </returns>
    private static Task ExecuteEntitySaveActionAsync<TViewModel>(TViewModel? currentViewModel, string input,
        Func<string, CancellationToken, Task> createAsync,
        Func<TViewModel, string, CancellationToken, Task> updateAsync,
        CancellationToken cancellationToken) where TViewModel : class
    {
        return currentViewModel is not null
            ? updateAsync(currentViewModel, input, cancellationToken)
            : createAsync(input, cancellationToken);
    }

    /// <summary>
    /// Validates the specified view model asynchronously using the provided validator type.
    /// </summary>
    /// <typeparam name="TValidator">The type of the validator to be used for validation. Must inherit from AbstractValidator<TEntity>.</typeparam>
    /// <typeparam name="TEntity">The type of the entity or view model to be validated.</typeparam>
    /// <param name="viewModel">The view model or entity instance to validate. Must not be null.</param>
    /// <param name="cancellationToken">A cancellation token to observe cancellation requests.</param>
    /// <returns>A task representing the asynchronous validation operation. The result contains the validation result with details about any validation failures.</returns>
    internal async Task<ValidationResult> ValidateAsync<TValidator, TEntity>(TEntity viewModel,
        CancellationToken cancellationToken)
        where TValidator : AbstractValidator<TEntity>
    {
        var validator = serviceProvider.GetRequiredService<TValidator>();
        var validationResult = await validator.ValidateAsync(viewModel, cancellationToken);

        if (validationResult.IsValid) logger.LogInformation("Validation successful for {EntityName}", typeof(TEntity).Name);
        else
        {
            logger.LogError("Validation failed for {EntityName} with result {@Errors}", typeof(TEntity).Name, validationResult.Errors);
        }

        return validationResult;
    }

    /// <summary>
    /// Displays a validation error message based on the provided validation result and logs the first error using the specified logging action.
    /// </summary>
    /// <param name="validationResult">The validation result containing details about validation failures. It must not be null and should contain at least one error.</param>
    /// <param name="logValidationError">An action to log detailed information about the validation failure. Must not be null.</param>
    private void ShowValidationError(ValidationResult validationResult, Action<ValidationFailure> logValidationError)
    {
        var error = validationResult.Errors.First();

        dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption, error.ErrorMessage,
            MsgBoxImage.Error);

        logValidationError(error);
    }

    /// <summary>
    /// Logs a detailed domain validation error message for the specified entity,
    /// providing information about the failure's context and details.
    /// </summary>
    /// <param name="entityName">The name of the entity associated with the validation failure. Cannot be null or empty.</param>
    /// <param name="error">The validation failure containing details about the error, including custom state data.</param>
    internal void LogDomainValidationError(string entityName, ValidationFailure error)
    {
        if (error.CustomState is not DomainValidationFailure domainError) return;

        logger.LogError("Validation failed for {EntityName} with error {ErrorCodeString}: {InternalMessage}",
            entityName, domainError.ErrorCodeString, domainError.InternalMessage);
    }

    /// <summary>
    /// Displays a confirmation dialog asking the user for confirmation to create an item,
    /// using the provided name as part of the prompt message.
    /// </summary>
    /// <param name="name">The name of the item to be created. Cannot be null or empty.</param>
    /// <returns>A boolean indicating whether the user confirmed the creation (true) or rejected it (false).</returns>
    internal bool AskCreateConfirmation(string name)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemQuestionCaption,
            string.Format(AccountResources.MessageBoxCreateItemQuestionContent, name),
            MessageBoxButton.YesNo,MsgBoxImage.Question);

        return response is MessageBoxResult.Yes;
    }

    /// <summary>
    /// Displays a confirmation dialog asking the user for confirmation to update an item,
    /// using the old and new values as part of the prompt message.
    /// </summary>
    /// <param name="oldName">The old name or value of the item being updated. Can be null if not available.</param>
    /// <param name="newName">The new name or value of the item being updated. Cannot be null.</param>
    /// <returns>A boolean indicating whether the user confirmed the update (true) or rejected it (false).</returns>
    internal bool AskUpdateConfirmation(string? oldName, string? newName)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemQuestionCaption,
            string.Format(AccountResources.MessageBoxEditItemQuestionContent, oldName, newName),
            MessageBoxButton.YesNo,MsgBoxImage.Question);

        return response is MessageBoxResult.Yes;
    }

    /// <summary>
    /// Displays a confirmation dialog to the user for pending updates, showing detailed information
    /// about the changes and allowing the user to accept or decline the update process.
    /// </summary>
    /// <param name="propertiesName">
    /// An array of property names that are being updated. These represent the labels or display names
    /// for the properties involved in the change.
    /// </param>
    /// <param name="oldValues">
    /// An array of the old values of the properties prior to the update. The order must match
    /// the <paramref name="propertiesName"/> array.
    /// </param>
    /// <param name="newValues">
    /// An array of the new values that the properties will be updated to. The order must match
    /// the <paramref name="propertiesName"/> array.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the user confirms the update; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the lengths of <paramref name="propertiesName"/>, <paramref name="oldValues"/>,
    /// and <paramref name="newValues"/> are not equal.
    /// </exception>
    private bool AskUpdateConfirmation(string?[] propertiesName, string?[] oldValues, string?[] newValues)
    {
        if (propertiesName.Length != oldValues.Length || oldValues.Length != newValues.Length)
        {
            throw new ArgumentException(@"the number of old names must be equal to the number of new names", nameof(oldValues));
        }

        var lines = propertiesName.Select((t, i) => $"- {t}: \"{oldValues[i]}\" → \"{newValues[i]}\"");
        var changes = string.Join(Environment.NewLine, lines);

        var response = dialogService.ShowMessageBox(SystemResources.MessageBoxEditItemsQuestionContent,
            string.Format(SystemResources.MessageBoxEditItemsQuestionContent, Environment.NewLine, changes),
            MessageBoxButton.YesNo,MsgBoxImage.Question);

        return response is MessageBoxResult.Yes;
    }

    /// <summary>
    /// Prompts the user to confirm whether they wish to proceed with updating the displayed changes
    /// in a tracked entity. The method uses the pending changes of the provided trackable entity
    /// to display old and new values for confirmation.
    /// </summary>
    /// <param name="dirtyTrackable">
    /// An object implementing the <see cref="IDirtyTrackable"/> interface, which provides access to pending
    /// changes including old and new display values of the entity being tracked.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the update was confirmed. Returns <c>true</c> if the user
    /// confirms, otherwise <c>false</c>.
    /// </returns>
    internal bool AskUpdateConfirmation(IDirtyTrackable dirtyTrackable)
    {
        var pendingChanges = dirtyTrackable.PendingChanges;
        var propertyNames = pendingChanges.Select(s => s.DisplayPropertyName).ToArray();
        var oldValues = pendingChanges.Select(s => s.OldValueDisplay).ToArray();
        var newValues = pendingChanges.Select(s => s.NewValueDisplay).ToArray();

        return AskUpdateConfirmation(propertyNames, oldValues, newValues);
    }

    /// <summary>
    /// Displays a confirmation dialog asking the user if they want to proceed with the deletion of the specified item.
    /// </summary>
    /// <param name="name">The name of the item to be deleted. Can be null if the item name is not available.</param>
    /// <returns>A <see cref="MessageBoxResult"/> indicating the user's choice, such as Yes or No.</returns>
    internal MessageBoxResult AskDeleteConfirmation(string? name)
    {
        return dialogService.ShowMessageBox(AccountResources.MessageBoxDeleteItemQuestionCaption,
            string.Format(AccountResources.MessageBoxDeleteItemQuestionContent, name),
            MessageBoxButton.YesNo, MsgBoxImage.Question);
    }

    /// <summary>
    /// Displays a message to the user indicating the result of a create operation.
    /// </summary>
    /// <param name="isSuccess">A boolean value indicating whether the create operation was successful.</param>
    /// <param name="name">The name of the item associated with the create operation.</param>
    internal void ShowCreateResultMessage(bool isSuccess, string name)
    {
        if (isSuccess)
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemSuccessCaption,
                string.Format(AccountResources.MessageBoxCreateItemSuccessContent, name), MsgBoxImage.Check);

            return;
        }

        dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption,
            string.Format(AccountResources.MessageBoxCreateItemErrorContent, name), MsgBoxImage.Error);
    }

    /// <summary>
    /// Displays a message to the user indicating the result of an update operation.
    /// </summary>
    /// <param name="isSuccess">A boolean value indicating whether the update operation was successful.</param>
    internal void ShowUpdateResultMessage(bool isSuccess)
    {
        if (isSuccess)
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemSuccessCaption,
                AccountResources.MessageBoxEditItemSuccessContent, MsgBoxImage.Check);

            return;
        }

        dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemErrorCaption,
            AccountResources.MessageBoxEditItemErrorContent, MsgBoxImage.Error);
    }

    /// <summary>
    /// Displays a message to the user indicating the result of a delete operation.
    /// </summary>
    /// <param name="isSuccess">A boolean value indicating whether the delete operation was successful.</param>
    /// <param name="name">The name of the entity that was attempted to be deleted. Can be null.</param>
    internal void ShowDeleteResultMessage(bool isSuccess, string? name)
    {
        if (isSuccess)
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxDeleteItemSuccessCaption,
                string.Format(AccountResources.MessageBoxDeleteItemSuccessContent, name),
                MsgBoxImage.Check);
            return;
        }

        dialogService.ShowMessageBox(AccountResources.MessageBoxDeletetemErrorCaption,
            AccountResources.MessageBoxDeleteteErrorContent,MsgBoxImage.Error);
    }

    /// <summary>
    /// Sends a notification message for each entity type and its associated identifiers
    /// when deletion actions are performed, ensuring that other parts of the system are
    /// informed about the changes. This method is called only when there are deleted items.
    /// </summary>
    /// <param name="deletedItems">
    /// A dictionary containing the types of entities that were deleted as keys,
    /// and arrays of the corresponding identifiers of the deleted items as values.
    /// If null, the method does nothing.
    /// </param>
    internal static void SendDeletedMessageIfNeeded(Dictionary<DependencyType, int[]>? deletedItems)
    {
        if (deletedItems is null) return;
        foreach (var key in deletedItems.Keys)
        {
            SendEntityChangedMessage(key, DataAction.Delete, deletedItems[key]);
        }
    }

    /// <summary>
    /// Sends a message indicating that an entity has been changed.
    /// This message is distributed using the WeakReferenceMessenger to notify subscribers about the change.
    /// </summary>
    /// <typeparam name="T">The type of the content associated with the entity that has changed.</typeparam>
    /// <param name="entityType">The type of the entity that has been changed, defined by the DependencyType enum.</param>
    /// <param name="dataAction">The action performed on the entity, defined by the DataAction enum.</param>
    /// <param name="content">The content or data associated with the changed entity.</param>
    internal static void SendEntityChangedMessage<T>(DependencyType entityType, DataAction dataAction, T content)
    {
        WeakReferenceMessenger.Default.Send(
            new EntityChangedMessage<T>(
                new EntityChanged<T>
                {
                    EntityType = entityType,
                    DataAction = dataAction,
                    Content = content
                }));
    }

    /// <summary>
    /// Represents the context for an input dialog interaction with named entities.
    /// This record is intended to encapsulate the input details and outcomes of the dialog,
    /// including whether the action should proceed, be canceled, or trigger a delete operation.
    /// </summary>
    private readonly record struct EntityInputDialogContext(
        string? Input,
        bool ShouldCancel,
        bool ShouldDelete);
}