using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages;

public partial class CurrencySymbolSummaryContentPage
{
    public ObservableCollection<TCurrency> Currencies { get; }

    public CurrencySymbolSummaryContentPage()
    {
        using var context = new DataBaseContext();
        Currencies = [..context.TCurrencies.OrderBy(s => s.Symbol)];

        InitializeComponent();
    }

    private async void ButtonSymbol_OnClicked(object? sender, EventArgs e)
    {
        await DisplayAlert("Symbol", $"Symbol: {((Button)sender!).Text}", "Ok");
    }
}