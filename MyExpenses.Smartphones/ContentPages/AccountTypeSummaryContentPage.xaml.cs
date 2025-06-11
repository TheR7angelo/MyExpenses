using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AccountTypeSummaryContentPage
{
    private int MaxLength { get; }

    public ObservableCollection<TAccountType> AccountTypes { get; } = [];

    public ICommand BackCommand { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // TaskCompletionSource is intentionally allocated here as it is the fundamental mechanism
    // for creating and controlling the completion of the Task exposed by `ResultDialog`.
    // This object is required to manually signal task completion (`SetResult`, `SetException`, etc.)
    // when the operation is resolved, ensuring proper asynchronous flow.
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public AccountTypeSummaryContentPage()
    {
        MaxLength = Utils.Converters.MaxLengthConverter.Convert(typeof(TAccountType), nameof(TAccountType.Name));

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // ReSharper disable once HeapView.DelegateAllocation
        // The Command object is explicitly created here to handle the user's interaction with the UI.
        // This allocation is necessary because `Command` encapsulates the behavior (in this case, `OnBackCommandPressed`)
        // and binds it to the associated UI element, such as a Button or a gesture.
        // This ensures proper separation between the UI and logic layers.
        BackCommand = new Command(OnBackCommandPressed);

        RefreshAccountTypes();

        InitializeComponent();
    }

    #region Action

    private void ButtonAccountType_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not TAccountType accountType) return;
        _ = HandleAddEditAccountType(accountType);
    }

    private void ButtonAddAccountType_OnClick(object? sender, EventArgs e)
        => _ = HandleAddEditAccountType();

    private void OnBackCommandPressed()
        => _ = HandleBackCommand();

    #endregion

    #region Function

    private async Task HandleAccountTypeResult(ECustomPopupEntryResult result, TAccountType newModePayment, TAccountType? oldModePayment)
    {
        switch (result)
        {
            case ECustomPopupEntryResult.Delete:
                await HandleDeleteAccountType(oldModePayment!);
                break;
            case ECustomPopupEntryResult.Valid when oldModePayment is null:
                await HandleAddNewAccountType(newModePayment);
                break;
            default:
                await HandleEditAccountType(newModePayment, oldModePayment!);
                break;
        }
    }

    private async Task HandleAddEditAccountType(TAccountType? accountType = null)
    {
        var placeHolder = AccountTypeManagementResources.TextBoxAccountTypeName;
        var modePaymentName = accountType?.Name ?? string.Empty;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new instance of CustomPopupEntry is created and initialized with specific properties such as MaxLenght,
        // PlaceholderText, EntryText, and CanDelete. This instance is configured to provide a customizable popup
        // for editing or interacting with a currency's symbol. This setup allows the user to input or modify data
        // interactively while maintaining flexibility and ensuring proper validation during the interaction.
        var customPopupEntry = new CustomPopupEntry
        {
            MaxLenght = MaxLength, PlaceholderText = placeHolder,
            EntryText = modePaymentName, CanDelete = accountType is not null
        };
        await this.ShowPopupAsync(customPopupEntry);

        var result = await customPopupEntry.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        var newAccountType = new TAccountType { Name = customPopupEntry.EntryText };

        if (result is not ECustomPopupEntryResult.Delete)
        {
            var newModePaymentIsError = await NewModePaymentIsError(newAccountType.Name);
            if (newModePaymentIsError) return;
        }
        else { newAccountType.Name = modePaymentName; }

        await HandleAccountTypeResult(result, newAccountType, accountType);
    }

    private async Task HandleAddNewAccountType(TAccountType newAccountType)
    {
        var response = await DisplayAlert(
            AccountTypeManagementResources.MessageBoxAddNewAccountTypeQuestionTitle,
            string.Format(AccountTypeManagementResources.MessageBoxAddNewAccountTypeQuestionMessage, newAccountType.Name),
            AccountTypeManagementResources.MessageBoxAddNewAccountTypeQuestionYesButton,
            AccountTypeManagementResources.MessageBoxAddNewAccountTypeQuestionNoButton);
        if (!response) return;

        var json = newAccountType.ToJson();
        Log.Information("Attempt to add new account type : {AccountType}", json);
        var (success, exception) = newAccountType.AddOrEdit();
        if (success)
        {
            Log.Information("New account type was successfully added");
            RefreshAccountType(newAccountType, true);

            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeSuccessTitle,
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeSuccessMessage,
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while adding new account type");
            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeErrorTitle,
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeErrorMessage,
                AccountTypeManagementResources.MessageBoxAddNewAccountTypeErrorOkButton);
        }
    }

    private async Task HandleBackCommand()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async Task HandleDeleteAccountType(TAccountType accountType)
    {
        var (success, exception) = accountType.Delete(true);
        DashBoardContentPage.Instance.RefreshAccountTotal();

        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();

        if (success)
        {
            Log.Information("Account type and all related accounts were successfully deleted");
            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxAccountTypeDeleteSuccessTitle,
                AccountTypeManagementResources.MessageBoxAccountTypeDeleteSuccessMessage,
                AccountTypeManagementResources.MessageBoxAccountTypeDeleteSuccessOkButton);
            RefreshAccountType(accountType, remove: true);
        }
        else
        {
            Log.Error(exception, "An error occurred while deleting currency symbol");
            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxAccountTypeDeleteErrorTitle,
                AccountTypeManagementResources.MessageBoxAccountTypeDeleteErrorMessage,
                AccountTypeManagementResources.MessageBoxAccountTypeDeleteErrorOkButton);
        }
    }

    private async Task HandleEditAccountType(TAccountType newModePayment, TAccountType oldAccountType)
    {
        oldAccountType.Name = newModePayment.Name;
        var (success, exception) = oldAccountType.AddOrEdit();
        if (success)
        {
            Log.Information("Account type was successfully edited");
            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxAccountTypeEditSuccessTitle,
                AccountTypeManagementResources.MessageBoxAccountTypeEditSuccessMessage,
                AccountTypeManagementResources.MessageBoxAccountTypeEditSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while editing currency symbol");
            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxAccountTypeEditErrorTitle,
                AccountTypeManagementResources.MessageBoxAccountTypeEditErrorMessage,
                AccountTypeManagementResources.MessageBoxAccountTypeEditErrorOkButton);
        }
    }

    private async Task<bool> NewModePaymentIsError(string? accountTypeName = null)
    {
        if (string.IsNullOrWhiteSpace(accountTypeName))
        {
            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorEmptyTitle,
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorEmptyMessage,
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorEmptyOkButton);
            return true;
        }

        // ReSharper disable once HeapView.DelegateAllocation
        var alreadyExist = AccountTypes.Any(s => s.Name!.Equals(accountTypeName));
        if (alreadyExist)
        {
            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorAlreadyExistTitle,
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorAlreadyExistMessage,
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorAlreadyExistOkButton);
            return true;
        }

        return false;
    }

    private void RefreshAccountType(TAccountType accountType, bool add = false, bool remove = false)
    {
        switch (add)
        {
            case true when remove:
                throw new ArgumentException("'add' and 'remove' cannot both be true at the same time.");
            case true:
                AccountTypes.AddAndSort(accountType, s => s.Name!);
                break;
            default:
                AccountTypes.Remove(accountType);
                break;
        }
    }

    private void RefreshAccountTypes()
    {
        AccountTypes.Clear();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        AccountTypes.AddRange(context.TAccountTypes.OrderBy(s => s.Name));
    }

    #endregion
}