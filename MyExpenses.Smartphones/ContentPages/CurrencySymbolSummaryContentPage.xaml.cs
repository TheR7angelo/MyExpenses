using System.Collections.ObjectModel;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.CurrencySymbolSummaryContentPage;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages;

public partial class CurrencySymbolSummaryContentPage
{
    public static readonly BindableProperty ButtonValidTextProperty = BindableProperty.Create(nameof(ButtonValidText),
        typeof(string), typeof(CurrencySymbolSummaryContentPage), default(string));

    public string ButtonValidText
    {
        get => (string)GetValue(ButtonValidTextProperty);
        set => SetValue(ButtonValidTextProperty, value);
    }

    public static readonly BindableProperty SymbolTextProperty = BindableProperty.Create(nameof(SymbolText),
        typeof(string), typeof(CurrencySymbolSummaryContentPage), default(string));

    public string SymbolText
    {
        get => (string)GetValue(SymbolTextProperty);
        set => SetValue(SymbolTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText),
        typeof(string), typeof(CurrencySymbolSummaryContentPage), default(string));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public ObservableCollection<TCurrency> Currencies { get; }

    public CurrencySymbolSummaryContentPage()
    {
        using var context = new DataBaseContext();
        Currencies = [..context.TCurrencies.OrderBy(s => s.Symbol)];

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();


    private void UpdateLanguage()
    {
        PlaceholderText = CurrencySymbolSummaryContentPageResources.PlaceholderText;
        ButtonValidText = CurrencySymbolSummaryContentPageResources.ButtonValidText;
    }

    // TODO work
    private async void ButtonSymbol_OnClicked(object? sender, EventArgs e)
    {
        await DisplayAlert("Symbol", $"Symbol: {((Button)sender!).Text}", "Ok");
    }

    private async void ButtonValid_OnClicked(object? sender, EventArgs e)
    {
        var validate = await ValidateCurrencySymbol();
        if (!validate) return;

        var response = await DisplayAlert("Question", $"Do you really want to add {SymbolText} as a currency symbol ?",
            "Yes", "No");
        if (!response) return;

        var newCurrency = new TCurrency
        {
            Symbol = SymbolText,
            DateAdded = DateTime.Now
        };

        var (success, exception) = newCurrency.AddOrEdit();
        if (success)
        {
            await DisplayAlert("Success", "Currency symbol was successfully added", "Ok");
            Currencies.Add(newCurrency);

            SymbolText = string.Empty;
        }
        else
        {
            await DisplayAlert("Error", "An error occurred while adding currency symbol, please retry", "Ok");
        }
    }

    private async Task<bool> ValidateCurrencySymbol()
    {
        if (SymbolText.Equals(string.Empty))
        {
            await DisplayAlert(
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorEmptyTitle,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorEmptyMessage,
                CurrencySymbolSummaryContentPageResources.MessageBoxValidateCurrencySymbolErrorEmptyOkButton);
            return false;
        }

        var alreadyExist = Currencies.Any(s => s.Symbol!.Equals(SymbolText));
        if (alreadyExist)
        {
            await DisplayAlert("Error", "Symbol already exist", "Ok");
            return false;
        }

        return true;
    }
}