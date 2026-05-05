using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Wpf.Services;
using MyExpenses.Wpf.Windows;

namespace MyExpenses.Wpf.Pages;

/// <summary>
/// Represents the Account Management page in the application. This page is designed to allow users
/// to view and manage their accounts and associated data. The page interacts with
/// <see cref="IAccountPresentationService"/> to retrieve and manage account-related information.
/// </summary>
/// <remarks>
/// This class is part of the WPF application's UI layer and is implemented as a partial class,
/// with its corresponding XAML defining the view layout and behavior. It uses a combination of
/// MVVM patterns and message registration for interaction with the application state and services.
/// </remarks>
public partial class AccountManagementPage
{
    /// <summary>
    /// Gets the collection of TotalByAccountViewModel objects that represent the aggregated
    /// total amounts grouped by accounts. This property is used to display and manage the
    /// summarized data of accounts in the Account Management page.
    /// </summary>
    /// <remarks>
    /// The <see cref="TotalByAccounts"/> property is populated and maintained by various internal
    /// operations such as retrieving data from the account presentation service, handling changes
    /// to account entities, and interacting with user commands. It supports sorting and updating
    /// to ensure the data is displayed in a user-friendly and consistent manner.
    /// </remarks>
    public ObservableCollection<TotalByAccountViewModel> TotalByAccounts { get; } = [];

    /// <summary>
    /// Provides access to the application's navigation services, allowing the page to handle
    /// navigational actions such as transitioning between views or opening modal dialogs.
    /// </summary>
    /// <remarks>
    /// The <see cref="_navigationServices"/> field is initialized through dependency injection
    /// in the constructor of the <see cref="AccountManagementPage"/>. It is used to invoke
    /// navigation-related operations, such as displaying pages or managing modal windows for
    /// account-related actions.
    /// </remarks>
    private readonly INavigationServices _navigationServices;

    /// <summary>
    /// Provides access to the account presentation service, which is responsible for retrieving and managing
    /// account-related data required by the Account Management page. This service facilitates operations such
    /// as fetching account summaries, retrieving detailed account information, and interacting with various
    /// account-related entities.
    /// </summary>
    /// <remarks>
    /// The <see cref="_accountPresentationService"/> field is used internally to perform asynchronous operations
    /// like collecting data for account summaries (<see cref="TotalByAccounts"/>) and handling updates to account
    /// entities. It interacts with implementations of <see cref="IAccountPresentationService"/> to ensure
    /// that the page remains in sync with the underlying data and business logic.
    /// </remarks>
    private readonly IAccountPresentationService _accountPresentationService;

    /// <summary>
    /// Provides logging functionality for the <see cref="AccountManagementPage"/> class, enabling
    /// the recording of runtime information, warnings, errors, and debugging details related to
    /// the Account Management page's lifecycle and operations.
    /// </summary>
    /// <remarks>
    /// The <see cref="_logger"/> field is initialized via dependency injection and configured
    /// to log messages specific to the <see cref="AccountManagementPage"/>. It is used to
    /// assist in tracing application behavior, troubleshooting issues, and monitoring the
    /// execution flow within the page.
    /// </remarks>
    private readonly ILogger<AccountManagementPage> _logger;

    /// <summary>
    /// Represents the Account Management page in the application, responsible for displaying
    /// and managing accounts. It provides functionality to retrieve and manage account-related
    /// information by interacting with the <see cref="IAccountPresentationService"/>.
    /// </summary>
    /// <remarks>
    /// This class is part of the WPF application's UI layer and follows the MVVM pattern.
    /// It uses XAML for its view layout and behavior. The class also implements message
    /// registration to handle interactions and state changes during its lifecycle.
    /// </remarks>
    public AccountManagementPage(INavigationServices navigationServices,
        IAccountPresentationService accountPresentationService,
        ILogger<AccountManagementPage> logger)
    {
        _navigationServices = navigationServices;
        _accountPresentationService = accountPresentationService;
        _logger = logger;

        InitializeComponent();

        RegisterMessages();
        Loaded += OnLoaded;
    }

