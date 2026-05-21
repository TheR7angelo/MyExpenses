using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

/// <summary>
/// Represents the view model for managing accounts in the application.
/// </summary>
/// <remarks>
/// The <c>AccountManagementViewModel</c> class is responsible for providing commands and data
/// required for account management functionalities. It interacts with services such as
/// navigation, dialog handling, and logging to support user interactions for managing accounts.
/// This class is typically used as the data context for a corresponding view in the MVVM pattern
/// and provides binding support for account-related operations.
/// </remarks>
public partial class AccountManagementViewModel : ViewModelBase
{
    /// <summary>
    /// Service dependency used for handling account-related presentation logic.
    /// </summary>
    /// <remarks>
    /// The <c>_accountService</c> field is an instance of <see cref="IAccountPresentationService"/>
    /// that provides methods to retrieve and manage account data for the view model.
    /// It is primarily responsible for interacting with the application's account presentation layer
    /// and fetching the necessary data to populate UI components.
    /// This service ensures separation of concerns between the view model and the underlying
    /// data access/presentation operations.
    /// </remarks>
    private readonly IAccountPresentationService _accountService;

    /// <summary>
    /// Mapper dependency used for transforming between DTO and ViewModel representations of account-related data.
    /// </summary>
    /// <remarks>
    /// The <c>_accountDtoViewModelMapper</c> field is an instance of <see cref="IAccountDtoViewModelMapper"/>
    /// responsible for handling mapping operations between data transfer objects (DTOs) and their corresponding
    /// view models. It supports bi-directional conversion operations, allowing the application to
    /// translate data for UI representation and back for storage or processing logic.
    /// This mapper is critical in ensuring a clear separation between the business logic layer and the presentation layer
    /// by abstracting the transformation logic required to bridge these components.
    /// </remarks>
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;

    /// <summary>
    /// Service dependency used for handling navigation-related operations within the view model.
    /// </summary>
    /// <remarks>
    /// The <c>_navigation</c> field is an instance of <see cref="INavigationWindowService"/>
    /// and provides methods to facilitate navigation between different views in the application.
    /// It is used to manage navigation actions such as displaying add account or edit account views,
    /// ensuring seamless interaction flow within the user interface.
    /// This field enables the view model to delegate navigation responsibilities,
    /// adhering to the separation of concerns principle in the MVVM architecture.
    /// </remarks>
    private readonly INavigationWindowService _navigationWindow;

    /// <summary>
    /// Service dependency used for handling dialog interactions within the view model.
    /// </summary>
    /// <remarks>
    /// The <c>_dialog</c> field is an instance of <see cref="IDialogService"/> that provides
    /// functionality for displaying dialog boxes and messages to the user. It is used to manage
    /// various types of user notifications, such as error messages, confirmation dialogs, and input prompts,
    /// encapsulating the user interface logic for dialog interactions.
    /// This field enables the view model to remain decoupled from UI-specific details while still supporting
    /// user interactions requiring modal or non-modal dialogs.
    /// </remarks>
    private readonly IDialogService _dialog;

    /// <summary>
    /// Logger instance used for logging events, errors, and information
    /// within the context of the <see cref="AccountManagementViewModel"/>.
    /// </summary>
    /// <remarks>
    /// The <c>_logger</c> field is an instance of <see cref="ILogger{TCategoryName}"/>
    /// specifically for the <see cref="AccountManagementViewModel"/> class. It is used
    /// to capture diagnostic messages, errors, and other logging-related activities
    /// throughout the lifecycle of the view model. This allows for easier debugging
    /// and monitoring of account management operations and ensures detailed error
    /// reporting when exceptions occur.
    /// </remarks>
    private readonly ILogger<AccountManagementViewModel> _logger;

    /// <summary>
    /// Collection of account totals represented by instances of <see cref="TotalByAccountViewModel"/>.
    /// </summary>
    /// <remarks>
    /// The <c>TotalByAccounts</c> property provides an observable collection of view models,
    /// each of which represents a specific account and its associated total.
    /// This property is intended to be used as a data source for account-related UI components,
    /// such as lists or grids, enabling presentation and interaction with account data.
    /// The collection is dynamically updated to reflect changes, including additions, deletions,
    /// and modifications, ensuring consistency between the UI and underlying data.
    /// The property is populated using account-related services and supports operations
    /// such as sorting, updating, and clearing items.
    /// </remarks>
    public ObservableCollection<TotalByAccountViewModel> TotalByAccounts { get; } = [];

