using System.Collections.ObjectModel;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages;

public partial class ModePaymentManagementContentPage
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(ModePaymentManagementContentPage));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty ModePaymentTextProperty = BindableProperty.Create(nameof(ModePaymentText),
        typeof(string), typeof(ModePaymentManagementContentPage));

    public string ModePaymentText
    {
        get => (string)GetValue(ModePaymentTextProperty);
        set => SetValue(ModePaymentTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(ModePaymentManagementContentPage));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public int MaxLength { get; }

    public ObservableCollection<TModePayment> ModePayments { get; } = [];

    public ModePaymentManagementContentPage()
    {
        MaxLength = Utils.Converters.MaxLengthConverter.Convert(typeof(TModePayment), nameof(TModePayment.Name));
        RefreshModePayments();

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    // private void ButtonValid_OnClicked(object? sender, EventArgs e)
    //     => _ = HandleButtonValid();
    //
    // private void ButtonSymbol_OnClicked(object? sender, EventArgs e)
    //     => _ = HandleButtonSymbol(sender);

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #endregion
    //
    // #region Function
    //
    // private async Task HandleBackCommand()
    // {
    //     _taskCompletionSource.SetResult(true);
    //     await Navigation.PopAsync();
    // }
    //
    // private async Task HandleButtonValid()
    // {
    //     var validate = await ValidateCurrencySymbol();
    //     if (!validate) return;
    //
    //     var response = await DisplayAlert(
    //         CurrencySymbolManagementResources.MessageBoxAddNewCurrencyQuestionTitle,
    //         string.Format(CurrencySymbolManagementResources.MessageBoxAddNewCurrencyQuestionMessage, SymbolText),
    //         CurrencySymbolManagementResources.MessageBoxAddNewCurrencyQuestionYesButton,
    //         CurrencySymbolManagementResources.MessageBoxAddNewCurrencyQuestionNoButton);
    //     if (!response) return;
    //
    //     // ReSharper disable once HeapView.ObjectAllocation.Evident
    //     // An instance of TCurrency is created here to represent a new currency with its essential properties.
    //     // The `Symbol` is set using `SymbolText`, which represents the user-provided currency value, and
    //     // `DateAdded` is initialized to the current date and time. This ensures that the new currency object
    //     // is properly prepared for further operations such as adding it to the database or the application state.
    //     var newCurrency = new TCurrency
    //     {
    //         Symbol = SymbolText,
    //         DateAdded = DateTime.Now
    //     };
    //
    //     var json = newCurrency.ToJson();
    //     Log.Information("Attempt to add new currency symbol : {Symbol}", json);
    //     var (success, exception) = newCurrency.AddOrEdit();
    //     if (success)
    //     {
    //         Log.Information("New currency symbol was successfully added");
    //         Currencies.AddAndSort(newCurrency, s => s.Symbol!);
    //         SymbolText = string.Empty;
    //
    //         await DisplayAlert(
    //             CurrencySymbolManagementResources.MessageBoxAddNewCurrencySuccessTitle,
    //             CurrencySymbolManagementResources.MessageBoxAddNewCurrencySuccessMessage,
    //             CurrencySymbolManagementResources.MessageBoxAddNewCurrencySuccessOkButton);
    //     }
    //     else
    //     {
    //         Log.Error(exception, "An error occurred while adding new currency symbol");
    //         await DisplayAlert(
    //             CurrencySymbolManagementResources.MessageBoxAddNewCurrencyErrorTitle,
    //             CurrencySymbolManagementResources.MessageBoxAddNewCurrencyErrorMessage,
    //             CurrencySymbolManagementResources.MessageBoxAddNewCurrencyErrorOkButton);
    //     }
    // }
    //
    // private async Task HandleButtonSymbol(object? sender)
    // {
    //     if (sender is not Button button) return;
    //     if (button.BindingContext is not TCurrency currency) return;
    //
    //     var tempCurrency = currency.DeepCopy()!;
    //     await ShowCustomPopupEntryForCurrency(tempCurrency);
    // }
    //
    // private async Task HandleCurrencyDelete(TCurrency currency)
    // {
    //     var (success, exception) = currency.Delete(true);
    //     DashBoardContentPage.Instance.RefreshAccountTotal();
    //
    //     CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
    //     if (success)
    //     {
    //         Log.Information("Currency symbol and all related accounts were successfully deleted");
    //         await DisplayAlert(
    //             CurrencySymbolManagementResources.MessageBoxCurrencyDeleteSuccessTitle,
    //             CurrencySymbolManagementResources.MessageBoxCurrencyDeleteSuccessMessage,
    //             CurrencySymbolManagementResources.MessageBoxCurrencyDeleteSuccessOkButton);
    //     }
    //     else
    //     {
    //         Log.Error(exception, "An error occurred while deleting currency symbol");
    //         await DisplayAlert(
    //             CurrencySymbolManagementResources.MessageBoxCurrencyDeleteErrorTitle,
    //             CurrencySymbolManagementResources.MessageBoxCurrencyDeleteErrorMessage,
    //             CurrencySymbolManagementResources.MessageBoxCurrencyDeleteErrorOkButton);
    //     }
    // }
    //
    // private async Task HandleCurrencyEdit(TCurrency currency)
    // {
    //     var (success, exception) = currency.AddOrEdit();
    //     if (success)
    //     {
    //         Log.Information("Currency symbol was successfully edited");
    //         await DisplayAlert(
    //             CurrencySymbolManagementResources.MessageBoxCurrencyEditSuccessTitle,
    //             CurrencySymbolManagementResources.MessageBoxCurrencyEditSuccessMessage,
    //             CurrencySymbolManagementResources.MessageBoxCurrencyEditSuccessOkButton);
    //     }
    //     else
    //     {
    //         Log.Error(exception, "An error occurred while editing currency symbol");
    //         await DisplayAlert(
    //             CurrencySymbolManagementResources.MessageBoxCurrencyEditErrorTitle,
    //             CurrencySymbolManagementResources.MessageBoxCurrencyEditErrorMessage,
    //             CurrencySymbolManagementResources.MessageBoxCurrencyEditErrorOkButton);
    //     }
    // }
    //
    // private async Task HandleCurrencyResult(TCurrency currency, ECustomPopupEntryResult result)
    // {
    //     var json = currency.ToJson();
    //     if (result is ECustomPopupEntryResult.Valid)
    //     {
    //         var validate = await ValidateCurrencySymbol(currency.Symbol!);
    //         if (!validate) return;
    //
    //         var response = await DisplayAlert(
    //             CurrencySymbolManagementResources.MessageBoxCurrencyEditQuestionTitle,
    //             string.Format(CurrencySymbolManagementResources.MessageBoxCurrencyEditQuestionMessage, Environment.NewLine),
    //             CurrencySymbolManagementResources.MessageBoxCurrencyEditQuestionYesButton,
    //             CurrencySymbolManagementResources.MessageBoxCurrencyEditQuestionNoButton);
    //         if (!response) return;
    //
    //         Log.Information("Attempt to edit currency symbol : {Symbol}", json);
    //         await HandleCurrencyEdit(currency);
    //
    //         return;
    //     }
    //
    //     var deleteResponse = await DisplayAlert(
    //         CurrencySymbolManagementResources.MessageBoxCurrencyDeleteQuestionTitle,
    //         string.Format(CurrencySymbolManagementResources.MessageBoxCurrencyDeleteQuestionMessage, Environment.NewLine),
    //         CurrencySymbolManagementResources.MessageBoxCurrencyDeleteQuestionYesButton,
    //         CurrencySymbolManagementResources.MessageBoxCurrencyDeleteQuestionNoButton);
    //
    //     if (!deleteResponse) return;
    //
    //     await Task.Delay(TimeSpan.FromMilliseconds(100));
    //     this.ShowCustomPopupActivityIndicator(CurrencySymbolManagementResources.ActivityIndicatorDeleteCurrency);
    //     await Task.Delay(TimeSpan.FromMilliseconds(100));
    //
    //     Log.Information("Attempt to delete currency symbol : {Symbol}", json);
    //     await HandleCurrencyDelete(currency);
    // }

    private void RefreshModePayments()
    {
        ModePayments.Clear();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        ModePayments.AddRange(context.TModePayments.OrderBy(s => s.Name));
    }

    // private async Task ShowCustomPopupEntryForCurrency(TCurrency currency)
    // {
    //     var placeHolder = CurrencySymbolManagementResources.TextBoxCurrencySymbol;
    //
    //     // ReSharper disable once HeapView.ObjectAllocation.Evident
    //     // A new instance of CustomPopupEntry is created and initialized with specific properties such as MaxLenght,
    //     // PlaceholderText, EntryText, and CanDelete. This instance is configured to provide a customizable popup
    //     // for editing or interacting with a currency's symbol. This setup allows the user to input or modify data
    //     // interactively while maintaining flexibility and ensuring proper validation during the interaction.
    //     var customPopupEntry = new CustomPopupEntry { MaxLenght = MaxLength, PlaceholderText = placeHolder, EntryText = currency.Symbol!, CanDelete = true };
    //     await this.ShowPopupAsync(customPopupEntry);
    //
    //     var result = await customPopupEntry.ResultDialog;
    //     if (result is ECustomPopupEntryResult.Cancel) return;
    //
    //     currency.Symbol = customPopupEntry.EntryText;
    //     await HandleCurrencyResult(currency, result);
    //     RefreshCurrencies();
    // }

    private void UpdateLanguage()
    {
        PlaceholderText = "PlaceholderText"; // CurrencySymbolManagementResources.TextBoxCurrencySymbol;
        ButtonValidText = "ButtonValidText"; // CurrencySymbolManagementResources.ButtonValidText;
    }

    // private async Task<bool> ValidateCurrencySymbol(string? currencySymbol = null)
    // {
    //     // ReSharper disable once HeapView.ClosureAllocation
    //     var symbolToTest = string.IsNullOrWhiteSpace(currencySymbol)
    //         ? SymbolText
    //         : currencySymbol;
    //
    //     if (string.IsNullOrWhiteSpace(symbolToTest))
    //     {
    //         await DisplayAlert(
    //             CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorEmptyTitle,
    //             CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorEmptyMessage,
    //             CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorEmptyOkButton);
    //         return false;
    //     }
    //
    //     // ReSharper disable once HeapView.DelegateAllocation
    //     var alreadyExist = Currencies.Any(s => s.Symbol!.Equals(symbolToTest));
    //     if (alreadyExist)
    //     {
    //         await DisplayAlert(
    //             CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistTitle,
    //             CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistMessage,
    //             CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistOkButton);
    //         return false;
    //     }
    //
    //     return true;
    // }
    //
    // #endregion
}