using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AddEditAccountContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Objects;
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
            AddEditAccountContentPageResources.MessageBoxDeleteAccountQuestionTitle,
            string.Format(AddEditAccountContentPageResources.MessageBoxDeleteAccountQuestionMessage, Environment.NewLine),
            AddEditAccountContentPageResources.MessageBoxDeleteAccountQuestionYesButton,
            AddEditAccountContentPageResources.MessageBoxDeleteAccountQuestionNoButton);

        if (!response) return;

        await Task.Delay(TimeSpan.FromMilliseconds(100));
        this.ShowCustomPopupActivityIndicator(AddEditAccountContentPageResources.CustomPopupActivityIndicatorDeleteAccount);
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        var json = Account.ToJson();
        Log.Information("Attempting to delete account : {Json}", json);

        var (success, exception) = Account.Delete(true);
        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();

        if (success)
        {
            Log.Information("Successful account deletion");
            await DisplayAlert(
                AddEditAccountContentPageResources.MessageBoxDeleteAccountSuccessTitle,
                AddEditAccountContentPageResources.MessageBoxDeleteAccountSuccessMessage,
                AddEditAccountContentPageResources.MessageBoxDeleteAccountSuccessOkButton);

            _taskCompletionSource.SetResult(true);
            await Navigation.PopAsync();
        }
        else
        {
            Log.Error(exception, "Failed account deletion");
            await DisplayAlert(
                AddEditAccountContentPageResources.MessageBoxDeleteAccountErrorTitle,
                AddEditAccountContentPageResources.MessageBoxDeleteAccountErrorMessage,
                AddEditAccountContentPageResources.MessageBoxDeleteAccountErrorOkButton);
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
                AddEditAccountContentPageResources.MessageBoxOnBackCommandPressedErrorTitle,
                AddEditAccountContentPageResources.MessageBoxOnBackCommandPressedErrorMessage,
                AddEditAccountContentPageResources.MessageBoxOnBackCommandPressedErrorOkButton);
            return;
        }

        await DisplayAlert(
            AddEditAccountContentPageResources.MessageBoxOnBackCommandPressedSuccessTitle,
            AddEditAccountContentPageResources.MessageBoxOnBackCommandPressedSuccessMessage,
            AddEditAccountContentPageResources.MessageBoxOnBackCommandPressedSuccessOkButton);

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

    public void SetAccount(TAccount? account = null, int? id = null)
    {
        if (account is not null) account.CopyPropertiesTo(Account);
        else if (id is not null)
        {
            account = Accounts.First(s => s.Id.Equals(id.Value));
            account.CopyPropertiesTo(Account);
        }
        else throw new ArgumentNullException(nameof(id), @"account id is null");

        OriginalAccount = account.DeepCopy();
    }

    private void UpdateLanguage()
    {
        PlaceholderText = AddEditAccountContentPageResources.PlaceholderText;
        LabelTextTitleCurrency = AddEditAccountContentPageResources.LabelTextTitleCurrency;
        LabelTextTitleAccountType = AddEditAccountContentPageResources.LabelTextTitleAccountType;

        ButtonValidText = AddEditAccountContentPageResources.ButtonValidText;
        ButtonDeleteText = AddEditAccountContentPageResources.ButtonDeleteText;
        ButtonCancelText = AddEditAccountContentPageResources.ButtonCancelText;
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
            nameof(TAccount.Name) => nameof(AddEditAccountContentPageResources.MessageBoxButtonValidationNameError),
            nameof(TAccount.AccountTypeFk) => nameof(AddEditAccountContentPageResources.MessageBoxButtonValidationAccountTypeFkError),
            nameof(TAccount.CurrencyFk) => nameof(AddEditAccountContentPageResources.MessageBoxButtonValidationCurrencyFkError),
            _ => null
        };

        var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
            ? propertyError.ErrorMessage!
            : AddEditAccountContentPageResources.ResourceManager.GetString(messageErrorKey)!;

        await DisplayAlert(AddEditAccountContentPageResources.MessageBoxValidAccountErrorTitle,
            localizedErrorMessage, AddEditAccountContentPageResources.MessageBoxValidAccountErrorOkButton);

        return isValid;
    }

    #endregion
}