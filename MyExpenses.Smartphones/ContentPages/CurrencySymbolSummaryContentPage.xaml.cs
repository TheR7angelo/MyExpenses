using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CurrencySymbolSummaryContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Objects;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class CurrencySymbolSummaryContentPage
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(CurrencySymbolSummaryContentPage), default(string));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty SymbolTextProperty = BindableProperty.Create(nameof(SymbolText),
        typeof(string), typeof(CurrencySymbolSummaryContentPage), default(string));

    public string SymbolText
    {
        get => (string)GetValue(SymbolTextProperty);
        set => SetValue(SymbolTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(CurrencySymbolSummaryContentPage), default(string));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public int MaxLength { get; } = 24;

    public ObservableCollection<TCurrency> Currencies { get; } = [];

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public CurrencySymbolSummaryContentPage()
    {
        RefreshCurrencies();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private async void ButtonValid_OnClicked(object? sender, EventArgs e)
    {
        var validate = await ValidateCurrencySymbol();
        if (!validate) return;

        var response = await DisplayAlert(
            CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionTitle,
            string.Format(CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionMessage, SymbolText),
            CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionYesButton,
            CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionNoButton);
        if (!response) return;

        var newCurrency = new TCurrency
        {
            Symbol = SymbolText,
            DateAdded = DateTime.Now
        };

        var json = newCurrency.ToJson();
        Log.Information("Attempt to add new currency symbol : {Symbol}", json);
        var (success, exception) = newCurrency.AddOrEdit();
        if (success)
        {
            Log.Information("New currency symbol was successfully added");
            Currencies.AddAndSort(newCurrency, s => s.Symbol!);
            SymbolText = string.Empty;

            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencySuccessTitle,
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencySuccessMessage,
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencySuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while adding new currency symbol");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyErrorTitle,
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyErrorMessage,
                CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyErrorOkButton);
        }
    }

    private async void ButtonSymbol_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not TCurrency currency) return;

        var tempCurrency = currency.DeepCopy()!;
        await ShowCustomPopupEntryForCurrency(tempCurrency);
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    #endregion

    #region Function

    private async Task HandleCurrencyDelete(TCurrency currency)
    {
        var (success, exception) = currency.Delete(true);
        if (success)
        {
            Log.Information("Currency symbol and all related accounts were successfully deleted");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteSuccessTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteSuccessMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while deleting currency symbol");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteErrorTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteErrorMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteErrorOkButton);
        }
    }

    private async Task HandleCurrencyEdit(TCurrency currency)
    {
        var (success, exception) = currency.AddOrEdit();
        if (success)
        {
            Log.Information("Currency symbol was successfully edited");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditSuccessTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditSuccessMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while editing currency symbol");
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditErrorTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditErrorMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditErrorOkButton);
        }
    }

    private async Task HandleCurrencyResult(TCurrency currency, ECustomPopupEntryResult result)
    {
        var json = currency.ToJson();
        if (result is ECustomPopupEntryResult.Valid)
        {
            var validate = await ValidateCurrencySymbol(currency.Symbol!);
            if (!validate) return;

            var response = await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditQuestionTitle,
                string.Format(CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditQuestionMessage, Environment.NewLine),
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditQuestionYesButton,
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditQuestionNoButton);;
            if (!response) return;

            Log.Information("Attempt to edit currency symbol : {Symbol}", json);
            await HandleCurrencyEdit(currency);

            return;
        }

        var deleteResponse = await DisplayAlert(
            CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteQuestionTitle,
            string.Format(CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteQuestionMessage, Environment.NewLine),
            CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteQuestionYesButton,
            CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyDeleteQuestionNoButton);
        if (!deleteResponse) return;

        Log.Information("Attempt to delete currency symbol : {Symbol}", json);
        await HandleCurrencyDelete(currency);
    }

    private void RefreshCurrencies()
    {
        Currencies.Clear();

        using var context = new DataBaseContext();
        Currencies.AddRange(context.TCurrencies.OrderBy(s => s.Symbol));
    }

    private async Task ShowCustomPopupEntryForCurrency(TCurrency currency)
    {
        var placeHolder = CurrencySymbolSummaryContentPageResources.PlaceholderText;

        var customPopupEntry = new CustomPopupEntry { MaxLenght = MaxLength, PlaceholderText = placeHolder, EntryText = currency.Symbol!, CanDelete = true };
        await this.ShowPopupAsync(customPopupEntry);

        var result = await customPopupEntry.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        currency.Symbol = customPopupEntry.EntryText;
        await HandleCurrencyResult(currency, result);
        RefreshCurrencies();
    }

    private void UpdateLanguage()
    {
        PlaceholderText = CurrencySymbolSummaryContentPageResources.PlaceholderText;
        ButtonValidText = CurrencySymbolSummaryContentPageResources.ButtonValidText;
    }

    private async Task<bool> ValidateCurrencySymbol(string? currencySymbol = null)
    {
        var symbolToTest = string.IsNullOrWhiteSpace(currencySymbol)
            ? SymbolText
            : currencySymbol;

        if (string.IsNullOrWhiteSpace(symbolToTest))
        {
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorEmptyTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorEmptyMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorEmptyOkButton);
            return false;
        }

        var alreadyExist = Currencies.Any(s => s.Symbol!.Equals(symbolToTest));
        if (alreadyExist)
        {
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistOkButton);
            return false;
        }

        return true;
    }

    #endregion
}