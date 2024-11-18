using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AccountTypeSummaryContentPage;
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

    #region Action

    private async void ButtonAccountType_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not TAccountType accountType) return;

        var tempAccountType = accountType.DeepCopy()!;
        await ShowCustomPopupEntryForCurrency(tempAccountType);
    }

    private async void ButtonValid_OnClicked(object? sender, EventArgs e)
    {
        var validate = await ValidateAccountType();
        if (!validate) return;

        var response = await DisplayAlert(
            AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeQuestionTitle,
            string.Format(AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeQuestionMessage, AccountTypeName),
            AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeQuestionYesButton,
            AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeQuestionNoButton);
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

            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeSuccessTitle,
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeSuccessMessage,
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while adding new account type");
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeErrorTitle,
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeErrorMessage,
                AccountTypeSummaryContentPageResources.MesageBoxAddNewAccountTypeErrorOkButton);
        }
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private async void OnBackCommandPressed()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    #endregion

    #region Function

    private async Task HandleAccountTypeDelete(TAccountType accountType)
    {
        var (success, exception) = accountType.Delete(true);
        if (success)
        {
            Log.Information("Account type and all related accounts were successfully deleted");
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteSuccessTitle,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteSuccessMessage,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while deleting currency symbol");
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteErrorTitle,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteErrorMessage,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteErrorOkButton);
        }
    }

    private async Task HandleAccountTypeEdit(TAccountType accountType)
    {
        var (success, exception) = accountType.AddOrEdit();
        if (success)
        {
            Log.Information("Account type was successfully edited");
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditSuccessTitle,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditSuccessMessage,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred while editing currency symbol");
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditErrorTitle,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditErrorMessage,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditErrorOkButton);
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
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditQuestionTitle,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditQuestionMessage,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditQuestionYesButton,
                AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeEditQuestionNoButton);;
            if (!response) return;

            Log.Information("Attempt to edit account type : {AccountType}", json);
            await HandleAccountTypeEdit(accountType);

            return;
        }

        var deleteResponse = await DisplayAlert(
            AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteQuestionTitle,
            string.Format(AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteQuestionMessage, Environment.NewLine),
            AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteQuestionYesButton,
            AccountTypeSummaryContentPageResources.MessageBoxHandleAccountTypeDeleteQuestionNoButton);
        if (!deleteResponse) return;

        Log.Information("Attempt to delete currency symbol : {Symbol}", json);
        await HandleAccountTypeDelete(accountType);
    }

    private void RefreshAccountTypes()
    {
        AccountTypes.Clear();

        using var context = new DataBaseContext();
        AccountTypes.AddRange(context.TAccountTypes.OrderBy(s => s.Name));
    }

    private async Task ShowCustomPopupEntryForCurrency(TAccountType accountType)
    {
        var placeHolder = AccountTypeSummaryContentPageResources.PlaceholderText;

        var customPopupEntry = new CustomPopupEntry { MaxLenght = MaxLength, PlaceholderText = placeHolder, EntryText = accountType.Name!, CanDelete = true };
        await this.ShowPopupAsync(customPopupEntry);

        var result = await customPopupEntry.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        accountType.Name = customPopupEntry.EntryText;
        await HandleAccountTypeResult(accountType, result);
        RefreshAccountTypes();
    }

    private void UpdateLanguage()
    {
        PlaceholderText = AccountTypeSummaryContentPageResources.PlaceholderText;
        ButtonValidText = AccountTypeSummaryContentPageResources.ButtonValidText;
    }

    private async Task<bool> ValidateAccountType(string? accountTypeName = null)
    {
        var accountTypeNameToTest = string.IsNullOrWhiteSpace(accountTypeName)
            ? AccountTypeName
            : accountTypeName;

        if (string.IsNullOrWhiteSpace(accountTypeNameToTest))
        {
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorEmptyTitle,
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorEmptyMessage,
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorEmptyOkButton);
            return false;
        }

        var alreadyExist = AccountTypes.Any(s => s.Name!.Equals(accountTypeNameToTest));
        if (alreadyExist)
        {
            await DisplayAlert(
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorAlreadyExistTitle,
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorAlreadyExistMessage,
                AccountTypeSummaryContentPageResources.MessageBoxValidateAccountTypeErrorAlreadyExistOkButton);
            return false;
        }

        return true;
    }

    #endregion
}