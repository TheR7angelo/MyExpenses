using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Domain.Models.Systems;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
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

    private readonly ISystemPresentationService _systemPresentationService;

    /// <summary>
    /// The navigation service.
    /// </summary>
    private readonly INavigationService _navigationService;

    private readonly INavigationWindowService _navigationWindowService;

    /// <summary>
    /// The dialog service.
    /// </summary>
    private readonly IDialogService _dialog;

    /// <summary>
    /// The logger instance.
    /// </summary>
    private readonly ILogger<BankTransferManagementViewModel> _logger;

    /// <summary>
    /// Gets or sets a value indicating whether the bank transfer is prepared for execution.
    /// When true, shows the transfer preview; when false, shows the configuration form.
    /// </summary>
    [ObservableProperty]
    public partial bool BankTransferPrepared { get; set; }

    /// <summary>
    /// Gets the view model containing the bank transfer configuration data.
    /// </summary>
    public BankTransferViewModel BankTransferViewModel { get; set; } = new();

    /// <summary>
    /// Gets the history view model for the source account transfer entry.
    /// </summary>
    public HistoryViewModel FromHistoryViewModel { get; } = new() { IsPointed =  true };

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
    /// Gets the command to load all necessary data for the bank transfer management.
    /// </summary>
    public IAsyncRelayCommand LoadCommand { get; }

    /// <summary>
    /// Gets the command to prepare or cancel the bank transfer preparation state.
    /// </summary>
    public IRelayCommand<bool> PrepareBankTransferCommand { get; }

    /// <summary>
    /// Gets the command to add or edit a "From" account.
    /// </summary>
    public IRelayCommand<AccountViewModel?> AddFromAccountCommand { get; }

    /// <summary>
    /// Gets the command to add or edit a "To" account.
    /// </summary>
    public IRelayCommand<AccountViewModel?> AddToAccountCommand { get; }

    public IRelayCommand CancelBankTransferCommand { get; }

    private readonly BankTransferViewModelValidator _validatorBankTransferViewModelValidator;
    private readonly HistoryViewModelValidator _validatorHistoryViewModelValidator;

    /// <summary>
    /// Tracks which account (From or To) triggered the add/edit operation for proper auto-selection after creation.
    /// </summary>
    private AccountSource? _currentAccountSource;


    public BankTransferManagementViewModel(IAccountPresentationService accountService,
        IExpensePresentationService expensePresentationService, ISystemPresentationService systemPresentationService,
        INavigationService navigationService, INavigationWindowService navigationWindowService,
        IDialogService dialog,
        ILogger<BankTransferManagementViewModel> logger,
        BankTransferViewModelValidator validatorBankTransferViewModelValidator,
        HistoryViewModelValidator validatorHistoryViewModelValidator)
    {
        _accountService = accountService;
        _expensePresentationService = expensePresentationService;
        _systemPresentationService = systemPresentationService;
        _navigationService = navigationService;
        _navigationWindowService = navigationWindowService;
        _dialog = dialog;
        _logger = logger;
        _validatorBankTransferViewModelValidator = validatorBankTransferViewModelValidator;
        _validatorHistoryViewModelValidator = validatorHistoryViewModelValidator;

        CancelBankTransferCommand = new RelayCommand(CancelBankTransfer);
        PrepareBankTransferCommand = new AsyncRelayCommand<bool>(PrepareBankTransfer);
        AddFromAccountCommand = new RelayCommand<AccountViewModel?>(account => AddAccountAsync(account, AccountSource.From));
        AddToAccountCommand = new RelayCommand<AccountViewModel?>(account => AddAccountAsync(account, AccountSource.To));
        LoadCommand = new AsyncRelayCommand(LoadAsync);

        BankTransferViewModel.PropertyChanged += OnBankTransferViewModelPropertyChanged;

        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountViewModel>>(this, OnAccountChanged);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int>>(this, OnAccountDeleted);
    }

    private void OnAccountDeleted(object recipient, EntityChangedMessage<int> m)
    {
        if (m.Value.EntityType is not DependencyType.Account && m.Value.DataAction is not DataAction.Delete) return;

        ApplyDelete(m.Value.Content);
    }

    private async void OnAccountChanged(object recipient, EntityChangedMessage<AccountViewModel> m)
    {
        if (m.Value.EntityType is not DependencyType.Account) return;

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

    private async Task ApplyAddAsync(AccountViewModel vm)
    {
        Accounts.AddAndSort(vm, s => s.Name!);
        OnPropertyChanged(nameof(FromAccounts));
        OnPropertyChanged(nameof(ToAccounts));

        var item = await _accountService.GetTotalByAccountViewModelAsync(vm);
        if (item is not null) TotalByAccounts.Add(item);

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

    private void ApplyUpdate(AccountViewModel vm)
    {
        var item = Accounts.FirstOrDefault(s => s.Id == vm.Id);
        if (item is null) return;

        _accountService.Merge(vm, item);
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
        _navigationWindowService.ShowEditAccount(accountViewModel);
    }

    private void CancelBankTransfer()
        => _navigationService.GoBack();


    private async Task PrepareBankTransfer(bool isPrepared, CancellationToken cancellationToken = default)
    {
        _expensePresentationService.Merge(BankTransferViewModel, FromHistoryViewModel);
        FromHistoryViewModel.PlaceViewModel ??= await _systemPresentationService.GetPlaceViewModel(PlaceDomain.DefaultPlaceId, cancellationToken);

        var validatorBankTransferTask = _validatorBankTransferViewModelValidator.ValidateAsync(BankTransferViewModel, cancellationToken);
        var validatorHistoryTask = _validatorHistoryViewModelValidator.ValidateAsync(FromHistoryViewModel, cancellationToken);

        await Task.WhenAll(validatorBankTransferTask, validatorHistoryTask);

        var validationResultBankTransfer = validatorBankTransferTask.Result;
        var validationResultHistory = validatorHistoryTask.Result;

        if (validationResultBankTransfer.IsValid && validationResultHistory.IsValid) BankTransferPrepared = isPrepared;
        else
        {
            BankTransferViewModel.ValidateWithFluent(validationResultBankTransfer);
            FromHistoryViewModel.ValidateWithFluent(validationResultHistory);
        }
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
    private async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var categoryTypeTask = _expensePresentationService.GetAllCategoryTypeViewModelAsync(cancellationToken);
        var modePaymentTask = _expensePresentationService.GetAllModePaymentViewModelAsync(cancellationToken);
        var accountTask = _accountService.GetAllAccountViewModelAsync(cancellationToken);
        var totalByAccountTask = _accountService.GetAllTotalByAccountViewModelAsync(cancellationToken);

        await Task.WhenAll(categoryTypeTask, modePaymentTask, accountTask, totalByAccountTask);

        CategoryTypeViewModels.AddRangeAndSort(categoryTypeTask.Result, s => s.Name!);
        ModePaymentViewModels.AddRangeAndSort(modePaymentTask.Result, s => s.Name!);
        Accounts.AddRangeAndSort(accountTask.Result, s => s.Name!);
        TotalByAccounts.AddRange(totalByAccountTask.Result);

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