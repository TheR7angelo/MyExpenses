using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Resources.Resx.ExpenseResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public partial class BankTransferManagementViewModel : ViewModelBase
{
    /// <summary>
    /// The service for account-related operations.
    /// </summary>
    private readonly IAccountPresentationService _accountService;

    /// <summary>
    /// The service for expense-related operations.
    /// </summary>
    private readonly IExpensePresentationService _expensePresentationService;

    /// <summary>
    /// The navigation service.
    /// </summary>
    private readonly INavigationService _navigationService;

    /// <summary>
    /// The mapper for converting between account DTOs and view models.
    /// </summary>
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;

    /// <summary>
    /// The service responsible for managing navigation within the application's windows.
    /// </summary>
    private readonly INavigationWindowService _navigationWindowService;

    /// <summary>
    /// The dialog service.
    /// </summary>
    private readonly IDialogService _dialog;

    /// <summary>
    /// Gets or sets a value indicating whether the bank transfer is prepared for execution.
    /// When true, shows the transfer preview; when false, shows the configuration form.
    /// </summary>
    [ObservableProperty]
    public partial bool BankTransferPrepared { get; set; }

    /// <summary>
    /// Gets the view model containing the bank transfer configuration data.
    /// </summary>
    public BankTransferViewModel BankTransferViewModel { get; private set; } = new();

    /// <summary>
    /// Gets the history view model for the source account transfer entry.
    /// </summary>
    public HistoryViewModel FromHistoryViewModel { get; private set; } = new() { IsPointed =  true };

    /// <summary>
    /// Gets the currency symbol prefix to display when both accounts use the same currency.
    /// Returns null if accounts have different currencies.
    /// </summary>
    public string? ValuePrefix => BankTransferViewModel.FromAccount?.CurrencyViewModel?.Symbol == BankTransferViewModel.ToAccount?.CurrencyViewModel?.Symbol
        ? BankTransferViewModel.FromAccount?.CurrencyViewModel?.Symbol
        : null;

    /// <summary>
    /// Gets the collection of available category types for the transfer.
    /// </summary>
    public ObservableCollection<CategoryTypeViewModel> CategoryTypeViewModels { get; } = [];

    /// <summary>
    /// Gets the collection of available mode of payments for the transfer.
    /// </summary>
    public ObservableCollection<ModePaymentViewModel> ModePaymentViewModels { get; } = [];

    /// <summary>
    /// Gets the private collection of all available accounts.
    /// </summary>
    private ObservableCollection<AccountViewModel> Accounts { get; } = [];

    /// <summary>
    /// Gets the filtered collection of accounts available for selection as the source account.
    /// Excludes the currently selected destination account to prevent self-transfer.
    /// </summary>
    public IEnumerable<AccountViewModel> FromAccounts =>
        Accounts.Where(a => BankTransferViewModel.ToAccount is null || a.Id != BankTransferViewModel.ToAccount.Id);

    /// <summary>
    /// Gets the filtered collection of accounts available for selection as the destination account.
    /// Excludes the currently selected source account to prevent self-transfer.
    /// </summary>
    public IEnumerable<AccountViewModel> ToAccounts =>
        Accounts.Where(a => BankTransferViewModel.FromAccount is null || a.Id != BankTransferViewModel.FromAccount.Id);

    /// <summary>
    /// Gets the array of total values by account used for calculating transfer previews.
    /// </summary>
    private List<TotalByAccountViewModel> TotalByAccounts { get; set; } = [];

    /// <summary>
    /// Gets the old total value of the source account before the transfer.
    /// </summary>
    [ObservableProperty]
    public partial double FromTotalAccountOldValue { get; private set; }

    /// <summary>
    /// Gets the new total value of the source account after the transfer.
    /// </summary>
    [ObservableProperty]
    public partial double FromTotalAccountNewValue { get; private set; }

    /// <summary>
    /// Gets the old total value of the destination account before the transfer.
    /// </summary>
    [ObservableProperty]
    public partial double ToTotalAccountOldValue { get; private set; }

    /// <summary>
    /// Gets the new total value of the destination account after the transfer.
    /// </summary>
    [ObservableProperty]
    public partial double ToTotalAccountNewValue { get; private set; }

    /// <summary>
    /// Gets the command to add or edit a "From" account.
    /// </summary>
    public IRelayCommand<AccountViewModel?> AddFromAccountCommand { get; }

    /// <summary>
    /// Gets the command to add or edit a "To" account.
    /// </summary>
    public IRelayCommand<AccountViewModel?> AddToAccountCommand { get; }

    private readonly IExpenseActionService _expenseActionService;

    /// <summary>
    /// Tracks which account (From or To) triggered the add/edit operation for proper auto-selection after creation.
    /// </summary>
    private AccountSource? _currentAccountSource;

    private readonly ILogger<BankTransferManagementViewModel> _logger;

    /// <summary>
    /// ViewModel responsible for managing the bank transfer workflow. Provides commands for
    /// initiating, validating, and canceling bank transfers, as well as managing account setup
    /// and navigation. Integrates with multiple services to handle business logic and UI interactions.
    /// </summary>
    public BankTransferManagementViewModel(IAccountPresentationService accountService,
        IExpensePresentationService expensePresentationService,
        IExpenseActionService expenseActionService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        INavigationService navigationService, INavigationWindowService navigationWindowService,
        IDialogService dialog, ILogger<BankTransferManagementViewModel> logger)
    {
        _accountService = accountService;
        _expensePresentationService = expensePresentationService;
        _expenseActionService = expenseActionService;
        _navigationService = navigationService;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;
        _navigationWindowService = navigationWindowService;
        _dialog = dialog;
        _logger = logger;

        AddFromAccountCommand = new RelayCommand<AccountViewModel?>(account => AddAccountAsync(account, AccountSource.From));
        AddToAccountCommand = new RelayCommand<AccountViewModel?>(account => AddAccountAsync(account, AccountSource.To));

        BankTransferViewModel.PropertyChanged += OnBankTransferViewModelPropertyChanged;

        RegisterMessages();
    }

    /// <summary>
    /// Registers message handlers to respond to specific messaging events within the application.
    /// This method sets up message listeners for entity changes related to accounts and handles
    /// actions such as account updates or deletions to ensure the ViewModel stays synchronized with
    /// the state of the application.
    /// </summary>
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, OnAccountChanged);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int>>(this, OnAccountDeleted);
    }

    /// <summary>
    /// Validates and processes a bank transfer based on the provided bank transfer details and history view model.
    /// Prompts the user for confirmation to initiate another bank transfer or navigate back upon completion.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to observe while the operation is performed.</param>
    /// <returns>A task that represents the asynchronous operation of validating and processing the bank transfer.</returns>
    [RelayCommand]
    private async Task OnValidBankTransfer(CancellationToken cancellationToken = default)
    {
        try
        {
            await _expenseActionService.CreateBankTransfer(BankTransferViewModel, FromHistoryViewModel, cancellationToken);

            var response = _dialog.ShowMessageBox(ExpenseResources.MessageBoxValidBankTransferSuccessCaption,
                ExpenseResources.MessageBoxValidBankTransferSuccessContent,
                MessageBoxButton.YesNo, MsgBoxImage.Question);

            if (response is MessageBoxResult.Yes)
            {
                BankTransferViewModel = new BankTransferViewModel();
                FromHistoryViewModel = new HistoryViewModel();
            }
            else
            {
                _navigationService.GoBack();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating bank transfer");
            _dialog.ShowError(ExpenseResources.MessageBoxValidBankTransferErrorContent);
        }
    }

    /// <summary>
    /// Handles the deletion of an account by processing a message indicating that an account entity
    /// has been deleted. Verifies the entity type and action in the message before applying the deletion logic.
    /// </summary>
    /// <param name="recipient">The recipient object that is handling the message.</param>
    /// <param name="m">The message containing information about the deleted account, including the
    /// entity type, action, and content.</param>
    private void OnAccountDeleted(object recipient, EntityChangedMessage<int> m)
    {
        if (m.Value.EntityType is not DependencyType.Account && m.Value.DataAction is not DataAction.Delete) return;

        ApplyDelete(m.Value.Content);
    }

    /// <summary>
    /// Handles account-related changes by responding to the provided message. Depending on the
    /// data action specified in the message, this method applies either an addition or update
    /// operation for the affected account. Resets the current account source upon completion.
    /// </summary>
    /// <param name="recipient">The recipient object handling the message. Typically used for message dispatch context.</param>
    /// <param name="m">The message detailing the account change, including the action type and the updated or added account data.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the data action provided in the message is not supported.</exception>
    private async void OnAccountChanged(object recipient, EntityChangedMessage<AccountViewModel> m)
    {
        if (m.Value.EntityType is not DependencyType.Account) return;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (m.Value.DataAction)
        {
            case DataAction.Update:
                ApplyUpdate(m.Value.Content);
                break;

            case DataAction.Add:
                await ApplyAddAsync(m.Value.Content);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        _currentAccountSource = null;
    }

    /// <summary>
    /// Removes the account with the specified ID from the accounts collection
    /// and updates related properties and collections. Triggers property change
    /// notifications for UI updates.
    /// </summary>
    /// <param name="accountId">The ID of the account to remove from the collection.</param>
    private void ApplyDelete(int accountId)
    {
        var item = Accounts.FirstOrDefault(s => s.Id == accountId);
        if (item is null) return;

        Accounts.Remove(item);
        OnPropertyChanged(nameof(FromAccounts));
        OnPropertyChanged(nameof(ToAccounts));

        var totalByAccount = TotalByAccounts.FirstOrDefault(t => t.Id == accountId);
        if (totalByAccount is not null) TotalByAccounts.Remove(totalByAccount);
    }

    /// <summary>
    /// Asynchronously adds a new account to the Accounts collection, updates the related properties
    /// for available FromAccounts and ToAccounts, and fetches the current total by account data
    /// associated with the added account. Also updates the BankTransferViewModel depending on
    /// the current account source.
    /// </summary>
    /// <param name="vm">The account view model to be added to the Accounts collection.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the current account source is invalid during the assignment.
    /// </exception>
    private async Task ApplyAddAsync(AccountViewModel vm)
    {
        Accounts.AddAndSort(vm, s => s.Name!);
        OnPropertyChanged(nameof(FromAccounts));
        OnPropertyChanged(nameof(ToAccounts));

        var result = await _accountService.GetTotalByAccountViewModelAsync(vm);
        if (result.IsSuccess) TotalByAccounts!.AddAndSort(result.Value, s => s!.Name);

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (_currentAccountSource)
        {
            case AccountSource.From:
                BankTransferViewModel.FromAccount = vm;
                break;
            case AccountSource.To:
                BankTransferViewModel.ToAccount = vm;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Updates an existing account in the collection by merging it with the provided account data.
    /// Ensures that the corresponding account entry in the collection is updated with any changes
    /// from the provided <paramref name="vm"/> parameter.
    /// </summary>
    /// <param name="vm">The account information containing updated data to merge with an existing account.</param>
    private void ApplyUpdate(AccountViewModel vm)
    {
        var item = Accounts.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _accountDtoViewModelMapper.Merge(vm, item);
    }

    /// <summary>
    /// Opens the account edit/create dialog for the specified account source.
    /// For new accounts (Create), automatically selects them in the appropriate ComboBox after creation.
    /// For existing accounts (Update), just opens the edit dialog.
    /// </summary>
    /// <param name="accountViewModel">The account to edit, or null to create a new one.</param>
    /// <param name="source">Indicates whether this is for the "From" or "To" account selection.</param>
    private void AddAccountAsync(AccountViewModel? accountViewModel, AccountSource source)
    {
        _currentAccountSource = source;
        _navigationWindowService.ShowManageAccount(accountViewModel);
    }

    /// <summary>
    /// Cancels the current bank transfer process and navigates back to the previous view.
    /// Utilizes the navigation service to revert the UI state to the previous screen or context.
    /// Typically invoked when the user chooses to abort the bank transfer operation.
    /// </summary>
    [RelayCommand]
    private void CancelBankTransfer()
        => _navigationService.GoBack();

    /// <summary>
    /// Prepares the bank transfer process by validating the provided transfer details or resetting the state.
    /// </summary>
    /// <param name="isPrepared">Specifies whether the preparation process should validate the transfer details.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests during the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [RelayCommand]
    private async Task OnPrepareBankTransfer(bool isPrepared, CancellationToken cancellationToken = default)
    {
        if (isPrepared) BankTransferPrepared = await _expenseActionService.ValidateBankTransfer(BankTransferViewModel, FromHistoryViewModel, cancellationToken);
        else BankTransferPrepared = isPrepared;
    }

    /// <summary>
    /// Handles property changes on the BankTransferViewModel to update dependent properties and collections.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments containing the property name that changed.</param>
    private void OnBankTransferViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(BankTransferViewModel.FromAccount):
                OnPropertyChanged(nameof(ToAccounts));
                OnPropertyChanged(nameof(ValuePrefix));

                FromHistoryViewModel.AccountViewModel = BankTransferViewModel.FromAccount;
                break;

            case nameof(BankTransferViewModel.ToAccount):
                OnPropertyChanged(nameof(FromAccounts));
                OnPropertyChanged(nameof(ValuePrefix));
                break;

            case nameof(BankTransferViewModel.Value):
                if (BankTransferViewModel.FromAccount is not null)
                {
                    var totalByAccount = TotalByAccounts.FirstOrDefault(t => t.Id == BankTransferViewModel.FromAccount.Id);
                    FromTotalAccountOldValue = totalByAccount?.Total ?? 0;
                    FromTotalAccountNewValue = FromTotalAccountOldValue - BankTransferViewModel.Value ?? 0;
                }
                if (BankTransferViewModel.ToAccount is not null)
                {
                    var totalByAccount = TotalByAccounts.FirstOrDefault(t => t.Id == BankTransferViewModel.ToAccount.Id);
                    ToTotalAccountOldValue = totalByAccount?.Total ?? 0;
                    ToTotalAccountNewValue = ToTotalAccountOldValue + BankTransferViewModel.Value ?? 0;
                }
                break;
        }
    }

    /// <summary>
    /// Loads all necessary data for the bank transfer management including accounts, categories, and totals.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [RelayCommand]
    private async Task OnLoadAsync(CancellationToken cancellationToken = default)
    {
        var categoryTypeTask = _expensePresentationService.GetAllCategoryTypeViewModelAsync(cancellationToken);
        var modePaymentTask = _expensePresentationService.GetAllModePaymentViewModelAsync(cancellationToken);
        var accountTask = _accountService.GetAllAccountViewModelAsync(cancellationToken);
        var totalByAccountTask = _accountService.GetAllTotalByAccountViewModelAsync(cancellationToken);

        await Task.WhenAll(categoryTypeTask, modePaymentTask, accountTask, totalByAccountTask);

        CategoryTypeViewModels.AddRangeAndSort(categoryTypeTask.Result, s => s.Name!);
        ModePaymentViewModels.AddRangeAndSort(modePaymentTask.Result, s => s.Name!);

        var resultAccounts = accountTask.Result;
        if (!resultAccounts.IsSuccess) _dialog.ShowMessageBox(AccountResources.MessageBoxAddEditAccountTypeErrorCaption,
                AccountResources.MessageBoxLoadAccountErrorContent, MessageBoxButton.Ok, MsgBoxImage.Error);
        else Accounts.AddRangeAndSort(resultAccounts.Value!, s => s.Name!);

        var resultTotalByAccount = totalByAccountTask.Result;
        if (!resultTotalByAccount.IsSuccess) _dialog.ShowMessageBox(AccountResources.MessageBoxAddEditAccountTypeErrorCaption,
                AccountResources.MessageBoxLoadAccountErrorContent, MessageBoxButton.Ok, MsgBoxImage.Error);
        else TotalByAccounts.AddRange(resultTotalByAccount.Value!);

        OnPropertyChanged(nameof(FromAccounts));
        OnPropertyChanged(nameof(ToAccounts));
    }

    /// <summary>
    /// Indicates the source of the account add/edit operation.
    /// </summary>
    private enum AccountSource : byte
    {
        /// <summary>
        /// Operation triggered from the "From" account selection.
        /// </summary>
        From,

        /// <summary>
        /// Operation triggered from the "To" account selection.
        /// </summary>
        To
    }
}