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
    public static readonly BindableProperty LabelTextTransferPaymentModeProperty =
        BindableProperty.Create(nameof(LabelTextTransferPaymentMode), typeof(string),
            typeof(AddEditBankTransferContentPage), default(string));

    public string LabelTextTransferPaymentMode
    {
        get => (string)GetValue(LabelTextTransferPaymentModeProperty);
        set => SetValue(LabelTextTransferPaymentModeProperty, value);
    }

    public static readonly BindableProperty LabelTextTransferCategoryProperty =
        BindableProperty.Create(nameof(LabelTextTransferCategory), typeof(string),
            typeof(AddEditBankTransferContentPage), default(string));

    public string LabelTextTransferCategory
    {
        get => (string)GetValue(LabelTextTransferCategoryProperty);
        set => SetValue(LabelTextTransferCategoryProperty, value);
    }

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

    public static readonly BindableProperty SelectedCategoryTypeProperty = BindableProperty.Create(
        nameof(SelectedCategoryType), typeof(TCategoryType),
        typeof(AddEditBankTransferContentPage), default(TCategoryType));

    public TCategoryType? SelectedCategoryType
    {
        get => (TCategoryType?)GetValue(SelectedCategoryTypeProperty);
        set => SetValue(SelectedCategoryTypeProperty, value);
    }

    private TCategoryType? OriginalSelectedCategoryType { get; set; }

    public static readonly BindableProperty SelectedModePaymentProperty =
        BindableProperty.Create(nameof(SelectedModePayment), typeof(TModePayment),
            typeof(AddEditBankTransferContentPage), default(TModePayment));

    public TModePayment? SelectedModePayment
    {
        get => (TModePayment?)GetValue(SelectedModePaymentProperty);
        set => SetValue(SelectedModePaymentProperty, value);
    }

    private TModePayment? OriginalSelectedModePayment { get; set; }

    public TBankTransfer BankTransfer { get; } = new();
    public TBankTransfer? OriginalBankTransfer { get; private set; }

    private List<TAccount> Accounts { get; }
    public List<TCategoryType> CategoryTypes { get; }
    public List<TModePayment> ModePayments { get; }
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
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];

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
        var now = DateTime.Now;
        if (BankTransfer.Id is 0) AddTransactionHistories(now);
        else UpdateTransactionHistories(now);

        var json = BankTransfer.ToJson();

        Log.Information("Attempting to add edit bank transfer : {Json}", json);
        var (success, exception) = BankTransfer.AddOrEdit();

        if (success) Log.Information("Successful bank transfer editing");
        else Log.Error(exception, "Failed bank transfer editing");

        return success;
    }

    private void AddTransactionHistories(DateTime now)
    {
        var valueAbs = Math.Abs(BankTransfer.Value ?? 0);
        var fromHistory = CreateHistory(BankTransfer.FromAccountFk, -valueAbs, now);
        var toHistory = CreateHistory(BankTransfer.ToAccountFk, valueAbs, now);

        BankTransfer.THistories.Add(fromHistory);
        BankTransfer.THistories.Add(toHistory);
    }

    private void UpdateTransactionHistories(DateTime now)
    {
        using var context = new DataBaseContext();
        var fromHistory = context.THistories.First(s => s.BankTransferFk.Equals(BankTransfer.Id) && s.AccountFk.Equals(OriginalBankTransfer!.FromAccountFk));
        var toHistory = context.THistories.First(s => s.BankTransferFk.Equals(BankTransfer.Id) && s.AccountFk.Equals(OriginalBankTransfer!.ToAccountFk));

        UpdateHistory(fromHistory, BankTransfer.FromAccountFk, -Math.Abs(BankTransfer.Value ?? 0), now);
        UpdateHistory(toHistory, BankTransfer.ToAccountFk, Math.Abs(BankTransfer.Value ?? 0), now);

        BankTransfer.THistories.Clear();
        BankTransfer.THistories.Add(fromHistory);
        BankTransfer.THistories.Add(toHistory);
    }

    private THistory CreateHistory(int? accountFk, double value, DateTime now)
    {
        return new THistory
        {
            AccountFk = accountFk,
            Description = BankTransfer.MainReason,
            CategoryTypeFk = SelectedCategoryType?.Id,
            ModePaymentFk = SelectedModePayment?.Id,
            Value = value,
            Date = BankTransfer.Date,
            IsPointed = true,
            PlaceFk = 1,
            DateAdded = now,
            DatePointed = now,
        };
    }

    private void UpdateHistory(THistory history, int? accountFk, double value, DateTime now)
    {
        history.AccountFk = accountFk;
        history.Description = BankTransfer.MainReason;
        history.CategoryTypeFk = SelectedCategoryType?.Id;
        history.ModePaymentFk = SelectedModePayment?.Id;
        history.Value = value;
        history.Date = BankTransfer.Date;
        history.IsPointed = true;
        history.PlaceFk = 1;
        history.DatePointed = now;
    }

    private async Task<bool> ValidValidBankTransfer()
    {
        var validationContext = new ValidationContext(BankTransfer, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(BankTransfer, validationContext, validationResults, true);

        if (isValid)
        {
            if (SelectedCategoryType is null)
            {
                await DisplayAlert(AddEditBankTransferContentPageResources.MessageBoxValidBankTransferErrorTitle,
                    AddEditBankTransferContentPageResources.MessageBoxButtonValidBankTransferPrepareCategoryIsNullError,
                    AddEditBankTransferContentPageResources.MessageBoxValidBankTransferErrorOkButton);
                return false;
            }

            if (SelectedModePayment is null)
            {
                await DisplayAlert(AddEditBankTransferContentPageResources.MessageBoxValidBankTransferErrorTitle,
                    AddEditBankTransferContentPageResources.MessageBoxButtonValidBankTransferPrepareModePaymentIsNullError,
                    AddEditBankTransferContentPageResources.MessageBoxValidBankTransferErrorOkButton);
                return false;
            }

            return isValid;
        }

        var propertyError = validationResults.First();
        var propertyMemberName = propertyError.MemberNames.First();

        var messageErrorKey = propertyMemberName switch
        {
            nameof(TBankTransfer.FromAccountFk) => nameof(AddEditBankTransferContentPageResources
                .MessageBoxButtonValidationFromAccountFkError),
            nameof(TBankTransfer.ToAccountFk) => nameof(AddEditBankTransferContentPageResources
                .MessageBoxButtonValidationToAccountFkError),
            nameof(TBankTransfer.Value) => nameof(AddEditBankTransferContentPageResources
                .MessageBoxButtonValidationValueError),
            nameof(TBankTransfer.Date) => nameof(AddEditBankTransferContentPageResources
                .MessageBoxButtonValidationDateError),
            nameof(TBankTransfer.MainReason) => nameof(AddEditBankTransferContentPageResources
                .MessageBoxButtonValidationMainReasonError),
            _ => null
        };

        var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
            ? propertyError.ErrorMessage!
            : AddEditBankTransferContentPageResources.ResourceManager.GetString(messageErrorKey)!;

        await DisplayAlert(AddEditBankTransferContentPageResources.MessageBoxValidBankTransferErrorTitle,
            localizedErrorMessage, AddEditBankTransferContentPageResources.MessageBoxValidBankTransferErrorOkButton);

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

        LabelTextTransferCategory = AddEditBankTransferContentPageResources.LabelTextTransferCategoryProperty;
        LabelTextTransferPaymentMode = AddEditBankTransferContentPageResources.LabelTextTransferPaymentMode;
    }

    public void SetVBankTransferSummary(VBankTransferSummary? vBankTransferSummary)
    {
        if (vBankTransferSummary is null) return;

        var bankTransfer = vBankTransferSummary.Id.ToISql<TBankTransfer>()!;
        bankTransfer.CopyPropertiesTo(BankTransfer);
        OriginalBankTransfer = bankTransfer.DeepCopy();

        using var context = new DataBaseContext();
        var history = context.THistories.First(s => s.BankTransferFk.Equals(bankTransfer.Id));
        var categoryTypeId = context.TCategoryTypes.First(s => s.Id.Equals(history.CategoryTypeFk)).Id;
        var modePaymentId = context.TModePayments.First(s => s.Id.Equals(history.ModePaymentFk)).Id;

        SelectedCategoryType = CategoryTypes.FirstOrDefault(s => s.Id.Equals(categoryTypeId));
        OriginalSelectedCategoryType = SelectedCategoryType.DeepCopy();

        SelectedModePayment = ModePayments.FirstOrDefault(s => s.Id.Equals(modePaymentId));
        OriginalSelectedModePayment = SelectedModePayment.DeepCopy();

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
        if (OriginalBankTransfer is null)
        {
            var clearBankTransfer = new TBankTransfer();
            clearBankTransfer.CopyPropertiesTo(BankTransfer);

            SelectedCategoryType = null;
            SelectedModePayment = null;
        }
        else
        {
            OriginalBankTransfer!.CopyPropertiesTo(BankTransfer);
            SelectedCategoryType = CategoryTypes.FirstOrDefault(s => s.Id.Equals(OriginalSelectedCategoryType?.Id));
            SelectedModePayment = ModePayments.FirstOrDefault(s => s.Id.Equals(OriginalSelectedModePayment?.Id));
        }

        UpdateIsDirty();
    }

    private void EntryValue_OnTextChanged(object? sender, TextChangedEventArgs e)
        => UpdateIsDirty();

    private void UpdateIsDirty()
    {
        var bankTransferIsDirty = !BankTransfer.AreEqual(OriginalBankTransfer);
        var categoryIsDirty = !SelectedCategoryType.AreEqual(OriginalSelectedCategoryType);
        var modePaymentIsDirty = !SelectedModePayment.AreEqual(OriginalSelectedModePayment);

        IsDirty = bankTransferIsDirty || categoryIsDirty || modePaymentIsDirty;

        Title = IsDirty
            ? AddEditBankTransferContentPageResources.TitleIsDirty
            : string.Empty;
    }

    private void PickerCategory_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateIsDirty();

    private void PickerModePayment_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateIsDirty();
}