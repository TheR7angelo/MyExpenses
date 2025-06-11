using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.CurrencySymbolManagement;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class CurrencyManagementContentPage
{
    private int MaxLength { get; }

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

    public CurrencyManagementContentPage()
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

        InitializeComponent();
    }

    #region Action

    private void ButtonAddCurrency_OnClick(object? sender, EventArgs e)
        => _ = HandleAddEditCurrency();

    private void ButtonSymbol_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not TCurrency currency) return;
        _ = HandleAddEditCurrency(currency);
    }

    #endregion

        #region Function

    private async Task HandleAddEditCurrency(TCurrency? currency = null)
    {
        var placeHolder = CurrencySymbolManagementResources.TextBoxCurrencySymbol;

        var currencySymbol = currency?.Symbol ?? string.Empty;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new instance of CustomPopupEntry is created and initialized with specific properties such as MaxLenght,
        // PlaceholderText, EntryText, and CanDelete. This instance is configured to provide a customizable popup
        // for editing or interacting with a currency's symbol. This setup allows the user to input or modify data
        // interactively while maintaining flexibility and ensuring proper validation during the interaction.
        var customPopupEntry = new CustomPopupEntry
        {
            MaxLenght = MaxLength, PlaceholderText = placeHolder,
            EntryText = currencySymbol, CanDelete = currency is not null
        };
        await this.ShowPopupAsync(customPopupEntry);

        var result = await customPopupEntry.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        var newCurrency = new TCurrency { Symbol = customPopupEntry.EntryText };

        if (result is not ECustomPopupEntryResult.Delete)
        {
            var newCurrencyIsError = await NewCurrencyIsError(newCurrency.Symbol);
            if (newCurrencyIsError) return;
        }
        else { newCurrency.Symbol = currencySymbol; }

        await HandleModePaymentResult(result, newCurrency, currency);
    }

    private async Task HandleAddNewCurrency(TCurrency newCurrency)
    {
        var response = await DisplayAlert(
            CurrencySymbolManagementResources.MessageBoxAddNewCurrencyQuestionTitle,
            string.Format(CurrencySymbolManagementResources.MessageBoxAddNewCurrencyQuestionMessage, newCurrency.Symbol),
            CurrencySymbolManagementResources.MessageBoxAddNewCurrencyQuestionYesButton,
            CurrencySymbolManagementResources.MessageBoxAddNewCurrencyQuestionNoButton);
        if (!response) return;

        var json = newCurrency.ToJson();
        Log.Information("Attempt to add new currency symbol : {Symbol}", json);
        var (success, exception) = newCurrency.AddOrEdit();
        if (success)
        {
            Log.Information("New currency symbol was successfully added");
            RefreshCurrency(newCurrency, add: true);

            await DisplayAlert(
                CurrencySymbolManagementResources.MessageBoxAddNewCurrencySuccessTitle,
                CurrencySymbolManagementResources.MessageBoxAddNewCurrencySuccessMessage,
                CurrencySymbolManagementResources.MessageBoxAddNewCurrencySuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while adding new currency symbol");
            await DisplayAlert(
                CurrencySymbolManagementResources.MessageBoxAddNewCurrencyErrorTitle,
                CurrencySymbolManagementResources.MessageBoxAddNewCurrencyErrorMessage,
                CurrencySymbolManagementResources.MessageBoxAddNewCurrencyErrorOkButton);
        }
    }

    private async Task HandleBackCommand()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async Task HandleDeleteCurrency(TCurrency oldCurrency)
    {
        Log.Information("Attempting to remove the currency symbol \"{CurrencySymbol}\" with all relative element",
            oldCurrency.Symbol);
        var (success, exception) = oldCurrency.Delete(true);
        DashBoardContentPage.Instance.RefreshAccountTotal();

        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
        if (success)
        {
            Log.Information("Currency symbol and all related accounts were successfully deleted");
            await DisplayAlert(
                CurrencySymbolManagementResources.MessageBoxCurrencyDeleteSuccessTitle,
                CurrencySymbolManagementResources.MessageBoxCurrencyDeleteSuccessMessage,
                CurrencySymbolManagementResources.MessageBoxCurrencyDeleteSuccessOkButton);

            RefreshCurrency(oldCurrency, remove: true);
        }
        else
        {
            Log.Error(exception, "An error occurred while deleting currency symbol");
            await DisplayAlert(
                CurrencySymbolManagementResources.MessageBoxCurrencyDeleteErrorTitle,
                CurrencySymbolManagementResources.MessageBoxCurrencyDeleteErrorMessage,
                CurrencySymbolManagementResources.MessageBoxCurrencyDeleteErrorOkButton);
        }
    }

    private async Task HandleEditModePayment(TCurrency newCurrency, TCurrency oldCurrency)
    {
        oldCurrency.Symbol = newCurrency.Symbol;
        var (success, exception) = oldCurrency.AddOrEdit();
        if (success)
        {
            Log.Information("Currency symbol was successfully edited");
            await DisplayAlert(
                CurrencySymbolManagementResources.MessageBoxCurrencyEditSuccessTitle,
                CurrencySymbolManagementResources.MessageBoxCurrencyEditSuccessMessage,
                CurrencySymbolManagementResources.MessageBoxCurrencyEditSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while editing currency symbol");
            await DisplayAlert(
                CurrencySymbolManagementResources.MessageBoxCurrencyEditErrorTitle,
                CurrencySymbolManagementResources.MessageBoxCurrencyEditErrorMessage,
                CurrencySymbolManagementResources.MessageBoxCurrencyEditErrorOkButton);
        }
    }

    private async Task HandleModePaymentResult(ECustomPopupEntryResult result, TCurrency newCurrency, TCurrency? oldCurrency)
    {
        switch (result)
        {
            case ECustomPopupEntryResult.Delete:
                await HandleDeleteCurrency(oldCurrency!);
                break;
            case ECustomPopupEntryResult.Valid when oldCurrency is null:
                await HandleAddNewCurrency(newCurrency);
                break;
            default:
                await HandleEditModePayment(newCurrency, oldCurrency!);
                break;
        }
    }

    private async Task<bool> NewCurrencyIsError(string? currencySymbol = null)
    {
        if (string.IsNullOrWhiteSpace(currencySymbol))
        {
            await DisplayAlert(
                CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorEmptyTitle,
                CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorEmptyMessage,
                CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorEmptyOkButton);
            return true;
        }

        // ReSharper disable once HeapView.DelegateAllocation
        var alreadyExist = Currencies.Any(s => s.Symbol!.Equals(currencySymbol));
        if (alreadyExist)
        {
            await DisplayAlert(
                CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistTitle,
                CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistMessage,
                CurrencySymbolManagementResources.MessageBoxValidateCurrencySymbolErrorAlreadyExistOkButton);
            return true;
        }

        return false;
    }

    private void OnBackCommandPressed()
        => _ = HandleBackCommand();

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

    private void RefreshCurrency(TCurrency currency, bool add = false, bool remove = false)
    {
        switch (add)
        {
            case true when remove:
                throw new ArgumentException("'add' and 'remove' cannot both be true at the same time.");
            case true:
                Currencies.AddAndSort(currency, s => s.Symbol!);
                break;
            default:
                Currencies.Remove(currency);
                break;
        }
    }

    #endregion
}