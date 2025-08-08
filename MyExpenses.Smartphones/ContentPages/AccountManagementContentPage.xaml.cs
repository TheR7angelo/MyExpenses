using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Extensions;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.SharedUtils;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.AccountManagement;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.PopupFilter;
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
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(double), typeof(AccountManagementContentPage), 0d);

    public double TotalAllAccount
    {
        get => (double)GetValue(TotalAllAccountProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(TotalAllAccountProperty, value);
    }

    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; } = [];

    public AccountManagementContentPage()
    {
        RefreshAccountTotals();

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonImageViewAddAccount_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonImageViewAddAccountAsync();

    private void ButtonImageViewCreatBankTransfer_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonImageViewCreateBankTransferAsync();

    private void ButtonImageViewHistory_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonImageViewHistory();

    private void ButtonImageViewRemoveAccount_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonImageViewRemoveAccount();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void TapGestureRecognizerAccount_OnTapped(object? sender, TappedEventArgs e)
        => _ = HandleTapGestureRecognizerAccount(sender);

    #endregion

    #region Function

    private async Task HandleButtonImageViewAddAccountAsync()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This creates and pushes the page onto the navigation stack, enabling interaction with the user.
        var addEditAccountContentPage = new AddEditAccountContentPage();
        await addEditAccountContentPage.NavigateToAsync();

        var result = await addEditAccountContentPage.ResultDialog;
        if (result is not true) return;

        RefreshAccountTotals();
        DashBoardContentPage.Instance.RefreshAccountTotal();
    }

    private async Task HandleButtonImageViewCreateBankTransferAsync()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This creates and pushes the page onto the navigation stack, enabling interaction with the user.
        var addEditBankTransferContentPage = new AddEditBankTransferContentPage { IsNewBankTransfer = true };
        await addEditBankTransferContentPage.NavigateToAsync();

        var needToRefresh = await addEditBankTransferContentPage.ResultDialog;
        if (needToRefresh) RefreshAccountTotals();
    }

    private async Task HandleButtonImageViewHistory()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This creates and pushes the page onto the navigation stack, enabling interaction with the user.
        var bankTransferSummaryContentPage = new BankTransferSummaryContentPage();
        await bankTransferSummaryContentPage.NavigateToAsync();

        var needToRefresh = await bankTransferSummaryContentPage.ResultDialog;
        if (needToRefresh) RefreshAccountTotals();
    }

    private async Task HandleButtonImageViewRemoveAccount()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        await using var context = new DataBaseContext();
        var popupSearches = context.TAccounts.Select(s => Mapping.Mapper.Map<PopupSearch>(s)).AsEnumerable();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.Account);
        await this.ShowPopupAsync(popupFilter);

        var filteredItem = popupFilter.GetFilteredItemChecked().ToArray();
        if (filteredItem.Length is 0) return;

        var response = await DisplayAlert(
            AccountManagementResources.MessageBoxRemoveAccountQuestionTitle,
            string.Format(AccountManagementResources.MessageBoxRemoveAccountQuestionMessage, Environment.NewLine),
            AccountManagementResources.MessageBoxRemoveAccountQuestionYesButton,
            AccountManagementResources.MessageBoxRemoveAccountQuestionNoButton);
        if (!response) return;

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        List<TAccount>? deleteErrors = null;
        await this.ShowCustomPopupActivityIndicatorAsync(AccountManagementResources.ActivityIndicatorPleaseWaitTitle,
            AccountManagementResources.ActivityIndicatorDeleteAccountMessage, async () => deleteErrors = await HandleDeleteAccountsAsync(filteredItem));

        if (deleteErrors!.Count > 0)
        {
            await DisplayAlert(
                AccountManagementResources.MessageBoxRemoveAccountErrorTitle,
                string.Format(AccountManagementResources.MessageBoxRemoveAccountErrorMessage, deleteErrors.Count.ToString()),
                AccountManagementResources.MessageBoxRemoveAccountErrorOkButton);
        }
        else
        {
            await DisplayAlert(
                AccountManagementResources.MessageBoxRemoveAccountSuccessTitle,
                string.Format(AccountManagementResources.MessageBoxRemoveAccountSuccessMessage, filteredItem.Length.ToString()),
                AccountManagementResources.MessageBoxRemoveAccountSuccessOkButton);
        }
    }

    private async Task<List<TAccount>?> HandleDeleteAccountsAsync(IEnumerable<PopupSearch> filteredItem)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        List<TAccount>? deleteErrors = null;
        var accounts = filteredItem.ToTable<TAccount>()!;
        foreach (var account in accounts)
        {
            var json = account.ToJson();
            Log.Information("Attempt to delete account {Json}", json);

            var (success, exception) = account.Delete(true);
            if (success) Log.Information("Account was successfully deleted");
            else
            {
                Log.Error(exception, "Error while deleting account");

                deleteErrors ??= [];
                deleteErrors.Add(account);
            }
        }

        RefreshAccountTotals();
        DashBoardContentPage.Instance.RefreshAccountTotal();

        return deleteErrors;
    }

    private async Task HandleTapGestureRecognizerAccount(object? sender)
    {
        if (sender is not Grid grid) return;
        if (grid.BindingContext is not VTotalByAccount vTotalByAccount) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This creates and pushes the page onto the navigation stack, enabling interaction with the user.
        var addEditAccountContentPage = new AddEditAccountContentPage { CanDelete = true };
        addEditAccountContentPage.SetAccount(id: vTotalByAccount.Id);
        await addEditAccountContentPage.NavigateToAsync();

        var result = await addEditAccountContentPage.ResultDialog;
        if (result is not true) return;

        RefreshAccountTotals();
        DashBoardContentPage.Instance.RefreshAccountTotal();
    }

    private void RefreshAccountTotals()
    {
        VTotalByAccounts.Clear();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        VTotalByAccounts.AddRange(context.VTotalByAccounts.OrderBy(s => s.Name));
        TotalAllAccount = VTotalByAccounts.Sum(s => s.Total) ?? 0d;
    }

    private void UpdateLanguage()
    {
        LabelTextTransactionHistory = AccountManagementResources.LabelTextTransactionHistory;
        LabelTextTransactionTransfer = AccountManagementResources.LabelTextTransactionTransfer;
    }

    #endregion
}