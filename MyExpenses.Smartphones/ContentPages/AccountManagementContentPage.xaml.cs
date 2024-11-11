using System.Collections.ObjectModel;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AccountManagementContentPage;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AccountManagementContentPage
{
    public static readonly BindableProperty LabelTextTransactionTransferProperty =
        BindableProperty.Create(nameof(LabelTextTransactionTransfer), typeof(string),
            typeof(AccountManagementContentPage), default(string));

    public string LabelTextTransactionTransfer
    {
        get => (string)GetValue(LabelTextTransactionTransferProperty);
        set => SetValue(LabelTextTransactionTransferProperty, value);
    }

    public static readonly BindableProperty LabelTextTransactionHistoryProperty =
        BindableProperty.Create(nameof(LabelTextTransactionHistory), typeof(string),
            typeof(AccountManagementContentPage), default(string));

    public string LabelTextTransactionHistory
    {
        get => (string)GetValue(LabelTextTransactionHistoryProperty);
        set => SetValue(LabelTextTransactionHistoryProperty, value);
    }

    public static readonly BindableProperty TotalAllAccountProperty = BindableProperty.Create(nameof(TotalAllAccount),
        typeof(double), typeof(AccountManagementContentPage), default(double));

    public double TotalAllAccount
    {
        get => (double)GetValue(TotalAllAccountProperty);
        set => SetValue(TotalAllAccountProperty, value);
    }

    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; }

    public AccountManagementContentPage()
    {
        using var context = new DataBaseContext();
        VTotalByAccounts = [..context.VTotalByAccounts];
        TotalAllAccount = VTotalByAccounts.Sum(s => s.Total) ?? 0d;

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        LabelTextTransactionHistory = AccountManagementContentPageResources.LabelTextTransactionHistory;
        LabelTextTransactionTransfer = AccountManagementContentPageResources.LabelTextTransactionTransfer;
    }
}