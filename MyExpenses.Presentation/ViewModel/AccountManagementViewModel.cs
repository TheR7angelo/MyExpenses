using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Services;
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
public class AccountManagementViewModel : ViewModelBase
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
    /// Service dependency used for handling navigation-related operations within the view model.
    /// </summary>
    /// <remarks>
    /// The <c>_navigation</c> field is an instance of <see cref="INavigationService"/>
    /// and provides methods to facilitate navigation between different views in the application.
    /// It is used to manage navigation actions such as displaying add account or edit account views,
    /// ensuring seamless interaction flow within the user interface.
    /// This field enables the view model to delegate navigation responsibilities,
    /// adhering to the separation of concerns principle in the MVVM architecture.
    /// </remarks>
    private readonly INavigationService _navigation;

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
    /// Command responsible for asynchronously loading account-related data into the view model.
    /// </summary>
    /// <remarks>
    /// The <c>LoadCommand</c> property encapsulates the asynchronous operation to retrieve
    /// and prepare account data for display in the UI. It is typically executed during the
    /// initialization of the page or when a refresh of the account data is required.
    /// This command ensures proper handling of task execution and UI thread interaction,
    /// providing a responsive and consistent user experience.
    /// </remarks>
    public IAsyncRelayCommand LoadCommand { get; }

    /// <summary>
    /// Command invoked to delete a specific account represented by a <see cref="TotalByAccountViewModel"/> instance.
    /// </summary>
    /// <remarks>
    /// The <c>DeleteCommand</c> property is an implementation of <see cref="IRelayCommand{T}"/>
    /// that encapsulates the logic for deleting an account. It accepts a <see cref="TotalByAccountViewModel"/>
    /// object as a parameter, representing the account to be deleted. This command is typically
    /// bound to UI elements to handle user-initiated deletion actions, ensuring synchronization
    /// between the user interface and underlying application state.
    /// </remarks>
    public IRelayCommand<TotalByAccountViewModel> DeleteCommand { get; }

    /// <summary>
    /// Command responsible for handling the action of viewing account details within the accounts management feature.
    /// </summary>
    /// <remarks>
    /// The <c>ViewAccountCommand</c> is an asynchronous command of type <see cref="IAsyncRelayCommand{T}"/>
    /// where <typeparamref name="T"/> is a <see cref="TotalByAccountViewModel"/> instance.
    /// This command facilitates the navigation or display of details for a specific account
    /// when triggered, typically in response to user interaction in the UI.
    /// It is bound to UI elements in views such as <c>AccountManagementPage.xaml</c>
    /// and relies on the <see cref="AccountManagementViewModel"/> to provide the necessary logic for its execution.
    /// </remarks>
    public IAsyncRelayCommand<TotalByAccountViewModel> ViewAccountCommand { get; }

    /// <summary>
    /// Command used to initiate the process of adding a new account.
    /// </summary>
    /// <remarks>
    /// The <c>AddAccountCommand</c> is an instance of <see cref="IRelayCommand"/> that encapsulates
    /// the logic for handling user interactions related to adding accounts. It is bound to a user interface
    /// element, such as a button, in the view corresponding to the <c>AccountManagementViewModel</c>.
    /// Executing this command triggers the <c>AddAccount</c> method within the view model, allowing
    /// users to add accounts through the associated functionality.
    /// </remarks>
    public IRelayCommand AddAccountCommand { get; }

    /// <summary>
    /// Represents the ViewModel responsible for managing data and commands related to account management in the application.
    /// </summary>
    public AccountManagementViewModel(
        IAccountPresentationService accountService,
        INavigationService navigation,
        IDialogService dialog,
        ILogger<AccountManagementViewModel> logger)
    {
        _accountService = accountService;
        _navigation = navigation;
        _dialog = dialog;
        _logger = logger;

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        DeleteCommand = new RelayCommand<TotalByAccountViewModel>(Delete);
        ViewAccountCommand = new AsyncRelayCommand<TotalByAccountViewModel>(ViewAccountAsync);
        AddAccountCommand = new RelayCommand(AddAccount);

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
    private async Task LoadAsync()
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

        item.Name = vm.Name ?? string.Empty;
        item.Symbol = vm.CurrencyViewModel?.Symbol ?? string.Empty;
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
    private void Delete(TotalByAccountViewModel? item)
    {
        if (item is null) return;

        TotalByAccounts.Remove(item);
    }

    /// <summary>
    /// Asynchronously initiates the process to view the details of a specified account.
    /// </summary>
    /// <param name="item">The account to view, represented as a <c>TotalByAccountViewModel</c>. Pass <c>null</c> to do nothing.</param>
    /// <returns>A <c>Task</c> representing the asynchronous operation.</returns>
    private async Task ViewAccountAsync(TotalByAccountViewModel? item)
    {
        try
        {
            if (item is null) return;
            await _navigation.ShowEditAccountAsync(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing account");
            _dialog.ShowError(AccountResources.MessageBoxViewAccountErrorContent);
        }
    }

    /// <summary>
    /// Initiates the process to navigate to the view for adding a new account.
    /// </summary>
    /// <remarks>
    /// This method uses the navigation service to display the interface for adding a new account.
    /// </remarks>
    private void AddAccount()
    {
        _navigation.ShowAddAccount();
    }
}