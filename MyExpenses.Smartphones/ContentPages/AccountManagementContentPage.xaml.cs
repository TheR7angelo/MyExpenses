using System.Collections.ObjectModel;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AccountManagementContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;

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

    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; } = [];

    public AccountManagementContentPage()
    {
        RefreshAccountTotals();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private async void ButtonImageViewAddAccount_OnClicked(object? sender, EventArgs e)
    {
        var addEditAccountContentPage = new AddEditAccountContentPage();
        await Navigation.PushAsync(addEditAccountContentPage);

        var result = await addEditAccountContentPage.ResultDialog;
        if (result is not true) return;

        RefreshAccountTotals();
        DashBoardContentPage.Instance.RefreshAccountTotal();
    }

    private async void ButtonImageViewCreatBankTransfer_OnClicked(object? sender, EventArgs e)
    {
        var addEditBankTransferContentPage = new AddEditBankTransferContentPage { IsNewBankTransfer = true };
        await Navigation.PushAsync(addEditBankTransferContentPage);

        var needToRefresh = await addEditBankTransferContentPage.ResultDialog;
        if (needToRefresh) RefreshAccountTotals();
    }

    private async void ButtonImageViewHistory_OnClicked(object? sender, EventArgs e)
    {
        var bankTransferSummaryContentPage = new BankTransferSummaryContentPage();
        await Navigation.PushAsync(bankTransferSummaryContentPage);

        var needToRefresh = await bankTransferSummaryContentPage.ResultDialog;
        if (needToRefresh) RefreshAccountTotals();
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private async void TapGestureRecognizerAccount_OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid grid) return;
        if (grid.BindingContext is not VTotalByAccount vTotalByAccount) return;

        var addEditAccountContentPage = new AddEditAccountContentPage { CanDelete = true };
        addEditAccountContentPage.SetAccount(id: vTotalByAccount.Id);
        await Navigation.PushAsync(addEditAccountContentPage);

        var result = await addEditAccountContentPage.ResultDialog;
        if (result is not true) return;

        RefreshAccountTotals();
        DashBoardContentPage.Instance.RefreshAccountTotal();
    }

    #endregion

    #region Function

    private void RefreshAccountTotals()
    {
        VTotalByAccounts.Clear();

        using var context = new DataBaseContext();
        VTotalByAccounts.AddRange(context.VTotalByAccounts.OrderBy(s => s.Name));
        TotalAllAccount = VTotalByAccounts.Sum(s => s.Total) ?? 0d;
    }

    private void UpdateLanguage()
    {
        LabelTextTransactionHistory = AccountManagementContentPageResources.LabelTextTransactionHistory;
        LabelTextTransactionTransfer = AccountManagementContentPageResources.LabelTextTransactionTransfer;
    }

    #endregion

    private async void NavigateTo(Type type)
    {
        var contentPage = (ContentPage)Activator.CreateInstance(type)!;
        await Navigation.PushAsync(contentPage);
    }
}