    /// <summary>
    /// Represents the ViewModel responsible for managing data and commands related to account management in the application.
    /// </summary>
    public AccountManagementViewModel(
        IAccountPresentationService accountService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        INavigationWindowService navigationWindow,
        IDialogService dialog,
        ILogger<AccountManagementViewModel> logger)
    {
        _accountService = accountService;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;
        _navigationWindow = navigationWindow;
        _dialog = dialog;
        _logger = logger;

        RegisterMessages();
    }

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
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnAccountDeleted);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, OnAccountChanged);
    }

    /// <summary>
    /// Asynchronously loads and initializes data for the accounts displayed in the view model.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous load operation.
    /// If successful, the account data will be populated in the view model.
    /// </returns>
    [RelayCommand]
    private async Task OnLoadAsync()
    {
        try
        {
            await FillTotalByAccounts();
        }
        catch (OperationCanceledException)
        {
            // Pass
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading accounts");
            _dialog.ShowError(AccountResources.MessageBoxLoadAccountErrorContent);
        }
    }

    /// <summary>
    /// Asynchronously populates the TotalByAccounts collection with data retrieved from the account service,
    /// ordered by account name.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation of retrieving and organizing the account totals.</returns>
    private async Task FillTotalByAccounts()
    {
        TotalByAccounts.Clear();

        var items = await _accountService.GetAllTotalByAccountViewModelAsync();
        TotalByAccounts.AddRangeAndSort(items, s => s.Name);
    }

    /// <summary>
    /// Handles the deletion of accounts by marking associated accounts in the
    /// <see cref="TotalByAccounts"/> collection as being in a deleting state.
    /// </summary>
    /// <param name="recipient">The recipient object for which the message is intended.</param>
    /// <param name="m">An <see cref="EntityChangedMessage{T}"/> containing information
    /// about the deleted accounts.</param>
    private void OnAccountDeleted(object recipient, EntityChangedMessage<int[]> m)
    {
        if (m.Value.EntityType != DependencyType.Account ||
            m.Value.DataAction != DataAction.Delete)
            return;

        foreach (var item in TotalByAccounts.Where(s => m.Value.Content.Contains(s.Id)))
        {
            item.IsDeleting = true;
        }
    }

    /// <summary>
    /// Handles the changes to an account when an <see cref="EntityChangedMessage{AccountViewModel}"/> is received.
    /// Updates or adds account-related data in the view based on the type of data action specified in the message.
    /// </summary>
    /// <param name="recipient">The recipient object that receives the message. Typically, this is the current ViewModel instance.</param>
    /// <param name="m">The <see cref="EntityChangedMessage{AccountViewModel}"/> instance containing information about the account change, including the entity type, data action, and associated content.</param>
    private async void OnAccountChanged(object recipient, EntityChangedMessage<AccountViewModel> m)
    {
        if (m.Value.EntityType != DependencyType.Account) return;

        switch (m.Value.DataAction)
        {
            case DataAction.Update:
                ApplyUpdate(m.Value.Content);
                break;

            case DataAction.Add:
                await ApplyAddAsync(m.Value.Content);
                break;
        }
    }

    /// <summary>
    /// Updates the properties of an existing item in the TotalByAccounts collection
    /// based on the provided AccountViewModel.
    /// </summary>
    /// <param name="vm">An instance of AccountViewModel containing the updated data
    /// to be applied to the corresponding item in the TotalByAccounts collection.</param>
    private void ApplyUpdate(AccountViewModel vm)
    {
        var item = TotalByAccounts.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _accountDtoViewModelMapper.Merge(vm, item);
    }

    /// <summary>
    /// Asynchronously handles the addition of a new account and updates the collection of accounts,
    /// ensuring the items are sorted by account name.
    /// </summary>
    /// <param name="vm">The ViewModel representing the account to be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ApplyAddAsync(AccountViewModel vm)
    {
        var item = await _accountService.GetTotalByAccountViewModelAsync(vm);
        if (item is null) return;

        TotalByAccounts.AddAndSort(item, s => s.Name);
    }

    /// <summary>
    /// Removes the specified account from the collection of total accounts.
    /// </summary>
    /// <param name="item">The account to be removed from the collection. If null, the method has no effect.</param>
    [RelayCommand]
    private void OnDelete(TotalByAccountViewModel? item)
    {
        if (item is null) return;

        TotalByAccounts.Remove(item);
    }

    /// <summary>
    /// Opens the manage account view with the specified account information.
    /// </summary>
    /// <param name="item">The account details to manage, or null if no specific account is provided.</param>
    [RelayCommand]
    private void OnManageAccount(TotalByAccountViewModel? item)
    {
        _navigationWindow.ShowManageAccount(item);
    }
}