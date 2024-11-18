using System.Collections.ObjectModel;
using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CurrencySymbolSummaryContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;

namespace MyExpenses.Smartphones.ContentPages;

public partial class AccountTypeSummaryContentPage
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(AccountTypeSummaryContentPage), default(string));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty AccountTypeNameProperty = BindableProperty.Create(nameof(AccountTypeName),
        typeof(string), typeof(AccountTypeSummaryContentPage), default(string));

    public string AccountTypeName
    {
        get => (string)GetValue(AccountTypeNameProperty);
        set => SetValue(AccountTypeNameProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(AccountTypeSummaryContentPage), default(string));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public int MaxLength { get; } = 64;
    public ObservableCollection<TAccountType> AccountTypes { get; } = [];

    public ICommand BackCommand { get; set; }

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;
    
    public AccountTypeSummaryContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        RefreshAccountTypes();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        // TODO work
        PlaceholderText = CurrencySymbolSummaryContentPageResources.PlaceholderText;
        ButtonValidText = CurrencySymbolSummaryContentPageResources.ButtonValidText;
    }

    private void RefreshAccountTypes()
    {
        AccountTypes.Clear();

        using var context = new DataBaseContext();
        AccountTypes.AddRange(context.TAccountTypes.OrderBy(s => s.Name));
    }

    private async void OnBackCommandPressed()
    {
        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }


    private void ButtonValid_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ButtonAccountType_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}