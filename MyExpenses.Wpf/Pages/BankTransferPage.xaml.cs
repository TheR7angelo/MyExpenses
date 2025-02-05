using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.RegexUtils;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Resources.Resx.Pages.BankTransferPage;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace MyExpenses.Wpf.Pages;

public partial class BankTransferPage
{
    #region DependencyProperty

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty BankTransferPrepareProperty =
        DependencyProperty.Register(nameof(BankTransferPrepare), typeof(bool), typeof(BankTransferPage),
            new PropertyMetadata(false));

    public bool BankTransferPrepare
    {
        get => (bool)GetValue(BankTransferPrepareProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(BankTransferPrepareProperty, value);
    }

    public static readonly DependencyProperty VFromAccountProperty = DependencyProperty.Register(nameof(VFromAccount),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(VTotalByAccount), typeof(BankTransferPage), new PropertyMetadata(default(VTotalByAccount)));

    public VTotalByAccount? VFromAccount
    {
        get => (VTotalByAccount)GetValue(VFromAccountProperty);
        set => SetValue(VFromAccountProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty VFromAccountReduceProperty =
        DependencyProperty.Register(nameof(VFromAccountReduce), typeof(double), typeof(BankTransferPage),
            new PropertyMetadata(0d));

    public double? VFromAccountReduce
    {
        get => (double)GetValue(VFromAccountReduceProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(VFromAccountReduceProperty, value);
    }

    public static readonly DependencyProperty VToAccountProperty = DependencyProperty.Register(nameof(VToAccount),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(VTotalByAccount), typeof(BankTransferPage), new PropertyMetadata(default(VTotalByAccount)));

    public VTotalByAccount? VToAccount
    {
        get => (VTotalByAccount)GetValue(VToAccountProperty);
        set => SetValue(VToAccountProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty VToAccountIncreaseProperty =
        DependencyProperty.Register(nameof(VToAccountIncrease), typeof(double), typeof(BankTransferPage),
            new PropertyMetadata(0d));

    public double? VToAccountIncrease
    {
        get => (double)GetValue(VToAccountIncreaseProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(VToAccountIncreaseProperty, value);
    }

    public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(nameof(Category),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(TCategoryType), typeof(BankTransferPage), new PropertyMetadata(default(TCategoryType)));

    public TCategoryType? Category
    {
        get => (TCategoryType)GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }

    public static readonly DependencyProperty ModePaymentProperty = DependencyProperty.Register(nameof(ModePayment),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(TModePayment), typeof(BankTransferPage), new PropertyMetadata(default(TModePayment)));

    public TModePayment? ModePayment
    {
        get => (TModePayment)GetValue(ModePaymentProperty);
        set => SetValue(ModePaymentProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty IsPointedProperty = DependencyProperty.Register(nameof(IsPointed),
        typeof(bool), typeof(BankTransferPage), new PropertyMetadata(false));

    public bool IsPointed
    {
        get => (bool)GetValue(IsPointedProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(IsPointedProperty, value);
    }

    #endregion

    #region Property

    public List<TCategoryType> CategoryTypes { get; }
    public List<TModePayment> ModePayments { get; }

    private List<TAccount> Accounts { get; }

    public ObservableCollection<TAccount> FromAccounts { get; }
    public ObservableCollection<TAccount> ToAccounts { get; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // The allocation is necessary to initialize an instance of TBankTransfer for use within the current class.
    public TBankTransfer BankTransfer { get; } = new();
    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string DisplayMemberPathCategoryType { get; } = nameof(TCategoryType.Name);
    public string DisplayMemberPathModePayment { get; } = nameof(TModePayment.Name);
    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);

    public static readonly DependencyProperty ComboBoxFromAccountHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxFromAccountHintAssist), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxFromAccountHintAssist
    {
        get => (string)GetValue(ComboBoxFromAccountHintAssistProperty);
        set => SetValue(ComboBoxFromAccountHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxToAccountHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxToAccountHintAssist), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxToAccountHintAssist
    {
        get => (string)GetValue(ComboBoxToAccountHintAssistProperty);
        set => SetValue(ComboBoxToAccountHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxCategoryTypeHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxCategoryTypeHintAssist), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxCategoryTypeHintAssist
    {
        get => (string)GetValue(ComboBoxCategoryTypeHintAssistProperty);
        set => SetValue(ComboBoxCategoryTypeHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxModePaymentHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxModePaymentHintAssist), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ComboBoxModePaymentHintAssist
    {
        get => (string)GetValue(ComboBoxModePaymentHintAssistProperty);
        set => SetValue(ComboBoxModePaymentHintAssistProperty, value);
    }

    public static readonly DependencyProperty CheckBoxPointedHintAssistProperty =
        DependencyProperty.Register(nameof(CheckBoxPointedHintAssist), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string CheckBoxPointedHintAssist
    {
        get => (string)GetValue(CheckBoxPointedHintAssistProperty);
        set => SetValue(CheckBoxPointedHintAssistProperty, value);
    }

    public static readonly DependencyProperty DatePickerWhenHintAssistProperty =
        DependencyProperty.Register(nameof(DatePickerWhenHintAssist), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string DatePickerWhenHintAssist
    {
        get => (string)GetValue(DatePickerWhenHintAssistProperty);
        set => SetValue(DatePickerWhenHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxValueHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxValueHintAssist), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string TextBoxValueHintAssist
    {
        get => (string)GetValue(TextBoxValueHintAssistProperty);
        set => SetValue(TextBoxValueHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxMainReasonHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxMainReasonHintAssist), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string TextBoxMainReasonHintAssist
    {
        get => (string)GetValue(TextBoxMainReasonHintAssistProperty);
        set => SetValue(TextBoxMainReasonHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxAdditionalReasonHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxAdditionalReasonHintAssist), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string TextBoxAdditionalReasonHintAssist
    {
        get => (string)GetValue(TextBoxAdditionalReasonHintAssistProperty);
        set => SetValue(TextBoxAdditionalReasonHintAssistProperty, value);
    }

    public static readonly DependencyProperty ButtonPrepareValidContentProperty =
        DependencyProperty.Register(nameof(ButtonPrepareValidContent), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonPrepareValidContent
    {
        get => (string)GetValue(ButtonPrepareValidContentProperty);
        set => SetValue(ButtonPrepareValidContentProperty, value);
    }

    public static readonly DependencyProperty ButtonPrepareCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonPrepareCancelContent), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonPrepareCancelContent
    {
        get => (string)GetValue(ButtonPrepareCancelContentProperty);
        set => SetValue(ButtonPrepareCancelContentProperty, value);
    }

    public static readonly DependencyProperty ButtonPreviewValidContentProperty =
        DependencyProperty.Register(nameof(ButtonPreviewValidContent), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonPreviewValidContent
    {
        get => (string)GetValue(ButtonPreviewValidContentProperty);
        set => SetValue(ButtonPreviewValidContentProperty, value);
    }

    public static readonly DependencyProperty ButtonPreviewCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonPreviewCancelContent), typeof(string), typeof(BankTransferPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string ButtonPreviewCancelContent
    {
        get => (string)GetValue(ButtonPreviewCancelContentProperty);
        set => SetValue(ButtonPreviewCancelContentProperty, value);
    }

    #endregion

    public BankTransferPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();

        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];

        // ReSharper disable HeapView.ObjectAllocation.Evident
        // The use of ObservableCollection<T> is mandatory here to ensure that any changes
        // (additions, removals, or updates) made to the collection are automatically reflected
        // in the UI. ObservableCollection implements the INotifyCollectionChanged interface,
        // which notifies WPF binding mechanisms of such changes, allowing the UI to remain up-to-date
        // without manual intervention. This is essential for maintaining synchronization
        // between the data and the interface in real-time.
        FromAccounts = new ObservableCollection<TAccount>(Accounts);
        ToAccounts = new ObservableCollection<TAccount>(Accounts);
        // ReSharper restore HeapView.ObjectAllocation.Evident

        UpdateLanguage();
        InitializeComponent();

        UpdateLanguageDatePicker();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonCancelBankTransferPrepare_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).GoBack();

    private void ButtonValidBankTransferPrepare_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The serviceProvider and items are set to null because they are not required in this context.
        // The ValidationResults list will store any validation errors detected during the process.
        var validationContext = new ValidationContext(BankTransfer, serviceProvider: null, items: null);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using 'var' keeps the code concise and readable, as the type (List<ValidationResult>)
        // is evident from the initialization. The result will still be compatible with any method
        // that expects an ICollection<ValidationResult>, as List<T> implements the ICollection interface.
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(BankTransfer, validationContext, validationResults, true);

        if (!isValid)
        {
            var propertyError = validationResults.First();
            var propertyMemberName = propertyError.MemberNames.First();

            var messageErrorKey = propertyMemberName switch
            {
                nameof(TBankTransfer.FromAccountFk) => nameof(BankTransferPageResources
                    .MessageBoxButtonValidationFromAccountFkError),
                nameof(TBankTransfer.ToAccountFk) => nameof(BankTransferPageResources
                    .MessageBoxButtonValidationToAccountFkError),
                nameof(TBankTransfer.Value) => nameof(BankTransferPageResources.MessageBoxButtonValidationValueError),
                nameof(TBankTransfer.Date) => nameof(BankTransferPageResources.MessageBoxButtonValidationDateError),
                nameof(TBankTransfer.MainReason) => nameof(BankTransferPageResources
                    .MessageBoxButtonValidationMainReasonError),
                _ => null
            };

            var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
                ? propertyError.ErrorMessage!
                : BankTransferPageResources.ResourceManager.GetString(messageErrorKey)!;

            MsgBox.Show(localizedErrorMessage, MsgBoxImage.Error);
            return;
        }

        if (Category is null)
        {
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPrepareCategoryIsNullError,
                MsgBoxImage.Warning);
            return;
        }

        if (ModePayment is null)
        {
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPrepareModePaymentIsNullError,
                MsgBoxImage.Warning);
            return;
        }

        BankTransferPrepare = true;
    }

    private void ButtonValidBankTransferPreview_OnClick(object sender, RoutedEventArgs e)
    {
        var now = DateTime.Now;
        var valueAbs = Math.Abs(BankTransfer.Value ?? 0);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The THistories collection is created here to store the THistory instances for the transfer.
        var fromHistory = new THistory
        {
            AccountFk = BankTransfer.FromAccountFk,
            Description = BankTransfer.MainReason,
            CategoryTypeFk = Category?.Id,
            ModePaymentFk = ModePayment?.Id,
            Value = -valueAbs,
            Date = BankTransfer.Date,
            IsPointed = true,
            PlaceFk = 1,
            DateAdded = now,
            DatePointed = now
        };

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The THistories collection is created here to store the THistory instances for the transfer.
        var toHistory = new THistory
        {
            AccountFk = BankTransfer.ToAccountFk,
            Description = BankTransfer.MainReason,
            CategoryTypeFk = Category?.Id,
            ModePaymentFk = ModePayment?.Id,
            Value = valueAbs,
            Date = BankTransfer.Date,
            IsPointed = true,
            PlaceFk = 1,
            DateAdded = now,
            DatePointed = now
        };

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Ensuring that the THistories collection is initialized with an empty List<THistory>
        // if it is null. This prevents potential NullReferenceExceptions when attempting
        // to add items to the collection. It ensures that the property is always in a valid
        // state and ready for use regardless of its initial state.
        BankTransfer.THistories ??= new List<THistory>();
        BankTransfer.THistories.Add(fromHistory);
        BankTransfer.THistories.Add(toHistory);

        var (success, exception) = BankTransfer.AddOrEdit();
        if (success)
        {
            Log.Information(
                "The transfer has been successfully completed, {FromName} to {ToName} with value {ValueAbs}",
                VFromAccount!.Name, VToAccount!.Name, valueAbs);

            // Loop crash
            // var json = BankTransfer.ToJsonString();
            // Log.Information("{Json}", json);

            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPreviewSuccess, MsgBoxImage.Check);

            var response = MsgBox.Show(
                BankTransferPageResources.MessageBoxButtonValidBankTransferPreviewNewTransferQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNo);

            if (response is not MessageBoxResult.Yes) nameof(MainWindow.FrameBody).GoBack();
            else
            {
                BankTransfer.Reset();
                BankTransferPrepare = false;
            }
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPreviewError, MsgBoxImage.Error);
        }
    }

    private void ButtonCancelBankTransferPreview_OnClick(object sender, RoutedEventArgs e)
        => BankTransferPrepare = false;

    private void ButtonFromAddAccount_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The AddEditAccountWindow instance is created here to handle user interactions for editing or adding an account.
        // The FromAccountFk, if not null, is converted to the appropriate type and passed to the dialog for initialization.
        // ShowDialog() is used to display the window modally, pausing execution until the user closes it.
        // The DialogResult is checked to ensure the user confirmed their actions before proceeding with further logic.
        var addEditAccountWindow = new AddEditAccountWindow();
        var fromAccount = BankTransfer.FromAccountFk?.ToISql<TAccount>();
        if (fromAccount is not null) addEditAccountWindow.SetTAccount(fromAccount);

        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult is not true) return;

        if (addEditAccountWindow.DeleteAccount)
        {
            RemoveByAccountId(BankTransfer.FromAccountFk);
            return;
        }

        var editedAccount = addEditAccountWindow.Account;

        Log.Information("Attempting to edit the account \"{AccountName}\"", editedAccount.Name);
        var (success, exception) = editedAccount.AddOrEdit();
        if (success)
        {
            Log.Information("Account was successfully edited");
            var json = editedAccount.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(BankTransferPageResources.MessageBoxEditAccountSuccess, MsgBoxImage.Check);

            RemoveByAccountId(editedAccount.Id);
            Accounts.AddAndSort(editedAccount, s => s.Name!);
            FromAccounts.AddAndSort(editedAccount, s => s.Name!);
            BankTransfer.FromAccountFk = editedAccount.Id;
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(BankTransferPageResources.MessageBoxEditAccountError, MsgBoxImage.Warning);
        }
    }

    private void ButtonToAddAccount_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The AddEditAccountWindow instance is created here to allow the user to view or edit an account.
        // The ToAccountFk, if not null, is converted to the expected type and passed to the dialog for pre-populating the fields.
        // ShowDialog() displays the window modally, ensuring the current execution flow is paused until the dialog is closed.
        // The DialogResult is verified to check if the user confirmed the operation before continuing.
        var addEditAccountWindow = new AddEditAccountWindow();
        var fromAccount = BankTransfer.ToAccountFk?.ToISql<TAccount>();
        if (fromAccount is not null) addEditAccountWindow.SetTAccount(fromAccount);

        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult is not true) return;

        if (addEditAccountWindow.DeleteAccount)
        {
            RemoveByAccountId(BankTransfer.ToAccountFk);
            return;
        }

        var editedAccount = addEditAccountWindow.Account;

        Log.Information("Attempting to edit the account \"{AccountName}\"", editedAccount.Name);
        var (success, exception) = editedAccount.AddOrEdit();
        if (success)
        {
            Log.Information("Account was successfully edited");
            var json = editedAccount.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(BankTransferPageResources.MessageBoxEditAccountSuccess, MsgBoxImage.Check);

            RemoveByAccountId(editedAccount.Id);
            Accounts.AddAndSort(editedAccount, s => s.Name!);
            ToAccounts.AddAndSort(editedAccount, s => s.Name!);

            BankTransfer.ToAccountFk = editedAccount.Id;
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(BankTransferPageResources.MessageBoxEditAccountError, MsgBoxImage.Warning);
        }
    }

    private void Interface_OnLanguageChanged()
    {
        UpdateLanguage();
        UpdateLanguageDatePicker();
    }

    private void SelectorFromAccount_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var fromAccount = Accounts.FirstOrDefault(s => s.Id == BankTransfer.FromAccountFk);
        VFromAccount = fromAccount?.Id.ToISql<VTotalByAccount>();

        RefreshListToAccount();
        RefreshVFromAccountReduce();
    }

    private void SelectorToAccount_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var toAccount = Accounts.FirstOrDefault(s => s.Id == BankTransfer.ToAccountFk);
        VToAccount = toAccount?.Id.ToISql<VTotalByAccount>();

        RefreshListFromAccount();
        RefreshVToAccountIncrease();
    }

    private void TextBoxValue_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text;

        if (double.TryParse(txt, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            BankTransfer.Value = value;
        else if (!txt.EndsWith('.')) BankTransfer.Value = null;

        RefreshVFromAccountReduce();
        RefreshVToAccountIncrease();
    }

    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        e.Handled = txt.IsOnlyDecimal();
    }

    #endregion

    #region Function

    private IEnumerable<TAccount> GetAccountToAdd(IEnumerable<TAccount> accounts)
        // ReSharper disable HeapView.DelegateAllocation
        => Accounts.Where(account => accounts.All(s => s.Id != account.Id));
    // ReSharper restore HeapView.DelegateAllocation

    private void UpdateLanguage()
    {
        ComboBoxFromAccountHintAssist = BankTransferPageResources.ComboBoxFromAccountHintAssist;
        ComboBoxToAccountHintAssist = BankTransferPageResources.ComboBoxToAccountHintAssist;
        ComboBoxCategoryTypeHintAssist = BankTransferPageResources.ComboBoxCategoryTypeHintAssist;
        ComboBoxModePaymentHintAssist = BankTransferPageResources.ComboBoxModePaymentHintAssist;
        CheckBoxPointedHintAssist = BankTransferPageResources.CheckBoxPointedHintAssist;
        DatePickerWhenHintAssist = BankTransferPageResources.DatePickerWhenHintAssist;
        TextBoxValueHintAssist = BankTransferPageResources.TextBoxValueHintAssist;
        TextBoxMainReasonHintAssist = BankTransferPageResources.TextBoxMainReasonHintAssist;
        TextBoxAdditionalReasonHintAssist = BankTransferPageResources.TextBoxAdditionalReasonHintAssist;
        ButtonPrepareValidContent = BankTransferPageResources.ButtonPrepareValidContent;
        ButtonPrepareCancelContent = BankTransferPageResources.ButtonPrepareCancelContent;
        ButtonPreviewValidContent = BankTransferPageResources.ButtonPreviewValidContent;
        ButtonPreviewCancelContent = BankTransferPageResources.ButtonPreviewCancelContent;
    }

    private void UpdateLanguageDatePicker()
        => DatePicker.Language = System.Windows.Markup.XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);


    private void RefreshListFromAccount()
    {
        var accountToAdd = GetAccountToAdd(FromAccounts);

        FromAccounts.AddRangeAndSort(accountToAdd, s => s.Name!);

        var accountToRemove = FromAccounts.Where(s => s.Id == VToAccount?.Id).ToList();
        FromAccounts.RemoveRange(accountToRemove);
    }

    private void RefreshListToAccount()
    {
        var accountToAdd = GetAccountToAdd(ToAccounts);

        ToAccounts.AddRangeAndSort(accountToAdd, s => s.Name!);

        var accountToRemove = ToAccounts.Where(s => s.Id == VFromAccount?.Id).ToList();
        ToAccounts.RemoveRange(accountToRemove);
    }

    private void RefreshVFromAccountReduce()
    {
        VFromAccountReduce = VFromAccount is not null && BankTransfer.Value is not null
            ? Math.Round((VFromAccount.Total ?? 0) - Math.Abs(BankTransfer.Value ?? 0), 2)
            : 0;
    }

    private void RefreshVToAccountIncrease()
    {
        VToAccountIncrease = VToAccount is not null && BankTransfer.Value is not null
            ? Math.Round((VToAccount.Total ?? 0) + Math.Abs(BankTransfer.Value ?? 0), 2)
            : 0;
    }

    private void RemoveByAccountId(int? accountId)
    {
        var accountToRemove = Accounts.FirstOrDefault(s => s.Id == accountId);
        if (accountToRemove is not null) Accounts.Remove(accountToRemove);

        accountToRemove = FromAccounts.FirstOrDefault(s => s.Id == accountId);
        if (accountToRemove is not null) FromAccounts.Remove(accountToRemove);

        accountToRemove = ToAccounts.FirstOrDefault(s => s.Id == accountId);
        if (accountToRemove is not null) ToAccounts.Remove(accountToRemove);
    }

    #endregion
}