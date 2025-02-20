using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Objects;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.BankTransferManagement;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AddEditBankTransferContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditBankTransferContentPage
{
    public static readonly BindableProperty LabelTextTransferPaymentModeProperty =
        BindableProperty.Create(nameof(LabelTextTransferPaymentMode), typeof(string),
            typeof(AddEditBankTransferContentPage), defaultValue: null);

    public string LabelTextTransferPaymentMode
    {
        get => (string)GetValue(LabelTextTransferPaymentModeProperty);
        set => SetValue(LabelTextTransferPaymentModeProperty, value);
    }

    public static readonly BindableProperty LabelTextTransferCategoryProperty =
        BindableProperty.Create(nameof(LabelTextTransferCategory), typeof(string),
            typeof(AddEditBankTransferContentPage));

    public string LabelTextTransferCategory
    {
        get => (string)GetValue(LabelTextTransferCategoryProperty);
        set => SetValue(LabelTextTransferCategoryProperty, value);
    }

    public static readonly BindableProperty CustomEntryControlPlaceholderTextAdditionalReasonProperty =
        BindableProperty.Create(nameof(CustomEntryControlPlaceholderTextAdditionalReason), typeof(string),
            typeof(AddEditBankTransferContentPage));

    public string CustomEntryControlPlaceholderTextAdditionalReason
    {
        get => (string)GetValue(CustomEntryControlPlaceholderTextAdditionalReasonProperty);
        set => SetValue(CustomEntryControlPlaceholderTextAdditionalReasonProperty, value);
    }

    public static readonly BindableProperty CustomEntryControlPlaceholderTextMainReasonProperty =
        BindableProperty.Create(nameof(CustomEntryControlPlaceholderTextMainReason), typeof(string),
            typeof(AddEditBankTransferContentPage));

    public string CustomEntryControlPlaceholderTextMainReason
    {
        get => (string)GetValue(CustomEntryControlPlaceholderTextMainReasonProperty);
        set => SetValue(CustomEntryControlPlaceholderTextMainReasonProperty, value);
    }

    public static readonly BindableProperty LabelTextToAccountToProperty =
        BindableProperty.Create(nameof(LabelTextToAccountTo), typeof(string), typeof(AddEditBankTransferContentPage));

    public string LabelTextToAccountTo
    {
        get => (string)GetValue(LabelTextToAccountToProperty);
        set => SetValue(LabelTextToAccountToProperty, value);
    }

    public static readonly BindableProperty LabelTextTransferValueProperty =
        BindableProperty.Create(nameof(LabelTextTransferValue), typeof(string), typeof(AddEditBankTransferContentPage));

    public string LabelTextTransferValue
    {
        get => (string)GetValue(LabelTextTransferValueProperty);
        set => SetValue(LabelTextTransferValueProperty, value);
    }

    public static readonly BindableProperty ButtonCancelUpdateTextProperty =
        BindableProperty.Create(nameof(ButtonCancelUpdateText), typeof(string), typeof(AddEditBankTransferContentPage));

    public string ButtonCancelUpdateText
    {
        get => (string)GetValue(ButtonCancelUpdateTextProperty);
        set => SetValue(ButtonCancelUpdateTextProperty, value);
    }

    public static readonly BindableProperty ButtonCanBeDeletedTextProperty =
        BindableProperty.Create(nameof(ButtonCanBeDeletedText), typeof(string), typeof(AddEditBankTransferContentPage));

    public string ButtonCanBeDeletedText
    {
        get => (string)GetValue(ButtonCanBeDeletedTextProperty);
        set => SetValue(ButtonCanBeDeletedTextProperty, value);
    }

    public static readonly BindableProperty ButtonUpdateTextProperty = BindableProperty.Create(nameof(ButtonUpdateText),
        typeof(string), typeof(AddEditBankTransferContentPage));

    public string ButtonUpdateText
    {
        get => (string)GetValue(ButtonUpdateTextProperty);
        set => SetValue(ButtonUpdateTextProperty, value);
    }

    public static readonly BindableProperty FromAccountSymbolProperty =
        BindableProperty.Create(nameof(FromAccountSymbol), typeof(string), typeof(AddEditBankTransferContentPage));

    public string FromAccountSymbol
    {
        get => (string)GetValue(FromAccountSymbolProperty);
        set => SetValue(FromAccountSymbolProperty, value);
    }

    public static readonly BindableProperty CanBeDeletedProperty = BindableProperty.Create(nameof(CanBeDeleted),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(bool), typeof(AddEditBankTransferContentPage), false);

    public bool CanBeDeleted
    {
        get => (bool)GetValue(CanBeDeletedProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        init => SetValue(CanBeDeletedProperty, value);
    }

    public static readonly BindableProperty IsDirtyProperty = BindableProperty.Create(nameof(IsDirty), typeof(bool),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(AddEditBankTransferContentPage), false);

    public bool IsDirty
    {
        get => (bool)GetValue(IsDirtyProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(IsDirtyProperty, value);
    }

    public static readonly BindableProperty LabelTextTransferDateProperty =
        BindableProperty.Create(nameof(LabelTextTransferDate), typeof(string), typeof(AddEditBankTransferContentPage));

    public string LabelTextTransferDate
    {
        get => (string)GetValue(LabelTextTransferDateProperty);
        set => SetValue(LabelTextTransferDateProperty, value);
    }

    public static readonly BindableProperty LabelTextFromAccountFromProperty =
        BindableProperty.Create(nameof(LabelTextFromAccountFrom), typeof(string),
            typeof(AddEditBankTransferContentPage));

    public string LabelTextFromAccountFrom
    {
        get => (string)GetValue(LabelTextFromAccountFromProperty);
        set => SetValue(LabelTextFromAccountFromProperty, value);
    }

    public static readonly BindableProperty SelectedCategoryTypeProperty = BindableProperty.Create(
        nameof(SelectedCategoryType), typeof(TCategoryType),
        typeof(AddEditBankTransferContentPage));

    public TCategoryType? SelectedCategoryType
    {
        get => (TCategoryType?)GetValue(SelectedCategoryTypeProperty);
        set => SetValue(SelectedCategoryTypeProperty, value);
    }

    private TCategoryType? OriginalSelectedCategoryType { get; set; }

    public static readonly BindableProperty SelectedModePaymentProperty =
        BindableProperty.Create(nameof(SelectedModePayment), typeof(TModePayment),
            typeof(AddEditBankTransferContentPage));

    public TModePayment? SelectedModePayment
    {
        get => (TModePayment?)GetValue(SelectedModePaymentProperty);
        set => SetValue(SelectedModePaymentProperty, value);
    }

    private TModePayment? OriginalSelectedModePayment { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // A new instance of `TBankTransfer` is intentionally allocated here to represent the current
    // bank transfer being added or edited. By creating a new instance for each `AddEditBankTransferContentPage`,
    // we ensure that every page has its own separate state for the `BankTransfer` object, preventing
    // unexpected interactions or shared states across different pages. This helps preserve proper data
    // encapsulation and ensures the integrity of the bank transfer operations.
    public TBankTransfer BankTransfer { get; } = new();
    private TBankTransfer? OriginalBankTransfer { get; set; }

    private List<TAccount> Accounts { get; }
    public List<TCategoryType> CategoryTypes { get; }
    public List<TModePayment> ModePayments { get; }
    public ObservableCollection<TAccount> FromAccounts { get; } = [];
    public ObservableCollection<TAccount> ToAccounts { get; } = [];

    public ICommand BackCommand { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // TaskCompletionSource is intentionally allocated here as it is the fundamental mechanism
    // for creating and controlling the completion of the Task exposed by `ResultDialog`.
    // This object is required to manually signal task completion (`SetResult`, `SetException`, etc.)
    // when the operation is resolved, ensuring proper asynchronous flow.
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public bool IsNewBankTransfer { get; init; }

    public AddEditBankTransferContentPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // ReSharper disable once HeapView.DelegateAllocation
        // The Command object is explicitly created here to handle the user's interaction with the UI.
        // This allocation is necessary because `Command` encapsulates the behavior (in this case, `OnBackCommandPressed`)
        // and binds it to the associated UI element, such as a Button or a gesture.
        // This ensures proper separation between the UI and logic layers.
        BackCommand = new Command(OnBackCommandPressed);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];

        FromAccounts.AddRange(Accounts);
        ToAccounts.AddRange(Accounts);

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonCancelUpdateBankTransfer_OnClicked(object? sender, EventArgs e)
    {
        if (OriginalBankTransfer is null)
        {
            BankTransfer.Reset();
            SelectedCategoryType = null;
            SelectedModePayment = null;
        }
        else
        {
            OriginalBankTransfer!.CopyPropertiesTo(BankTransfer);
            SelectedCategoryType = OriginalSelectedCategoryType is null
                ? null
                // ReSharper disable once HeapView.DelegateAllocation
                : CategoryTypes.FirstOrDefault(s => s.Id.Equals(OriginalSelectedCategoryType.Id));
            SelectedModePayment = OriginalSelectedModePayment is null
                ? null
                // ReSharper disable once HeapView.DelegateAllocation
                : ModePayments.FirstOrDefault(s => s.Id.Equals(OriginalSelectedModePayment.Id));
        }

        UpdateIsDirty();
    }

    private void ButtonDeleteBankTransfer_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonDeleteBankTransfer();

    private void ButtonUpdateBankTransfer_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonUpdateBankTransfer();

    private void EntryValue_OnTextChanged(object? sender, TextChangedEventArgs e)
        => UpdateIsDirty();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void OnBackCommandPressed()
        => _ = HandleBackCommand();

    private void PickerCategory_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateIsDirty();

    private void PickerFromAccount_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        var accountIdToRemove = BankTransfer.FromAccountFk;
        var currentAccountId = BankTransfer.ToAccountFk;

        // ReSharper disable once HeapView.DelegateAllocation
        PickerToAccountFk.SelectedIndexChanged -= PickerToAccount_OnSelectedIndexChanged;

        UpdateAccountsCollection(accountIdToRemove, ToAccounts);
        BankTransfer.ToAccountFk = currentAccountId;

        // ReSharper disable once HeapView.DelegateAllocation
        PickerToAccountFk.SelectedIndexChanged += PickerToAccount_OnSelectedIndexChanged;

        UpdateIsDirty();
        UpdateFromAccountSymbol();
    }

    private void PickerModePayment_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateIsDirty();

    private void PickerToAccount_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        var accountIdToRemove = BankTransfer.ToAccountFk;
        var currentAccountId = BankTransfer.FromAccountFk;

        // ReSharper disable once HeapView.DelegateAllocation
        PickerFromAccountFk.SelectedIndexChanged -= PickerFromAccount_OnSelectedIndexChanged;

        UpdateAccountsCollection(accountIdToRemove, FromAccounts);
        BankTransfer.FromAccountFk = currentAccountId;

        // ReSharper disable once HeapView.DelegateAllocation
        PickerFromAccountFk.SelectedIndexChanged += PickerFromAccount_OnSelectedIndexChanged;

        UpdateIsDirty();
    }

    #endregion

    #region Function

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

        BankTransfer.DateAdded = now;
        BankTransfer.THistories.Add(fromHistory);
        BankTransfer.THistories.Add(toHistory);
    }

    private THistory CreateHistory(int? accountFk, double value, DateTime now)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using an object initializer here simplifies the creation of the THistory instance,
        // ensuring that all necessary properties are set at once. Explicitly returning 'new THistory'
        // is required to construct and return a fresh, complete object with the desired values.
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

    private async Task HandleButtonDeleteBankTransfer()
    {
        var response = await DisplayAlert(
            AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferTitle,
            AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferMessage,
            AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferYesButton,
            AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferNoButton);
        if (!response) return;

        var json = BankTransfer.ToJson();
        Log.Information("Attempting to delete bank transfer, {Json}", json);

        var (success, exception) = BankTransfer.Delete(true);

        if (success)
        {
            Log.Information("Bank transfer successfully deleted");
            await DisplayAlert(
                AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferSuccessTitle,
                AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferSuccessMessage,
                AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferSuccessOkButton);

            _taskCompletionSource.SetResult(true);
            await Navigation.PopAsync();
        }
        else
        {
            Log.Error(exception, "Failed to delete bank transfer");
            await DisplayAlert(
                AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferErrorTitle,
                AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferErrorMessage,
                AddEditBankTransferContentPageResources.MessageBoxDeleteBankTransferErrorOkButton);
        }
    }

    private async Task HandleButtonUpdateBankTransfer()
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
        await Navigation.PopAsync();
    }

    private async Task HandleBackCommand()
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

    // ReSharper disable once HeapView.ClosureAllocation
    private void UpdateAccountsCollection(int? accountIdToRemove, ObservableCollection<TAccount> collection)
    {
        // ReSharper disable once HeapView.DelegateAllocation
        var newCollection = Accounts.Where(s => s.Id != accountIdToRemove);

        collection.Clear();
        collection.AddRange(newCollection);
    }

    private void UpdateFromAccountSymbol()
    {
        if (BankTransfer.FromAccountFk is null)
        {
            FromAccountSymbol = string.Empty;
            return;
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();

        // ReSharper disable once HeapView.DelegateAllocation
        // ReSharper disable once HeapView.ClosureAllocation
        var account = Accounts.First(a => a.Id.Equals(BankTransfer.FromAccountFk!.Value));

        // ReSharper disable once HeapView.ObjectAllocation
        FromAccountSymbol = context.TCurrencies.First(s => s.Id.Equals(account.CurrencyFk)).Symbol!;
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

    private void UpdateIsDirty()
    {
        var bankTransferIsDirty = !BankTransfer.AreEqual(OriginalBankTransfer);
        var categoryIsDirty = !SelectedCategoryType.AreEqual(OriginalSelectedCategoryType);
        var modePaymentIsDirty = !SelectedModePayment.AreEqual(OriginalSelectedModePayment);

        IsDirty = bankTransferIsDirty || categoryIsDirty || modePaymentIsDirty;

        if (IsNewBankTransfer) return;

        Title = IsDirty
            ? AddEditBankTransferContentPageResources.TitleIsDirty
            : string.Empty;
    }

    private void UpdateLanguage()
    {
        ButtonUpdateText = IsNewBankTransfer
            ? BankTransferManagementResources.ButtonUpdateText
            : BankTransferManagementResources.ButtonAddNewBankTransferText;

        ButtonCanBeDeletedText = BankTransferManagementResources.ButtonCanBeDeletedText;
        ButtonCancelUpdateText = BankTransferManagementResources.ButtonCancelUpdateText;

        LabelTextFromAccountFrom = BankTransferManagementResources.ComboBoxFromAccountHintAssist;
        LabelTextTransferDate = BankTransferManagementResources.LabelTextTransferDate;
        LabelTextTransferValue = BankTransferManagementResources.LabelTextTransferValue;
        LabelTextToAccountTo = BankTransferManagementResources.ComboBoxToAccountHintAssist;

        CustomEntryControlPlaceholderTextMainReason = BankTransferManagementResources.TextBoxMainReasonHintAssist;
        CustomEntryControlPlaceholderTextAdditionalReason = BankTransferManagementResources.TextBoxAdditionalReasonHintAssist;

        LabelTextTransferCategory = BankTransferManagementResources.ComboBoxCategoryTypeHintAssist;
        LabelTextTransferPaymentMode = BankTransferManagementResources.ComboBoxModePaymentHintAssist;
    }

    private void UpdateTransactionHistories(DateTime now)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        var fromHistory = context.THistories.First(s => s.BankTransferFk.Equals(BankTransfer.Id) && s.AccountFk.Equals(OriginalBankTransfer!.FromAccountFk));
        var toHistory = context.THistories.First(s => s.BankTransferFk.Equals(BankTransfer.Id) && s.AccountFk.Equals(OriginalBankTransfer!.ToAccountFk));

        UpdateHistory(fromHistory, BankTransfer.FromAccountFk, -Math.Abs(BankTransfer.Value ?? 0), now);
        UpdateHistory(toHistory, BankTransfer.ToAccountFk, Math.Abs(BankTransfer.Value ?? 0), now);

        BankTransfer.THistories.Clear();
        BankTransfer.THistories.Add(fromHistory);
        BankTransfer.THistories.Add(toHistory);
    }

    public void SetVBankTransferSummary(VBankTransferSummary? vBankTransferSummary)
    {
        if (vBankTransferSummary is null) return;

        // ReSharper disable once HeapView.ClosureAllocation
        var bankTransfer = vBankTransferSummary.Id.ToISql<TBankTransfer>()!;
        bankTransfer.CopyPropertiesTo(BankTransfer);
        OriginalBankTransfer = bankTransfer.DeepCopy();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        var history = context.THistories.First(s => s.BankTransferFk.Equals(bankTransfer.Id));
        var categoryTypeId = context.TCategoryTypes.First(s => s.Id.Equals(history.CategoryTypeFk)).Id;
        var modePaymentId = context.TModePayments.First(s => s.Id.Equals(history.ModePaymentFk)).Id;

        // ReSharper disable once HeapView.DelegateAllocation
        SelectedCategoryType = CategoryTypes.FirstOrDefault(s => s.Id.Equals(categoryTypeId));
        OriginalSelectedCategoryType = SelectedCategoryType.DeepCopy();

        // ReSharper disable once HeapView.DelegateAllocation
        SelectedModePayment = ModePayments.FirstOrDefault(s => s.Id.Equals(modePaymentId));
        OriginalSelectedModePayment = SelectedModePayment.DeepCopy();

        UpdateIsDirty();
        UpdateFromAccountSymbol();
    }

    private async Task<bool> ValidValidBankTransfer()
    {
        // The 'var' keyword is used for simplicity and clarity, as the types
        // (ValidationContext and List<ValidationResult>) are visible from the initialization.
        // ValidationContext is instantiated to provide context for validation, and
        // a new List<ValidationResult> is created to collect potential validation errors.
        // ReSharper disable HeapView.ObjectAllocation.Evident
        var validationContext = new ValidationContext(BankTransfer, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        // ReSharper restore HeapView.ObjectAllocation.Evident

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

    #endregion
}