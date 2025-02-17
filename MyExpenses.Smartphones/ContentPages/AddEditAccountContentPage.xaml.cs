using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Objects;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditAccountContentPage
{
    public static readonly BindableProperty ButtonCancelTextProperty = BindableProperty.Create(nameof(ButtonCancelText),
        typeof(string), typeof(AddEditAccountContentPage));

    public string ButtonCancelText
    {
        get => (string)GetValue(ButtonCancelTextProperty);
        set => SetValue(ButtonCancelTextProperty, value);
    }

    public static readonly BindableProperty ButtonDeleteTextProperty = BindableProperty.Create(nameof(ButtonDeleteText),
        typeof(string), typeof(AddEditAccountContentPage));

    public string ButtonDeleteText
    {
        get => (string)GetValue(ButtonDeleteTextProperty);
        set => SetValue(ButtonDeleteTextProperty, value);
    }

    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(AddEditAccountContentPage));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty LabelTextTitleAccountTypeProperty =
        BindableProperty.Create(nameof(LabelTextTitleAccountType), typeof(string), typeof(AddEditAccountContentPage));

    public string LabelTextTitleAccountType
    {
        get => (string)GetValue(LabelTextTitleAccountTypeProperty);
        set => SetValue(LabelTextTitleAccountTypeProperty, value);
    }

    public static readonly BindableProperty LabelTextTitleCurrencyProperty =
        BindableProperty.Create(nameof(LabelTextTitleCurrency), typeof(string), typeof(AddEditAccountContentPage));

    public string LabelTextTitleCurrency
    {
        get => (string)GetValue(LabelTextTitleCurrencyProperty);
        set => SetValue(LabelTextTitleCurrencyProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(AddEditAccountContentPage));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty CanDeleteProperty = BindableProperty.Create(nameof(CanDelete), typeof(bool),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(AddEditAccountContentPage), false);

    public bool CanDelete
    {
        get => (bool)GetValue(CanDeleteProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        init => SetValue(CanDeleteProperty, value);
    }

    public ObservableCollection<TAccountType> AccountTypes { get; } = [];
    public ObservableCollection<TCurrency> Currencies { get; } = [];
    private List<TAccount> Accounts { get; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // A new instance of `TAccount` is intentionally allocated here to represent the currently
    // editable account. This allocation ensures that each instance of `AddEditAccountContentPage`
    // has its own independent `Account` object, avoiding unintended shared state between pages
    // or operations. This is crucial for enabling proper data encapsulation and ensuring that
    // the `Account` instance can be safely manipulated without affecting other parts of the application.
    public TAccount Account { get; } = new();
    private TAccount? OriginalAccount { get; set; }

    private bool EditAccount { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // TaskCompletionSource is intentionally allocated here as it is the fundamental mechanism
    // for creating and controlling the completion of the Task exposed by `ResultDialog`.
    // This object is required to manually signal task completion (`SetResult`, `SetException`, etc.)
    // when the operation is resolved, ensuring proper asynchronous flow.
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public AddEditAccountContentPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts];

        RefreshObservableCollectionDatabase();

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonAddEditAccountType_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonAddEditAccountType();

    private void ButtonAddEditCurrency_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonAddEditCurrency();

    private void ButtonCancel_OnClicked(object? sender, EventArgs e)
    {
        if (OriginalAccount is not null) OriginalAccount.CopyPropertiesTo(Account);
        else Account.Reset();
    }

    private void ButtonDelete_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonDelete();

    private void ButtonValid_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonValid();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion

    #region Function

    private bool AddOrEditAccount()
    {
        Account.DateAdded ??= DateTime.Now;

        var json = Account.ToJson();

        Log.Information("Attempting to add edit account : {Json}", json);
        var (success, exception) = Account.AddOrEdit();

        if (success) Log.Information("Successful account editing");
        else Log.Error(exception, "Failed account editing");

        return success;
    }

    private async Task HandleButtonAddEditAccountType()
    {
        var accountTypeFk = Account.AccountTypeFk;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Explicitly creating a new instance of AccountTypeSummaryContentPage.
        // This ensures that each time this method is executed, a fresh, independent page instance is used.
        // Reusing an existing instance could cause unexpected state sharing or UI issues.
        var accountTypeSummaryContentPage = new AccountTypeSummaryContentPage();
        await Navigation.PushAsync(accountTypeSummaryContentPage);

        var result = await accountTypeSummaryContentPage.ResultDialog;
        if (!result) return;

        RefreshAccountTypes();
        Account.AccountTypeFk = accountTypeFk;
    }

    private async Task HandleButtonAddEditCurrency()
    {
        var currencyFk = Account.CurrencyFk;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Explicitly creating a new instance of CurrencySymbolSummaryContentPage.
        // This ensures that a fresh instance is always used, avoiding potential state or UI inconsistencies
        // that might arise from reusing an existing instance of this page.
        var currencySymbolSummaryContentPage = new CurrencySymbolSummaryContentPage();
        await Navigation.PushAsync(currencySymbolSummaryContentPage);

        var result = await currencySymbolSummaryContentPage.ResultDialog;
        if (!result) return;

        RefreshCurrencies();
        Account.CurrencyFk = currencyFk;
    }

    private async Task HandleButtonDelete()
    {
        var response = await DisplayAlert(
            AddEditAccountResources.MessageBoxDeleteAccountQuestionTitle,
            string.Format(AddEditAccountResources.MessageBoxDeleteAccountQuestionMessage, Environment.NewLine),
            AddEditAccountResources.MessageBoxDeleteAccountQuestionYesButton,
            AddEditAccountResources.MessageBoxDeleteAccountQuestionNoButton);

        if (!response) return;

        await Task.Delay(TimeSpan.FromMilliseconds(100));
        this.ShowCustomPopupActivityIndicator(AddEditAccountResources.CustomPopupActivityIndicatorDeleteAccount);
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        var json = Account.ToJson();
        Log.Information("Attempting to delete account : {Json}", json);

        var (success, exception) = Account.Delete(true);
        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();

        if (success)
        {
            Log.Information("Successful account deletion");
            await DisplayAlert(
                AddEditAccountResources.MessageBoxDeleteAccountSuccessTitle,
                AddEditAccountResources.MessageBoxDeleteAccountSuccessMessage,
                AddEditAccountResources.MessageBoxDeleteAccountSuccessOkButton);

            _taskCompletionSource.SetResult(true);
            await Navigation.PopAsync();
        }
        else
        {
            Log.Error(exception, "Failed account deletion");
            await DisplayAlert(
                AddEditAccountResources.MessageBoxDeleteAccountErrorTitle,
                AddEditAccountResources.MessageBoxDeleteAccountErrorMessage,
                AddEditAccountResources.MessageBoxDeleteAccountErrorOkButton);
        }
    }

    private async Task HandleButtonValid()
    {
        var isValid = await ValidAccount();
        if (!isValid) return;

        var success = AddOrEditAccount();
        if (!success)
        {
            await DisplayAlert(
                AddEditAccountResources.MessageBoxButtonValidErrorTitle,
                AddEditAccountResources.MessageBoxButtonValidErrorMessage,
                AddEditAccountResources.MessageBoxButtonValidErrorOkButton);
            return;
        }

        var message = EditAccount
            ? AddEditAccountResources.MessageBoxEditAccountSuccessMessage
            : AddEditAccountResources.MessageBoxButtonValidSuccessMessage;

        await DisplayAlert(AddEditAccountResources.MessageBoxButtonValidSuccessTitle, message, AddEditAccountResources.MessageBoxButtonValidSuccessOkButton);

        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private void RefreshAccountTypes()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        AccountTypes.Clear();
        AccountTypes.AddRange(context.TAccountTypes.OrderBy(s => s.Name));
    }

    private void RefreshCurrencies()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        Currencies.Clear();
        Currencies.AddRange(context.TCurrencies.OrderBy(s => s.Symbol));
    }

    private void RefreshObservableCollectionDatabase()
    {
        RefreshCurrencies();
        RefreshAccountTypes();
    }

    // ReSharper disable once HeapView.ClosureAllocation
    public void SetAccount(TAccount? account = null, int? id = null)
    {
        if (account is not null) account.CopyPropertiesTo(Account);
        else if (id is not null)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            account = Accounts.First(s => s.Id.Equals(id.Value));
            account.CopyPropertiesTo(Account);
            EditAccount = true;
        }
        else throw new ArgumentNullException(nameof(id), @"account id is null");

        OriginalAccount = account.DeepCopy();
    }

    private void UpdateLanguage()
    {
        PlaceholderText = AddEditAccountResources.PlaceholderText;
        LabelTextTitleCurrency = AddEditAccountResources.LabelTextTitleCurrency;
        LabelTextTitleAccountType = AddEditAccountResources.LabelTextTitleAccountType;

        ButtonValidText = AddEditAccountResources.ButtonValidText;
        ButtonDeleteText = AddEditAccountResources.ButtonDeleteText;
        ButtonCancelText = AddEditAccountResources.ButtonCancelText;
    }

    private async Task<bool> ValidAccount()
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
        if (isValid) return isValid;

        var propertyError = validationResults.First();
        var propertyMemberName = propertyError.MemberNames.First();

        var messageErrorKey = propertyMemberName switch
        {
            nameof(TAccount.Name) => nameof(AddEditAccountResources.MessageBoxButtonValidationNameError),
            nameof(TAccount.AccountTypeFk) => nameof(AddEditAccountResources.MessageBoxButtonValidationAccountTypeFkError),
            nameof(TAccount.CurrencyFk) => nameof(AddEditAccountResources.MessageBoxButtonValidationCurrencyFkError),
            _ => null
        };

        var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
            ? propertyError.ErrorMessage!
            : AddEditAccountResources.ResourceManager.GetString(messageErrorKey)!;

        await DisplayAlert(AddEditAccountResources.MessageBoxValidAccountErrorTitle,
            localizedErrorMessage, AddEditAccountResources.MessageBoxValidAccountErrorOkButton);

        return isValid;
    }

    #endregion
}