using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Resources.Regex;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Pages;

public partial class BankTransferPage
{
    #region DependencyProperty

    public static readonly DependencyProperty BankTransferPrepareProperty =
        DependencyProperty.Register(nameof(BankTransferPrepare), typeof(bool), typeof(BankTransferPage),
            new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty VFromAccountProperty = DependencyProperty.Register(nameof(VFromAccount),
        typeof(VTotalByAccount), typeof(BankTransferPage), new PropertyMetadata(default(VTotalByAccount)));

    public static readonly DependencyProperty VFromAccountReduceProperty =
        DependencyProperty.Register(nameof(VFromAccountReduce), typeof(double), typeof(BankTransferPage),
            new PropertyMetadata(default(double)));

    public static readonly DependencyProperty VToAccountProperty = DependencyProperty.Register(nameof(VToAccount),
        typeof(VTotalByAccount), typeof(BankTransferPage), new PropertyMetadata(default(VTotalByAccount)));

    public static readonly DependencyProperty VToAccountIncreaseProperty =
        DependencyProperty.Register(nameof(VToAccountIncrease), typeof(double), typeof(BankTransferPage),
            new PropertyMetadata(default(double)));

    public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(nameof(Category),
        typeof(TCategoryType), typeof(BankTransferPage), new PropertyMetadata(default(TCategoryType)));

    public static readonly DependencyProperty ModePaymentProperty = DependencyProperty.Register(nameof(ModePayment),
        typeof(TModePayment), typeof(BankTransferPage), new PropertyMetadata(default(TModePayment)));

    public static readonly DependencyProperty IsPointedProperty = DependencyProperty.Register(nameof(IsPointed),
        typeof(bool), typeof(BankTransferPage), new PropertyMetadata(default(bool)));

    public bool BankTransferPrepare
    {
        get => (bool)GetValue(BankTransferPrepareProperty);
        set => SetValue(BankTransferPrepareProperty, value);
    }

    public VTotalByAccount? VFromAccount
    {
        get => (VTotalByAccount)GetValue(VFromAccountProperty);
        set => SetValue(VFromAccountProperty, value);
    }

    public double? VFromAccountReduce
    {
        get => (double)GetValue(VFromAccountReduceProperty);
        set => SetValue(VFromAccountReduceProperty, value);
    }

    public VTotalByAccount? VToAccount
    {
        get => (VTotalByAccount)GetValue(VToAccountProperty);
        set => SetValue(VToAccountProperty, value);
    }

    public double? VToAccountIncrease
    {
        get => (double)GetValue(VToAccountIncreaseProperty);
        set => SetValue(VToAccountIncreaseProperty, value);
    }

    public TCategoryType? Category
    {
        get => (TCategoryType)GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }

    public TModePayment? ModePayment
    {
        get => (TModePayment)GetValue(ModePaymentProperty);
        set => SetValue(ModePaymentProperty, value);
    }

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

    #endregion

    #region Required Property

    public required DashBoardPage DashBoardPage { get; set; }

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

    //TODO work
    private void ButtonValidBankTransferPrepare_OnClick(object sender, RoutedEventArgs e)
    {
        if (BankTransfer.FromAccountFk is null)
        {
            MsgBox.Show("From account cannot be empty", MsgBoxImage.Warning);
            return;
        }

        if (BankTransfer.ToAccountFk is null)
        {
            MsgBox.Show("To account cannot be empty", MsgBoxImage.Warning);
            return;
        }

        if (Category is null)
        {
            MsgBox.Show("Category cannot be empty", MsgBoxImage.Warning);
            return;
        }

        if (ModePayment is null)
        {
            MsgBox.Show("Mode payment cannot be empty", MsgBoxImage.Warning);
            return;
        }

        if (BankTransfer.Value is null)
        {
            MsgBox.Show("Value cannot be empty", MsgBoxImage.Warning);
            return;
        }

        if (BankTransfer.Date is null)
        {
            MsgBox.Show("Date cannot be empty", MsgBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(BankTransfer.MainReason))
        {
            MsgBox.Show("Main reason cannot be empty", MsgBoxImage.Warning);
            return;
        }

        BankTransferPrepare = true;
    }

    //TODO work
    private void ButtonValidBankTransferPreview_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("ButtonValidBankTransferPreview_OnClick");

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
    }

    //TODO work
    private void ButtonCancelBankTransferPreview_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("ButtonCancelBankTransferPreview_OnClick");
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

    #endregion
}