using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Objects;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CurrencySymbolSummaryContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class CurrencySymbolSummaryContentPage
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(CurrencySymbolSummaryContentPage));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty SymbolTextProperty = BindableProperty.Create(nameof(SymbolText),
        typeof(string), typeof(CurrencySymbolSummaryContentPage));

    public string SymbolText
    {
        get => (string)GetValue(SymbolTextProperty);
        set => SetValue(SymbolTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(CurrencySymbolSummaryContentPage));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public int MaxLength { get; }

    public ObservableCollection<TCurrency> Currencies { get; } = [];

    public ICommand BackCommand { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // TaskCompletionSource is intentionally allocated here as it is the fundamental mechanism
    // for creating and controlling the completion of the Task exposed by `ResultDialog`.
    // This object is required to manually signal task completion (`SetResult`, `SetException`, etc.)
    // when the operation is resolved, ensuring proper asynchronous flow.
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public CurrencySymbolSummaryContentPage()
    {
        MaxLength = Utils.Converters.MaxLengthConverter.Convert(typeof(TCurrency), nameof(TCurrency.Symbol));

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // ReSharper disable once HeapView.DelegateAllocation
        // The Command object is explicitly created here to handle the user's interaction with the UI.
        // This allocation is necessary because `Command` encapsulates the behavior (in this case, `OnBackCommandPressed`)
        // and binds it to the associated UI element, such as a Button or a gesture.
        // This ensures proper separation between the UI and logic layers.
        BackCommand = new Command(OnBackCommandPressed);

        RefreshCurrencies();

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonValid_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonValid();

    private void ButtonSymbol_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonSymbol(sender);

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void OnBackCommandPressed()
        => _ = HandleBackCommand();

    #endregion

    #region Function

    private async Task HandleBackCommand()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async Task HandleButtonValid()
    {
        var validate = await ValidateCurrencySymbol();
        if (!validate) return;

        var response = await DisplayAlert(
            CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionTitle,
            string.Format(CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionMessage, SymbolText),
            CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionYesButton,
            CurrencySymbolSummaryContentPageResources.MesageBoxAddNewCurrencyQuestionNoButton);
        if (!response) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of TCurrency is created here to represent a new currency with its essential properties.
        // The `Symbol` is set using `SymbolText`, which represents the user-provided currency value, and
        // `DateAdded` is initialized to the current date and time. This ensures that the new currency object
        // is properly prepared for further operations such as adding it to the database or the application state.
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

    private async Task HandleButtonSymbol(object? sender)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not TCurrency currency) return;

        var tempCurrency = currency.DeepCopy()!;
        await ShowCustomPopupEntryForCurrency(tempCurrency);
    }

    private async Task HandleCurrencyDelete(TCurrency currency)
    {
        var (success, exception) = currency.Delete(true);
        DashBoardContentPage.Instance.RefreshAccountTotal();

        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
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
                CurrencySymbolSummaryContentPageResources.MessageBoxHandleCurrencyEditQuestionNoButton);
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

        await Task.Delay(TimeSpan.FromMilliseconds(100));
        this.ShowCustomPopupActivityIndicator(CurrencySymbolSummaryContentPageResources.CustomPopupActivityIndicatorDeleteCurrency);
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Log.Information("Attempt to delete currency symbol : {Symbol}", json);
        await HandleCurrencyDelete(currency);
    }

    private void RefreshCurrencies()
    {
        Currencies.Clear();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        Currencies.AddRange(context.TCurrencies.OrderBy(s => s.Symbol));
    }

    private async Task ShowCustomPopupEntryForCurrency(TCurrency currency)
    {
        var placeHolder = CurrencySymbolSummaryContentPageResources.PlaceholderText;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new instance of CustomPopupEntry is created and initialized with specific properties such as MaxLenght,
        // PlaceholderText, EntryText, and CanDelete. This instance is configured to provide a customizable popup
        // for editing or interacting with a currency's symbol. This setup allows the user to input or modify data
        // interactively while maintaining flexibility and ensuring proper validation during the interaction.
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

        // ReSharper disable once HeapView.DelegateAllocation
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