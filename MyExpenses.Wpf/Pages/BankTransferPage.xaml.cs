using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Resources.Regex;
using MyExpenses.Wpf.Resources.Resx.Pages.BankTransferPage;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class BankTransferPage
{
    #region DependencyProperty

    public static readonly DependencyProperty BankTransferPrepareProperty =
        DependencyProperty.Register(nameof(BankTransferPrepare), typeof(bool), typeof(BankTransferPage),
            new PropertyMetadata(default(bool)));

    public bool BankTransferPrepare
    {
        get => (bool)GetValue(BankTransferPrepareProperty);
        set => SetValue(BankTransferPrepareProperty, value);
    }

    public static readonly DependencyProperty VFromAccountProperty = DependencyProperty.Register(nameof(VFromAccount),
        typeof(VTotalByAccount), typeof(BankTransferPage), new PropertyMetadata(default(VTotalByAccount)));

    public VTotalByAccount? VFromAccount
    {
        get => (VTotalByAccount)GetValue(VFromAccountProperty);
        set => SetValue(VFromAccountProperty, value);
    }

    public static readonly DependencyProperty VFromAccountReduceProperty =
        DependencyProperty.Register(nameof(VFromAccountReduce), typeof(double), typeof(BankTransferPage),
            new PropertyMetadata(default(double)));

    public double? VFromAccountReduce
    {
        get => (double)GetValue(VFromAccountReduceProperty);
        set => SetValue(VFromAccountReduceProperty, value);
    }

    public static readonly DependencyProperty VToAccountProperty = DependencyProperty.Register(nameof(VToAccount),
        typeof(VTotalByAccount), typeof(BankTransferPage), new PropertyMetadata(default(VTotalByAccount)));

    public VTotalByAccount? VToAccount
    {
        get => (VTotalByAccount)GetValue(VToAccountProperty);
        set => SetValue(VToAccountProperty, value);
    }

    public static readonly DependencyProperty VToAccountIncreaseProperty =
        DependencyProperty.Register(nameof(VToAccountIncrease), typeof(double), typeof(BankTransferPage),
            new PropertyMetadata(default(double)));

    public double? VToAccountIncrease
    {
        get => (double)GetValue(VToAccountIncreaseProperty);
        set => SetValue(VToAccountIncreaseProperty, value);
    }

    public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(nameof(Category),
        typeof(TCategoryType), typeof(BankTransferPage), new PropertyMetadata(default(TCategoryType)));

    public TCategoryType? Category
    {
        get => (TCategoryType)GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }

    public static readonly DependencyProperty ModePaymentProperty = DependencyProperty.Register(nameof(ModePayment),
        typeof(TModePayment), typeof(BankTransferPage), new PropertyMetadata(default(TModePayment)));

    public TModePayment? ModePayment
    {
        get => (TModePayment)GetValue(ModePaymentProperty);
        set => SetValue(ModePaymentProperty, value);
    }

    public static readonly DependencyProperty IsPointedProperty = DependencyProperty.Register(nameof(IsPointed),
        typeof(bool), typeof(BankTransferPage), new PropertyMetadata(default(bool)));

    public bool IsPointed
    {
        get => (bool)GetValue(IsPointedProperty);
        set => SetValue(IsPointedProperty, value);
    }

    #endregion

    #region Property

    public List<TCategoryType> CategoryTypes { get; }
    public List<TModePayment> ModePayments { get; }

    private List<TAccount> Accounts { get; }

    public ObservableCollection<TAccount> FromAccounts { get; }
    public ObservableCollection<TAccount> ToAccounts { get; }

    public TBankTransfer BankTransfer { get; } = new();
    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string DisplayMemberPathCategoryType { get; } = nameof(TCategoryType.Name);
    public string DisplayMemberPathModePayment { get; } = nameof(TModePayment.Name);
    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);

    public string ComboBoxFromAccountHintAssist { get; } = BankTransferPageResources.ComboBoxFromAccountHintAssist;
    public string ComboBoxToAccountHintAssist { get; } = BankTransferPageResources.ComboBoxToAccountHintAssist;
    public string ComboBoxCategoryTypeHintAssist { get; } = BankTransferPageResources.ComboBoxCategoryTypeHintAssist;
    public string ComboBoxModePaymentHintAssist { get; } = BankTransferPageResources.ComboBoxModePaymentHintAssist;
    public string CheckBoxPointedHintAssist { get; } = BankTransferPageResources.CheckBoxPointedHintAssist;
    public string DatePickerWhenHintAssist { get; } = BankTransferPageResources.DatePickerWhenHintAssist;
    public string TextBoxValueHintAssist { get; } = BankTransferPageResources.TextBoxValueHintAssist;
    public string TextBoxMainReasonHintAssist { get; } = BankTransferPageResources.TextBoxMainReasonHintAssist;
    public string TextBoxAdditionalReasonHintAssist { get; } = BankTransferPageResources.TextBoxAdditionalReasonHintAssist;
    public string ButtonPrepareValidContent { get; } = BankTransferPageResources.ButtonPrepareValidContent;
    public string ButtonPrepareCancelContent { get; } = BankTransferPageResources.ButtonPrepareCancelContent;
    public string ButtonPreviewValidContent { get; } = BankTransferPageResources.ButtonPreviewValidContent;
    public string ButtonPreviewCancelContent { get; } = BankTransferPageResources.ButtonPreviewCancelContent;

    #endregion

    #region Required Property

    public required DashBoardPage DashBoardPage { get; init; }

    #endregion

    public BankTransferPage()
    {
        using var context = new DataBaseContext();
        CategoryTypes = [..context.TCategoryTypes.OrderBy(s => s.Name)];
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        FromAccounts = new ObservableCollection<TAccount>(Accounts);
        ToAccounts = new ObservableCollection<TAccount>(Accounts);

        InitializeComponent();

        DatePicker.Language = System.Windows.Markup.XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
    }

    #region Action

    private void ButtonCancelBankTransferPrepare_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).GoBack();

    private void ButtonValidBankTransferPrepare_OnClick(object sender, RoutedEventArgs e)
    {
        if (BankTransfer.FromAccountFk is null)
        {
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPrepareFromAccountFkIsNullError, MsgBoxImage.Warning);
            return;
        }

        if (BankTransfer.ToAccountFk is null)
        {
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPrepareToAccountFkIsNullError, MsgBoxImage.Warning);
            return;
        }

        if (Category is null)
        {
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPrepareCategoryIsNullError, MsgBoxImage.Warning);
            return;
        }

        if (ModePayment is null)
        {
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPrepareModePaymentIsNullError, MsgBoxImage.Warning);
            return;
        }

        if (BankTransfer.Value is null)
        {
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPrepareValueIsNullError, MsgBoxImage.Warning);
            return;
        }

        if (BankTransfer.Date is null)
        {
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPrepareDateIsNullError, MsgBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(BankTransfer.MainReason))
        {
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPrepareMainReasonIsNullError, MsgBoxImage.Warning);
            return;
        }

        BankTransferPrepare = true;
    }

    private void ButtonValidBankTransferPreview_OnClick(object sender, RoutedEventArgs e)
    {
        var valueAbs = Math.Abs(BankTransfer.Value ?? 0);
        var fromHistory = new THistory
        {
            AccountFk = BankTransfer.FromAccountFk,
            Description = BankTransfer.MainReason,
            CategoryTypeFk = Category?.Id,
            ModePaymentFk = ModePayment?.Id,
            Value = -valueAbs,
            Date = BankTransfer.Date,
            Pointed = IsPointed
        };

        var toHistory = new THistory
        {
            AccountFk = BankTransfer.ToAccountFk,
            Description = BankTransfer.MainReason,
            CategoryTypeFk = Category?.Id,
            ModePaymentFk = ModePayment?.Id,
            Value = valueAbs,
            Date = BankTransfer.Date,
            Pointed = IsPointed
        };

        BankTransfer.THistories.Add(fromHistory);
        BankTransfer.THistories.Add(toHistory);

        var (success, exception) = BankTransfer.AddOrEdit();
        if (success)
        {
            Log.Information("The transfer has been successfully completed, {FromName} to {ToName} with value {ValueAbs}",
                VFromAccount!.Name, VToAccount!.Name, valueAbs);
            MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPreviewSuccess, MsgBoxImage.Check);

            DashBoardPage.RefreshAccountTotal();
            DashBoardPage.RefreshRadioButtonSelected();

            var response = MsgBox.Show(BankTransferPageResources.MessageBoxButtonValidBankTransferPreviewNewTransferQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNo);

            if (response != MessageBoxResult.Yes) nameof(MainWindow.FrameBody).GoBack();
            else
            {
                var bankTransfer = new TBankTransfer();
                bankTransfer.CopyPropertiesTo(BankTransfer);
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
        var addEditAccountWindow = new AddEditAccountWindow();
        var fromAccount = BankTransfer.FromAccountFk?.ToTAccount();
        if (fromAccount != null) addEditAccountWindow.SetTAccount(fromAccount);

        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult != true) return;

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
        var addEditAccountWindow = new AddEditAccountWindow();
        var fromAccount = BankTransfer.ToAccountFk?.ToTAccount();
        if (fromAccount != null) addEditAccountWindow.SetTAccount(fromAccount);

        addEditAccountWindow.ShowDialog();
        if (addEditAccountWindow.DialogResult != true) return;

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

    private void SelectorFromAccount_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var fromAccount = Accounts.FirstOrDefault(s => s.Id == BankTransfer.FromAccountFk);
        VFromAccount = fromAccount?.ToVTotalByAccount();

        RefreshListToAccount();
        RefreshVFromAccountReduce();
    }

    private void SelectorToAccount_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var toAccount = Accounts.FirstOrDefault(s => s.Id == BankTransfer.ToAccountFk);
        VToAccount = toAccount?.ToVTotalByAccount();

        RefreshListFromAccount();
        RefreshVToAccountIncrease();
    }

    private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
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

    private void RefreshListFromAccount()
    {
        var accountToAdd = from account in Accounts
            let a = FromAccounts.FirstOrDefault(s => s.Id == account.Id)
            where a is null
            select account;

        FromAccounts.AddRangeAndSort(accountToAdd, s => s.Name!);

        var accountToRemove = FromAccounts.Where(s => s.Id == VToAccount?.Id).ToList();
        FromAccounts.RemoveRange(accountToRemove);
    }

    private void RefreshListToAccount()
    {
        var accountToAdd = from account in Accounts
            let a = ToAccounts.FirstOrDefault(s => s.Id == account.Id)
            where a is null
            select account;

        ToAccounts.AddRangeAndSort(accountToAdd, s => s.Name!);

        var accountToRemove = ToAccounts.Where(s => s.Id == VFromAccount?.Id).ToList();
        ToAccounts.RemoveRange(accountToRemove);
    }

    private void RefreshVFromAccountReduce()
    {
        VFromAccountReduce = VFromAccount is not null && BankTransfer.Value is not null
            ? VFromAccount.Total - Math.Abs((double)BankTransfer.Value)
            : 0;
    }

    private void RefreshVToAccountIncrease()
    {
        VToAccountIncrease = VToAccount is not null && BankTransfer.Value is not null
            ? VToAccount?.Total + Math.Abs((double)BankTransfer.Value)
            : 0;
    }

    private void RemoveByAccountId(int? accountId)
    {
        var vTotalByAccount = accountId?.ToVTotalByAccount();
        if (vTotalByAccount is not null) DashBoardPage.VTotalByAccounts.Remove(vTotalByAccount);

        var accountToRemove = Accounts.FirstOrDefault(s => s.Id == BankTransfer.FromAccountFk);
        if (accountToRemove is not null) Accounts.Remove(accountToRemove);

        accountToRemove = ToAccounts.FirstOrDefault(s => s.Id == BankTransfer.FromAccountFk);
        if (accountToRemove is not null) ToAccounts.Remove(accountToRemove);

        accountToRemove = FromAccounts.FirstOrDefault(s => s.Id == BankTransfer.FromAccountFk);
        if (accountToRemove is not null) FromAccounts.Remove(accountToRemove);

        DashBoardPage.RefreshRadioButtonSelected();
        DashBoardPage.RefreshAccountTotal();
    }

    #endregion
}