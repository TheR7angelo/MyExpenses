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
    public TAccount Account { get; } = new();
    private TAccount? OriginalAccount { get; set; }

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public AddEditAccountContentPage()
    {
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
        var account = OriginalAccount ?? new TAccount();
        account.CopyPropertiesTo(Account);
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
        using var context = new DataBaseContext();
        AccountTypes.Clear();
        AccountTypes.AddRange(context.TAccountTypes.OrderBy(s => s.Name));
    }

    private void RefreshCurrencies()
    {
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
            account = Accounts.First(s => s.Id.Equals(id));
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
        var validationContext = new ValidationContext(Account, serviceProvider: null, items: null);
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