    #region Initialization

    /// <summary>
    /// Registers message handlers for entity change events in the application.
    /// This method uses the <see cref="WeakReferenceMessenger"/> to subscribe
    /// to messages about changes to account-related entities, enabling the page
    /// to react to updates or deletions of accounts.
    /// </summary>
    /// <remarks>
    /// The registration process connects specific message types to their corresponding
    /// handlers, such as <see cref="OnAccountDeleted"/> for account deletions and
    /// <see cref="OnAccountChanged"/> for account modifications. This ensures that
    /// the page remains in sync with the application’s state and reflects current data.
    /// </remarks>
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnAccountDeleted);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, OnAccountChanged);
    }

    /// <summary>
    /// Handles the <see cref="FrameworkElement.Loaded"/> event for the <see cref="AccountManagementPage"/>.
    /// This method is triggered when the page has been initialized and is ready for interaction.
    /// It performs initial setup tasks such as populating the <see cref="TotalByAccounts"/> collection
    /// by fetching account data from the <see cref="IAccountPresentationService"/>.
    /// </summary>
    /// <param name="sender">The source of the event, typically the <see cref="AccountManagementPage"/> itself.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> that contains the event data.</param>
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Loaded -= OnLoaded;
            await FillTotalByAccounts();
        }
        catch (Exception ex)
        {
            // TODO trad
            _logger.LogError(ex, "Error while loading accounts");
            MessageBox.Show("An error occurred while loading accounts. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Asynchronously retrieves account totals using the <see cref="IAccountPresentationService"/>
    /// and updates the <see cref="TotalByAccounts"/> collection. The collection is cleared,
    /// populated with new data, and sorted by the account name.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation of fetching,
    /// sorting, and updating the account totals.
    /// </returns>
    private async Task FillTotalByAccounts()
    {
        var totalByAccounts = await _accountPresentationService.GetAllTotalByAccountViewModelAsync();
        TotalByAccounts.AddRangeAndSort(totalByAccounts, s => s.Name);
    }

    #endregion

    #region Message Handlers

    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// Handles the deletion of accounts by responding to messages of type
    /// <see cref="EntityChangedMessage{T}"/> with a payload of <see cref="int[]"/>.
    /// This method marks the corresponding accounts in the <see cref="TotalByAccounts"/>
    /// collection as being deleted by setting their <c>IsDeleting</c> property to true.
    /// </summary>
    /// <param name="recipient">
    /// The recipient of the message, usually the instance of the class that
    /// registered for the message. In this case, it is the current page.
    /// </param>
    /// <param name="m">
    /// The message instance containing details about the account deletion.
    /// The <see cref="EntityChanged{T}.EntityType"/> must be
    /// <see cref="DependencyType.Account"/> and the <see cref="EntityChanged{T}.DataAction"/>
    /// must be <see cref="DataAction.Delete"/> for the deletion logic to execute.
    /// </param>
    private void OnAccountDeleted(object recipient, EntityChangedMessage<int[]> m)
    {
        if (m.Value.EntityType is not DependencyType.Account ||
            m.Value.DataAction is not DataAction.Delete)
            return;

        foreach (var item in TotalByAccounts.Where(s => m.Value.Content.Contains(s.Id)))
        {
            item.IsDeleting = true;
        }
    }

    /// <summary>
    /// Handles account-related change notifications and updates the page state accordingly.
    /// This method is invoked when an <see cref="EntityChangedMessage{T}"/> of type <see cref="AccountViewModel"/>
    /// is received, signaling a modification or addition of an account entity.
    /// </summary>
    /// <param name="recipient">
    /// The recipient object that is handling the message. Typically, this will be the page or component
    /// registered for the message.
    /// </param>
    /// <param name="m">
    /// The incoming message containing the details of the account change, including the
    /// <see cref="EntityChanged{T}.EntityType"/> to identify the type of entity affected, and the
    /// corresponding <see cref="AccountViewModel"/> instance with the updated or added data.
    /// </param>
    private async void OnAccountChanged(object recipient, EntityChangedMessage<AccountViewModel> m)
    {
        try
        {
            if (m.Value is not { EntityType: DependencyType.Account, Content: var accountViewModel })
                return;

            switch (m.Value.DataAction)
            {
                case DataAction.Update:
                    ApplyUpdate(accountViewModel);
                    break;

                case DataAction.Add:
                    await ApplyAddAsync(accountViewModel);
                    break;
            }
        }
        catch (Exception e)
        {
            // TODO trad
            _logger.LogError(e, "Error while handling account change event");
            MessageBox.Show("An error occurred while handling account change event. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #endregion

    #region Business Logic

    /// <summary>
    /// Updates the properties of an existing account item in the <see cref="TotalByAccounts"/>
    /// collection based on the data provided in the specified <see cref="AccountViewModel"/>.
    /// </summary>
    /// <param name="vm">
    /// An instance of <see cref="AccountViewModel"/> containing the updated data for the account.
    /// </param>
    private void ApplyUpdate(AccountViewModel vm)
    {
        var item = TotalByAccounts.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        item.Name = vm.Name ?? string.Empty;
        item.Symbol = vm.CurrencyViewModel?.Symbol ?? string.Empty;
    }

    /// <summary>
    /// Asynchronously adds a new account to the <see cref="TotalByAccounts"/> collection after retrieving
    /// the corresponding <see cref="TotalByAccountViewModel"/> from the <see cref="IAccountPresentationService"/>.
    /// The item is added and sorted in the collection by its name.
    /// </summary>
    /// <param name="vm">The <see cref="AccountViewModel"/> representing the account to be added.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    private async Task ApplyAddAsync(AccountViewModel vm)
    {
        var item = await _accountPresentationService.GetTotalByAccountViewModelAsync(vm);
        if (item is null) return;

        TotalByAccounts.AddAndSort(item, s => s.Name);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Deletes the specified account item from the collection of accounts displayed on the Account Management page.
    /// </summary>
    /// <param name="parameter">
    /// An object representing the account item to be deleted. Must be of type <see cref="TotalByAccountViewModel"/>.
    /// If the parameter is not of the expected type, the method will not perform any operation.
    /// </param>
    [RelayCommand]
    private void DeleteSelf(object parameter)
    {
        if (parameter is not TotalByAccountViewModel item) return;
        TotalByAccounts.Remove(item);
    }

    #endregion

    #region UI Actions

    /// <summary>
    /// Handles the click event for the "Add New Account" button. This method opens the
    /// <see cref="AddEditAccountWindow"/> dialog window to allow the user to add or edit
    /// account details.
    /// </summary>
    /// <param name="sender">The source of the event, typically the button that was clicked.</param>
    /// <param name="e">The event data associated with the click event.</param>
    private void ButtonAddNewAccount_OnClick(object sender, RoutedEventArgs e)
        => _navigationServices.ShowAddAccount();

    /// <summary>
    /// Handles the click event for the "View Account" button. This method retrieves the associated
    /// <see cref="TotalByAccountViewModel"/> from the button's DataContext, opens the
    /// <see cref="AddEditAccountWindow"/>, and asynchronously loads the selected account's data
    /// into the window for further actions.
    /// </summary>
    /// <param name="sender">
    /// The source of the event, typically the button that was clicked.
    /// </param>
    /// <param name="e">
    /// The event arguments containing information about the click event.
    /// </param>
    private async void ButtonVAccount_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not Button button) return;
            if (button.DataContext is not TotalByAccountViewModel vm) return;

            await _navigationServices.ShowEditAccountAsync(vm);
        }
        catch (Exception exception)
        {
            // TODO trad
            _logger.LogError(exception, "Error while viewing account details");
            MessageBox.Show("An error occurred while viewing account details. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #endregion
}