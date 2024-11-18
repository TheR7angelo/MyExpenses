using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AccountTypeSummaryContentPage;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CurrencySymbolSummaryContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Objects;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AccountTypeSummaryContentPage
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(AccountTypeSummaryContentPage), default(string));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty AccountTypeNameProperty = BindableProperty.Create(nameof(AccountTypeName),
        typeof(string), typeof(AccountTypeSummaryContentPage), default(string));

    public string AccountTypeName
    {
        get => (string)GetValue(AccountTypeNameProperty);
        set => SetValue(AccountTypeNameProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(AccountTypeSummaryContentPage), default(string));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public int MaxLength { get; } = 64;
    public ObservableCollection<TAccountType> AccountTypes { get; } = [];

    public ICommand BackCommand { get; set; }

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;
    
    public AccountTypeSummaryContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        RefreshAccountTypes();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        PlaceholderText = AccountTypeSummaryContentPageResources.PlaceholderText;
        ButtonValidText = AccountTypeSummaryContentPageResources.ButtonValidText;
    }

    private void RefreshAccountTypes()
    {
        AccountTypes.Clear();

        using var context = new DataBaseContext();
        AccountTypes.AddRange(context.TAccountTypes.OrderBy(s => s.Name));
    }

    private async void OnBackCommandPressed()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async void ButtonValid_OnClicked(object? sender, EventArgs e)
    {
        var validate = await ValidateAccountType();
        if (!validate) return;

        // TODO trad
        var response = await DisplayAlert(
            CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionTitle,
            string.Format(CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionMessage, AccountTypeName),
            CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionYesButton,
            CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionNoButton);
        if (!response) return;

        var newAccountType = new TAccountType
        {
            Name = AccountTypeName,
            DateAdded = DateTime.Now
        };

        var json = newAccountType.ToJson();
        Log.Information("Attempt to add new account type : {AccountType}", json);
        var (success, exception) = newAccountType.AddOrEdit();
        if (success)
        {
            Log.Information("New account type was successfully added");
            AccountTypes.AddAndSort(newAccountType, s => s.Name!);
            AccountTypeName = string.Empty;

            // TODO trad
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencySuccessTitle,
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencySuccessMessage,
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencySuccessOkButton);
        }
        else
        {
            // TODO trad
            Log.Error(exception, "An error occurred while adding new account type");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyErrorTitle,
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyErrorMessage,
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyErrorOkButton);
        }
    }

    private async Task<bool> ValidateAccountType(string? accountTypeName = null)
    {
        var accountTypeNameToTest = string.IsNullOrWhiteSpace(accountTypeName)
            ? AccountTypeName
            : accountTypeName;

        if (string.IsNullOrWhiteSpace(accountTypeNameToTest))
        {
            // TODO trad
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorEmptyTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorEmptyMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorEmptyOkButton);
            return false;
        }

        var alreadyExist = AccountTypes.Any(s => s.Name!.Equals(accountTypeNameToTest));
        if (alreadyExist)
        {
            // TODO trad
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistOkButton);
            return false;
        }

        return true;
    }

    private async void ButtonAccountType_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not TAccountType accountType) return;

        var tempAccountType = accountType.DeepCopy()!;
        await ShowCustomPopupEntryForCurrency(tempAccountType);
    }

    private async Task ShowCustomPopupEntryForCurrency(TAccountType accountType)
    {
        // TODO trad
        var placeHolder = CurrencySymbolSummaryContentPageResources.PlaceholderText;

        var customPopupEntry = new CustomPopupEntry { MaxLenght = MaxLength, PlaceholderText = placeHolder, EntryText = accountType.Name!, CanDelete = true };
        await this.ShowPopupAsync(customPopupEntry);

        var result = await customPopupEntry.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        accountType.Name = customPopupEntry.EntryText;
        await HandleAccountTypeResult(accountType, result);
        RefreshAccountTypes();
    }

    private async Task HandleAccountTypeResult(TAccountType accountType, ECustomPopupEntryResult result)
    {
        var json = accountType.ToJson();
        if (result is ECustomPopupEntryResult.Valid)
        {
            var validate = await ValidateAccountType(accountType.Name!);
            if (!validate) return;

            // TODO trad
            var response = await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditQuestionTitle,
                string.Format(CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditQuestionMessage, Environment.NewLine),
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditQuestionYesButton,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditQuestionNoButton);;
            if (!response) return;

            Log.Information("Attempt to edit account type : {AccountType}", json);
            await HandleAccountTypeEdit(accountType);

            return;
        }

        // TODO trad
        var deleteResponse = await DisplayAlert(
            CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteQuestionTitle,
            string.Format(CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteQuestionMessage, Environment.NewLine),
            CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteQuestionYesButton,
            CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteQuestionNoButton);
        if (!deleteResponse) return;

        // TODO trad
        Log.Information("Attempt to delete currency symbol : {Symbol}", json);
        await HandleAccountTypeDelete(accountType);
    }

    private async Task HandleAccountTypeDelete(TAccountType accountType)
    {
        var (success, exception) = accountType.Delete(true);
        if (success)
        {
            // TODO trad
            Log.Information("Account type and all related accounts were successfully deleted");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteSuccessTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteSuccessMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteSuccessOkButton);
        }
        else
        {
            // TODO trad
            Log.Error(exception, "An error occurred while deleting currency symbol");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteErrorTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteErrorMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteErrorOkButton);
        }
    }

    private async Task HandleAccountTypeEdit(TAccountType accountType)
    {
        var (success, exception) = accountType.AddOrEdit();
        if (success)
        {
            // TODO trad
            Log.Information("Account type was successfully edited");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditSuccessTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditSuccessMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditSuccessOkButton);
        }
        else
        {
            // TODO trad
            Log.Error(exception, "An error occurred while editing currency symbol");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditErrorTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditErrorMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditErrorOkButton);
        }
    }
}