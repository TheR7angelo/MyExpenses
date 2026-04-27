using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.RegexUtils;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;
using MyExpenses.Sql.Context;
using Serilog;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TAccount Account { get; } = new();
    public AccountViewModel AccountViewModel { get; } = new();
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public THistory History { get; } = new() { IsPointed = true };

    public HistoryViewModel HistoryViewModel { get; } = new() { IsPointed = true };
    public static string SelectedValuePathCategoryType => nameof(TCategoryType.Id);

    public ObservableCollection<AccountTypeViewModel> AccountTypes { get; } = [];
    public ObservableCollection<CurrencyViewModel> Currencies { get; } = [];
    public ObservableCollection<CategoryTypeViewModel> CategoryTypes { get; } = [];

    #endregion

    private readonly IAccountPresentationService _accountPresentationService;
    private readonly IExpensePresentationService _expensePresentationService;
    private readonly IDialogService _dialogService;
    private readonly IAccountDtoViewModelMapper _accountDtoViewModelMapper;
    private readonly ILogger<AddEditAccountWindow> _logger;

    public AddEditAccountWindow(IAccountPresentationService accountPresentationService,
        IExpensePresentationService expensePresentationService,
        IDialogService dialogService,
        IAccountDtoViewModelMapper accountDtoViewModelMapper,
        ILogger<AddEditAccountWindow> logger)
    {
        _accountPresentationService = accountPresentationService;
        _expensePresentationService = expensePresentationService;
        _dialogService = dialogService;
        _accountDtoViewModelMapper = accountDtoViewModelMapper;
        _logger = logger;

        _ = FillCollection();

        InitializeComponent();

        RegisterEntityChangeHandlers();

        Closed += (_, _) => WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    // ReSharper disable once CognitiveComplexity
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

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO continue here

        var response = Dialogs.MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxDeleteAccountQuestionTitle,
            string.Format(AddEditAccountResources.MessageBoxDeleteAccountQuestionMessage, Account.Name),
            System.Windows.MessageBoxButton.YesNoCancel, MsgBoxImage.Question);
        if (response is not System.Windows.MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the account \"{AccountToDeleteName}\"", Account.Name);
        var (success, exception) = Account.Delete(true);

        if (success)
        {
            Log.Information("Account was successfully removed");
            Dialogs.MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxDeleteAccountSuccessTitle,
                AddEditAccountResources.MessageBoxDeleteAccountSuccessMessage, MsgBoxImage.Check);

            DeleteAccount = true;
            DialogResult = true;
            Close();
            return;
        }

        Log.Error(exception, "An error occurred please retry");
        Dialogs.MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxDeleteAccountErrorTitle,
            AddEditAccountResources.MessageBoxDeleteAccountErrorMessage, MsgBoxImage.Error);
    }

    private async void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO correct

        // var error = await CheckIsError();
        // if (error) return;
        //
        // DialogResult = true;
        // Close();

        var validator = App.ServiceProvider.GetRequiredService<AccountViewModelValidator>();
        var valResult = await validator.ValidateAsync(AccountViewModel, CancellationToken.None);

        AccountViewModel.ValidateWithFluent(valResult);
    }

    private void TextBoxStartingBalance_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        e.Handled = !txt.IsOnlyDecimal();
    }

    #endregion

    #region Function

    private async Task<bool> CheckIsError()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Creating a new ValidationContext for the Account object to perform data validation.
        // The serviceProvider and items are set to null because they are not required in this context.
        // The ValidationResults list will store any validation errors detected during the process.
        var validationContext = new ValidationContext(Account, serviceProvider: null, items: null);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using 'var' keeps the code concise and readable, as the type (List<ValidationResult>)
        // is evident from the initialization. The result will still be compatible with any method
        // that expects an ICollection<ValidationResult>, as List<T> implements the ICollection interface.
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(Account, validationContext, validationResults, true);
        if (isValid)
        {
            // var alreadyExiste = await CheckAccountName(Account.Name);
            // if (alreadyExiste)
            // {
            //     MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxErrorAccountNameAlreadyExists, MsgBoxImage.Warning);
            //     return true;
            // }

            if (EnableStartingBalance is false) return false;

            if (string.IsNullOrEmpty(History.Description))
            {
                Dialogs.MsgBox.MsgBox.Show(AddEditAccountResources.MessageBoxErrorAccountStartingBalanceDescriptionCannotByEmpty,
                    MsgBoxImage.Warning);
                return true;
            }
        }

        var propertyError = validationResults.First();
        var propertyMemberName = propertyError.MemberNames.First();

        var messageErrorKey = propertyMemberName switch
        {
            nameof(TAccount.Name) => nameof(AddEditAccountResources.MessageBoxButtonValidationAccountNameError),
            nameof(TAccount.AccountTypeFk) => nameof(AddEditAccountResources.MessageBoxButtonValidationAccountTypeFkError),
            nameof(TAccount.CurrencyFk) => nameof(AddEditAccountResources.MessageBoxButtonValidationCurrencyFkError),
            _ => null
        };

        var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
            ? propertyError.ErrorMessage!
            : AddEditAccountResources.ResourceManager.GetString(messageErrorKey)!;

        Dialogs.MsgBox.MsgBox.Show(localizedErrorMessage, MsgBoxImage.Warning);

        return true;
    }

    private async Task FillCollection()
    {
        await Task.WhenAll(
            _expensePresentationService.GetAllCategoryTypeViewModelAsync().LoadAndSortAsync(CategoryTypes, x => x.Name!),
            _accountPresentationService.GetAllCurrencyViewModelAsync().LoadAndSortAsync(Currencies, x => x.Symbol!),
            _accountPresentationService.GetAllAccountTypeViewModelAsync().LoadAndSortAsync(AccountTypes, x => x.Name!)
        );
    }

    public void SetTAccount(TAccount account)
    {
        account.CopyPropertiesTo(Account);
        EditAccount = true;
    }

    #endregion

    public async Task LoadAsync(TotalByAccountViewModel totalByAccountViewModel)
    {
        var accountViewModel = await _accountPresentationService.GetAccount(totalByAccountViewModel);
        if (accountViewModel is null) return;

        accountViewModel.IsEditing = true;
        _accountDtoViewModelMapper.Merge(accountViewModel, AccountViewModel);
    }
}