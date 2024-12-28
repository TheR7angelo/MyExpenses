using System.Collections.Immutable;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Tables;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AccountManagementContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AccountManagementContentPage
{
    public static readonly BindableProperty LabelTextTransactionTransferProperty =
        BindableProperty.Create(nameof(LabelTextTransactionTransfer), typeof(string),
            typeof(AccountManagementContentPage));

    public string LabelTextTransactionTransfer
    {
        get => (string)GetValue(LabelTextTransactionTransferProperty);
        set => SetValue(LabelTextTransactionTransferProperty, value);
    }

    public static readonly BindableProperty LabelTextTransactionHistoryProperty =
        BindableProperty.Create(nameof(LabelTextTransactionHistory), typeof(string),
            typeof(AccountManagementContentPage));

    public string LabelTextTransactionHistory
    {
        get => (string)GetValue(LabelTextTransactionHistoryProperty);
        set => SetValue(LabelTextTransactionHistoryProperty, value);
    }

    public static readonly BindableProperty TotalAllAccountProperty = BindableProperty.Create(nameof(TotalAllAccount),
        typeof(double), typeof(AccountManagementContentPage), 0d);

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

    private async void ButtonImageViewRemoveAccount_OnClicked(object? sender, EventArgs e)
    {
        var mapper = Mapping.Mapper;
        await using var context = new DataBaseContext();
        var accountsDerives = context.TAccounts.Select(s => mapper.Map<TAccountDerive>(s)).AsEnumerable();

        var customPopupFilterAccount = new CustomPopupFilterAccount(accountsDerives);
        await this.ShowPopupAsync(customPopupFilterAccount);

        var filteredItem = customPopupFilterAccount.GetFilteredItemChecked().ToImmutableList();
        if (filteredItem.IsEmpty) return;

        var response = await DisplayAlert(
            AccountManagementContentPageResources.MessageBoxRemoveAccountQuestionTitle,
            string.Format(AccountManagementContentPageResources.MessageBoxRemoveAccountQuestionMessage, Environment.NewLine),
            AccountManagementContentPageResources.MessageBoxRemoveAccountQuestionYesButton,
            AccountManagementContentPageResources.MessageBoxRemoveAccountQuestionNoButton);
        if (!response) return;

        await Task.Delay(TimeSpan.FromMilliseconds(100));
        this.ShowCustomPopupActivityIndicator(AccountManagementContentPageResources.CustomPopupActivityIndicatorDeleteAccount);
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        var deleteErrors = new List<TAccount>();
        foreach (var accountDerive in filteredItem)
        {
            TAccount account = accountDerive;

            var json = account.ToJson();
            Log.Information("Attempt to delete account {Json}", json);

            var (success, exception) = account.Delete(true);
            if (success) Log.Information("Account was successfully deleted");
            else
            {
                Log.Error(exception, "Error while deleting account");
                deleteErrors.Add(account);
            }
        }

        RefreshAccountTotals();
        DashBoardContentPage.Instance.RefreshAccountTotal();

        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();

        if (deleteErrors.Count > 0)
        {
            await DisplayAlert(
                AccountManagementContentPageResources.MessageBoxRemoveAccountErrorTitle,
                string.Format(AccountManagementContentPageResources.MessageBoxRemoveAccountErrorMessage, deleteErrors.Count),
                AccountManagementContentPageResources.MessageBoxRemoveAccountErrorOkButton);
        }
        else
        {
            await DisplayAlert(
                AccountManagementContentPageResources.MessageBoxRemoveAccountSuccessTitle,
                string.Format(AccountManagementContentPageResources.MessageBoxRemoveAccountSuccessMessage, filteredItem.Count),
                AccountManagementContentPageResources.MessageBoxRemoveAccountSuccessOkButton);
        }
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
}