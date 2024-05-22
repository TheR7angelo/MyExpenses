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

namespace MyExpenses.Wpf.Pages;

public partial class BankTransferPage
{
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

    private List<TAccount> Accounts { get; }

    public ObservableCollection<TAccount> FromAccounts { get; }
    public ObservableCollection<TAccount> ToAccounts { get; }

    public TBankTransfer BankTransfer { get; } = new();
    public string DisplayMemberPathAccount { get; } = nameof(TAccount.Name);
    public string SelectedValuePathAccount { get; } = nameof(TAccount.Id);

    public required DashBoardPage DashBoardPage { get; set; }

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

    //TODO work
    public BankTransferPage()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];
        FromAccounts = new ObservableCollection<TAccount>(Accounts);
        ToAccounts = new ObservableCollection<TAccount>(Accounts);

        InitializeComponent();

        DatePicker.Language = System.Windows.Markup.XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
    }

    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var txt = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        e.Handled = txt.IsOnlyDecimal();
    }

    private void SelectorFromAccount_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var fromAccount = Accounts.FirstOrDefault(s => s.Id == BankTransfer.FromAccountFk);
        VFromAccount = fromAccount?.ToVTotalByAccount();

        RefreshListToAccount();
        RefreshVFromAccountReduce();
    }

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

    private void SelectorToAccount_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var toAccount = Accounts.FirstOrDefault(s => s.Id == BankTransfer.ToAccountFk);
        VToAccount = toAccount?.ToVTotalByAccount();

        RefreshListFromAccount();
        RefreshVToAccountIncrease();
    }

    private void RefreshVToAccountIncrease()
    {
        VToAccountIncrease = VToAccount is not null && BankTransfer.Value is not null
            ? VToAccount?.Total + Math.Abs((double)BankTransfer.Value)
            : 0;
    }

    private void ButtonValidBankTransferPrepare_OnClick(object sender, RoutedEventArgs e)
        => BankTransferPrepare = true;

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
}