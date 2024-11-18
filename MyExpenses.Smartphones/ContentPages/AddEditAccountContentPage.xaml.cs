using System.Collections.ObjectModel;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.AddEditAccountContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AddEditAccountContentPage
{
    public static readonly BindableProperty LabelTextTitleAccountTypeProperty =
        BindableProperty.Create(nameof(LabelTextTitleAccountType), typeof(string), typeof(AddEditAccountContentPage),
            default(string));

    public string LabelTextTitleAccountType
    {
        get => (string)GetValue(LabelTextTitleAccountTypeProperty);
        set => SetValue(LabelTextTitleAccountTypeProperty, value);
    }

    public static readonly BindableProperty LabelTextTitleCurrencyProperty =
        BindableProperty.Create(nameof(LabelTextTitleCurrency), typeof(string), typeof(AddEditAccountContentPage),
            default(string));

    public string LabelTextTitleCurrency
    {
        get => (string)GetValue(LabelTextTitleCurrencyProperty);
        set => SetValue(LabelTextTitleCurrencyProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(AddEditAccountContentPage), default(string));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty CanDeleteProperty = BindableProperty.Create(nameof(CanDelete), typeof(bool),
        typeof(AddEditAccountContentPage), default(bool));

    public bool CanDelete
    {
        get => (bool)GetValue(CanDeleteProperty);
        set => SetValue(CanDeleteProperty, value);
    }

    public ObservableCollection<TAccountType> AccountTypes { get; } = [];
    public ObservableCollection<TCurrency> Currencies { get; } = [];
    private List<TAccount> Accounts { get; }
    public TAccount Account { get; } = new();

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public AddEditAccountContentPage()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts];

        RefreshObservableCollectionDatabase();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void RefreshObservableCollectionDatabase()
    {
        RefreshCurrencies();
        RefreshAccountTypes();
    }

    private void RefreshCurrencies()
    {
        using var context = new DataBaseContext();
        Currencies.Clear();
        Currencies.AddRange(context.TCurrencies.OrderBy(s => s.Symbol));
    }

    private void RefreshAccountTypes()
    {
        using var context = new DataBaseContext();
        AccountTypes.Clear();
        AccountTypes.AddRange(context.TAccountTypes.OrderBy(s => s.Name));
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        PlaceholderText = AddEditAccountContentPageResources.PlaceholderText;
        LabelTextTitleCurrency = AddEditAccountContentPageResources.LabelTextTitleCurrency;
        LabelTextTitleAccountType = AddEditAccountContentPageResources.LabelTextTitleAccountType;
    }

    public void SetAccount(TAccount? account = null, int? id = null)
    {
        if (account is not null) account.CopyPropertiesTo(Account);
        else if (id is not null)
        {
            account = Accounts.First(s => s.Id.Equals(id));
            account.CopyPropertiesTo(Account);
        }
        else throw new ArgumentNullException(nameof(id), @"account id is null");
    }

    private async void ButtonAddEditCurrency_OnClick(object? sender, EventArgs e)
    {
        var currencyFk = Account.CurrencyFk;

        var currencySymbolSummaryContentPage = new CurrencySymbolSummaryContentPage();
        await Navigation.PushAsync(currencySymbolSummaryContentPage);

        var result = await currencySymbolSummaryContentPage.ResultDialog;
        if (!result) return;

        RefreshCurrencies();
        Account.CurrencyFk = currencyFk;
    }

    private async void ButtonAddEditAccountType_OnClick(object? sender, EventArgs e)
    {
        var accountTypeFk = Account.AccountTypeFk;

        var accountTypeSummaryContentPage = new AccountTypeSummaryContentPage();
        await Navigation.PushAsync(accountTypeSummaryContentPage);

        var result = await accountTypeSummaryContentPage.ResultDialog;
        if (!result) return;

        RefreshAccountTypes();
        Account.AccountTypeFk = accountTypeFk;
    }
}