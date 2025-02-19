using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Objects;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AccountTypeSummaryContentPage
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(AccountTypeSummaryContentPage));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty AccountTypeNameProperty = BindableProperty.Create(nameof(AccountTypeName),
        typeof(string), typeof(AccountTypeSummaryContentPage));

    public string AccountTypeName
    {
        get => (string)GetValue(AccountTypeNameProperty);
        set => SetValue(AccountTypeNameProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(AccountTypeSummaryContentPage));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public int MaxLength { get; }

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

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonAccountType_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonAccountType(sender);

    private void ButtonValid_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonValid();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void OnBackCommandPressed()
        => _ = HandleBackCommand();

    #endregion

    #region Function

    private async Task HandleAccountTypeDelete(TAccountType accountType)
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

    private async Task HandleAccountTypeEdit(TAccountType accountType)
    {
        var (success, exception) = accountType.AddOrEdit();
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

    private async Task HandleAccountTypeResult(TAccountType accountType, ECustomPopupEntryResult result)
    {
        var json = accountType.ToJson();
        if (result is ECustomPopupEntryResult.Valid)
        {
            var validate = await ValidateAccountType(accountType.Name!);
            if (!validate) return;

            var response = await DisplayAlert(
                AccountTypeManagementResources.MessageBoxAccountTypeEditQuestionTitle,
                AccountTypeManagementResources.MessageBoxAccountTypeEditQuestionMessage,
                AccountTypeManagementResources.MessageBoxAccountTypeEditQuestionYesButton,
                AccountTypeManagementResources.MessageBoxAccountTypeEditQuestionNoButton);
            if (!response) return;

            Log.Information("Attempt to edit account type : {AccountType}", json);
            await HandleAccountTypeEdit(accountType);

            return;
        }

        var deleteResponse = await DisplayAlert(
            AccountTypeManagementResources.MessageBoxAccountTypeDeleteQuestionTitle,
            string.Format(AccountTypeManagementResources.MessageBoxAccountTypeDeleteQuestionMessage, Environment.NewLine),
            AccountTypeManagementResources.MessageBoxAccountTypeDeleteQuestionYesButton,
            AccountTypeManagementResources.MessageBoxAccountTypeDeleteQuestionNoButton);
        if (!deleteResponse) return;

        await Task.Delay(TimeSpan.FromMilliseconds(100));
        this.ShowCustomPopupActivityIndicator(AccountTypeManagementResources.ActivityIndicatorDeleteAccountType);
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Log.Information("Attempt to delete currency symbol : {Symbol}", json);
        await HandleAccountTypeDelete(accountType);
    }

    private async Task HandleBackCommand()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

        private async Task HandleButtonAccountType(object? sender)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not TAccountType accountType) return;

        var tempAccountType = accountType.DeepCopy()!;
        await ShowCustomPopupEntryForCurrency(tempAccountType);
    }

    private async Task HandleButtonValid()
    {
        var validate = await ValidateAccountType();
        if (!validate) return;

        var response = await DisplayAlert(
            AccountTypeManagementResources.MessageBoxAddNewAccountTypeQuestionTitle,
            string.Format(AccountTypeManagementResources.MessageBoxAddNewAccountTypeQuestionMessage, AccountTypeName),
            AccountTypeManagementResources.MessageBoxAddNewAccountTypeQuestionYesButton,
            AccountTypeManagementResources.MessageBoxAddNewAccountTypeQuestionNoButton);
        if (!response) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The allocation of the TAccountType object is necessary as it represents a new instance
        // of the data structure being created. This object encapsulates the account type's properties,
        // such as `Name` and `DateAdded`, which will be stored or processed further.
        // This allocation is intentional and fundamental to the purpose of adding a new account type.
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

    private async Task ShowCustomPopupEntryForCurrency(TAccountType accountType)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new CustomPopupEntry instance is necessary to display a popup dialog.
        // This object encapsulates the properties (`MaxLength`, `PlaceholderText`, etc.) specific to the popup's configuration.
        // It is intentionally allocated to dynamically adapt to the context of the operation (e.g., editing or creating an account type).
        var customPopupEntry = new CustomPopupEntry
        {
            MaxLenght = MaxLength,
            PlaceholderText = PlaceholderText,
            EntryText = accountType.Name!,
            CanDelete = true
        };

        await this.ShowPopupAsync(customPopupEntry);

        var result = await customPopupEntry.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        accountType.Name = customPopupEntry.EntryText;
        await HandleAccountTypeResult(accountType, result);
        RefreshAccountTypes();
    }

    private void UpdateLanguage()
    {
        PlaceholderText = AccountTypeManagementResources.TextBoxAccountTypeName;
        ButtonValidText = AccountTypeManagementResources.ButtonValidContent;
    }

    private async Task<bool> ValidateAccountType(string? accountTypeName = null)
    {
        // ReSharper disable once HeapView.ClosureAllocation
        var accountTypeNameToTest = string.IsNullOrWhiteSpace(accountTypeName)
            ? AccountTypeName
            : accountTypeName;

        if (string.IsNullOrWhiteSpace(accountTypeNameToTest))
        {
            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorEmptyTitle,
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorEmptyMessage,
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorEmptyOkButton);
            return false;
        }

        // ReSharper disable once HeapView.DelegateAllocation
        var alreadyExist = AccountTypes.Any(s => s.Name!.Equals(accountTypeNameToTest));
        if (alreadyExist)
        {
            await DisplayAlert(
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorAlreadyExistTitle,
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorAlreadyExistMessage,
                AccountTypeManagementResources.MessageBoxValidateAccountTypeErrorAlreadyExistOkButton);
            return false;
        }

        return true;
    }

    #endregion
}