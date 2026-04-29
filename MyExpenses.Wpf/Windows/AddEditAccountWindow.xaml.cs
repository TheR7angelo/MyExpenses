using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Domain.Models.Expenses;
using Domain.Models.Systems;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditAccountWindow
{
    #region DependecyProperty

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EnableStartingBalanceProperty =
        DependencyProperty.Register(nameof(EnableStartingBalance), typeof(bool), typeof(AddEditAccountWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EnableStartingBalance
    {
        get => (bool)GetValue(EnableStartingBalanceProperty);
        set => SetValue(EnableStartingBalanceProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditAccountProperty =
        DependencyProperty.Register(nameof(EditAccount), typeof(bool), typeof(AddEditAccountWindow),
            new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditAccount
    {
        get => (bool)GetValue(EditAccountProperty);
        set => SetValue(EditAccountProperty, value);
    }

    #endregion

    #region Property

    public bool DeleteAccount { get; private set; }

    // TODO to delete
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TAccount Account { get; } = new();
    public AccountViewModel AccountViewModel { get; } = new();

    // TODO to delete
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public THistory History { get; } = new() { IsPointed = true };

    public HistoryViewModel HistoryViewModel { get; } = new() { IsPointed = true };

    public ObservableCollection<AccountTypeViewModel> AccountTypes { get; } = [];
    public ObservableCollection<CurrencyViewModel> Currencies { get; } = [];
    public ObservableCollection<CategoryTypeViewModel> CategoryTypes { get; } = [];

    #endregion

    private readonly IAccountPresentationService _accountPresentationService;
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly ISystemPresentationService _systemPresentationService;
    private readonly IDialogService _dialogService;
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;
    private readonly ILogger<AddEditAccountWindow> _logger;

    public AddEditAccountWindow(IAccountPresentationService accountPresentationService,
        IExpensePresentationService expensePresentationService,
        ISystemPresentationService systemPresentationService,
        IDialogService dialogService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        ILogger<AddEditAccountWindow> logger)
    {
        _accountPresentationService = accountPresentationService;
        _expensePresentationService = expensePresentationService;
        _systemPresentationService = systemPresentationService;
        _dialogService = dialogService;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;
        _logger = logger;

        _ = FillCollection();

        InitializeComponent();

        RegisterEntityChangeHandlers();

        Closed += (_, _) => WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    private void RegisterEntityChangeHandlers()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<AccountTypeViewModel>>(this, (_, m) =>
        {
            if (m.Value is not { EntityType: DependencyType.AccountType, DataAction: DataAction.Add, Content: var accountType }) return;
            AccountTypes.AddAndSort(accountType, s => s.Name!);
            AccountViewModel.AccountTypeViewModel = accountType;
        });

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<CategoryTypeViewModel>>(this, (_, m) =>
        {
            if (m.Value is not { EntityType: DependencyType.CategoryType, DataAction: DataAction.Add, Content: var categoryType }) return;
            CategoryTypes.AddAndSort(categoryType, s => s.Name!);
            HistoryViewModel.CategoryTypeViewModel = categoryType;
        });

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<CurrencyViewModel>>(this, (_, m) =>
        {
            if (m.Value is not { EntityType: DependencyType.Currency, DataAction: DataAction.Add, Content: var currency }) return;
            Currencies.AddAndSort(currency, s => s.Symbol!);
            AccountViewModel.CurrencyViewModel = currency;
        });

        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int>>(this, (_, message) =>
        {
            if (message.Value is not { DataAction: DataAction.Delete, Content: var id }) return;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (message.Value.EntityType)
            {
                case DependencyType.AccountType:
                    AccountTypes.Remove(AccountTypes.First(x => x.Id == id));
                    break;
                case DependencyType.CategoryType:
                    CategoryTypes.Remove(CategoryTypes.First(x => x.Id == id));
                    break;
                case DependencyType.Currency:
                    Currencies.Remove(Currencies.First(x => x.Id == id));
                    break;
            }
        });
    }

    #region Action

    private async void ButtonAddEditAccountType_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var accountActionService = App.ServiceProvider.GetRequiredService<IActionService>();
            await accountActionService.ManageAccountTypeAction(AccountViewModel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while managing account type action");
            _dialogService.ShowMessageBox(AccountResources.MessageBoxAddEditAccountTypeErrorCaption,
                AccountResources.MessageBoxAddEditAccountTypeErrorContent, MsgBoxImage.Error);
        }
    }

    private async void ButtonAddCategoryType_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var accountActionService = App.ServiceProvider.GetRequiredService<IActionService>();
            await accountActionService.ManageCategoryTypeAction(HistoryViewModel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while managing category type action");
            _dialogService.ShowMessageBox(AccountResources.MessageBoxAddEditAccountTypeErrorCaption,
                AccountResources.MessageBoxAddEditAccountTypeErrorContent, MsgBoxImage.Error);
        }
    }

    private async void ButtonAddCurrency_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var accountActionService = App.ServiceProvider.GetRequiredService<IActionService>();
            await accountActionService.ManageCurrencyAction(AccountViewModel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while managing category type action");
            _dialogService.ShowMessageBox(AccountResources.MessageBoxAddEditAccountTypeErrorCaption,
                AccountResources.MessageBoxAddEditAccountTypeErrorContent, MsgBoxImage.Error);
        }
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var accountActionService = App.ServiceProvider.GetRequiredService<IActionService>();
            await accountActionService.DeleteAccount(AccountViewModel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while deleting account action");
            _dialogService.ShowMessageBox(AccountResources.MessageBoxAddEditAccountTypeErrorCaption,
                AccountResources.MessageBoxAddEditAccountTypeErrorContent, MsgBoxImage.Error);
        }
    }

    private async void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (EnableStartingBalance && HistoryViewModel.PlaceViewModel is null)
            {
                HistoryViewModel.PlaceViewModel = await _systemPresentationService.GetPlaceViewModel(PlaceDomain.DefaultPlaceId);
            }

            if (EnableStartingBalance && HistoryViewModel.ModePaymentViewModel is null)
            {
                HistoryViewModel.ModePaymentViewModel = await _expensePresentationService.GetModePaymentViewModel(ModePaymentDomain.DefaultModePaymentId);
            }

            bool result;

            var accountActionService = App.ServiceProvider.GetRequiredService<IActionService>();
            if (EditAccount) result = await accountActionService.UpdateAccount(AccountViewModel);
            // TODO correct
            else if (EnableStartingBalance) result = await accountActionService.CreateAccount(AccountViewModel, HistoryViewModel);
            else result = await accountActionService.CreateAccount(AccountViewModel);

            if (!result) return;

            DialogResult = true;
            Close();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while managing account action");
            _dialogService.ShowMessageBox(AccountResources.MessageBoxAddEditAccountTypeErrorCaption,
                AccountResources.MessageBoxAddEditAccountTypeErrorContent, MsgBoxImage.Error);
        }
    }

    #endregion

    #region Function

    private async Task FillCollection()
    {
        await Task.WhenAll(
            _expensePresentationService.GetAllCategoryTypeViewModelAsync().LoadAndSortAsync(CategoryTypes, x => x.Name!),
            _accountPresentationService.GetAllCurrencyViewModelAsync().LoadAndSortAsync(Currencies, x => x.Symbol!),
            _accountPresentationService.GetAllAccountTypeViewModelAsync().LoadAndSortAsync(AccountTypes, x => x.Name!)
        );
    }

    public async Task LoadAsync(TotalByAccountViewModel totalByAccountViewModel)
    {
        var accountViewModel = await _accountPresentationService.GetAccount(totalByAccountViewModel);
        if (accountViewModel is null) return;

        _accountDtoViewModelMapper.Merge(accountViewModel, AccountViewModel);

        AccountViewModel.AccountTypeViewModel = accountViewModel.AccountTypeViewModel is null
            ? null
            : AccountTypes.FirstOrDefault(s => s.Id == accountViewModel.AccountTypeViewModel.Id);

        AccountViewModel.CurrencyViewModel = accountViewModel.CurrencyViewModel is null
            ? null
            : Currencies.FirstOrDefault(s => s.Id == accountViewModel.CurrencyViewModel.Id);
        AccountViewModel.AcceptChanges();

        EditAccount = true;
    }

    public void SetTAccount(TAccount account)
    {
        account.CopyPropertiesTo(Account);
        EditAccount = true;
    }

    #endregion
}