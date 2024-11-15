using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AddEditBankTransferContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Objects;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditBankTransferContentPage
{
    public static readonly BindableProperty CustomEntryControlPlaceholderTextAdditionalReasonProperty =
        BindableProperty.Create(nameof(CustomEntryControlPlaceholderTextAdditionalReason), typeof(string),
            typeof(AddEditBankTransferContentPage), default(string));

    public string CustomEntryControlPlaceholderTextAdditionalReason
    {
        get => (string)GetValue(CustomEntryControlPlaceholderTextAdditionalReasonProperty);
        set => SetValue(CustomEntryControlPlaceholderTextAdditionalReasonProperty, value);
    }

    public static readonly BindableProperty CustomEntryControlPlaceholderTextMainReasonProperty =
        BindableProperty.Create(nameof(CustomEntryControlPlaceholderTextMainReason), typeof(string),
            typeof(AddEditBankTransferContentPage), default(string));

    public string CustomEntryControlPlaceholderTextMainReason
    {
        get => (string)GetValue(CustomEntryControlPlaceholderTextMainReasonProperty);
        set => SetValue(CustomEntryControlPlaceholderTextMainReasonProperty, value);
    }

    public static readonly BindableProperty LabelTextToAccountToProperty =
        BindableProperty.Create(nameof(LabelTextToAccountTo), typeof(string), typeof(AddEditBankTransferContentPage),
            default(string));

    public string LabelTextToAccountTo
    {
        get => (string)GetValue(LabelTextToAccountToProperty);
        set => SetValue(LabelTextToAccountToProperty, value);
    }

    public static readonly BindableProperty LabelTextTransferValueProperty =
        BindableProperty.Create(nameof(LabelTextTransferValue), typeof(string), typeof(AddEditBankTransferContentPage),
            default(string));

    public string LabelTextTransferValue
    {
        get => (string)GetValue(LabelTextTransferValueProperty);
        set => SetValue(LabelTextTransferValueProperty, value);
    }

    public static readonly BindableProperty ButtonCancelUpdateTextProperty =
        BindableProperty.Create(nameof(ButtonCancelUpdateText), typeof(string), typeof(AddEditBankTransferContentPage),
            default(string));

    public string ButtonCancelUpdateText
    {
        get => (string)GetValue(ButtonCancelUpdateTextProperty);
        set => SetValue(ButtonCancelUpdateTextProperty, value);
    }

    public static readonly BindableProperty ButtonCanBeDeletedTextProperty =
        BindableProperty.Create(nameof(ButtonCanBeDeletedText), typeof(string), typeof(AddEditBankTransferContentPage),
            default(string));

    public string ButtonCanBeDeletedText
    {
        get => (string)GetValue(ButtonCanBeDeletedTextProperty);
        set => SetValue(ButtonCanBeDeletedTextProperty, value);
    }

    public static readonly BindableProperty ButtonUpdateTextProperty = BindableProperty.Create(nameof(ButtonUpdateText),
        typeof(string), typeof(AddEditBankTransferContentPage), default(string));

    public string ButtonUpdateText
    {
        get => (string)GetValue(ButtonUpdateTextProperty);
        set => SetValue(ButtonUpdateTextProperty, value);
    }

    public static readonly BindableProperty FromAccountSymbolProperty =
        BindableProperty.Create(nameof(FromAccountSymbol), typeof(string), typeof(AddEditBankTransferContentPage),
            default(string));

    public string FromAccountSymbol
    {
        get => (string)GetValue(FromAccountSymbolProperty);
        set => SetValue(FromAccountSymbolProperty, value);
    }

    public static readonly BindableProperty CanBeDeletedProperty = BindableProperty.Create(nameof(CanBeDeleted),
        typeof(bool), typeof(AddEditBankTransferContentPage), default(bool));

    public bool CanBeDeleted
    {
        get => (bool)GetValue(CanBeDeletedProperty);
        set => SetValue(CanBeDeletedProperty, value);
    }

    public static readonly BindableProperty IsDirtyProperty = BindableProperty.Create(nameof(IsDirty), typeof(bool),
        typeof(AddEditBankTransferContentPage), default(bool));

    public bool IsDirty
    {
        get => (bool)GetValue(IsDirtyProperty);
        set => SetValue(IsDirtyProperty, value);
    }

    public static readonly BindableProperty LabelTextTransferDateProperty =
        BindableProperty.Create(nameof(LabelTextTransferDate), typeof(string), typeof(AddEditBankTransferContentPage),
            default(string));

    public string LabelTextTransferDate
    {
        get => (string)GetValue(LabelTextTransferDateProperty);
        set => SetValue(LabelTextTransferDateProperty, value);
    }

    public static readonly BindableProperty LabelTextFromAccountFromProperty =
        BindableProperty.Create(nameof(LabelTextFromAccountFrom), typeof(string),
            typeof(AddEditBankTransferContentPage), default(string));

    public string LabelTextFromAccountFrom
    {
        get => (string)GetValue(LabelTextFromAccountFromProperty);
        set => SetValue(LabelTextFromAccountFromProperty, value);
    }

    public TBankTransfer BankTransfer { get; } = new();
    public TBankTransfer? OriginalBankTransfer { get; private set; }

    private List<TAccount> Accounts { get; }
    public ObservableCollection<TAccount> FromAccounts { get; } = [];
    public ObservableCollection<TAccount> ToAccounts { get; } = [];

    public ICommand BackCommand { get; set; }

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public AddEditBankTransferContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        FromAccounts.AddRange(Accounts);
        ToAccounts.AddRange(Accounts);

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private async void OnBackCommandPressed()
    {
        if (IsDirty)
        {
            var response = await DisplayAlert(
                AddEditBankTransferContentPageResources.MessageBoxOnBackCommandPressedTitle,
                AddEditBankTransferContentPageResources.MessageBoxOnBackCommandPressedMessage,
                AddEditBankTransferContentPageResources.MessageBoxOnBackCommandPressedYesButton,
                AddEditBankTransferContentPageResources.MessageBoxOnBackCommandPressedNoButton);

            if (response)
            {
                var isValidBankTransfer = await ValidValidBankTransfer();
                if (!isValidBankTransfer) return;

                var success = AddOrEditBankTransfer();
                if (!success)
                {
                    await DisplayAlert(
                        AddEditBankTransferContentPageResources.MessageBoxOnBackCommandPressedErrorTitle,
                        AddEditBankTransferContentPageResources.MessageBoxOnBackCommandPressedErrorMessage,
                        AddEditBankTransferContentPageResources.MessageBoxOnBackCommandPressedErrorOkButton);
                    return;
                }

                _taskCompletionSource.SetResult(true);
            }
            else _taskCompletionSource.SetResult(false);
        }

        await Navigation.PopAsync();
    }

    private bool AddOrEditBankTransfer()
    {
        var json = BankTransfer.ToJson();

        Log.Information("Attempting to add edit bank transfer : {Json}", json);
        var (success, exception) = BankTransfer.AddOrEdit();

        if (success) Log.Information("Successful bank transfer editing");
        else Log.Error(exception, "Failed bank transfer editing");

        return success;
    }

    private async Task<bool> ValidValidBankTransfer()
    {
        var validationContext = new ValidationContext(BankTransfer, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(BankTransfer, validationContext, validationResults, true);

        if (isValid) return isValid;

        var propertyError = validationResults.First();
        var propertyMemberName = propertyError.MemberNames.First();

        var messageErrorKey = propertyMemberName switch
        {
            nameof(TBankTransfer.FromAccountFk) => nameof(AddEditBankTransferContentPageResources.MessageBoxButtonValidationFromAccountFkError),
            nameof(TBankTransfer.ToAccountFk) => nameof(AddEditBankTransferContentPageResources.MessageBoxButtonValidationToAccountFkError),
            nameof(TBankTransfer.Value) => nameof(AddEditBankTransferContentPageResources.MessageBoxButtonValidationValueError),
            nameof(TBankTransfer.Date) => nameof(AddEditBankTransferContentPageResources.MessageBoxButtonValidationDateError),
            nameof(TBankTransfer.MainReason) => nameof(AddEditBankTransferContentPageResources.MessageBoxButtonValidationMainReasonError),
            _ => null
        };

        var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
            ? propertyError.ErrorMessage!
            : AddEditBankTransferContentPageResources.ResourceManager.GetString(messageErrorKey)!;

        await DisplayAlert(AddEditBankTransferContentPageResources.MessageBoxValidBankTransferErrorTitle, localizedErrorMessage, AddEditBankTransferContentPageResources.MessageBoxValidBankTransferErrorOkButton);

        return isValid;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        ButtonUpdateText = AddEditBankTransferContentPageResources.ButtonUpdateText;
        ButtonCanBeDeletedText = AddEditBankTransferContentPageResources.ButtonCanBeDeletedText;
        ButtonCancelUpdateText = AddEditBankTransferContentPageResources.ButtonCancelUpdateText;

        LabelTextFromAccountFrom = AddEditBankTransferContentPageResources.LabelTextFromAccountFrom;
        LabelTextTransferDate = AddEditBankTransferContentPageResources.LabelTextTransferDate;
        LabelTextTransferValue = AddEditBankTransferContentPageResources.LabelTextTransferValue;
        LabelTextToAccountTo = AddEditBankTransferContentPageResources.LabelTextToAccountTo;

        CustomEntryControlPlaceholderTextMainReason = AddEditBankTransferContentPageResources.CustomEntryControlPlaceholderTextMainReason;
        CustomEntryControlPlaceholderTextAdditionalReason = AddEditBankTransferContentPageResources.CustomEntryControlPlaceholderTextAdditionalReason;
    }

    public void SetVBankTransferSummary(VBankTransferSummary? vBankTransferSummary)
    {
        if (vBankTransferSummary is null) return;

        var bankTransfer = vBankTransferSummary.Id.ToISql<TBankTransfer>()!;
        bankTransfer.CopyPropertiesTo(BankTransfer);
        OriginalBankTransfer = bankTransfer.DeepCopy();

        UpdateIsDirty();
        UpdateFromAccountSymbol();
    }

    private void UpdateFromAccountSymbol()
    {
        using var context = new DataBaseContext();
        var account = Accounts.First(a => a.Id.Equals(BankTransfer.FromAccountFk));
        FromAccountSymbol = context.TCurrencies.First(s => s.Id.Equals(account.CurrencyFk)).Symbol!;
    }

    private void PickerFromAccount_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        var accountIdToRemove = BankTransfer.FromAccountFk;
        var currentAccountId = BankTransfer.ToAccountFk;

        PickerToAccountFk.SelectedIndexChanged -= PickerToAccount_OnSelectedIndexChanged;

        UpdateAccountsCollection(accountIdToRemove, ToAccounts);
        BankTransfer.ToAccountFk = currentAccountId;

        PickerToAccountFk.SelectedIndexChanged += PickerToAccount_OnSelectedIndexChanged;

        UpdateIsDirty();
        UpdateFromAccountSymbol();
    }

    private void PickerToAccount_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        var accountIdToRemove = BankTransfer.ToAccountFk;
        var currentAccountId = BankTransfer.FromAccountFk;

        PickerFromAccountFk.SelectedIndexChanged -= PickerFromAccount_OnSelectedIndexChanged;

        UpdateAccountsCollection(accountIdToRemove, FromAccounts);
        BankTransfer.FromAccountFk = currentAccountId;

        PickerFromAccountFk.SelectedIndexChanged += PickerFromAccount_OnSelectedIndexChanged;

        UpdateIsDirty();
    }

    private void UpdateAccountsCollection(int? accountIdToRemove, ObservableCollection<TAccount> collection)
    {
        var newCollection = Accounts.Where(s => s.Id != accountIdToRemove);

        collection.Clear();
        collection.AddRange(newCollection);
    }

    private void ButtonUpdateBankTransfer_OnClicked(object? sender, EventArgs e)
    {
        // TODO work
        throw new NotImplementedException();
    }

    private void ButtonDeleteBankTransfer_OnClicked(object? sender, EventArgs e)
    {
        // TODO work
        throw new NotImplementedException();
    }

    private void ButtonCancelUpdateBankTransfer_OnClicked(object? sender, EventArgs e)
    {
        // TODO work
        throw new NotImplementedException();
    }

    private void EntryValue_OnTextChanged(object? sender, TextChangedEventArgs e)
        => UpdateIsDirty();

    private void UpdateIsDirty()
    {
        IsDirty = !BankTransfer.AreEqual(OriginalBankTransfer);

        Title = IsDirty
            ? "Changes in progress"
            : string.Empty;
    }
}